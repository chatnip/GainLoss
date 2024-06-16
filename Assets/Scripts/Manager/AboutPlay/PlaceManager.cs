//Refactoring v1.0
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UniRx;
using DG.Tweening;
using Spine.Unity;
using System;
using UnityEngine.UI;

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
    [SerializeField] Button comebackHomeBtn;

    [Header("=== Get Content")]
    [SerializeField] RectTransform getReasoningContentRT;
    [SerializeField] TMP_Text getReasoningContentNameTxt;
    [SerializeField] TMP_Text getReasoningContentDescTxt;

    [Header("=== Place Components")]
    [SerializeField] List<IDBtn> placeBtnList = new();
    [SerializeField] List<PlaceSet> placeSetList = new();
    Dictionary<IDBtn, PlaceSet> placeIdBtnGODict;

    [Header("=== About Data")]
    [SerializeField] List<string> canGoPlaceInChapter = new List<string>();
    [SerializeField] public List<InteractObject> needInteractIOs = new List<InteractObject>();
   
    [SerializeField] IDBtn currentIdBtn;

    //Other Value
    Sequence getReasoningContentSeq;

    #endregion

    #region Framework & Base Set

    public void Offset()
    {
        // Off
        goingSomewhereloadingCG.gameObject.SetActive(false);

        // Set Place Btn
        foreach (IDBtn placeBtn in placeBtnList)
        {
            // Set Btn Setting
            placeBtn.buttonText.text = DataManager.Instance.Get_LocationName(placeBtn.buttonID);
            placeBtn.button.image.sprite = placeBtn.inputBasicImage;

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
                DataManager.Instance.Get_AllLocationIDChapter(GameManager.Instance.currentChapter);

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
        getReasoningContentRT.anchoredPosition = new Vector2(getReasoningContentRT.sizeDelta.x, 0);
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

                if (placeDict.Key != placeBtnList[0])
                {
                    // 방송 시작, 끝 다이얼로그 ID 저장
                    StreamController.Instance.startSDialogID = DataManager.Instance.Get_StartSDialog(idBtn.buttonID);
                    StreamController.Instance.endSDialogID = DataManager.Instance.Get_EndSDialog(idBtn.buttonID);

                    // 상호작용해야하는 오브젝트들
                    needInteractIOs = GetAllIO(placeDict.Value.MapInteractObjectParentTF);
                }
            }
            else
            {
                placeDict.Value.MapGO.SetActive(false);
                placeDict.Value.MapGO.transform.position = new Vector3(100f, 0f, 0f);
            }
            
        }


        // Set Text UI
        HUD_currentPlactTxt.text = DataManager.Instance.Get_LocationName(idBtn.buttonID);

        // Reset
        PlayerController.Instance.ft_resetPlayerSpot();
        InteractObjectBtnController.Instance.SetOn_InteractiveOB();
    }

    #endregion

    #region Going Somewhere

    public void StartGoingSomewhereLoading(float delay)
    {
        PlayerInputController.Instance.MoveStop(); 
        PlayerController.Instance.ResetAnime();
        PhoneHardware.Instance.PhoneOff();
        currentIdBtn.button.interactable = false;

        StartCoroutine(GoingSomewhereLoading(currentIdBtn, delay));
    }

    private IEnumerator GoingSomewhereLoading(IDBtn idBtn, float delay)
    {
        yield return new WaitForEndOfFrame();
        GameManager.Instance.canInput = false;

        // Set Loading Canvas
        currentPlaceTxt.text = 
            $"\"{DataManager.Instance.Get_LocationName(idBtn.buttonID)}\"";
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
        InteractObjectBtnController.Instance.SetOff_InteractiveOB();

        // Desc Panel -> Off
        GameSystem.Instance.objPanelBtn.gameObject.SetActive(false);
        if (GameManager.Instance.currentActPart == GameManager.e_currentActPart.UseActivity) 
        { ActivityController.Instance.activityGageWindowRT.gameObject.SetActive(true); }
        else 
        { ActivityController.Instance.activityGageWindowRT.gameObject.SetActive(false); }

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
    
    private List<InteractObject> GetAllIO(Transform PlaceGO)
    {
        List<InteractObject > list = new List<InteractObject>();
        Transform[] allChildren = PlaceGO.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if(child.TryGetComponent(out InteractObject io))
            {
                io.IsInteracted = false;
                list.Add(io);
            }

        }
        return list;
    }
    public void CheckCanGoHome()
    {
        if (needInteractIOs.Count == 0)
        {
            comebackHomeBtn.TryGetComponent(out RectTransform btnRT);
            btnRT.DOAnchorPos(new Vector2(0f, 0f), 1f).SetEase(Ease.OutCubic);
        }
    }

    public void GetReasoningContent(string materialID)
    {
        if (getReasoningContentSeq == null)
        { 
            getReasoningContentSeq = DOTween.Sequence();
            getReasoningContentSeq.OnComplete(() => { getReasoningContentSeq = null; });
        }

        string name = DataManager.Instance.Get_MaterialName(materialID);
        string desc = DataManager.Instance.Get_MaterialDesc(materialID);

        Debug.Log(name);
        Debug.Log(desc);

        getReasoningContentSeq.Append(getReasoningContentRT.DOAnchorPos(new Vector2(0, 0), 1f)
                .OnStart(() =>
                {
                    getReasoningContentNameTxt.text = name;
                    getReasoningContentDescTxt.text = desc;
                }));
        getReasoningContentSeq.AppendInterval(1f); 
        getReasoningContentSeq.Append(getReasoningContentRT.DOAnchorPos(new Vector2(getReasoningContentRT.sizeDelta.x, 0), 1f));
    }

    #endregion
}

[Serializable]
public class PlaceSet
{
    public GameObject MapGO;
    public Transform MapInteractObjectParentTF;
    public Color BGColor;
    public PlaceSet(GameObject mapGO, Color bgColor)
    {
        MapGO = mapGO;
        BGColor = bgColor;
    }
}