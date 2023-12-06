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

        foreach (var place in placeList) // ������ ���̾�α� ��ȸ
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
        //loadingâ ������
        StartLoading();

        //�÷����� ��¥�� �������ٴ� Text
        string TextTemp;
        TextTemp = GameManager.CurrentDay + " day goes by.";
        nextDayText.text = TextTemp;

        yield return new WaitForSeconds(time + 0.5f);

        //Data�� �ٽ� ����
        nextDayText.DOFade(0, time);
        SetData();

        yield return new WaitForSeconds(time);

        //�����ϴ� ��¥�� �����ִ� Text
        nextDayText.text = "";
        nextDayText.color = new Color(1, 1, 1, 1);
        TextTemp = "Today is " + GameManager.CurrentDay + " Day, now.";
        nextDayText.DOText(TextTemp, time);

        yield return new WaitForSeconds(time * 2);

        nextDayText.DOFade(0, time);

        yield return new WaitForSeconds(time);

        //loadingâ ������
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
