using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;
using DG.Tweening;
using System;

public class Desktop : MonoBehaviour, IInteract
{
    #region Value

    [Header("*Manager")]
    [SerializeField] PlayerInputController PlayerInputController;
    [SerializeField] DataManager DataManager;
    [SerializeField] WordManager WordManager;
    [SerializeField] ActionEventManager ActionEventManager;
    [SerializeField] StreamManager StreamManager;
    [SerializeField] GameSystem GameSystem;
    [SerializeField] ComputerInteract computerInteract;
    [SerializeField] ScheduleManager ScheduleManager;


    [Header("*Public")]
    [SerializeField] Image BlackScreen;

    [Header("*SNS")]
    [SerializeField] Button snsOpenBtn;
    [SerializeField] Button snsExitBtn;
    [SerializeField] GameObject snsWindow;

    [Header("*FanCafe")]
    [SerializeField] Button fancafeOpenBtn;
    [SerializeField] Button fancafeExitBtn;
    [SerializeField] GameObject fancafeWindow;

    [Header("*Stream")]
    [SerializeField] public Button streamOpenBtn;
    [SerializeField] public GameObject streamWindow;
    [SerializeField] public GameObject resultWindow;
    [SerializeField] public Button streamStartBtn;
    [SerializeField] Button streamEndBtn;

    [Header("*Todo")]
    [SerializeField] Button todoExitBtn;
    [SerializeField] public GameObject todoWindow;

    [Header("*Preliminary Survey")] // = PS
    [SerializeField] Button PSOpenBtn;
    [SerializeField] public GameObject PSWindow;

    [Header("*ConfirmPopup")]
    [SerializeField] DesktopSoftwere desktopSoftwere;
    [SerializeField] Button popupExitBtn;
    [SerializeField] Button confirmBtn;
    [SerializeField] TMP_Text confirmText;
    [SerializeField] public GameObject confirmPopup;

    [Header("*WindowFrame")]
    [SerializeField] public float AppearTime;
    [SerializeField] public float AppearStartSize;

    [SerializeField] public float DisappearTime;
    [SerializeField] public float DisappearLastSize;

    IDisposable disposable;

    #endregion

    #region Main

    private void Awake()
    {
        /*
        snsOpenBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                desktopSoftwere = DesktopSoftwere.SNS;
                ConfirmPopupSetting();
            });

        fancafeOpenBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                desktopSoftwere = DesktopSoftwere.FanCafe;
                ConfirmPopupSetting();
            });
        */
        PSOpenBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                desktopSoftwere = DesktopSoftwere.PreliminarySurvey;
                ConfirmPopupSetting();
            });

        streamOpenBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                desktopSoftwere = DesktopSoftwere.Stream;
                ConfirmPopupSetting();
            });

        streamStartBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                StartStream();
            });

        streamEndBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                EndScheduleThis();
            });

        popupExitBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                //confirmPopup.SetActive(false);
                DisappearPopup(confirmPopup);
            });

        todoExitBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                //todoWindow.SetActive(false);
                DisappearPopup(todoWindow);
            });
    }

    public void StartStream()
    {
        EffectfulWindow.DisappearEffectful(todoWindow.GetComponent<RectTransform>(), DisappearTime, DisappearLastSize, Ease.Linear);
        //todoWindow.SetActive(false);

        EffectfulWindow.AppearEffectful(streamWindow.GetComponent<RectTransform>(), AppearTime, AppearStartSize, Ease.Linear);
        //streamWindow.SetActive(true);

        StreamManager.currentStreamEvent = WordManager.currentStreamEvent;
        StreamManager.StartDialog(StreamManager.currentStreamEventID);
    }
    public void EndScheduleThis()
    {
        TurnOff();
        //ActionEventManager.TurnOnLoading();
        computerInteract.StartCoroutine(computerInteract.ScreenZoomOut());
        ScheduleManager.PassBtnOn();
    }
    public void DisappearPopup(GameObject Popup)
    {
        EffectfulWindow.DisappearEffectful(Popup.GetComponent<RectTransform>(), DisappearTime, DisappearLastSize, Ease.Linear);
        PlayerInputController.SetSectionBtns(new List<List<Button>>
        {
            new List<Button> { snsOpenBtn },
            new List<Button> { fancafeOpenBtn },
            new List<Button> { streamOpenBtn },
            new List<Button> { PSOpenBtn }
        }, this);

        if(disposable != null) { disposable.Dispose(); }
    }

    private void OnEnable()
    {
        TurnOn();
    }

    public void Interact()
    {
        #region Open Btns

        if (PlayerInputController.SelectBtn == snsOpenBtn) 
        { return; }
        else if (PlayerInputController.SelectBtn == fancafeOpenBtn) 
        { return; }
        else if(PlayerInputController.SelectBtn == streamOpenBtn) 
        {
            desktopSoftwere = DesktopSoftwere.Stream;
            ConfirmPopupSetting();
            PlayerInputController.SetSectionBtns(new List<List<Button>> { new List<Button> { confirmBtn } }, this);
            return;
        }
        else if(PlayerInputController.SelectBtn == PSOpenBtn)
        {
            desktopSoftwere = DesktopSoftwere.PreliminarySurvey;
            ConfirmPopupSetting();
            PlayerInputController.SetSectionBtns(new List<List<Button>> { new List<Button> { confirmBtn } }, this);
            return;
        }

        #endregion

        #region Confirm Btns

        if(PlayerInputController.SelectBtn == popupExitBtn)
        {
            EffectfulWindow.DisappearEffectful(confirmPopup.GetComponent<RectTransform>(), DisappearTime, DisappearLastSize, Ease.Linear);
            return;
        }
        else if(PlayerInputController.SelectBtn == confirmBtn)
        {
            if (ScheduleManager.currentPrograssScheduleID == "S01")
            {
                confirmPopup.SetActive(false);
                EffectfulWindow.AppearEffectful(PSWindow.GetComponent<RectTransform>(), AppearTime, AppearStartSize, Ease.Linear);
                return;
            }
            else if(ScheduleManager.currentPrograssScheduleID == "S03")
            {
                WordManager.TodoReset();
                WordManager.InitWord();

                confirmPopup.SetActive(false);
                EffectfulWindow.AppearEffectful(todoWindow.GetComponent<RectTransform>(), AppearTime, AppearStartSize, Ease.Linear);
                return;
            }
        }

        #endregion
    }

    #endregion

    #region Confirm

    private void ConfirmPopupSetting()
    {
        switch (desktopSoftwere)
        {
            case DesktopSoftwere.SNS:
                SNSConfirm();
                break;
            case DesktopSoftwere.FanCafe:
                FanCafeConfirm();
                break;
            case DesktopSoftwere.Stream:
                StreamConfirm();
                break;
            case DesktopSoftwere.PreliminarySurvey:
                PreliminarySurveyConfirm();
                break;
        }    
    }

    private void SNSConfirm()
    {
        confirmText.text = "";
        disposable = confirmBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                confirmPopup.SetActive(false);

                disposable.Dispose();
            });
    }

    private void FanCafeConfirm()
    {
        disposable = confirmBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                confirmPopup.SetActive(false);
                disposable.Dispose();
            });
        
    }

    private void StreamConfirm()
    {
        confirmText.text = "<size=150%>방송</size>을\n보시겠습니까?";
        disposable = confirmBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                WordManager.TodoReset();
                WordManager.InitWord();

                confirmPopup.SetActive(false);
                EffectfulWindow.AppearEffectful(todoWindow.GetComponent<RectTransform>(), AppearTime, AppearStartSize, Ease.Linear);

                disposable.Dispose();
            });

        EffectfulWindow.AppearEffectful(confirmPopup.GetComponent<RectTransform>(), AppearTime, AppearStartSize, Ease.Linear);
    }

    private void PreliminarySurveyConfirm()
    {
        confirmText.text = "<size=150%>사전 조사</size>를\n하시겠습니까?";
        disposable = confirmBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                confirmPopup.SetActive(false);
                EffectfulWindow.AppearEffectful(PSWindow.GetComponent<RectTransform>(), AppearTime, AppearStartSize, Ease.Linear);

                disposable.Dispose();
            });
        EffectfulWindow.AppearEffectful(confirmPopup.GetComponent<RectTransform>(), AppearTime, AppearStartSize, Ease.Linear);
    }

    #endregion

    #region Turn ON/OFF

    private void TurnOff()
    {
        BlackScreen.color = Color.black;
        BlackScreen.gameObject.SetActive(true);

        BlackScreen.DOFade(1, 1)
            .OnComplete(() =>
            {
                BlackScreen.gameObject.SetActive(true);
            });
    }

    private void TurnOn()
    {
        //snsWindow.SetActive(false);
        //fancafeWindow.SetActive(false);
        List<Button> OpenBtns = new List<Button>() { streamOpenBtn, snsOpenBtn, fancafeOpenBtn, PSOpenBtn };
        if (ScheduleManager.currentPrograssScheduleID == "S01") { setAbleInteractBtn(PSOpenBtn, OpenBtns); }
        else if (ScheduleManager.currentPrograssScheduleID == "S03") { setAbleInteractBtn(streamOpenBtn, OpenBtns); }
        resultWindow.SetActive(false);
        streamWindow.SetActive(false);
        todoWindow.SetActive(false);
        BlackScreen.DOFade(0, 1)
            .SetEase(Ease.InSine)
            .OnComplete(() =>
            {
                BlackScreen.gameObject.SetActive(false);
            });
        setDesktopSectionBtns();
    }
    private void setAbleInteractBtn(Button AbleBtn, List<Button> Btns)
    {
        foreach(Button btn in Btns)
        {
            if(btn == AbleBtn) { btn.interactable = true; }
            else { btn.interactable = false; }
        }
    }

    public void setDesktopSectionBtns()
    {
        PlayerInputController.SetSectionBtns(new List<List<Button>> { 
            new List<Button> { snsOpenBtn },
            new List<Button> { fancafeOpenBtn },
            new List<Button> { streamOpenBtn },
            new List<Button> { PSOpenBtn } }, this);
    }

    #endregion
}


public enum DesktopSoftwere
{ 
    SNS,
    FanCafe,
    Stream,
    PreliminarySurvey
}
