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
    [SerializeField] Button phoneExitBtn;

    [Header("=== visit Place Btn")]
    [SerializeField] List<Button> PlaceBtns;

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
    [SerializeField] Button popupMoveBtn;

    [Header("=== Other")]
    [SerializeField] Image appOpenBackgroundImg;
    [SerializeField] Image appIconImg;

    // Dotween
    Sequence OpenAppSeq;
    Sequence OpenMapSeq;

    #endregion

    #region Framework & Base Set

    public void Offset()
    {
        #region Btn

        phoneExitBtn
            .OnClickAsObservable()
            .Subscribe(btn =>
            {
                ClosePopup(0f);
                PhoneHardware.Instance.PhoneOff();
            });
        popupCancelBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                ClosePopup(0.5f);
            });
        popupMoveBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                PlaceManager.Instance.StartGoingSomewhereLoading(1.5f);
            });

        #endregion

        popupRT.DOAnchorPos(new Vector2(0, -popupRT.rect.height), 0.5f);
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

    #region Effectful

    private void OpenApp(GameObject App, Image ClickImg, float dotweenTime)
    {
        if (!GameManager.Instance.canInput) { return; }
        GameManager.Instance.canInput = false;

        if (DOTween.IsTweening(OpenAppSeq)) { DOTween.Complete(OpenAppSeq); }
        OpenAppSeq = DOTween.Sequence();

        // BG 초기 설정
        if(appOpenBackgroundImg.TryGetComponent(out RectTransform BGRT))
        {
            BGRT.localScale = Vector3.zero;
            appOpenBackgroundImg.DOFade(0, 0); 
            OpenAppSeq.Append(BGRT.DOScale(Vector3.one, dotweenTime)
                .SetEase(Ease.OutCubic)
                .OnComplete(() =>
                {
                    App.gameObject.SetActive(true);
                }));
        }

        // Icon 초기 설정
        appIconImg.DOFade(0, 0);
        appIconImg.sprite = ClickImg.sprite;

        // Dotween 애니메이션
        appOpenBackgroundImg.gameObject.SetActive(true);
        appIconImg.gameObject.SetActive(true);

        
        OpenAppSeq.Join(appOpenBackgroundImg.DOFade(1, dotweenTime));
        OpenAppSeq.Join(appIconImg.DOFade(1, 0.1f));

        OpenAppSeq.AppendInterval(dotweenTime);

        OpenAppSeq.Append(appOpenBackgroundImg.DOFade(0, dotweenTime).OnComplete(() => { appOpenBackgroundImg.gameObject.SetActive(false); }));
        OpenAppSeq.Join(appIconImg.DOFade(0, dotweenTime).OnComplete(() => { appIconImg.gameObject.SetActive(false); }));

        OpenAppSeq.OnComplete(() => { GameManager.Instance.canInput = true; });
    }

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

    public void OpenPopup(IDBtn currentIdBtn, float alpha, float time)
    {
        int index = PlaceManager.Instance.placeBtnList.IndexOf(currentIdBtn);

        popupNameTxt.text = DataManager.Instance.PlaceCSVDatas[LanguageManager.Instance.languageNum][currentIdBtn.buttonID].ToString();
        popupDescTxt.text = PlaceManager.Instance.visitReasons[index];


        popupBG.color = new Color(0, 0, 0, 0);
        popupBG.DOFade(alpha, time)
            .OnStart(() =>
            {
                popupBG.gameObject.SetActive(true);
            });
        popupRT.DOAnchorPos(Vector2.zero, time)
            .OnStart(() => { GameManager.Instance.canInput = false; })
            .OnComplete(() => { GameManager.Instance.canInput = true; });
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
