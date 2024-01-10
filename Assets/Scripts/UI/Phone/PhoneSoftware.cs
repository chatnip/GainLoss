using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Rendering;
using DG.Tweening;

public class PhoneSoftware : MonoBehaviour
{
    [Header("*Manager")]
    [SerializeField] GameManager GameManager;
    [SerializeField] BehaviorManager BehaviorManager;

    [Header("*Create Schedule")]
    [SerializeField] GameObject CreateScheduleGO;
    [SerializeField] GameObject ScheculeBtnGO;

    [SerializeField] TMP_Text DayText;
    [SerializeField] TMP_Text DayOfWeekText;

    [SerializeField] Button WatchingTheStreamingBtn;
    [SerializeField] TMP_Text WatchingTheStreamingText;
    [SerializeField] Button SiteSurveyBtn;
    [SerializeField] TMP_Text SiteSurveyText;
    [SerializeField] Button PreliminarySurveyBtn;
    [SerializeField] TMP_Text PreliminarySurveyText;
    [SerializeField] Button DoingPartTimeJobBtn;
    [SerializeField] TMP_Text DoingPartTimeJobText;

    [SerializeField] Button[] SelectedScheduleBtns;
    [SerializeField] TMP_Text[] SelectedScheduleTexts;
    int Turn;
    Dictionary<string, int> TurnEachButton;

    [SerializeField] Button DecisionBtn;
    [SerializeField] TMP_Text DecisionWarningText;

    [Header("*Software")]
    [SerializeField] Button lockScreen;
    [SerializeField] public Button mapBtn;
    [SerializeField] GameObject map;
    [SerializeField] Button wordpadBtn;
    [SerializeField] GameObject wordpad;
    [SerializeField] Button backBtn;

    [Header("*Day")]
    [SerializeField] TMP_Text MapBtnDay;

    private void Awake()
    {
        #region Set Create Schedule Btn

        Turn = 0;
        TurnEachButton = new Dictionary<string, int>()
        { 
            { PreliminarySurveyBtn.name, 1},
            { SiteSurveyBtn.name, 2},
            { DoingPartTimeJobBtn.name, 3},
            { WatchingTheStreamingBtn.name, 4}
        };

        WatchingTheStreamingBtn.OnClickAsObservable()
            .Subscribe(btn =>
            { InputSchedule(WatchingTheStreamingBtn, WatchingTheStreamingText); });
        SiteSurveyBtn.OnClickAsObservable()
            .Subscribe(btn =>
            { InputSchedule(SiteSurveyBtn, SiteSurveyText); });
        PreliminarySurveyBtn.OnClickAsObservable()
            .Subscribe(btn =>
            { InputSchedule(PreliminarySurveyBtn, PreliminarySurveyText); });
        DoingPartTimeJobBtn.OnClickAsObservable()
            .Subscribe(btn =>
            { InputSchedule(DoingPartTimeJobBtn, DoingPartTimeJobText); });

        SelectedScheduleBtns[0].OnClickAsObservable()
            .Subscribe(btn =>
            {
                if (SelectedScheduleTexts[0].text != "" && SelectedScheduleTexts[0].text != null)
                {
                    SelectedScheduleTexts[0].text = null;
                    SelectedScheduleTexts[0].gameObject.name = null;
                    Turn--;
                }
                if(SelectedScheduleTexts[1].text != "" && SelectedScheduleTexts[1].text != null)
                {
                    string s_temp = SelectedScheduleTexts[1].text;
                    SelectedScheduleTexts[0].text = s_temp;
                    SelectedScheduleTexts[1].text = null;

                    s_temp = SelectedScheduleTexts[1].gameObject.name;
                    SelectedScheduleTexts[0].gameObject.name = s_temp;
                    SelectedScheduleTexts[1].gameObject.name = null;
                }
            });
        SelectedScheduleBtns[1].OnClickAsObservable()
            .Subscribe(btn =>
            {
                if (SelectedScheduleTexts[1].text != "" && SelectedScheduleTexts[1].text != null)
                {
                    SelectedScheduleTexts[1].text = null;
                    SelectedScheduleTexts[1].gameObject.name = null;
                    Turn--;
                }
            });

        DecisionBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                if (SelectedScheduleTexts[1].gameObject.name != null && SelectedScheduleTexts[1].gameObject.name != "")
                {
                    List<string> s_Temp = new List<string>
                    {
                        SelectedScheduleTexts[0].name,
                        SelectedScheduleTexts[1].name
                    };
                    BehaviorManager.currentSelectedBehaviors = s_Temp;
                    foreach (string name in s_Temp)
                    {
                        Debug.Log("할 일 정함: " + name);
                    }
                    CreateScheduleGO.SetActive(false);

                }
                else
                {
                    DOTween.Kill(DecisionWarningText);
                    DecisionWarningText.color = Color.white;
                    DecisionWarningText.DOFade(0, 1);
                }
            });

        #endregion

        #region Past
        lockScreen
            .OnClickAsObservable()
            .Subscribe(btn =>
            {
                lockScreen.gameObject.SetActive(false);
            });

        mapBtn
            .OnClickAsObservable()
            .Subscribe(btn =>
            {
                map.SetActive(true);
            });

        wordpadBtn
            .OnClickAsObservable()
            .Subscribe(btn =>
            {
                wordpad.SetActive(true);
            });
        
        backBtn
            .OnClickAsObservable()
            .Subscribe(btn =>
            {
                map.SetActive(false);
                wordpad.SetActive(false);
            });
        #endregion
    }

    #region About Create Schedule

    private void InputSchedule(Button ScheduleBtn, TMP_Text tmp)
    {
        string ScheduleText = tmp.text;
        if (Turn == 0)
        {
            if (SelectedScheduleTexts[Turn].text == null || SelectedScheduleTexts[Turn].text == "")
            {
                SelectedScheduleTexts[Turn].text = ScheduleText;
                SelectedScheduleTexts[Turn].gameObject.name = ScheduleBtn.gameObject.name;
                Turn++;
            }
        }
        else if(Turn == 1)
        {
            string FirstText = SelectedScheduleTexts[Turn - 1].text;
            if ((SelectedScheduleTexts[Turn].text == null || SelectedScheduleTexts[Turn].text == "") &&
                (FirstText != ScheduleText))
            {
                SelectedScheduleTexts[Turn].text = ScheduleText;
                SelectedScheduleTexts[Turn].gameObject.name = ScheduleBtn.gameObject.name;
                Turn++;
            }
        }
        if(Turn >= 2)
        {
            SetOrdering(SelectedScheduleTexts[0].gameObject.name, SelectedScheduleTexts[1].gameObject.name);
        }
    }
    private void SetOrdering(string firstBtn, string secondBtn)
    {
        if(TurnEachButton[firstBtn] > TurnEachButton[secondBtn])
        {
            string Temp = SelectedScheduleTexts[0].text;
            SelectedScheduleTexts[0].text = SelectedScheduleTexts[1].text;
            SelectedScheduleTexts[1].text = Temp;

            string Temp2 = SelectedScheduleTexts[0].gameObject.name;
            SelectedScheduleTexts[0].gameObject.name = SelectedScheduleTexts[1].gameObject.name;
            SelectedScheduleTexts[1].gameObject.name = Temp2;
        }
    }

    #endregion

    public void ResetUI()
    {
        #region LockScreen

        lockScreen.gameObject.SetActive(true);

        #endregion

        #region CreateSchedule

        Turn = 0;
        CreateScheduleGO.SetActive(true);

        Transform[] allChildren = ScheculeBtnGO.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            foreach(string CanBehavior in BehaviorManager.currentHaveBehaviors)
            {
                if(CanBehavior == child.name)
                {
                    if(child.TryGetComponent(out Button btn))
                    {
                        btn.interactable = true;
                    }
                }
            }
        }

        DayText.text = "DAY " + GameManager.currentMainInfo.day;
        DayOfWeekText.text = GameManager.currentMainInfo.TodayOfTheWeek;

        foreach(TMP_Text text in SelectedScheduleTexts)
        {
            text.gameObject.name = null;
            text.text = null; 
        }
        

        

        #endregion

        #region Past

        mapBtn.interactable = true;
        map.SetActive(false);
        wordpadBtn.interactable = true;
        wordpad.SetActive(false);

        MapBtnDay.text = GameManager.currentMainInfo.day + " Day";

        #endregion

    }

    private void OnEnable()
    {
        ResetUI();
    }
}
