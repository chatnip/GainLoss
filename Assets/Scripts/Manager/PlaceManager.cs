//Refactoring v1.0
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UniRx;
using DG.Tweening;
using Spine.Unity;
using System.Linq;

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
    [SerializeField] List<GameObject> placeGOList = new();
    [SerializeField] List<Color> placeColorList = new();
    Dictionary<IDBtn, PlaceSet> placeIdBtnGODict;

    [Header("=== About Data")]
    [SerializeField] public List<string> canGoPlaceInChapter = new List<string>();


    #endregion

    #region Framework & Base Set

    public void Offset()
    {
        // Set Place Btn
        foreach (IDBtn placeBtn in placeBtnList)
        {
            // Suvscribe Btn
            if (placeBtn == placeBtnList[0])
            {
                placeBtn.button.OnClickAsObservable()
                    .Subscribe(btn =>
                    {
                        if (!GameManager.Instance.canInput) { return; }
                        PhoneHardware.Instance.PhoneOff();
                    });
            }
            else
            {
                placeBtn.button.OnClickAsObservable()
                    .Subscribe(placeBV =>
                    {
                        if (!GameManager.Instance.canInput) { return; }
                        StartGoingSomewhereLoading(placeBtn, 1.5f);
                        placeBtn.button.interactable = false;

                    });
            }
            
            // Check Interactable
            canGoPlaceInChapter =
                DataManager.Instance.ChapterCSVDatas[9][GameManager.Instance.currentChapter].ToString().Split('/').ToList();
            
            if (canGoPlaceInChapter.Contains(placeBtn.buttonID))
            { placeBtn.button.interactable = true; }
            else 
            { placeBtn.button.interactable = false; }
        }

        // Set Dict
        placeIdBtnGODict = new Dictionary<IDBtn, PlaceSet>();
        for (int i = 0; i < placeBtnList.Count; i++)
        { placeIdBtnGODict.Add(placeBtnList[i], new PlaceSet(placeGOList[i], placeColorList[i])); }

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
        foreach (KeyValuePair<IDBtn, PlaceSet> placeDict in placeIdBtnGODict)
        {
            if(placeDict.Key == idBtn)
            {
                placeDict.Value.MapGO.SetActive(true);
                placeDict.Value.MapGO.transform.position = new Vector3(0f, 0f, 0f);
                mainCamera.backgroundColor = placeDict.Value.BGColor;
            }
            else
            {
                placeDict.Value.MapGO.SetActive(false);
                placeDict.Value.MapGO.transform.position = new Vector3(100f, 0f, 0f);
            }
            
        }

        HUD_currentPlactTxt.text = DataManager.Instance.PlaceCSVDatas[GameManager.Instance.languageNum][idBtn.buttonID].ToString();

        PlayerController.Instance.ft_resetPlayerSpot();
        SetInteractionObjects.Instance.SetOn_InteractiveOB();
    }

    #endregion

    #region Going Somewhere

    public void StartGoingSomewhereLoading(IDBtn idBtn, float delay)
    {
        GameManager.Instance.canInput = false;
        PlayerInputController.Instance.StopMove();
        PhoneHardware.Instance.PhoneOff();

        StartCoroutine(GoingSomewhereLoading(idBtn, delay));
    }

    private IEnumerator GoingSomewhereLoading(IDBtn idBtn, float delay)
    {
        // Set Loading Canvas
        currentPlaceTxt.text = 
            $"\"{DataManager.Instance.PlaceCSVDatas[GameManager.Instance.languageNum][idBtn.buttonID]}\"(으)로 이동합니다.";
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
        GameSystem.Instance.NpcDescOff();
        GameSystem.Instance.ObjDescOff();

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