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
    [SerializeField] DataManager DataManager;
    [SerializeField] WordManager WordManager;
    [SerializeField] ActionEventManager ActionEventManager;
    [SerializeField] StreamManager StreamManager;
    [SerializeField] GameSystem GameSystem;
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

    [Header("*WindowFrame")]
    [SerializeField] float AppearTime;
    [SerializeField] float AppearStartSize;

    [SerializeField] float DisappearTime;
    [SerializeField] float DisappearLastSize;

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
                EffectfulWindow.DisappearEffectful(todoWindow.GetComponent<RectTransform>(), DisappearTime, DisappearLastSize, Ease.Linear);
                //todoWindow.SetActive(false);

                EffectfulWindow.AppearEffectful(streamWindow.GetComponent<RectTransform>(), AppearTime, AppearStartSize, Ease.Linear);
                //streamWindow.SetActive(true);

                StreamManager.currentStreamEvent = WordManager.currentStreamEvent;
                StreamManager.StartDialog(StreamManager.currentStreamEventID);
            });

        streamEndBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                TurnOff();
                ActionEventManager.TurnOnLoading();
                computerInteract.StartCoroutine(computerInteract.ScreenZoomOut(false));
            });

        popupExitBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                //confirmPopup.SetActive(false);
                EffectfulWindow.DisappearEffectful(confirmPopup.GetComponent<RectTransform>(), DisappearTime, DisappearLastSize, Ease.Linear);
            });

        todoExitBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                //todoWindow.SetActive(false);
                EffectfulWindow.DisappearEffectful(todoWindow.GetComponent<RectTransform>(), DisappearTime, DisappearLastSize, Ease.Linear);
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
        //confirmPopup.SetActive(true);
        confirmBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                WordManager.TodoReset();
                WordManager.InitWord();
                // wordManager.WordBtnListSet();
                // wordManager.WordActionBtnListSet();

                confirmPopup.SetActive(false);
                //todoWindow.SetActive(true);
                EffectfulWindow.AppearEffectful(todoWindow.GetComponent<RectTransform>(), AppearTime, AppearStartSize, Ease.Linear);
            });

        EffectfulWindow.AppearEffectful(confirmPopup.GetComponent<RectTransform>(), AppearTime, AppearStartSize, Ease.Linear);
    }

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
        resultWindow.SetActive(false);
        streamWindow.SetActive(false);
        todoWindow.SetActive(false);
        BlackScreen.DOFade(0, 1)
            .SetEase(Ease.InSine)
            .OnComplete(() =>
            {
                BlackScreen.gameObject.SetActive(false);
            });
    }
    private void OnEnable()
    {
        TurnOn();
    }


    /*public static void AppearEffectful(RectTransform RT, List<Button> btns, float time, float size)
    {
        foreach (Button btn in btns) { btn.interactable = false; }

        RT.transform.localScale = Vector3.one * size;

        RT.gameObject.SetActive(true);
        RT.transform.DOScale(Vector3.one, time)
            .OnComplete(() =>
            {
                foreach (Button btn in btns) { btn.interactable = true; }
            });
    }
    public static void DisappearEffectful(RectTransform RT, List<Button> btns, float time, float size)
    {
        foreach (Button btn in btns) { btn.interactable = false; }

        RT.transform.localScale = Vector3.one;

        RT.transform.DOScale(Vector3.one * size, time)
            .OnComplete(() =>
            {
                foreach (Button btn in btns) { btn.interactable = true; }
                RT.gameObject.SetActive(false);
            });
    }*/
    
}


public enum DesktopSoftwere
{ 
    SNS,
    FanCafe,
    Stream
}
