using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class LoadingManager : Singleton<LoadingManager>
{
    #region Value

    [Header("=== LoadingWindow")]
    [SerializeField] TMP_Text passDayExplanationText;
    [SerializeField] TMP_Text savingPrograssText;
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

    #region Framework & Base Set
    public void Offset()
    {
        loading.gameObject.SetActive(true);
        loading.alpha = 1.0f;
        StartCoroutine(Post_ShowNextDayText(1f));
    }

    protected override void Awake()
    {
        base.Awake();
    }

    #endregion

    #region PassDay Loading

    public void StartLoading()
    {
        GameSystem.Instance.canInput = false;
        PlayerInputController.Instance.MoveStop();
        PlayerController.Instance.ResetAnime();
        StartCoroutine(Past_ShowNextDayText(1f));
    }

    private IEnumerator Past_ShowNextDayText(float time)
    {
        savingPrograssText.text = "< Saving... >";
        savingPrograssText.DOFade(0f, 0.4f).SetLoops(-1, LoopType.Yoyo);

        passDayExplanationText.color = Color.white;
        string TextTemp = "DAY [" + GameSystem.Instance.mainInfo.Day + "]";

        loading.gameObject.SetActive(true);
        loading.DOFade(1f, 1f);

        savingPrograssText.text = "Saving...";

        passDayExplanationText.text = TextTemp;
        yield return new WaitForSeconds(time + 0.5f);

        passDayExplanationText.DOFade(0, time);

        GameSystem.Instance.mainInfo.Day++;

        string strName = GameSystem.Instance.mainInfo.TodayOfTheWeek;
        int DayOrdinal1 = (int)Enum.Parse(typeof(WeekDays), strName) + 1;
        if (DayOrdinal1 >= 7)
        { GameSystem.Instance.mainInfo.TodayOfTheWeek = Enum.GetName(typeof(WeekDays), 0); }
        else { GameSystem.Instance.mainInfo.TodayOfTheWeek = Enum.GetName(typeof(WeekDays), DayOrdinal1); }

        StartCoroutine(Post_ShowNextDayText(1f));
    }

    private IEnumerator Post_ShowNextDayText(float time)
    {
        GameSystem.Instance.canInput = false;
        GameSystem.Instance.mainInfo.CurrentActivity = DataManager.Instance.Get_GiveActivity(GameManager.Instance.currentChapter);

        
        ActivityController.Instance.SetActivityGageUI(0f);
        

        DOTween.Kill(savingPrograssText);
        PlayerController.Instance.gameObject.transform.position = new Vector3(0f, 0f, 0f);
        PlayerController.Instance.gameObject.transform.rotation = Quaternion.identity;

        savingPrograssText.text = "< Saved >";
        savingPrograssText.DOFade(1f, 1f);

        dayText.text = GameSystem.Instance.mainInfo.Day.ToString();

        passDayExplanationText.color = Color.white;

        yield return new WaitForSeconds(time);

        passDayExplanationText.text = "";
        passDayExplanationText.color = Color.white;
        string TextTemp = "DAY [" + GameSystem.Instance.mainInfo.Day + "]";
        passDayExplanationText.DOText(TextTemp, time);
        yield return new WaitForSeconds(time * 2);
        passDayExplanationText.DOFade(0, time);
        yield return new WaitForSeconds(time);

        EndLoading();
    }

    private void EndLoading()
    {
        Sequence loadingSequence = DOTween.Sequence();
        loadingSequence.Append(loading.DOFade(0f, 1f))
        .OnComplete(() =>
        {
            loading.gameObject.SetActive(false);
            GameSystem.Instance.canInput = true;
            PlayerInputController.Instance.CanMove = true;

            if (DataManager.Instance.Get_ChapterStartDay(GameManager.Instance.currentChapter) == GameSystem.Instance.mainInfo.Day)
            {
                string startDialogID = DataManager.Instance.Get_StartDialog(GameManager.Instance.currentChapter);
                if (startDialogID != "")
                { DialogManager.Instance.ObjDescOn(null, startDialogID, true); }
            }
            else if (DataManager.Instance.Get_ChapterEndDay(GameManager.Instance.currentChapter) == GameSystem.Instance.mainInfo.Day)
            {
                GameSystem.Instance.SeteCurrentActPart(GameSystem.e_currentActPart.ReasoningDay);
            }
            else
            {
                GameSystem.Instance.SeteCurrentActPart(GameSystem.e_currentActPart.UseActivity);
            }
        });
    }

    #endregion

}
