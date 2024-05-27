//Refactoring v1.0
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UniRx;
using DG.Tweening;
using Spine.Unity;
using System.Linq;
using System;
using Unity.VisualScripting;

public class PlaceManager : Singleton<PlaceManager>
{
    #region Value

    [Header("=== Camera")]
    [SerializeField] Camera mainCamera;
    
    [Header("=== Loading")]
    [Header("-- Go Somewhere")]
    [SerializeField] CanvasGroup goingSomewhereloadingCG;
    [SerializeField] TMP_Text currentPlaceTxt;
    [SerializeField] SkeletonGraphic step_SG;
    [Header("-- Main UI")]
    [SerializeField] TMP_Text HUD_currentPlactTxt;

    [Header("=== Place Components")]
    [SerializeField] public List<IDBtn> placeBtnList = new();
    [SerializeField] List<PlaceSet> placeSetList = new();
    Dictionary<IDBtn, PlaceSet> placeIdBtnGODict;

    [Header("=== About Data")]
    [SerializeField] public List<string> canGoPlaceInChapter = new List<string>();
    [SerializeField] public List<string> visitReasons = new List<string>();
    IDBtn currentIdBtn;
    #endregion

    #region Framework & Base Set

    public void Offset()
    {
        // Set Place Btn
        foreach (IDBtn placeBtn in placeBtnList)
        {
            // Set Language Text
            LanguageManager.Instance.SetLanguageTxt(placeBtn.buttonText);

            // Suvscribe Btn
            if (placeBtn == placeBtnList[0])
            {
                placeBtn.button.OnClickAsObservable()
                    .Subscribe(btn =>
                    {
                        if (!GameManager.Instance.canInput) { return; }

                        PhoneHardware.Instance.PhoneOff();
                        PlayerInputController.Instance.CanMove = true;
                    });
            }
            else
            {
                placeBtn.button.OnClickAsObservable()
                    .Subscribe(placeBV =>
                    {
                        if (!GameManager.Instance.canInput) { return; }

                        currentIdBtn = placeBtn;
                        PhoneSoftware.Instance.OpenPopup(currentIdBtn, 0.5f, 0.5f);
                    });
            }
            
            // Check Interactable
            canGoPlaceInChapter =
                DataManager.Instance.ChapterCSVDatas[9][GameManager.Instance.currentChapter].ToString().Split('/').ToList();
            visitReasons =
                DataManager.Instance.ChapterCSVDatas[12 + LanguageManager.Instance.languageNum][GameManager.Instance.currentChapter].ToString().Split("/").ToList();


            if (canGoPlaceInChapter.Contains(placeBtn.buttonID))
            { placeBtn.button.interactable = true; }
            else 
            { placeBtn.button.interactable = false; }
        }

        // Set Dict
        placeIdBtnGODict = new Dictionary<IDBtn, PlaceSet>();
        for (int i = 0; i < placeBtnList.Count; i++)
        { placeIdBtnGODict.Add(placeBtnList[i], placeSetList[i]); }

        // Spawn Room
        SetCurrent3DMap(placeBtnList[0]); 
    }

    protected override void Awake()
    {
        base.Awake();
    }

    #endregion

    #region Spawn3Dmap

    public void SetCurrent3DMap(IDBtn idBtn)
    {
        // Spawn Map
        foreach (KeyValuePair<IDBtn, PlaceSet> placeDict in placeIdBtnGODict)
        {
            if(placeDict.Key == idBtn)
            {
                placeDict.Value.MapGO.SetActive(true);
                placeDict.Value.MapGO.transform.position = new Vector3(0f, 0f, 0f);
                mainCamera.backgroundColor = placeDict.Value.BGColor;
                
                // Spawn Map Object ( No Home )
                if(placeDict.Key != placeBtnList[0])
                {
                    placeIdBtnGODict[idBtn].Inevitable_InteractObjects = new List<BasicInteractObject>();
                    placeIdBtnGODict[idBtn].Inevitable_InteractNPCs = new List<NpcInteractObject>();

                    foreach (BasicInteractObject BIO in placeDict.Value.InteractObjects)
                    {
                        BIO.IsInteracted = false;
                        if (DataManager.Instance.ChapterCSVDatas[10][GameManager.Instance.currentChapter].ToString().Split('/').ToList().Contains(BIO.ID))
                        { BIO.gameObject.SetActive(true); placeIdBtnGODict[idBtn].Inevitable_InteractObjects.Add(BIO); }
                        else
                        { BIO.gameObject.SetActive(false); }
                    }
                    foreach (NpcInteractObject NIO in placeDict.Value.InteractNPCs)
                    {
                        NIO.IsInteracted = false;
                        if (DataManager.Instance.ChapterCSVDatas[11][GameManager.Instance.currentChapter].ToString().Split('/').ToList().Contains(NIO.ID))
                        { NIO.gameObject.SetActive(true); placeIdBtnGODict[idBtn].Inevitable_InteractNPCs.Add(NIO); }
                        else
                        { NIO.gameObject.SetActive(false); }
                    }
                }
            }
            else
            {
                placeDict.Value.MapGO.SetActive(false);
                placeDict.Value.MapGO.transform.position = new Vector3(100f, 0f, 0f);
            }
            
        }

        
        // Set Text UI
        HUD_currentPlactTxt.text = DataManager.Instance.PlaceCSVDatas[LanguageManager.Instance.languageNum][idBtn.buttonID].ToString();

        // Reset
        PlayerController.Instance.ft_resetPlayerSpot();
        SetInteractionObjects.Instance.SetOn_InteractiveOB();
    }

    #endregion

    #region Going Somewhere

    public void StartGoingSomewhereLoading(float delay)
    {
        GameManager.Instance.canInput = false;
        PlayerInputController.Instance.StopMove();
        PhoneHardware.Instance.PhoneOff();
        currentIdBtn.button.interactable = false;

        StartCoroutine(GoingSomewhereLoading(currentIdBtn, delay));
    }

    private IEnumerator GoingSomewhereLoading(IDBtn idBtn, float delay)
    {
        // Set Loading Canvas
        LanguageManager.Instance.SetLanguageTxt(currentPlaceTxt);
        currentPlaceTxt.text = 
            $"\"{DataManager.Instance.PlaceCSVDatas[LanguageManager.Instance.languageNum][idBtn.buttonID]}\"";
        goingSomewhereloadingCG.alpha = 0f;
        goingSomewhereloadingCG.DOFade(1, delay);
        goingSomewhereloadingCG.gameObject.SetActive(true);

        // Set Spine
        step_SG.TryGetComponent(out RectTransform rectTransform);
        if (rectTransform.localScale.x < 0)
        {
            Vector3 v3 = rectTransform.localScale;
            v3.x *= -1;
            rectTransform.localScale = v3;
        }
        rectTransform.anchoredPosition = new Vector2(-400f, 0f);

        step_SG.AnimationState.SetEmptyAnimations(0);
        step_SG.AnimationState.SetAnimation(trackIndex: 0, "animation", loop: false);


        yield return new WaitForSeconds(delay);

        // Interact Objects -> Off
        SetInteractionObjects.Instance.SetOff_InteractiveOB();

        // Desc Panel -> Off
        GameSystem.Instance.objPanelBtn.gameObject.SetActive(false);
        GameSystem.Instance.npcPanelBtn.gameObject.SetActive(false);

        // Gen Map
        SetCurrent3DMap(idBtn);

        yield return new WaitForSeconds(delay);

        // End
        goingSomewhereloadingCG.DOFade(0, delay)
            .OnComplete(() =>
            {
                EndGoingSomewhereLoading();
            });
    }

    private void EndGoingSomewhereLoading()
    {
        GameManager.Instance.canInput = true;
        PlayerInputController.Instance.CanMove = true;
        goingSomewhereloadingCG.gameObject.SetActive(false);
    }

    #endregion

}

[Serializable]
public class PlaceSet
{
    public GameObject MapGO;
    public Color BGColor;
    public List<BasicInteractObject> InteractObjects;
    [HideInInspector] public List<BasicInteractObject> Inevitable_InteractObjects;
    public List<NpcInteractObject> InteractNPCs;
    [HideInInspector] public List<NpcInteractObject> Inevitable_InteractNPCs;
    public PlaceSet(GameObject mapGO, Color bgColor)
    {
        MapGO = mapGO;
        BGColor = bgColor;
    }
}