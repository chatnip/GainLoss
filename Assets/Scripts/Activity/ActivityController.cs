//Refactoring v1.0
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


public class ActivityController : Singleton<ActivityController>, IInteract
{
    #region Value

    [Header("=== Gage")]
    [SerializeField] public RectTransform activityGageWindowRT;
    [SerializeField] Image gageImg;
    [SerializeField] RectTransform markImg;
    [SerializeField] TMP_Text amountNumTxt;

    [Header("=== Question Window")]
    [SerializeField] RectTransform activityQuestionWindowRT;
    [SerializeField] RectTransform aboutAbilityPanelRT;
    [SerializeField] TMP_Text questionContentTxt;
    [SerializeField] TMP_Text kindOfGageByActivityTxt;
    [SerializeField] TMP_Text amountNumInWindowTxt;
    [SerializeField] Image gageInWindowImg;
    [SerializeField] Button noBtn;
    [SerializeField] Button yesBtn;

    [HideInInspector] public e_HomeInteractType currentQuestionWindowType;
    public Dictionary<e_HomeInteractType, QuestionWindowConfig> questionWindowConfigDict;

    #endregion

    #region Enum

    public enum e_HomeInteractType
    {
        // 관찰력       // 설득력    // 정신력        //외출하기
        Observational, Persuasive, MentalStrength, GoOutside
    }

    #endregion

    #region Framework & Base Set

    public void Offset()
    {
        // Set Dict
        questionWindowConfigDict = new Dictionary<e_HomeInteractType, QuestionWindowConfig>
        { 
            { e_HomeInteractType.Observational, new QuestionWindowConfig("사진 더미를 보며 시간 좀 때워볼까?", "관찰력") },
            { e_HomeInteractType.Persuasive, new QuestionWindowConfig("책을 하나 읽을까?", "설득력") },
            { e_HomeInteractType.MentalStrength, new QuestionWindowConfig("침대에서 조금 잘까?", "정신력") },
            { e_HomeInteractType.GoOutside, new QuestionWindowConfig("외출할까?", "") }
        };

        // Set UI
        ft_setActivityGageUI();

        activityQuestionWindowRT.anchoredPosition = new Vector2(1200, activityQuestionWindowRT.anchoredPosition.y);
        noBtn.interactable = false;
        yesBtn.interactable = false;

        // Set Btn
        SetBtn();
    }

    public void SetBtn()
    {
        noBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                QuestionWindow_ActiveOff(0.25f);
            });
        yesBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                DoEachHomeInteract(currentQuestionWindowType);
            });
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

    #region Gage

    private void ft_setActivityGageUI()
    {
        // Set Fill Gage
        float value = (float)GameManager.Instance.MainInfo.currentActivity / GameManager.Instance.MainInfo.maxActivity;
        gageImg.fillAmount = value;

        // Set Num
        amountNumTxt.text = GameManager.Instance.MainInfo.currentActivity.ToString();

        // Set Triangle Pos
        float RT_X = activityGageWindowRT.rect.width;
        RT_X /= GameManager.Instance.MainInfo.maxActivity;
        if (markImg.TryGetComponent(out RectTransform markRT)) 
        { markRT.anchoredPosition = new Vector2(RT_X * GameManager.Instance.MainInfo.currentActivity, 0); }
    }

    #endregion

    #region Question Window

    public void QuestionWindow_ActiveOn(e_HomeInteractType HI_Type, float time)
    {
        if (!GameManager.Instance.CanInput) { return; }
        GameManager.Instance.CanInput = false;
        PlayerInputController.Instance.StopMove();

        // Exp
        if (ObjectInteractionButtonGenerator.Instance.SectionIsThis)
        { ObjectInteractionButtonGenerator.Instance.SetOnOffInteractObjectBtn(); }

        PlayerInputController.Instance.SetSectionBtns(new List<List<Button>> { new List<Button> { noBtn, yesBtn } }, this);
        
        // set Txt
        questionContentTxt.text = 
            questionWindowConfigDict[HI_Type].QuestionContent;
        
        kindOfGageByActivityTxt.text =
            questionWindowConfigDict[HI_Type].ActivityKind + "+" + questionWindowConfigDict[HI_Type].Inc.ToString();
        
        amountNumInWindowTxt.text = 
            GameManager.Instance.MainInfo.currentActivity.ToString() + "/" + GameManager.Instance.MainInfo.maxActivity.ToString();

        // Set Fill Gage
        float value = (float)GameManager.Instance.MainInfo.currentActivity / GameManager.Instance.MainInfo.maxActivity;
        gageInWindowImg.fillAmount = value;

        if (questionWindowConfigDict[HI_Type].ActivityKind != null && questionWindowConfigDict[HI_Type].ActivityKind != "")
        { aboutAbilityPanelRT.gameObject.SetActive(true); }
        else
        { aboutAbilityPanelRT.gameObject.SetActive(false); }

        activityQuestionWindowRT.DOAnchorPos(new Vector2(720, activityQuestionWindowRT.anchoredPosition.y), time)
            .OnComplete(() =>
            {
                yesBtn.interactable = true;
                noBtn.interactable = true; 
                GameManager.Instance.CanInput = true;
            });
    }
    public void QuestionWindow_ActiveOff(float time)
    {
        if (!GameManager.Instance.CanInput) { return; }
        GameManager.Instance.CanInput = false;
        PlayerInputController.Instance.CanMove = false;

        yesBtn.interactable = false;
        noBtn.interactable = false;

        activityQuestionWindowRT.DOAnchorPos(new Vector2(1200, activityQuestionWindowRT.anchoredPosition.y), time)
            .OnComplete(() => 
            {
                GameManager.Instance.CanInput = true;
                PlayerInputController.Instance.CanMove = true;
            });
    }

    #endregion

    private void DoEachHomeInteract(e_HomeInteractType HI_Type)
    {
        Debug.Log(HI_Type.ToString());
    }

}

public class QuestionWindowConfig
{
    public string QuestionContent;
    public string ActivityKind;
    public int Inc = 1;

    public QuestionWindowConfig(string questionContent, string activityKind)
    {
        QuestionContent = questionContent;
        ActivityKind = activityKind;
    }

}
