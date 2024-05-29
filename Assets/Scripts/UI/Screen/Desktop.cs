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

    [Header("=== Property")]
    [SerializeField] ComputerInteract ComputerInteract;
    [SerializeField] TaskBar TaskBar;

    [Header("=== Confirm Popup")]
    [SerializeField] string confirmID;
    [SerializeField] RectTransform confirmPopupRT;
    [SerializeField] Button confirmYesBtn;
    [SerializeField] List<Button> confirmNoBtns;
    [SerializeField] TMP_Text confirmText;

    [Header("=== Base UI")]
    [SerializeField] Image BlackScreen;

    [Header("=== WindowFrame")]
    [SerializeField] public float AppearTime;
    [SerializeField] public float AppearStartSize;
    [SerializeField] public float DisappearTime;
    [SerializeField] public float DisappearLastSize;

    [Header("=== App Btn")]
    [SerializeField] public List<IDBtn> appBtn;

    [Header("=== App Controller")]
    [SerializeField] List<DesktopController> appControllers;


    [Header(" App Window")]
    [SerializeField] public GameObject streamWindow;
    [SerializeField] public GameObject resultWindow;

    #endregion

    #region Framework & Base Set

    public void Offset()
    {
        // Set Btn
        foreach(IDBtn idBtn in appBtn)
        {
            LanguageManager.Instance.SetLanguageTxt(idBtn.buttonText);
            idBtn.buttonText.text = DataManager.Instance.DesktopAppCSVDatas[LanguageManager.Instance.languageNum][idBtn.buttonID].ToString();
            idBtn.button.OnClickAsObservable()
                .Subscribe(btn =>
                {
                    if (!GameManager.Instance.canInput) { return; }

                    string ID = idBtn.buttonID;
                    confirmID = ID;
                    OpenConfirmPopup(ID);
                });
        }
        
        foreach(Button popupExitBtn in confirmNoBtns)
        {
            popupExitBtn.OnClickAsObservable()
                .Subscribe(btn =>
                {
                    if (!GameManager.Instance.canInput) { return; }

                    confirmID = null;
                    EffectfulWindow.DisappearEffectful(confirmPopupRT, DisappearTime, DisappearLastSize, Ease.Linear);
                });
        }

        confirmYesBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                Debug.Log(confirmID);
                foreach(DesktopController DC in appControllers)
                {
                    if(DC.desktopAppID == confirmID)
                    {
                        DC.ActiveOn();
                    }
                }
            });

        // Set GameObject
        DesktopCamera.gameObject.SetActive(false);
        confirmPopupRT.gameObject.SetActive(false);
    }

    protected override void Awake()
    {
        base.Awake();
    }

    #endregion

    #region For Pad

    public void Interact()
    {

    }

    #endregion

    #region Confirm

    private void OpenConfirmPopup(string desktopID)
    {
        this.confirmText.text = 
            "<size=150%>" + DataManager.Instance.DesktopAppCSVDatas[LanguageManager.Instance.languageNum][desktopID].ToString() + "</size>\n" + 
            DataManager.Instance.DesktopAppCSVDatas[LanguageManager.Instance.languageTypeAmount * 1 + LanguageManager.Instance.languageNum][desktopID].ToString();
        EffectfulWindow.AppearEffectful(confirmPopupRT, AppearTime, AppearStartSize, Ease.Linear);
    }

    #endregion

    #region Turn ON/OFF

    public void TurnOff()
    {
        ComputerInteract.ScreenOff();

        // TaskBar
        TaskBar.Offset();
    }

    public void TurnOn()
    {
        ComputerInteract.ScreenOn();

        // Black Screen
        BlackScreen.gameObject.SetActive(true);
        BlackScreen.color = Color.black;
        BlackScreen.DOFade(0, 1)
            .SetEase(Ease.InSine)
            .OnComplete(() =>
            {
                BlackScreen.gameObject.SetActive(false);
            });

        // Set Special Btn
        if (PlaceManager.Instance.isStreamingTime) { appBtn[0].button.interactable = true; }
        else { appBtn[0].button.interactable = false; }
    }

    #endregion
}


