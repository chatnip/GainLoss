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
using UnityEngine.UI;
using NaughtyAttributes.Test;

public class PlaceManager : Singleton<PlaceManager>
{
    #region Value

    [Header("=== Property")]
    [SerializeField] ComputerInteract ComputerInteract;

    [Header("=== Camera")]
    [SerializeField] Camera mainCamera;
    
    [Header("=== Loading")]
    [Header("-- Go Somewhere")]
    [SerializeField] CanvasGroup goingSomewhereloadingCG;
    [SerializeField] TMP_Text currentPlaceTxt;
    [SerializeField] SkeletonGraphic step_SG;
    [Header("-- Main UI")]
    [SerializeField] TMP_Text HUD_currentPlactTxt;
    [SerializeField] GameObject activityGageGO;
    [SerializeField] Button comebackHomeBtn;

    [Header("=== Place Components")]
    [SerializeField] public List<IDBtn> placeBtnList = new();
    [SerializeField] List<PlaceSet> placeSetList = new();
    Dictionary<IDBtn, PlaceSet> placeIdBtnGODict;

    [Header("=== About Data")]
    [SerializeField] public List<string> canGoPlaceInChapter = new List<string>();
    [SerializeField] public List<string> visitReasons = new List<string>();
    [SerializeField] public IDBtn currentIdBtn;

    #endregion

    #region Framework & Base Set

    public void Offset()
    {
        // Set Place Btn
        foreach (IDBtn placeBtn in placeBtnList)
        {
            // Set Btn Setting
            placeBtn.buttonText.text = DataManager.Instance.PlaceCSVDatas[LanguageManager.Instance.languageNum][placeBtn.buttonID].ToString();
            placeBtn.button.image.sprite = placeBtn.inputBasicImage;

            // Set Language Text
            LanguageManager.Instance.SetLanguageTxt(placeBtn.buttonText);

            // Subscribe Btn
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
                        PhoneSoftware.Instance.ZoomInPlaceMap(currentIdBtn, 1.2f, 0.5f);
                        PhoneSoftware.Instance.OpenPopup(currentIdBtn, 0.5f);
                    });
            }


            // Check Interactable
            canGoPlaceInChapter =
                DataManager.Instance.ChapterCSVDatas[LanguageManager.Instance.languageTypeAmount * 2 + 6][GameManager.Instance.currentChapter].ToString().Split('/').ToList();
            visitReasons =
                DataManager.Instance.ChapterCSVDatas[LanguageManager.Instance.languageTypeAmount + LanguageManager.Instance.languageNum][GameManager.Instance.currentChapter].ToString().Split("/").ToList();

            // Place Btn Set
            if (canGoPlaceInChapter.Contains(placeBtn.buttonID))
            { placeBtn.button.interactable = true; }
            else 
            { placeBtn.button.interactable = false; }

            comebackHomeBtn.TryGetComponent(out RectTransform btnRT);
            btnRT.anchoredPosition = new Vector2(-300f, 0f);


            // Set Btn Subs
            comebackHomeBtn.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    if (!GameManager.Instance.canInput) { return; }

                    currentIdBtn = placeBtnList[0];
                    StartGoingSomewhereLoading(1.5f);
                    comebackHomeBtn.TryGetComponent(out RectTransform btnRT);
                    btnRT.DOAnchorPos(new Vector2(-300f, 0f), 1f).SetEase(Ease.OutCubic);

                    GameManager.Instance.currentActPart = GameManager.e_currentActPart.StreamingTime;
                });

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
                if (placeDict.Key != placeBtnList[0])
                {
                    // Set Stream Reservation
                    StreamController.Instance.SetstreamReservationID(idBtn.buttonID);
                    placeIdBtnGODict[idBtn].Inevitable_InteractObjects = new List<InteractObject>();

                    foreach (InteractObject IO in placeDict.Value.InteractObjects)
                    {
                        IO.IsInteracted = false;
                        if (DataManager.Instance.ChapterCSVDatas[LanguageManager.Instance.languageTypeAmount * 2 + 7][GameManager.Instance.currentChapter].ToString().Split('/').ToList().Contains(IO.ID))
                        { IO.gameObject.SetActive(true); placeIdBtnGODict[idBtn].Inevitable_InteractObjects.Add(IO); }
                        else
                        { IO.gameObject.SetActive(false); }
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
        PlayerInputController.Instance.MoveStop(); 
        PlayerController.Instance.resetAnime();
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
        if (GameManager.Instance.currentActPart == GameManager.e_currentActPart.UseActivity) 
        { activityGageGO.SetActive(true); }
        else 
        { activityGageGO.SetActive(false); }

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

    #region Place Object
    
    public void Exclude_InevitableIO(InteractObject ExcludeIO)
    {
        if (placeIdBtnGODict[currentIdBtn].Inevitable_InteractObjects.Contains(ExcludeIO))
        { placeIdBtnGODict[currentIdBtn].Inevitable_InteractObjects.Remove(ExcludeIO); }
        CheckCanGoHome();
    }
    public void CheckCanGoHome()
    {
        if(placeIdBtnGODict[currentIdBtn].Inevitable_InteractObjects.Count == 0)
        {
            comebackHomeBtn.TryGetComponent(out RectTransform btnRT);
            btnRT.DOAnchorPos(new Vector2(0f, 0f), 1f).SetEase(Ease.OutCubic);
        }
    }

    #endregion
}

[Serializable]
public class PlaceSet
{
    public GameObject MapGO;
    public Color BGColor;
    public List<InteractObject> InteractObjects;
    [Tooltip("자동 초기화")] public List<InteractObject> Inevitable_InteractObjects;
    public PlaceSet(GameObject mapGO, Color bgColor)
    {
        MapGO = mapGO;
        BGColor = bgColor;
    }
}