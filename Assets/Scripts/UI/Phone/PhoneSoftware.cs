//Refactoring v1.0
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;
using DG.Tweening;
using System;
using System.Linq;

public class PhoneSoftware : Singleton<PhoneSoftware>
{
    #region Value

    [Header("=== Screen By Condition")]
    [SerializeField] public GameObject visitPlaceScreen;
    [SerializeField] public GameObject optionScreen;

    [Header("=== Main Btn")]
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
                if (!GameSystem.Instance.canInput) { return; }

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

    }

    protected override void Awake()
    {
        base.Awake();
    }

    private void LateUpdate()
    {
        realTimeText.text = ft_getRealTimeString_H_M();
    }

    #endregion

    #region Base Info

    public void SetBaseInfo()
    {
        #region Base Info

        //PlayerInputController.Instance.interact = this;

        DayText.text = "DAY " + GameSystem.Instance.mainInfo.Day;
        DayOfWeekText.text = GameSystem.Instance.mainInfo.TodayOfTheWeek;

        if (phoneExitBtn.TryGetComponent(out RectTransform RT)) { RT.localScale = Vector3.one; }

        #endregion
    }

    #endregion

    #region About Map

    public void ResetMapIcons()
    {
        for (int i = 0; i < PlaceBtns.Count; i++)
        {
            PlaceBtns[i].TryGetComponent(out RectTransform BtnRT);
            PlaceBtns[i].TryGetComponent(out CanvasGroup BtnCG);

            BtnRT.sizeDelta = Vector2.zero;
            BtnCG.alpha = 0.0f;
        }
    }
    public void OpenMap()
    {
        GameSystem.Instance.canInput = false;
        this.OpenMapSeq = DOTween.Sequence();

        for (int i = 0; i < PlaceBtns.Count; i++)
        {
            PlaceBtns[i].TryGetComponent(out RectTransform BtnRT);
            PlaceBtns[i].TryGetComponent(out CanvasGroup BtnCG);

            DOTween.Kill(BtnRT.localScale);

            OpenMapSeq.Append(BtnRT.DOSizeDelta(Vector2.one * 300, 0.2f)
                .SetEase(Ease.OutBack));
            OpenMapSeq.Join(BtnCG.DOFade(1, 0.2f));
        }

        this.OpenMapSeq
            .OnUpdate(() => { if (GameSystem.Instance.canInput) { GameSystem.Instance.canInput = false; } })
            .OnComplete(() => { GameSystem.Instance.canInput = true; });

        
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
            .OnStart(() => { GameSystem.Instance.canInput = false; })
            .OnComplete(() => { GameSystem.Instance.canInput = true; });


        IDisposable cancelIDis = null;
        IDisposable applyIDis = null;

        #region Visit Place Type

        if (PhoneHardware.Instance.PhoneStateExtra == PhoneHardware.e_phoneStateExtra.visitPlace)
        {
            popupNameTxt.text = DataManager.Instance.Get_LocationName(currentIdBtn.buttonID);
            popupDescTxt.text = DataManager.Instance.Get_LocationDesc(GameManager.Instance.currentChapter, currentIdBtn.buttonID);

            cancelIDis = popupCancelBtn.OnClickAsObservable()
                .Subscribe(btn =>
                {
                    if (!GameSystem.Instance.canInput) { return; }

                    // Main System
                    ZoomOutPlaceMap(0.5f);
                    ClosePopup(0.5f); 

                    foreach (IDisposable iDis in popupIDisList) { iDis.Dispose(); }
                });
            applyIDis = popupApplyBtn.OnClickAsObservable()
                .Subscribe(btn =>
                {
                    if (!GameSystem.Instance.canInput) { return; }

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
            popupNameTxt.text = DataManager.Instance.Get_PhoneOptionName(currentIdBtn.buttonID);
            popupDescTxt.text = DataManager.Instance.Get_PhoneOptionDesc(currentIdBtn.buttonID);

            cancelIDis = popupCancelBtn.OnClickAsObservable()
                .Subscribe(btn =>
                {
                    if (!GameSystem.Instance.canInput) { return; }

                    // Main System
                    ClosePopup(0.5f);

                    foreach (IDisposable iDis in popupIDisList) { iDis.Dispose(); }
                });
            applyIDis = popupApplyBtn.OnClickAsObservable()
                .Subscribe(btn =>
                {
                    if (!GameSystem.Instance.canInput) { return; }

                    // Main System
                    PhoneOptionManager.Instance.DoPhoneOption();
                    ClosePopup(0f);

                    foreach (IDisposable iDis in popupIDisList) { iDis.Dispose(); }
                });
        }

        #endregion


        popupIDisList.Add(cancelIDis); popupIDisList.Add(applyIDis);
    }

    public void ClosePopup(float time)
    {
        popupBG.DOFade(0f, time)
            .OnComplete(() =>
            {
                popupBG.gameObject.SetActive(false);
            });
        popupRT.DOAnchorPos(new Vector2(0, -popupRT.rect.height), time)
            .OnStart(() => { GameSystem.Instance.canInput = false; })
            .OnComplete(() => { GameSystem.Instance.canInput = true; });

        #region Visit

        ZoomOutPlaceMap(0);

        #endregion
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
