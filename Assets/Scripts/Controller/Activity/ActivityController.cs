//Refactoring v1.0
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using static PhoneHardware;


public class ActivityController : Singleton<ActivityController>
{
    #region Value

    [Header("=== Gage")]
    [SerializeField] public RectTransform activityGageWindowRT;
    [SerializeField] List<Image> gageActualImgs;
    [SerializeField] List<Image> gageUsePreviewImgs;
    [SerializeField] RectTransform markImg;
    [SerializeField] TMP_Text amountNumTxt;

    [Header("=== Question Window")]
    [SerializeField] RectTransform activityQuestionWindowRT;
    [SerializeField] RectTransform aboutAbilityPanelRT;
    [SerializeField] TMP_Text questionContentTxt;
    [SerializeField] TMP_Text kindOfGageByActivityTxt;
    [SerializeField] TMP_Text amountNumInWindowTxt;
    [SerializeField] Button noBtn;
    [SerializeField] Button yesBtn;

    [Header("=== Animation")]
    [SerializeField] AnimationClip observationalAnim;
    [SerializeField] AnimationClip persuasiveAnim;
    [SerializeField] AnimationClip mentalStrengthAnim;
    [SerializeField] AnimationClip goOutsideAnim;

    // Other Value
    [HideInInspector] public e_HomeInteractType currentQuestionWindowType;
    public Dictionary<e_HomeInteractType, QuestionWindowConfig> questionWindowConfigDict;
    public Dictionary<e_HomeInteractType, int> questionWindowAbilitiyDict;

    // Dotween
    Sequence SAG_UI;
    Sequence SAG_UI_Use;

    #endregion

    #region Enum

    public enum e_HomeInteractType
    {
        // 관찰력       // 설득력    // 정신력        //외출하기  //추리하기
        Observational, Persuasive, MentalStrength, GoOutside, Reasoning
    }

    #endregion

    #region Framework & Base Set

    public void Offset()
    {
        int LNum = LanguageManager.Instance.languageNum;
        // Set Dict
        questionWindowConfigDict = new Dictionary<e_HomeInteractType, QuestionWindowConfig>
        { 
            {
                e_HomeInteractType.Reasoning, new QuestionWindowConfig(
                DataManager.Instance.ObjectCSVDatas[LanguageManager.Instance.languageTypeAmount * 2 + LNum]["O005"].ToString(),
                DataManager.Instance.ObjectCSVDatas[LanguageManager.Instance.languageTypeAmount * 3 + LNum]["O005"].ToString(),
                "ReasoningAnim",
                0, 0)
            },
            { 
                e_HomeInteractType.Observational, new QuestionWindowConfig(
                DataManager.Instance.ObjectCSVDatas[LanguageManager.Instance.languageTypeAmount * 2 + LNum]["O003"].ToString(),
                DataManager.Instance.ObjectCSVDatas[LanguageManager.Instance.languageTypeAmount * 3 + LNum]["O003"].ToString(), 
                "observationalAnim", 
                1, 1) 
            },
            { 
                e_HomeInteractType.Persuasive, new QuestionWindowConfig(
                DataManager.Instance.ObjectCSVDatas[LanguageManager.Instance.languageTypeAmount * 2 + LNum]["O002"].ToString(),
                DataManager.Instance.ObjectCSVDatas[LanguageManager.Instance.languageTypeAmount * 3 + LNum]["O002"].ToString(),
                "persuasiveAnim", 
                1, 1) 
            },
            { 
                e_HomeInteractType.MentalStrength, new QuestionWindowConfig(
                DataManager.Instance.ObjectCSVDatas[LanguageManager.Instance.languageTypeAmount * 2 + LNum]["O001"].ToString(),
                DataManager.Instance.ObjectCSVDatas[LanguageManager.Instance.languageTypeAmount * 3 + LNum]["O001"].ToString(),
                "mentalStrengthAnim", 
                1, 1) 
            },
            {
                e_HomeInteractType.GoOutside, new QuestionWindowConfig(
                DataManager.Instance.ObjectCSVDatas[LanguageManager.Instance.languageTypeAmount * 2 + LNum]["O000"].ToString(),
                DataManager.Instance.ObjectCSVDatas[LanguageManager.Instance.languageTypeAmount * 3 + LNum]["O000"].ToString(),
                "goOutsideAnim", 
                0, 0) 
            }
        };
        questionWindowAbilitiyDict = new Dictionary<e_HomeInteractType, int>
        { 
            { e_HomeInteractType.Observational, GameManager.Instance.mainInfo.ObservationalAbility },
            { e_HomeInteractType.Persuasive, GameManager.Instance.mainInfo.PersuasiveAbility },
            { e_HomeInteractType.MentalStrength, GameManager.Instance.mainInfo.MentalStrengthAbility }
        };

        // Set UI
        SetActivityGageUI(0.25f);
        LanguageManager.Instance.SetLanguageTxt(questionContentTxt);
        LanguageManager.Instance.SetLanguageTxt(kindOfGageByActivityTxt);

        activityQuestionWindowRT.anchoredPosition = new Vector2(1200, activityQuestionWindowRT.anchoredPosition.y);
        noBtn.interactable = false;
        yesBtn.interactable = false;

        // Set Btn
        noBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                if (!GameManager.Instance.canInput) { return; }

                SetActivityGageUI(0.25f);
                QuestionWindow_ActiveOff(0.25f);
            });
        yesBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                if (!GameManager.Instance.canInput) { return; }

                if (currentQuestionWindowType == e_HomeInteractType.GoOutside)
                { 
                    StartCoroutine(GoOutside()); 
                }
                else if(currentQuestionWindowType == e_HomeInteractType.Reasoning)
                {
                    ReasoningController.Instance.ActiveOn(0.5f);
                }
                else
                {
                    if (GameManager.Instance.mainInfo.CurrentActivity <= 0) { return; }
                    GetAbility_Start(currentQuestionWindowType); 
                }
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

    private void SetActivityGageUI(float dotweenTime)
    {
        // Dotween
        DOTween.Kill(SAG_UI); 
        DOTween.Kill(SAG_UI_Use); 
        SAG_UI = DOTween.Sequence();
        SAG_UI_Use = DOTween.Sequence();

        // Set Fill Gage
        float value = (float)GameManager.Instance.mainInfo.CurrentActivity / GameManager.Instance.mainInfo.MaxActivity;
        foreach(Image img in gageActualImgs)
        { SAG_UI.Join(DOTween.To(() => img.fillAmount, x => img.fillAmount = x, value, dotweenTime)); }
        foreach (Image img in gageUsePreviewImgs)
        { SAG_UI_Use.Join(DOTween.To(() => img.fillAmount, x => img.fillAmount = x, 0, dotweenTime)); }


        // Set Num
        amountNumTxt.text = GameManager.Instance.mainInfo.CurrentActivity.ToString();

        // Set Triangle Pos
        float RT_X = activityGageWindowRT.rect.width;
        RT_X /= GameManager.Instance.mainInfo.MaxActivity;
        if (markImg.TryGetComponent(out RectTransform markRT)) 
        { SAG_UI.Join(markRT.DOAnchorPos(new Vector2(RT_X * GameManager.Instance.mainInfo.CurrentActivity, 0), dotweenTime)); }
    }

    private void SetActivityGageUI_Use(e_HomeInteractType previewHI_Type, float dotweenTime)
    {
        // Dotween
        DOTween.Kill(SAG_UI_Use);
        SAG_UI_Use = DOTween.Sequence();

        // Set Gage
        SetActivityGageUI(dotweenTime);
        int previewActivity = GameManager.Instance.mainInfo.CurrentActivity - questionWindowConfigDict[previewHI_Type].DecActivity;
        float value = 1 - ((float)previewActivity / GameManager.Instance.mainInfo.MaxActivity);
        foreach (Image img in gageUsePreviewImgs)
        { SAG_UI_Use.Join(DOTween.To(() => img.fillAmount, x => img.fillAmount = x, value, dotweenTime)); }
    }

    #endregion

    #region Question Window

    public void QuestionWindow_ActiveOn(e_HomeInteractType HI_Type, float time)
    {
        PlayerInputController.Instance.MoveStop();
        PlayerController.Instance.resetAnime();
        GameManager.Instance.canInput = false;
        GameManager.Instance.canInteractObject = false;

        // Exp
        if (ObjectInteractionButtonGenerator.Instance.SectionIsThis)
        { ObjectInteractionButtonGenerator.Instance.SetOnOffInteractObjectBtn(); }
        
        // set Txt
        if(currentQuestionWindowType == e_HomeInteractType.GoOutside // 외출하지만, 행동력이 남아있을 시
            && GameManager.Instance.mainInfo.CurrentActivity > 0)
        {
            questionContentTxt.text =
                questionWindowConfigDict[HI_Type].QuestionContent + "\n<size=70%><color=red>" + questionWindowConfigDict[e_HomeInteractType.GoOutside].ActivityKind + "</size></color>";
            kindOfGageByActivityTxt.text = "";
        }
        else if (currentQuestionWindowType == e_HomeInteractType.Reasoning // 추리지만, 마지막 날짜가 아닐 경우
            && GameManager.Instance.mainInfo.Day != Convert.ToInt32(DataManager.Instance.ChapterCSVDatas[LanguageManager.Instance.languageTypeAmount * 2 + 2][GameManager.Instance.currentChapter]))
        {
            questionContentTxt.text =
                questionWindowConfigDict[HI_Type].QuestionContent + "\n<size=70%><color=red>" + questionWindowConfigDict[e_HomeInteractType.Reasoning].ActivityKind + "</size></color>";
            kindOfGageByActivityTxt.text = "";
        }
        else
        {
            questionContentTxt.text =
                questionWindowConfigDict[HI_Type].QuestionContent; 
            kindOfGageByActivityTxt.text =
                questionWindowConfigDict[HI_Type].ActivityKind + "+" + questionWindowConfigDict[HI_Type].IncAbility.ToString();
        }
        
        
        amountNumInWindowTxt.text = 
            GameManager.Instance.mainInfo.CurrentActivity.ToString() + "/" + GameManager.Instance.mainInfo.MaxActivity.ToString();

        // Set Fill Gage
        SetActivityGageUI_Use(currentQuestionWindowType, 0.25f);

        if (HI_Type == e_HomeInteractType.Reasoning || HI_Type == e_HomeInteractType.GoOutside)
        { aboutAbilityPanelRT.gameObject.SetActive(false); }
        else
        { aboutAbilityPanelRT.gameObject.SetActive(true); }

        activityQuestionWindowRT.DOAnchorPos(new Vector2(720, activityQuestionWindowRT.anchoredPosition.y), time)
            .OnComplete(() =>
            {
                GameManager.Instance.canInput = true;
                yesBtn.interactable = true;
                noBtn.interactable = true; 
            });
    }
    public void QuestionWindow_ActiveOff(float time)
    {
        PlayerInputController.Instance.CanMove = false;
        GameManager.Instance.canInput = false;

        yesBtn.interactable = false;
        noBtn.interactable = false;

        activityQuestionWindowRT.DOAnchorPos(new Vector2(1200, activityQuestionWindowRT.anchoredPosition.y), time)
            .OnComplete(() =>
            {
                GameManager.Instance.canInput = true;
                GameManager.Instance.canInteractObject = true;
                PlayerInputController.Instance.CanMove = true;           
            });
    }

    #endregion

    #region Go Outside

    private IEnumerator GoOutside()
    {
        QuestionWindow_ActiveOff(0.25f);
        yield return new WaitForSeconds(0.26f);
        StartCoroutine(PhoneHardware.Instance.Start_PhoneOn(e_phoneStateExtra.visitPlace));
    }

    #endregion

    #region Get Ability

    private void GetAbility_Start(e_HomeInteractType HI_Type)
    {
        GameManager.Instance.canInput = false;

        QuestionWindowConfig QWC = questionWindowConfigDict[HI_Type];
        StartCoroutine(PlayAnim(QWC.AnimString));
    }

    public IEnumerator PlayAnim(string animName)
    {
        PlayerController.Instance._animator.Play(animName);

        yield return new WaitForFixedUpdate();

        float animLength = PlayerController.Instance._animator.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(animLength);

        GameManager.Instance.canInput = true;
        GetAbility_End(currentQuestionWindowType);
    }

    private void GetAbility_End(e_HomeInteractType HI_Type)
    {
        GameManager.Instance.mainInfo.IncAbility(
            HI_Type, questionWindowConfigDict[HI_Type].IncAbility,
            questionWindowConfigDict[HI_Type].DecActivity);

        QuestionWindow_ActiveOff(0.25f);
        SetActivityGageUI(0.25f);
        GameSystem.Instance.SetAbilityUI();
    }

    #endregion

}

public class QuestionWindowConfig
{
    public string QuestionContent;
    public string ActivityKind;
    public string AnimString;

    public int DecActivity = 1;
    public int IncAbility = 1;

    public QuestionWindowConfig(string questionContent, string activityKind, string animString, int incAbility, int decActivity)
    {
        QuestionContent = questionContent;
        ActivityKind = activityKind;
        AnimString = animString;
        IncAbility = incAbility;
        DecActivity = decActivity;
    }

}
