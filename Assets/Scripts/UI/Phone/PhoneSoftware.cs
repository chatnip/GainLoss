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
    [SerializeField] ScheduleManager ScheduleManager;
    [SerializeField] PhoneHardware PhoneHardware;
    [SerializeField] SchedulePrograss SchedulePrograss;

    [Header("*Create Schedule")]
    [SerializeField] GameObject CreateScheduleGO;
    [SerializeField] GameObject ScheculeBtnGO;

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

    [Header("*Everytime Set Text")]
    [SerializeField] List<TMP_Text> DayText;
    [SerializeField] List<TMP_Text> DayOfWeekText;

    [Header("*Software")]
    [SerializeField] Button lockScreen;
    [SerializeField] public Button mapBtn;
    [SerializeField] GameObject map;
    [SerializeField] Button AIL_padBtn;
    [SerializeField] GameObject AIL_pad;
    [SerializeField] Button EXE_padBtn;
    [SerializeField] GameObject EXE_pad;
    [SerializeField] Button backBtn;


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
                    DOTween.Kill(DecisionWarningText);
                    List<string> ScheduleStrings = new List<string>
                    {
                        SelectedScheduleTexts[0].name,
                        SelectedScheduleTexts[1].name
                    };

                    ScheduleManager.currentSelectedSchedule = ScheduleStrings;
                    ScheduleManager.currentPrograssSchedule = ScheduleManager.currentSelectedSchedule[0];
                    SchedulePrograss.Set_InAMScheduleUI();
                    CreateScheduleGO.SetActive(false);
                    PhoneHardware.PhoneOff();
                }
                else
                {
                    DOTween.Kill(DecisionWarningText);
                    DecisionWarningText.color = Color.white;
                    DecisionWarningText.DOFade(0, 1);
                }
            });

        #endregion

        #region Base
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

        AIL_padBtn
            .OnClickAsObservable()
            .Subscribe(btn =>
            {
                AIL_pad.SetActive(true);
            });
        EXE_padBtn
            .OnClickAsObservable()
            .Subscribe(btn =>
            {
                EXE_pad.SetActive(true);
            });

        backBtn
            .OnClickAsObservable()
            .Subscribe(btn =>
            {
                map.SetActive(false);
                AIL_pad.SetActive(false);
                if (!map.activeSelf && !AIL_pad.activeSelf)
                { PhoneHardware.PhoneOff(); }
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

        #region Schedule
        
        //Create
        if(ScheduleManager.currentSelectedSchedule.Count <= 0)
        {
            Turn = 0;

            CreateScheduleGO.SetActive(true);
            SchedulePrograss.Set_InStartScheduleUI();

            Transform[] allChildren = ScheculeBtnGO.GetComponentsInChildren<Transform>();
            foreach (Transform child in allChildren)
            {
                foreach (string CanBehavior in ScheduleManager.currentHaveSchedule)
                {
                    if (CanBehavior == child.name)
                    {
                        if (child.TryGetComponent(out Button btn))
                        {
                            btn.interactable = true;
                        }
                    }
                }
            }

            foreach (TMP_Text text in SelectedScheduleTexts)
            {
                text.gameObject.name = null;
                text.text = null;
            }
        }

        




        #endregion

        #region Everytime

        foreach (TMP_Text txt in DayText)
        {
            txt.text = "DAY " + GameManager.currentMainInfo.day;
        }
        foreach (TMP_Text txt in DayOfWeekText)
        {
            txt.text = GameManager.currentMainInfo.TodayOfTheWeek;
        }

        //Set
        if (ScheduleManager.currentPrograssSchedule == "SiteSurvey") { mapBtn.interactable = true; }
        else { mapBtn.interactable = false; }
        map.SetActive(false);
        AIL_padBtn.interactable = true;
        AIL_pad.SetActive(false);
        EXE_padBtn.interactable = true;
        EXE_pad.SetActive(false);

        #endregion

    }

    private void OnEnable()
    {
        ResetUI();
    }
}
