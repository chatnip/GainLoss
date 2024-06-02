//Refactoring v1.0
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;
using DG.Tweening;
using System;
using System.Linq;

public class PhoneSoftware : Singleton<PhoneSoftware>, IInteract
{
    #region Value

    [Header("=== Screen By Condition")]
    [SerializeField] public GameObject visitPlaceScreen;
    [SerializeField] public GameObject optionScreen;

    [Header("=== Main Btn")]
    [SerializeField] Button resumeBtn;
    [SerializeField] Button restartBtn;
    [SerializeField] Button titleBtn;
    [SerializeField] Button quitBtn;
    [SerializeField] Button phoneExitBtn;

    [Header("=== visit Place Btn")]
    [SerializeField] List<Button> PlaceBtns;
    [SerializeField] RectTransform BG_RT;

    [Header("=== Base Info")]
    [SerializeField] TMP_Text DayText;
    [SerializeField] TMP_Text DayOfWeekText;
    [SerializeField] TMP_Text realTimeText;

    [Header("=== Popup")]
    [SerializeField] Image popupBG;
    [SerializeField] RectTransform popupRT;
    [SerializeField] TMP_Text popupNameTxt;
    [SerializeField] TMP_Text popupDescTxt;
    [SerializeField] Button popupCancelBtn;
    [SerializeField] Button popupApplyBtn;

    // Dotween
    Sequence OpenMapSeq;

    // Other Value
    List<IDisposable> popupIDisList = new List<IDisposable>();

    #endregion

    #region Framework & Base Set

    public void Offset()
    {
        #region Btn

        phoneExitBtn
            .OnClickAsObservable()
            .Subscribe(btn =>
            {
                if (!GameManager.Instance.canInput) { return; }

                ClosePopup(0f);
                PhoneHardware.Instance.PhoneOff();
                PlayerInputController.Instance.CanMove = true;
            });
        
        #endregion

        popupRT.DOAnchorPos(new Vector2(0, -popupRT.rect.height), 0.5f);
        popupBG.color = new Color(0f, 0f, 0f, 0f);
        popupBG.gameObject.SetActive(false);

        this.visitPlaceScreen.gameObject.SetActive(false);
        this.optionScreen.gameObject.SetActive(false);
        this.gameObject.transform.parent.gameObject.SetActive(false);

        List<TMP_Text> languageTmpT = new List<TMP_Text>
        { DayText, DayOfWeekText, realTimeText, popupNameTxt, popupDescTxt };
        LanguageManager.Instance.SetLanguageTxts(languageTmpT); 
    }

    protected override void Awake()
    {
        base.Awake();
    }

    private void LateUpdate()
    {
        realTimeText.text = ft_getRealTimeString_H_M();
    }

    public void Interact()
    {
        #region Site Survey

        if (PlayerInputController.Instance.SelectBtn.gameObject.name == "Home")
        { 
            PhoneHardware.Instance.PhoneOff(); 
            PlayerInputController.Instance.ClearSeletedBtns(); return; 
        }
        else
        {
            PlayerInputController.Instance.ClearSeletedBtns(); 
            return; 
        }

        #endregion
    }

    #endregion

    #region Base Info

    public void SetBaseInfo()
    {
        #region Base Info

        PlayerInputController.Instance.interact = this;

        DayText.text = "DAY " + GameManager.Instance.mainInfo.Day;
        DayOfWeekText.text = GameManager.Instance.mainInfo.TodayOfTheWeek;

        if (phoneExitBtn.TryGetComponent(out RectTransform RT)) { RT.localScale = Vector3.one; }

        #endregion
    }

    #endregion

    #region About Map

    public void OpenMap()
    {
        GameManager.Instance.canInput = false;
        this.OpenMapSeq = DOTween.Sequence();

        List<List<Button>> btns = new List<List<Button>>() { PlaceBtns };
        PlayerInputController.Instance.SetSectionBtns(btns, this);

        OpenMapSeq.AppendInterval(2f);
        for (int i = 0; i < PlaceBtns.Count; i++)
        {
            PlaceBtns[i].TryGetComponent(out RectTransform BtnRT);
            PlaceBtns[i].TryGetComponent(out CanvasGroup BtnCG);

            BtnRT.sizeDelta = Vector2.zero;
            DOTween.Kill(BtnRT.localScale);
            BtnCG.alpha = 0.0f;

            OpenMapSeq.Append(BtnRT.DOSizeDelta(Vector2.one * 300, 0.2f)
                .SetEase(Ease.OutBack));
            OpenMapSeq.Join(BtnCG.DOFade(1, 0.2f));
        }

        this.OpenMapSeq.OnComplete(() => { GameManager.Instance.canInput = true; });

        
    }

    public void ZoomInPlaceMap(IDBtn currentIdBtn, float IncScale, float time)
    {
        if (DOTween.IsTweening(BG_RT)) { DOTween.Kill(BG_RT); }
        Sequence seq = DOTween.Sequence();

        currentIdBtn.TryGetComponent(out RectTransform btnRT);
        seq.Append(BG_RT.DOAnchorPos(new Vector2(-btnRT.anchoredPosition.x, -btnRT.anchoredPosition.y), time)
            .SetEase(Ease.OutCubic));
        seq.Join(BG_RT.DOScale(IncScale, time).SetEase(Ease.OutCubic));
    }

    public void ZoomOutPlaceMap(float time)
    {
        if (DOTween.IsTweening(BG_RT)) { DOTween.Kill(BG_RT); }
        Sequence seq = DOTween.Sequence();

        seq.Append(BG_RT.DOAnchorPos(new Vector2(0, 0), time)
            .SetEase(Ease.OutCubic));
        seq.Join(BG_RT.DOScale(1f, time).SetEase(Ease.OutCubic));
    }

    #endregion

    #region Popup

    public void OpenPopup(IDBtn currentIdBtn, float time)
    {
        popupBG.color = new Color(0, 0, 0, 0);
        popupBG.DOFade(1f, time)
            .OnStart(() =>
            {
                popupBG.gameObject.SetActive(true);
            });
        popupRT.DOAnchorPos(Vector2.zero, time)
            .OnStart(() => { GameManager.Instance.canInput = false; })
            .OnComplete(() => { GameManager.Instance.canInput = true; });


        IDisposable cancelIDis = null;
        IDisposable applyIDis = null;

        #region Visit Place Type

        if (PhoneHardware.Instance.PhoneStateExtra == PhoneHardware.e_phoneStateExtra.visitPlace)
        {
            int index = PlaceManager.Instance.placeBtnList.IndexOf(currentIdBtn);

            popupNameTxt.text = DataManager.Instance.PlaceCSVDatas[LanguageManager.Instance.languageNum][currentIdBtn.buttonID].ToString();
            popupDescTxt.text = PlaceManager.Instance.visitReasons[index];

            cancelIDis = popupCancelBtn.OnClickAsObservable()
                .Subscribe(btn =>
                {
                    if (!GameManager.Instance.canInput) { return; }

                    // Main System
                    ZoomOutPlaceMap(0.5f);
                    ClosePopup(0.5f); 

                    foreach (IDisposable iDis in popupIDisList) { iDis.Dispose(); }
                });
            applyIDis = popupApplyBtn.OnClickAsObservable()
                .Subscribe(btn =>
                {
                    if (!GameManager.Instance.canInput) { return; }

                    // Main System
                    ZoomOutPlaceMap(0f);
                    ClosePopup(0f);
                    PlaceManager.Instance.StartGoingSomewhereLoading(1.5f);

                    foreach (IDisposable iDis in popupIDisList) { iDis.Dispose(); }
                });
        }

        #endregion

        #region Option Type

        else if (PhoneHardware.Instance.PhoneStateExtra == PhoneHardware.e_phoneStateExtra.option)
        {
            popupNameTxt.text = DataManager.Instance.PhoneOptionAppCSVDatas[LanguageManager.Instance.languageNum][currentIdBtn.buttonID].ToString();
            popupDescTxt.text = DataManager.Instance.PhoneOptionAppCSVDatas[LanguageManager.Instance.languageTypeAmount + LanguageManager.Instance.languageNum][currentIdBtn.buttonID].ToString();

            cancelIDis = popupCancelBtn.OnClickAsObservable()
                .Subscribe(btn =>
                {
                    if (!GameManager.Instance.canInput) { return; }

                    // Main System
                    ClosePopup(0.5f);

                    foreach (IDisposable iDis in popupIDisList) { iDis.Dispose(); }
                });
            applyIDis = popupApplyBtn.OnClickAsObservable()
                .Subscribe(btn =>
                {
                    if (!GameManager.Instance.canInput) { return; }

                    // Main System
                    MainOptionManager.Instance.mainOptionAppPlaysDict[currentIdBtn]();
                    ClosePopup(0f);

                    foreach (IDisposable iDis in popupIDisList) { iDis.Dispose(); }
                });
        }

        #endregion


        popupIDisList.Add(cancelIDis); popupIDisList.Add(applyIDis);
    }

    private void ClosePopup(float time)
    {
        popupBG.DOFade(0f, time)
            .OnComplete(() =>
            {
                popupBG.gameObject.SetActive(false);
            });
        popupRT.DOAnchorPos(new Vector2(0, -popupRT.rect.height), time)
            .OnStart(() => { GameManager.Instance.canInput = false; })
            .OnComplete(() => { GameManager.Instance.canInput = true; });
    }


    #endregion

    #region Real Time

    private string ft_getRealTimeString_H_M()
    {
        string AllRealTime = DateTime.Now.ToString(("yyyy-MM-dd HH:mm:ss tt"));
        string H = DateTime.Now.ToString(("HH"));
        string m = DateTime.Now.ToString(("mm"));

        return H + ":" + m;
    }

    #endregion
}
