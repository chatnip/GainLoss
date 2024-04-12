using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;
using DG.Tweening;
using System;
using System.Linq;

public class PhoneSoftware : MonoBehaviour, IInteract
{
    #region Value

    [Header("*Manager")]
    [SerializeField] GameManager GameManager;
    [SerializeField] ScheduleManager ScheduleManager;
    [SerializeField] PhoneHardware PhoneHardware;
    [SerializeField] SchedulePrograss SchedulePrograss;
    [SerializeField] PlayerInputController PlayerInputController;
    [SerializeField] PlaceManager PlaceManager;
    [SerializeField] PartTimeJobManager PartTimeJobManager;
    [SerializeField] PreliminarySurveyManager PreliminarySurveyManager;
    [SerializeField] TutorialManager TutorialManager;

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

    [Header("*Site Survey")]
    [SerializeField] List<Button> PlaceBtns;
    [SerializeField] GameObject map;

    [Header("*Everytime Set Text")]
    [SerializeField] List<TMP_Text> DayText;
    [SerializeField] List<TMP_Text> DayOfWeekText;
    [SerializeField] List<TMP_Text> realTimeText;
    [SerializeField] TMP_Text EventText;

    [Header("*Software")]
    [SerializeField] Button lockScreen;

    [Header("*AppEffecful")]
    [SerializeField] Image appOpenBackgroundImg;
    [SerializeField] Image appIconImg;

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

    #endregion

    #region Main
    private void Awake()
    {
        ft_setEventText(EventText, "♡♥ 마린과의 첫만남 ♥♡");

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
                AIL_pad_Btn.TryGetComponent(out Image img);
                openApp(AIL_pad, img);
                //AIL_pad.SetActive(true);
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
                EXE_pad_Btn.TryGetComponent(out Image img);
                openApp(EXE_pad, img);
                //EXE_pad.SetActive(true);
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
                Place_pad_Btn.TryGetComponent(out Image img);
                openApp(Place_pad, img);
                //Place_pad.SetActive(true);
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
            { ClearSelectedScheduleFirstBtns(); });
        SelectedScheduleBtns[1].OnClickAsObservable()
            .Subscribe(btn =>
            { ClearSelectedScheduleSecondBtns(); });

        DecisionBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                Decision();
            });

        #endregion

        #region Site Survey

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
    }

    private void OnEnable()
    {
        PlayerInputController.interact = this;
        ResetUI();
    }

    private void LateUpdate()
    {
        ft_setRealTime(realTimeText);
    }

    public void Interact()
    {
        #region base

        if (PlayerInputController.SelectBtn == AIL_pad_Btn) 
        {
            AIL_pad_Btn.TryGetComponent(out Image img);
            openApp(AIL_pad, img);
            return; 
        }
        else if (PlayerInputController.SelectBtn == EXE_pad_Btn) 
        {
            EXE_pad_Btn.TryGetComponent(out Image img);
            openApp(EXE_pad, img);
            return; 
        }
        else if (PlayerInputController.SelectBtn == Place_pad_Btn) 
        {
            Place_pad_Btn.TryGetComponent(out Image img);
            openApp(Place_pad, img);
            return; 
        }

        #endregion Set Create Schedule Btn

        #region  Set Create Schedule Btn

        if (PlayerInputController.SelectBtn == WatchingTheStreamingBtn) 
        { InputSchedule(WatchingTheStreamingBtn, WatchingTheStreamingText); return; }
        else if (PlayerInputController.SelectBtn == SiteSurveyBtn) 
        { InputSchedule(SiteSurveyBtn, SiteSurveyText); return; }
        else if (PlayerInputController.SelectBtn == PreliminarySurveyBtn) 
        { InputSchedule(PreliminarySurveyBtn, PreliminarySurveyText); return; }
        else if (PlayerInputController.SelectBtn == DoingPartTimeJobBtn)
        { InputSchedule(DoingPartTimeJobBtn, DoingPartTimeJobText); return; }
        else if (PlayerInputController.SelectBtn == SelectedScheduleBtns[0])
        { ClearSelectedScheduleFirstBtns(); return; }
        else if (PlayerInputController.SelectBtn == SelectedScheduleBtns[1])
        { ClearSelectedScheduleSecondBtns(); return; }
        else if (PlayerInputController.SelectBtn == DecisionBtn)
        { Decision(); return; }

        #endregion

        #region Site Survey

        if (PlayerInputController.SelectBtn.gameObject.name == "Home")
        { PhoneHardware.PhoneOff(); PlayerInputController.ClearSeletedBtns(); return; }
        else
        { PlaceManager.SetPlaceBtnSet(PlayerInputController.SelectBtn.GetComponent<IDBtn>().buttonValue); PlayerInputController.ClearSeletedBtns(); return; }

        #endregion
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
    private void ClearSelectedScheduleFirstBtns()
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
    }
    private void ClearSelectedScheduleSecondBtns()
    {
        if (SelectedScheduleTexts[1].text != "" && SelectedScheduleTexts[1].text != null)
        {
            SelectedScheduleTexts[1].text = null;
            Turn--;
        }
    }
    private void Decision()
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
            foreach (string ScheduleString in ScheduleStrings)
            {
                Ids.Add((string)DataManager.ScheduleDatas[3].FirstOrDefault(x => (string)x.Value == ScheduleString).Key);
            }

            ScheduleManager.currentSelectedScheduleID = Ids;
            ScheduleManager.PassBtnOn(); 
            //ScheduleManager.currentPrograssScheduleID = ScheduleManager.currentSelectedScheduleID[0];
            //SchedulePrograss.Set_InAMScheduleUI();
            //TutorialManager.OpenTutorialWindow(ScheduleManager.currentPrograssScheduleID);
            //ScheduleManager.SetDotweenGuide();
            CreateScheduleGO.SetActive(false);
            PhoneHardware.PhoneOff();
            //SchedulePrograss.SetExplanation(ScheduleManager.currentPrograssScheduleID);
            //PartTimeJobManager.distinctionPartTimeJob();
        }
        else
        {
            DOTween.Kill(DecisionWarningText);
            DecisionWarningText.color = Color.white;
            DecisionWarningText.DOFade(0, 1);
        }
    }

    #endregion

    #region Reset

    public void ResetUI()
    {
        #region LockScreen

        //lockScreen.gameObject.SetActive(true);

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

            Debug.Log("사전조사 남았는지 판별");
            int remainPS = PreliminarySurveyManager.PSSOs_FindClue_Available.Count +
                PreliminarySurveyManager.PSSOs_Extract_Available.Count;
            if (remainPS == 0)
            {
                PreliminarySurveyBtn.interactable = false;
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

    public void SetCurrentScheduleUI(bool IsThisSchedule)
    {
        List<List<Button>> btns = new List<List<Button>>();
        if (!IsThisSchedule)
        {
            btns = new List<List<Button>>
            { new List<Button> { AIL_pad_Btn, EXE_pad_Btn, Place_pad_Btn } };
            PlayerInputController.SetSectionBtns(btns, this);
        }
        else 
        {
            string id = ScheduleManager.currentPrograssScheduleID;
            switch (id)
            {
                case "S00":
                    CreateScheduleGO.SetActive(true);
                    btns = new List<List<Button>>()
                    { 
                        new List<Button>() { PreliminarySurveyBtn, SiteSurveyBtn, WatchingTheStreamingBtn, DoingPartTimeJobBtn },
                        new List<Button>() { SelectedScheduleBtns[0] },
                        new List<Button>() { SelectedScheduleBtns[1] },
                        new List<Button>() { DecisionBtn } 
                    };
                    PlayerInputController.SetSectionBtns(btns, this);
                    break;
                case "S02":
                    map.SetActive(true);
                    btns = new List<List<Button>>() { PlaceBtns };
                    PlayerInputController.SetSectionBtns(btns, this);

                    /*foreach(Button btn in PlaceBtns)
                    {
                        if(btn.TryGetComponent(out BasicInteractBtn BIB))
                        {
                            BIB.enabled = false;
                        }
                    }*/
                    
                    Sequence sequence = DOTween.Sequence();
                    sequence.AppendInterval(2f);
                    for(int i = 0; i < PlaceBtns.Count; i++)
                    {
                        PlaceBtns[i].TryGetComponent(out RectTransform BtnRT);
                        PlaceBtns[i].TryGetComponent(out CanvasGroup BtnCG);

                        BtnRT.sizeDelta = Vector2.zero;
                        DOTween.Kill(BtnRT.localScale);
                        BtnCG.alpha = 0.0f;

                        sequence.Append(BtnRT.DOSizeDelta(Vector2.one * 300, 0.2f)
                            .SetEase(Ease.OutBack));
                        sequence.Join(BtnCG.DOFade(1, 0.2f));
                    }

                    /*sequence.OnComplete(() =>
                    {
                        foreach (Button btn in PlaceBtns)
                        {
                            if (btn.TryGetComponent(out BasicInteractBtn BIB))
                            {
                                BIB.enabled = true;
                            }
                        }
                    });*/

                    break;
            }
        }
        
    }

    #endregion

    #region Effectful ( App Open )

    private void openApp(GameObject App, Image ClickImg)
    {
        appOpenBackgroundImg.TryGetComponent(out RectTransform BGRT);
        Sequence seq = DOTween.Sequence();

        // BG 초기 설정
        DOTween.Kill(appOpenBackgroundImg.gameObject);
        BGRT.localScale = Vector3.zero;
        appOpenBackgroundImg.DOFade(0, 0);

        // Icon 초기 설정
        appIconImg.DOFade(0, 0);
        appIconImg.sprite = ClickImg.sprite;

        // Dotween 애니메이션
        appOpenBackgroundImg.gameObject.SetActive(true);
        appIconImg.gameObject.SetActive(true);

        seq.Append(BGRT.DOScale(Vector3.one, 0.2f)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                App.gameObject.SetActive(true);
            }));
        seq.Join(appOpenBackgroundImg.DOFade(1, 0.2f));
        seq.Join(appIconImg.DOFade(1, 0.1f));

        seq.AppendInterval(0.2f);

        seq.Append(appOpenBackgroundImg.DOFade(0, 0.15f)
            .OnComplete(() =>
            {
                appOpenBackgroundImg.gameObject.SetActive(false);
            }));
        seq.Join(appIconImg.DOFade(0, 0.15f)
            .OnComplete(() =>
            {
                appIconImg.gameObject.SetActive(false);
            }));


    }

    #endregion

    #region Real Time

    private void ft_setRealTime(List<TMP_Text> allTimeTmp)
    {
        foreach (TMP_Text timeTmp in allTimeTmp)
        {
            if(timeTmp.text != ft_getRealTimeString_H_M()) 
            { timeTmp.text = ft_getRealTimeString_H_M(); }
            
        }
    }
    private string ft_getRealTimeString_H_M()
    {
        string AllRealTime = DateTime.Now.ToString(("yyyy-MM-dd HH:mm:ss tt"));
        string H = DateTime.Now.ToString(("HH"));
        string m = DateTime.Now.ToString(("mm"));

        return H + ":" + m;
    }

    #endregion

    #region Event Text

    private void ft_setEventText(TMP_Text targetComp, string Text)
    {
        string TextSet = "<size=50%>" + Text + "</size>\n";
        string TextStaticDay = "<size=100%>" + "20XX.XX.XX" + "</size>";

        targetComp.text = TextSet + TextStaticDay;       
    }


    #endregion
}
