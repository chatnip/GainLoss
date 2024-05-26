using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;
using DG.Tweening;
using System;

public class Desktop : Singleton<Desktop>, IInteract
{
    #region Value

    [Header("=== Camera")]
    [SerializeField] Camera DesktopCamera;

    [Header("*Manager")]
    [SerializeField] StreamManager StreamManager;
    [SerializeField] ComputerInteract computerInteract;


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
    [SerializeField] GameObject PSPopupWindow;

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

    [Header("*Ext Btn")]
    [SerializeField] List<Button> ExceptionBtns;

    IDisposable disposable;

    public bool CanUseThisSentence = true;

    #endregion

    #region Framework & Base Set

    public void Offset()
    {
        // Set Btn
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


        // Set Off
        DesktopCamera.gameObject.SetActive(false);
    }

    protected override void Awake()
    {
        base.Awake();
        
    }

    public void StartStream()
    {
        EffectfulWindow.DisappearEffectful(todoWindow.GetComponent<RectTransform>(), DisappearTime, DisappearLastSize, Ease.Linear);
        //todoWindow.SetActive(false);

        EffectfulWindow.AppearEffectful(streamWindow.GetComponent<RectTransform>(), AppearTime, AppearStartSize, Ease.Linear);
        //streamWindow.SetActive(true);

        //StreamManager.currentStreamEvent = WordManager.currentStreamEvent;
        StreamManager.StartDialog(StreamManager.currentStreamEventID);
    }
    public void EndScheduleThis()
    {
        TurnOff();
        //ActionEventManager.TurnOnLoading();
        computerInteract.StartCoroutine(computerInteract.ScreenZoomOut());
    }
    public void DisappearPopup(GameObject Popup)
    {
        EffectfulWindow.DisappearEffectful(Popup.GetComponent<RectTransform>(), DisappearTime, DisappearLastSize, Ease.Linear);
        PlayerInputController.Instance.SetSectionBtns(new List<List<Button>>
        {
            new List<Button> { snsOpenBtn },
            new List<Button> { fancafeOpenBtn },
            new List<Button> { streamOpenBtn },
            new List<Button> { PSOpenBtn }
        }, this);

        if(disposable != null) { disposable.Dispose(); }
    }

    public void Interact()
    {
        #region Open Btns

        if (PlayerInputController.Instance.SelectBtn == snsOpenBtn) 
        { return; }
        else if (PlayerInputController.Instance.SelectBtn == fancafeOpenBtn) 
        { return; }
        else if(PlayerInputController.Instance.SelectBtn == streamOpenBtn) 
        {
            desktopSoftwere = DesktopSoftwere.Stream;
            ConfirmPopupSetting();
            PlayerInputController.Instance.SetSectionBtns(new List<List<Button>> { new List<Button> { confirmBtn } }, this);
            return;
        }
        else if(PlayerInputController.Instance.SelectBtn == PSOpenBtn)
        {
            desktopSoftwere = DesktopSoftwere.PreliminarySurvey;
            ConfirmPopupSetting();
            PlayerInputController.Instance.SetSectionBtns(new List<List<Button>> { new List<Button> { confirmBtn } }, this);
            return;
        }

        #endregion

        #region Confirm Btns

        if(PlayerInputController.Instance.SelectBtn == popupExitBtn)
        {
            EffectfulWindow.DisappearEffectful(confirmPopup.GetComponent<RectTransform>(), DisappearTime, DisappearLastSize, Ease.Linear);
            return;
        }
        else if(PlayerInputController.Instance.SelectBtn == confirmBtn)
        {
            
        }

        #endregion
    }

    private void OnDisable()
    {
        ExceptionBtnsTurnOn();
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
                //WordManager.TodoReset();
                //WordManager.InitWord();

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
                EffectfulWindow.AppearEffectful(PSPopupWindow.GetComponent<RectTransform>(), AppearTime, AppearStartSize, Ease.Linear);

                disposable.Dispose();
            });
        EffectfulWindow.AppearEffectful(confirmPopup.GetComponent<RectTransform>(), AppearTime, AppearStartSize, Ease.Linear);
    }

    #endregion

    #region Turn ON/OFF

    public void TurnOff()
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
        ExceptionBtnsTurnOff();
        List<Button> OpenBtns = new List<Button>() { streamOpenBtn, snsOpenBtn, fancafeOpenBtn, PSOpenBtn };
        confirmPopup.SetActive(false);
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
        PlayerInputController.Instance.SetSectionBtns(new List<List<Button>> { 
            new List<Button> { snsOpenBtn },
            new List<Button> { fancafeOpenBtn },
            new List<Button> { streamOpenBtn },
            new List<Button> { PSOpenBtn } }, this);
    }

    #endregion

    public void ExceptionBtnsTurnOn()
    {
        foreach (Button btn in ExceptionBtns)
        {
            btn.interactable = true;
        }
    }
    private void ExceptionBtnsTurnOff()
    {
        foreach (Button btn in ExceptionBtns)
        {
            btn.interactable = false;
        }
    }
}


public enum DesktopSoftwere
{ 
    SNS,
    FanCafe,
    Stream,
    PreliminarySurvey
}
