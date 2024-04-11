using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class ActionEventManager : Manager<ActionEventManager>
{
    [Header("*Property")]
    [SerializeField] GameManager GameManager;
    [SerializeField] GameSystem GameSystem;
    [SerializeField] JsonManager JsonManager;
    [SerializeField] ScheduleManager ScheduleManager;
    [SerializeField] PreliminarySurveyManager PreliminarySurveyManager;
    [SerializeField] SchedulePrograss SchedulePrograss;
    [SerializeField] PlaceManager PlaceManager;
    [SerializeField] TutorialManager TutorialManager;

    [Header("*Input")]
    [SerializeField] PlayerInputController PlayerInputController;


    [Header("*LoadingWindow")]
    [SerializeField] TMP_Text PassDayExplanationText;
    [SerializeField] TMP_Text SavingPrograssText;
    [SerializeField] CanvasGroup loading;

    [Header("*UI")]
    [SerializeField] TMP_Text moneyText;
    [SerializeField] TMP_Text dayText;

    [Header("*")]
    [SerializeField] CanvasGroup EndBlackScreenCG;
    [SerializeField] TMP_Text EndTitleTxt;
    [SerializeField] TMP_Text EndContentTxt;
    [SerializeField] TMP_Text EndCreditText;


    /*[Header("*Place")]
    [HideInInspector] public string currentActionEventID;
    [SerializeField] List<PlaceDataBase> placeList = new();
    [SerializeField] Transform placeParent;
    [SerializeField] GameObject currentPlace;
    [SerializeField] GameObject home;
    */

    //[SerializeField] private ReactiveProperty<PlaceDataBase> placeData = new ReactiveProperty<PlaceDataBase>();

    enum WeekDays
    {
        Monday = 0, Tuesday = 1, Wednesday = 2, Thursday = 3, Friday = 4, Saturday = 5, Sunday = 6
    }

    protected override void Awake()
    {
        loading.gameObject.SetActive(true);
        loading.alpha = 1.0f;
        StartCoroutine(Post_ShowNextDayText(1f));

        
    }

    #region PassDay Loading

    public void TurnOnLoading()
    {
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

        #region Test Version
        Debug.Log("30게이지 넘는지 판별하는 구간 (언리엑세스 버젼)");
        if(GameManager.currentMainInfo.overloadGage >= 30) 
        { ft_EndGameForBetaVersion(); }
        #endregion

        setMainUI();
        PlaceManager.InitPlace();

        PassDayExplanationText.color = Color.white;

        yield return new WaitForSeconds(time);

        //SchedulePrograss.SetExplanation("S00");
        ScheduleManager.ResetDay();
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
        //Day++
        GameManager.currentMainInfo.day++;

        //Next Day of the Week
        string strName = GameManager.currentMainInfo.TodayOfTheWeek;
        int DayOrdinal1 = (int)Enum.Parse(typeof(WeekDays), strName) + 1;
        if( DayOrdinal1 >= 7 )
        { GameManager.currentMainInfo.TodayOfTheWeek = Enum.GetName(typeof(WeekDays), 0); }
        else { GameManager.currentMainInfo.TodayOfTheWeek = Enum.GetName(typeof(WeekDays), DayOrdinal1); }

        /*// UI
        foreach (TMP_Text tmpText in DayUI)
        { tmpText.text = "Day " + GameManager.currentMainInfo.day; }
        foreach (TMP_Text tmpText in DayOfWeekUI)
        { tmpText.text = GameManager.currentMainInfo.TodayOfTheWeek; }*/

    }

    private void EndLoading()
    {
        Sequence loadingSequence = DOTween.Sequence();
        loadingSequence.Append(loading.DOFade(0f, 1f))
        .OnComplete(() =>
        {
            loading.gameObject.SetActive(false);
            if( !TutorialManager.tutorial_ScreenCG.gameObject.activeSelf ) { PlayerInputController.CanMove = true; }
            
            //GameSystem.GameStart();
        });
    }

    private void setMainUI()
    {
        moneyText.text = GameManager.currentMainInfo.money.ToString();
        dayText.text = GameManager.currentMainInfo.day.ToString();
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


        GameSystem.SetPlayerTransform();

        SavingPrograssText.text = "< Saved >";
        SavingPrograssText.DOFade(1f, 1f);


        Debug.Log("사전 조사 데이터 세팅");
        PreliminarySurveyManager.ft_setAPSSOs();
    }

    #endregion

    private void ft_EndGameForBetaVersion()
    {
        StopAllCoroutines();

        Sequence seq = DOTween.Sequence();

        EndBlackScreenCG.gameObject.SetActive(true);
        EndBlackScreenCG.alpha = 0f;
        seq.Append(EndBlackScreenCG.DOFade(1f, 1f));

        seq.AppendInterval(3f);

        seq.Append(EndContentTxt.DOFade(0f, 1f));
        seq.Join(EndTitleTxt.DOFade(0f, 1f));

        EndCreditText.TryGetComponent(out RectTransform CRT);
        CRT.anchoredPosition = new Vector2(0f, -1250f);
        seq.Append(CRT.DOAnchorPos(new Vector2(0f, 1250f), 10f));

        seq.AppendInterval(3f);

        seq.OnComplete(() =>
        {

            UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
        });
    }
}
