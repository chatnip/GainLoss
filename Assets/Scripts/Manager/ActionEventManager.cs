using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEditor.ShaderKeywordFilter;
using TMPro;

public class ActionEventManager : Manager<ActionEventManager>
{
    [Header("*Manager")]
    [SerializeField] GameManager GameManager;
    [SerializeField] GameSystem GameSystem;

    [Header("*Place")]
    [SerializeField] List<Place> placeList = new();
    [HideInInspector] public string currentActionEventID;

    [Header("*UI")]
    [SerializeField] CanvasGroup loading;
    [SerializeField] TMP_Text nextDayText;

    private Place currentPlace;

    public void PlaceSetting()
    {
        TurnOnLoading();

        foreach (var place in placeList) // 베이직 다이얼로그 순회
        {
            if (place.placeID.Contains(currentActionEventID))
            {
                
            }
        }
    }

    public void TurnOnLoading()
    {
        StartCoroutine(ShowNextDayText(1f));
    }


    private IEnumerator ShowNextDayText(float time)
    {
        //loading창 켜지기
        StartLoading();

        //플레이한 날짜가 지나갔다는 Text
        string TextTemp;
        TextTemp = GameManager.CurrentDay + " day goes by.";
        nextDayText.text = TextTemp;

        yield return new WaitForSeconds(time + 0.5f);

        //Data들 다시 세팅
        nextDayText.DOFade(0, time);
        SetData();

        yield return new WaitForSeconds(time);

        //시작하는 날짜를 보여주는 Text
        nextDayText.text = "";
        nextDayText.color = new Color(1, 1, 1, 1);
        TextTemp = "Today is " + GameManager.CurrentDay + " Day, now.";
        nextDayText.DOText(TextTemp, time);

        yield return new WaitForSeconds(time * 2);

        nextDayText.DOFade(0, time);

        yield return new WaitForSeconds(time);

        //loading창 꺼지기
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
