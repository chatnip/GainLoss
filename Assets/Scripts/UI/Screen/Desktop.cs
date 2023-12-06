using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;
using DG.Tweening;

public class Desktop : MonoBehaviour
{
    [Header("*Manager")]
    [SerializeField] StreamManager StreamManager;
    [SerializeField] ActionEventManager ActionEventManager;
    [SerializeField] GameSystem GameSystem;
    [SerializeField] ComputerInteract ComputerInteract;

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
    [SerializeField] GameObject streamWindow;
    [SerializeField] GameObject resultWindow;
    [SerializeField] Button streamStartBtn;
    [SerializeField] Button streamEndBtn;

    [Header("*Todo")]
    [SerializeField] Button todoExitBtn;
    [SerializeField] GameObject todoWindow;

    [Header("*ConfirmPopup")]
    [SerializeField] DesktopSoftwere desktopSoftwere;
    [SerializeField] Button popupExitBtn;
    [SerializeField] Button confirmBtn;
    [SerializeField] TMP_Text confirmText;
    [SerializeField] GameObject confirmPopup;


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

        streamOpenBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                desktopSoftwere = DesktopSoftwere.Stream;
                ConfirmPopupSetting();
            });

        streamStartBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                todoWindow.SetActive(false);
                streamWindow.SetActive(true);
                StreamManager.StartDialog(StreamManager.currentStreamEventID);
            });

        streamEndBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                ActionEventManager.TurnOnLoading();
                ComputerInteract.StartCoroutine(ComputerInteract.ScreenZoomOut());
            });

        popupExitBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                confirmPopup.SetActive(false);
            });

        todoExitBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                todoWindow.SetActive(false);
            });
    }

    private void ConfirmPopupSetting()
    {
        switch(desktopSoftwere)
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
        }    
    }

    private void SNSConfirm()
    {
        confirmPopup.SetActive(false);
        confirmBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                confirmPopup.SetActive(false);
            });
    }

    private void FanCafeConfirm()
    {
        confirmPopup.SetActive(false);
        confirmBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                confirmPopup.SetActive(false);
            });
    }

    private void StreamConfirm()
    {     
        confirmPopup.SetActive(true);
        confirmBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                confirmPopup.SetActive(false);
                todoWindow.SetActive(true);
            });
    }

}


public enum DesktopSoftwere
{ 
    SNS,
    FanCafe,
    Stream
}
