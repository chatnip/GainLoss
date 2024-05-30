//Refactoring v1.0
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;
using DG.Tweening;

public class DesktopController : Singleton<DesktopController>, IInteract
{
    #region Value

    [Header("=== Camera")]
    [SerializeField] Camera DesktopCamera;

    [Header("=== Property")]
    [SerializeField] ComputerInteract ComputerInteract;
    [SerializeField] TaskBar TaskBar;

    [Header("=== Confirm Popup")]
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
    [SerializeField] IDBtn currentAppBtn;

    [Header("=== App Controller")]
    [SerializeField] List<GameObject> appWindows;

    Dictionary<IDBtn, GameObject> appWindowsDict;


    [Header(" App Window")]
    [SerializeField] public GameObject streamWindow;
    [SerializeField] public GameObject resultWindow;

    #endregion

    #region Framework & Base Set

    public void Offset()
    {
        // Set Dict
        appWindowsDict = new Dictionary<IDBtn, GameObject>();
        for (int i = 0; i < appBtn.Count; i++)
        {
            appWindowsDict.Add(appBtn[i], appWindows[i]);
        }

        // Set Btn
        foreach (IDBtn idBtn in appBtn)
        {
            LanguageManager.Instance.SetLanguageTxt(idBtn.buttonText);
            idBtn.buttonText.text = DataManager.Instance.DesktopAppCSVDatas[LanguageManager.Instance.languageNum][idBtn.buttonID].ToString();
            idBtn.button.OnClickAsObservable()
                .Subscribe(btn =>
                {
                    if (!GameManager.Instance.canInput) { return; }

                    currentAppBtn = idBtn;
                    OpenConfirmPopup(currentAppBtn);
                });
        }
        
        foreach(Button popupExitBtn in confirmNoBtns)
        {
            popupExitBtn.OnClickAsObservable()
                .Subscribe(btn =>
                {
                    if (!GameManager.Instance.canInput) { return; }

                    EffectfulWindow.DisappearEffectful(confirmPopupRT, DisappearTime, DisappearLastSize, Ease.Linear);
                });
        }

        confirmYesBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                appWindowsDict[currentAppBtn].gameObject.SetActive(true);
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

    private void OpenConfirmPopup(IDBtn idBtn)
    {
        this.confirmText.text = 
            "<size=150%>" + DataManager.Instance.DesktopAppCSVDatas[LanguageManager.Instance.languageNum][idBtn.buttonID].ToString() + "</size>\n" + 
            DataManager.Instance.DesktopAppCSVDatas[LanguageManager.Instance.languageTypeAmount * 1 + LanguageManager.Instance.languageNum][idBtn.buttonID].ToString();
        EffectfulWindow.AppearEffectful(confirmPopupRT, AppearTime, AppearStartSize, Ease.Linear);
    }

    #endregion

    #region Turn ON/OFF

    public void TurnOff()
    {
        ComputerInteract.ScreenOff();
    }

    public void TurnOn()
    {
        ComputerInteract.ScreenOn();

        // TaskBar
        TaskBar.Offset();

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
        if (StreamController.Instance.isStreamingTime) { appBtn[0].button.interactable = true; }
        else { appBtn[0].button.interactable = false; }
    }

    #endregion
}


