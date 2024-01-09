using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;
using TMPro;

public class ActionEventManager : Manager<ActionEventManager>
{
    [Header("*Manager")]
    [SerializeField] GameManager GameManager;
    [SerializeField] GameSystem GameSystem;
    [SerializeField] JsonManager JsonManager;

    [Header("*LoadingWindow")]
    [SerializeField] TMP_Text CurrentChapterText;
    [SerializeField] TMP_Text PassDayExplanationText;
    [SerializeField] TMP_Text SavingPrograssText;
    [SerializeField] CanvasGroup loading;
    [SerializeField] TMP_Text MainSceneUI_Day;


    [Header("*Place")]
    [HideInInspector] public string currentActionEventID;
    [SerializeField] List<PlaceDataBase> placeList = new();
    [SerializeField] Transform placeParent;
    [SerializeField] GameObject currentPlace;
    [SerializeField] GameObject home;


    [SerializeField] private ReactiveProperty<PlaceDataBase> placeData = new ReactiveProperty<PlaceDataBase>();


    protected override void Awake()
    {
        //base.Awake();
        SetChapterText();
        AppearTextObject(1f);
        loading.gameObject.SetActive(true);
        loading.alpha = 1.0f;
        StartCoroutine(Post_ShowNextDayText(1f));

        placeData
            .Where(data => data != null)
            .Subscribe(data =>
            {
                StartCoroutine(ParsePlace(data));
            });
    }

    #region Place

    public IEnumerator PlaceSetting()
    {
        GameSystem.TurnOnLoading();
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(ChangePlacePerData(FindPlace()));
    }

    private PlaceDataBase FindPlace() // 장소를 리소스에서 찾기
    {
        var allPlace = ResourceData<PlaceDataBase>.GetDatas("Place/PlaceData");
        return Array.Find(allPlace, x => x.placeID == currentActionEventID.Substring(0, 3));
    }

    private IEnumerator ParsePlace(PlaceDataBase data) // 장소 세팅하기
    {
        yield return new WaitForEndOfFrame();
        home.gameObject.SetActive(false);
        yield return new WaitForEndOfFrame();
        currentPlace = Instantiate(data.place, placeParent);
        GameSystem.playerPos.position = data.spawnPos;
    }

    public IEnumerator ChangePlacePerData(PlaceDataBase placeData) // UniRX 데이터에 찾은 장소 넣기
    {
        this.placeData.Value = null;
        yield return new WaitForEndOfFrame();
        this.placeData.Value = placeData;
    }

    #endregion

    #region PassDay Loading

    public void TurnOnLoading()
    {
        SetChapterText();
        AppearTextObject(1f);
        StartCoroutine(Past_ShowNextDayText(1f));
    }
    private IEnumerator Past_ShowNextDayText(float time)
    {
        BeforeSaveDatas();

        PassDayExplanationText.color = Color.white;
        string TextTemp = "";
        TextTemp = "DAY [" + GameManager.currentMainInfo.day + "]";

        StartLoading();

        PassDayExplanationText.text = TextTemp;
        yield return new WaitForSeconds(time + 0.5f);

        PassDayExplanationText.DOFade(0, time);

        SetDayText();

        StartCoroutine(Post_ShowNextDayText(1f));
    }
    private IEnumerator Post_ShowNextDayText(float time)
    {
        SaveDatas();

        PassDayExplanationText.color = Color.white;

        yield return new WaitForSeconds(time);

        PassDayExplanationText.text = "";
        PassDayExplanationText.color = Color.white;
        string TextTemp = "DAY [" + GameManager.currentMainInfo.day + "]";
        PassDayExplanationText.DOText(TextTemp, time);
        yield return new WaitForSeconds(time * 2);
        PassDayExplanationText.DOFade(0, time);
        yield return new WaitForSeconds(time);

        EndLoading();
    }
    private void StartLoading()
    {
        Sequence loadingSequence = DOTween.Sequence();
        loadingSequence.OnStart(() =>
        {
            loading.gameObject.SetActive(true);
            loading.DOFade(1f, 1f);

            SavingPrograssText.text = "Saving...";
        });
    }
    private void SetDayText()
    {
        GameManager.currentMainInfo.day++;

        MainSceneUI_Day.text = "Day " + GameManager.currentMainInfo.day;
    }
    private void EndLoading()
    {
        Sequence loadingSequence = DOTween.Sequence();
        loadingSequence.Append(loading.DOFade(0f, 1f))
        .OnComplete(() =>
        {
            loading.gameObject.SetActive(false);
            //GameSystem.GameStart();
        });
    }

    #endregion

    #region Set Chapter

    private void SetChapterText()
    {
        int currentChapter = GameManager.currentMainInfo.chapter;
        CurrentChapterText.text = "CHAPTER [ " + currentChapter.ToString() + " ]";
    }

    private void AppearTextObject(float durTime)
    {
        //Color Set
        CurrentChapterText.color = new Color(255, 255, 255, 0);

        //Pos Set
        RectTransform rectTransform = CurrentChapterText.gameObject.GetComponent<RectTransform>();
        Vector2 offset = rectTransform.anchoredPosition;
        rectTransform.anchoredPosition = new Vector2(offset.x + rectTransform.rect.width, offset.y);

        CurrentChapterText.DOFade(1f, durTime);
        rectTransform.DOAnchorPos(offset, durTime);
    }

    #endregion

    #region SaveDatas

    private void BeforeSaveDatas()
    {
        SavingPrograssText.text = "< Saving... >";
        SavingPrograssText.DOFade(0f, 0.4f).SetLoops(-1, LoopType.Yoyo);
    }

    private void SaveDatas()
    {
        DOTween.Kill(SavingPrograssText);
        JsonManager.SaveAllGameDatas();
        SavingPrograssText.text = "< Saved >";
        SavingPrograssText.DOFade(1f, 1f);
    }

    #endregion
}
