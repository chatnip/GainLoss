using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class LoadingManager : Singleton<LoadingManager>
{
    #region Value

    [Header("=== LoadingWindow")]
    [SerializeField] TMP_Text PassDayExplanationText;
    [SerializeField] TMP_Text SavingPrograssText;
    [SerializeField] CanvasGroup loading;

    [Header("=== UI")]
    [SerializeField] TMP_Text dayText;

    #endregion

    #region Enum
    enum WeekDays
    {
        Monday = 0, Tuesday = 1, Wednesday = 2, Thursday = 3, Friday = 4, Saturday = 5, Sunday = 6
    }

    #endregion

    #region Framework

    protected override void Awake()
    {
        base.Awake();
    }

    #endregion

    #region Offset

    public void Offset()
    {
        loading.gameObject.SetActive(true);
        loading.alpha = 1.0f;
        StartCoroutine(Post_ShowNextDayText(1f));
    }

    #endregion

    #region PassDay Loading

    public void StartLoading()
    {
        GameManager.Instance.CanInput = false;
        StartCoroutine(Past_ShowNextDayText(1f));
    }

    private IEnumerator Past_ShowNextDayText(float time)
    {
        SavingPrograssText.text = "< Saving... >";
        SavingPrograssText.DOFade(0f, 0.4f).SetLoops(-1, LoopType.Yoyo);

        PassDayExplanationText.color = Color.white;
        string TextTemp = "DAY [" + GameManager.Instance.MainInfo.day + "]";

        loading.gameObject.SetActive(true);
        loading.DOFade(1f, 1f);

        SavingPrograssText.text = "Saving...";

        PassDayExplanationText.text = TextTemp;
        yield return new WaitForSeconds(time + 0.5f);

        PassDayExplanationText.DOFade(0, time);

        GameManager.Instance.MainInfo.day++;

        string strName = GameManager.Instance.MainInfo.TodayOfTheWeek;
        int DayOrdinal1 = (int)Enum.Parse(typeof(WeekDays), strName) + 1;
        if (DayOrdinal1 >= 7)
        { GameManager.Instance.MainInfo.TodayOfTheWeek = Enum.GetName(typeof(WeekDays), 0); }
        else { GameManager.Instance.MainInfo.TodayOfTheWeek = Enum.GetName(typeof(WeekDays), DayOrdinal1); }

        StartCoroutine(Post_ShowNextDayText(1f));
    }

    private IEnumerator Post_ShowNextDayText(float time)
    {
        GameManager.Instance.CanInput = false;

        DOTween.Kill(SavingPrograssText);
        GameSystem.Instance.SetPlayerTransform();

        SavingPrograssText.text = "< Saved >";
        SavingPrograssText.DOFade(1f, 1f);

        //Debug.Log("사전 조사 데이터 세팅");
        //PreliminarySurveyManager.ft_setAPSSOs();

        dayText.text = GameManager.Instance.MainInfo.day.ToString();

        //PlaceManager.Instance.InitPlace();

        PassDayExplanationText.color = Color.white;

        yield return new WaitForSeconds(time);

        PassDayExplanationText.text = "";
        PassDayExplanationText.color = Color.white;
        string TextTemp = "DAY [" + GameManager.Instance.MainInfo.day + "]";
        PassDayExplanationText.DOText(TextTemp, time);
        yield return new WaitForSeconds(time * 2);
        PassDayExplanationText.DOFade(0, time);
        yield return new WaitForSeconds(time);

        EndLoading();
    }

    private void EndLoading()
    {
        //ScheduleManager.ResetDay();
        Sequence loadingSequence = DOTween.Sequence();
        loadingSequence.Append(loading.DOFade(0f, 1f))
        .OnComplete(() =>
        {
            loading.gameObject.SetActive(false);
            GameManager.Instance.CanInput = true;
            PlayerInputController.Instance.CanMove = true;
        });
    }

    #endregion

}
