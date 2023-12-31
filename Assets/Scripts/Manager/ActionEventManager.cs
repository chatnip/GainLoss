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

    [Header("*LoadingWindow")]
    [SerializeField] TMP_Text PassDayExplanationText;
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

        placeData
            .Where(data => data != null)
            .Subscribe(data =>
            {
                StartCoroutine(ParsePlace(data));
            });
    }

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

    public void TurnOnLoading()
    {
        StartCoroutine(ShowNextDayText(1f));
    }
    private IEnumerator ShowNextDayText(float time)
    {
        PassDayExplanationText.color = Color.white;
        string TextTemp;
        TextTemp = "DAY [" + GameManager.CurrentDay + "]";

        StartLoading();

        PassDayExplanationText.text = TextTemp;
        yield return new WaitForSeconds(time + 0.5f);

        PassDayExplanationText.DOFade(0, time);
        SetData();
        yield return new WaitForSeconds(time);

        PassDayExplanationText.text = "";
        PassDayExplanationText.color = Color.white;
        TextTemp = "DAY [" + GameManager.CurrentDay + "]";
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
        });
    }    
    private void SetData()
    {
        GameManager.CurrentDay++;

        MainSceneUI_Day.text = "Day " + GameManager.CurrentDay;
    }
    private void EndLoading()
    {
        Sequence loadingSequence = DOTween.Sequence();
        loadingSequence.Append(loading.DOFade(0f, 1f))
        .OnComplete(() =>
        {
            loading.gameObject.SetActive(false);
            GameSystem.GameStart();
        });
    }


}
