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
    [SerializeField] GameObject activityGageGO;
    [SerializeField] Button comebackHomeBtn;

    [Header("=== Place Components")]
    [SerializeField] List<IDBtn> placeBtnList = new();
    [SerializeField] List<PlaceSet> placeSetList = new();
    Dictionary<IDBtn, PlaceSet> placeIdBtnGODict;

    [Header("=== About Data")]
    [SerializeField] List<string> canGoPlaceInChapter = new List<string>();
    [SerializeField] public List<InteractObject> needInteractIOs = new List<InteractObject>();
   
    [SerializeField] IDBtn currentIdBtn;

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


                Debug.Log("Spawn Map Object ( No Home )"); 
                if (placeDict.Key != placeBtnList[0])
                {
                    // 방송 시작, 끝 다이얼로그 ID 저장
                    StreamController.Instance.startSDialogID = DataManager.Instance.Get_StartSDialog(idBtn.buttonID);
                    StreamController.Instance.endSDialogID = DataManager.Instance.Get_EndSDialog(idBtn.buttonID);

                    // 상호작용해야하는 오브젝트들
                    needInteractIOs = GetAllIO(placeDict.Value.MapGO.transform);
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

    #endregion
}

[Serializable]
public class PlaceSet
{
    public GameObject MapGO;
    public Color BGColor;
    public PlaceSet(GameObject mapGO, Color bgColor)
    {
        MapGO = mapGO;
        BGColor = bgColor;
    }
}