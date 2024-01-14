using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Rendering;
using DG.Tweening;
using System;
using System.Security.Cryptography;
using System.Linq;

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

    [SerializeField] Button DecisionBtn;
    [SerializeField] TMP_Text DecisionWarningText;

    [Header("*Everytime Set Text")]
    [SerializeField] List<TMP_Text> DayText;
    [SerializeField] List<TMP_Text> DayOfWeekText;

    [Header("*Software")]
    [SerializeField] Button lockScreen;
    //[SerializeField] public Button map_Btn;
    //[SerializeField] public Button map_backBtn;
    [SerializeField] GameObject map;
    [Header("*ail")]
    [SerializeField] Button AIL_pad_Btn;
    [SerializeField] Button AIL_pad_backBtn;
    [SerializeField] GameObject AIL_pad;
    [Header("*exe")]
    [SerializeField] Button EXE_pad_Btn;
    [SerializeField] Button EXE_pad_backBtn;
    [SerializeField] GameObject EXE_pad;
    [Header("*place")]
    [SerializeField] Button Place_pad_Btn;
    [SerializeField] Button Place_pad_backBtn;
    [SerializeField] GameObject Place_pad;
    [Header("*Allback")]
    [SerializeField] Button backBtn;

    #region Main
    private void Awake()
    {
        #region Set Create Schedule Btn

        Turn = 0;

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
                    Turn--;
                }
                if (SelectedScheduleTexts[1].text != "" && SelectedScheduleTexts[1].text != null)
                {
                    string s_temp = SelectedScheduleTexts[1].text;
                    SelectedScheduleTexts[0].text = s_temp;
                    SelectedScheduleTexts[1].text = null;
                }
            });
        SelectedScheduleBtns[1].OnClickAsObservable()
            .Subscribe(btn =>
            {
                if (SelectedScheduleTexts[1].text != "" && SelectedScheduleTexts[1].text != null)
                {
                    SelectedScheduleTexts[1].text = null;
                    Turn--;
                }
            });

        DecisionBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                if ((SelectedScheduleTexts[0].text != null && SelectedScheduleTexts[0].text != "") &&
                (SelectedScheduleTexts[1].text != null && SelectedScheduleTexts[1].text != ""))
                {
                    DOTween.Kill(DecisionWarningText);
                    List<string> ScheduleStrings = new List<string>
                    {
                        SelectedScheduleTexts[0].text,
                        SelectedScheduleTexts[1].text
                    };
                    List<string> Ids = new List<string>();
                    foreach(string ScheduleString in ScheduleStrings)
                    {
                        Ids.Add((string)DataManager.ScheduleDatas[3].FirstOrDefault(x => (string)x.Value == ScheduleString).Key);
                    }

                    ScheduleManager.currentSelectedScheduleID = Ids;
                    ScheduleManager.currentPrograssScheduleID = ScheduleManager.currentSelectedScheduleID[0];
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

        #region site Survey

        /*map_Btn
            .OnClickAsObservable()
            .Subscribe(btn =>
            {
                map.SetActive(true);
            });

        map_backBtn
            .OnClickAsObservable () 
            .Subscribe(btn =>
            { 
                map.SetActive(false); 
            });*/

        #endregion

        #region Base

        lockScreen
            .OnClickAsObservable()
            .Subscribe(btn =>
            {
                lockScreen.gameObject.SetActive(false);
            });


        AIL_pad_Btn
            .OnClickAsObservable()
            .Subscribe(btn =>
            {
                AIL_pad.SetActive(true);
            });
        AIL_pad_backBtn
            .OnClickAsObservable()
            .Subscribe(btn =>
            {
                AIL_pad.SetActive(false);
            });


        EXE_pad_Btn
            .OnClickAsObservable()
            .Subscribe(btn =>
            {
                EXE_pad.SetActive(true);
            });
        EXE_pad_backBtn
            .OnClickAsObservable()
            .Subscribe(btn =>
            {
                EXE_pad.SetActive(false);
            });

        Place_pad_Btn
            .OnClickAsObservable()
            .Subscribe(btn =>
            {
                Place_pad.SetActive(true);
            });
        Place_pad_backBtn
            .OnClickAsObservable()
            .Subscribe(btn =>
            {
                Place_pad.SetActive(false);
            });

        backBtn
            .OnClickAsObservable()
            .Subscribe(btn =>
            {
                PhoneHardware.PhoneOff();
            });
        #endregion
    }
    private void OnEnable()
    {
        ResetUI();
    }
    #endregion

    #region About Create Schedule

    private void InputSchedule(Button ScheduleBtn, TMP_Text tmp)
    {
        string ScheduleText = tmp.text;
        if (Turn == 0)
        {
            SelectedScheduleTexts[0].text = ScheduleText;
            Turn++;
        }
        else if (Turn == 1)
        {
            if (SelectedScheduleTexts[0].text != ScheduleText)
            {
                SelectedScheduleTexts[1].text = ScheduleText;
                Turn++;
            }
        }
        if (Turn >= 2)
        {
            SetOrdering(SelectedScheduleTexts[0].text, SelectedScheduleTexts[1].text);
        }
    }
    private void SetOrdering(string firstText, string secondText)
    {
        string firstKey = "";
        string SecondKey = "";
        foreach (string Kor in DataManager.ScheduleDatas[3].Values)
        {
            if (firstText == Kor)
            { firstKey = DataManager.ScheduleDatas[3].FirstOrDefault(x => (string)x.Value == Kor).Key; }
            if (secondText == Kor)
            { SecondKey = DataManager.ScheduleDatas[3].FirstOrDefault(x => (string)x.Value == Kor).Key; }
        }
        if (Convert.ToInt32(DataManager.ScheduleDatas[1][firstKey]) > Convert.ToInt32(DataManager.ScheduleDatas[1][SecondKey]))
        {
            string temp = SelectedScheduleTexts[0].text;
            SelectedScheduleTexts[0].text = SelectedScheduleTexts[1].text;
            SelectedScheduleTexts[1].text = temp;
        }


    }

    #endregion

    #region Reset

    public void ResetUI()
    {
        #region LockScreen

        lockScreen.gameObject.SetActive(true);

        #endregion

        #region Schedule

        //Create
        if (ScheduleManager.currentSelectedScheduleID.Count <= 0)
        {
            Turn = 0;
            //CreateScheduleGO.SetActive(true);

            //SchedulePrograss.Set_InStartScheduleUI();

            for (int i = 0; i < ScheculeBtnGO.transform.childCount; i++)
            {
                Button BtnGO = ScheculeBtnGO.transform.GetChild(i).GetComponent<Button>();
                foreach (string CanScheduleID in ScheduleManager.currentHaveScheduleID)
                {
                    string Kor = DataManager.ScheduleDatas[3][CanScheduleID].ToString();
                    if (BtnGO.transform.GetChild(0).TryGetComponent(out TMP_Text tmp_Text))
                    {
                        if (tmp_Text.text == Kor)
                        {
                            BtnGO.interactable = true;
                        }
                    }
                }
            }

            foreach (TMP_Text text in SelectedScheduleTexts)
            {
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
        /*if (ScheduleManager.currentPrograssScheduleID == "S02") { map_Btn.interactable = true; }
        else { map_Btn.interactable = false; }*/
        CreateScheduleGO.SetActive(false);
        map.SetActive(false);

        AIL_pad_Btn.interactable = true;
        AIL_pad.SetActive(false);

        EXE_pad_Btn.interactable = true;
        EXE_pad.SetActive(false);
         
        Place_pad_Btn.interactable = true;
        Place_pad.SetActive(false);

        #endregion

    }

    public void SetCurrentScheduleUI()
    {
        string id = ScheduleManager.currentPrograssScheduleID;
        switch (id)
        {
            case "S00":
                CreateScheduleGO.SetActive(true); 
                break;
            case "S02":
                map.SetActive(true);
                break;
        }
    }

    #endregion

}
