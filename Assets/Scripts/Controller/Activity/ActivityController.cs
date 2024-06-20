//Refactoring v1.0
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


public class ActivityController : Singleton<ActivityController>
{

    #region Value

    [Header("=== Gage")]
    [SerializeField] public RectTransform activityGageWindowRT;
    [SerializeField] List<Image> gageActualImgs;
    [SerializeField] List<Image> gageUsePreviewImgs;
    [SerializeField] RectTransform markImg;
    [SerializeField] List<TMP_Text> activityTxts;
    [SerializeField] TMP_Text amountNumTxt;

    [Header("=== Question Window")]
    [SerializeField] RectTransform activityQuestionWindowRT;
    [SerializeField] RectTransform aboutAbilityPanelRT;
    [SerializeField] TMP_Text questionContentTxt;
    [SerializeField] TMP_Text kindOfGageToAbilityTxt;
    [SerializeField] TMP_Text descGageToActivityTxt;
    [SerializeField] TMP_Text amountNumInWindowTxt;
    [SerializeField] Button noBtn;
    [SerializeField] Button yesBtn;

    [Header("=== Animation")]
    [SerializeField] AnimationClip observationalAnim;
    [SerializeField] AnimationClip persuasiveAnim;
    [SerializeField] AnimationClip mentalStrengthAnim;
    [SerializeField] AnimationClip goOutsideAnim;

    [Header("=== Ability UI")]
    [SerializeField] TMP_Text obsTxt;
    [SerializeField] TMP_Text socTxt;
    [SerializeField] TMP_Text menTxt;

    [Header("=== Btn")]
    [SerializeField] Button EndDayBtn;

    [Header("===  Arrow")]
    [SerializeField] List<SpriteRenderer> abilitySRs;
    [SerializeField] SpriteRenderer doorSR;

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
        // Set Dict
        questionWindowConfigDict = new Dictionary<e_HomeInteractType, QuestionWindowConfig>
        {
            { e_HomeInteractType.GoOutside, new QuestionWindowConfig("101", 0, 0, goOutsideAnim) },
            { e_HomeInteractType.Observational, new QuestionWindowConfig("103", 1, 1, observationalAnim) },
            { e_HomeInteractType.MentalStrength, new QuestionWindowConfig("104", 1, 1, mentalStrengthAnim) },
            { e_HomeInteractType.Persuasive, new QuestionWindowConfig("105", 1, 1, persuasiveAnim) }
        };
        questionWindowAbilitiyDict = new Dictionary<e_HomeInteractType, int>
        { 
            { e_HomeInteractType.Observational, GameManager.Instance.mainInfo.observation },
            { e_HomeInteractType.Persuasive, GameManager.Instance.mainInfo.sociability },
            { e_HomeInteractType.MentalStrength, GameManager.Instance.mainInfo.mentality }
        };

        // Set UI
        SetActivityGageUI(0.25f);

        activityQuestionWindowRT.anchoredPosition = new Vector2(1200, activityQuestionWindowRT.anchoredPosition.y);
        noBtn.interactable = false;
        yesBtn.interactable = false;

        foreach (TMP_Text tmp in activityTxts)
        { tmp.text = MainInfo.abilityTypeLanguage["Activity"][Convert.ToInt32(LanguageManager.Instance.languageID)]; }
        obsTxt.text = DataManager.Instance.Get_HomeObjectExtra("103");
        socTxt.text = DataManager.Instance.Get_HomeObjectExtra("105");
        menTxt.text = DataManager.Instance.Get_HomeObjectExtra("104");

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
                else
                {
                    if (GameManager.Instance.mainInfo.CurrentActivity <= 0) { return; }
                    GetAbility_Start(currentQuestionWindowType); 
                }
            });

        // Set EndBtn
        EndDayBtn.TryGetComponent(out RectTransform btnRT);
        btnRT.anchoredPosition = new Vector2(-300f, 0f);
        EndDayBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                if (!GameManager.Instance.canInput) { return; }
                EndDayBtn.TryGetComponent(out RectTransform btnRT);
                btnRT.DOAnchorPos(new Vector2(-300f, 0f), 1f).SetEase(Ease.OutCubic);
                LoadingManager.Instance.StartLoading();
                Debug.Log("하루 종료");
            });
    }

    protected override void Awake()
    {
        base.Awake();
    }

    #endregion

    #region Gage

    public void SetActivityGageUI(float dotweenTime)
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

        // Set Arrow
        SetArrow();
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
        Debug.Log("Reconfirm: " + HI_Type.ToString());
        PlayerInputController.Instance.MoveStop();
        PlayerController.Instance.ResetAnime();
        GameManager.Instance.canInput = false;
        GameManager.Instance.canInteractObject = false;

        // Exp
        if (InteractObjectBtnGenerator.Instance.SectionIsThis)
        { InteractObjectBtnGenerator.Instance.SetOnOffInteractObjectBtn(); }
        
        // set Txt
        if(currentQuestionWindowType == e_HomeInteractType.GoOutside)
        {
            questionContentTxt.text =
                questionWindowConfigDict[HI_Type].QuestionContent; 
            descGageToActivityTxt.text = "";
            // 외출하지만, 행동력이 남아있을 시
            if (GameManager.Instance.mainInfo.CurrentActivity > 0)
            { kindOfGageToAbilityTxt.text = "<color=red>" + questionWindowConfigDict[e_HomeInteractType.GoOutside].ActivityKind + "</color>"; }
            else
            { kindOfGageToAbilityTxt.text = ""; }
        }
        else if (currentQuestionWindowType == e_HomeInteractType.Reasoning) // 추리일 때
        {
            ReasoningController.Instance.ActiveOn(0.5f); return;
        }
        else
        {
            questionContentTxt.text =
                questionWindowConfigDict[HI_Type].QuestionContent; 
            kindOfGageToAbilityTxt.text =
                questionWindowConfigDict[HI_Type].ActivityKind + "+" + questionWindowConfigDict[HI_Type].IncAbility.ToString();
            descGageToActivityTxt.text =
                MainInfo.abilityTypeLanguage["Activity"][Convert.ToInt32(LanguageManager.Instance.languageID)] + "-" + questionWindowConfigDict[HI_Type].DecActivity;
        }
        
        
        amountNumInWindowTxt.text = 
            GameManager.Instance.mainInfo.CurrentActivity.ToString() + "/" + GameManager.Instance.mainInfo.MaxActivity.ToString();

        // Set Fill Gage
        SetActivityGageUI_Use(currentQuestionWindowType, 0.25f);

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
        StartCoroutine(PhoneHardware.Instance.Start_PhoneOn(PhoneHardware.e_phoneStateExtra.visitPlace));
    }

    #endregion

    #region Get Ability

    private void GetAbility_Start(e_HomeInteractType HI_Type)
    {

        QuestionWindowConfig QWC = questionWindowConfigDict[HI_Type];
        StartCoroutine(PlayAnim_AboutActivity(QWC.AnimClip));
    }

    public IEnumerator PlayAnim_AboutActivity(AnimationClip AC)
    {
        yield return null;

        GameManager.Instance.canInput = false;

        if(AC != null)
        {
            PlayerController.Instance.PlayInteractAnim(AC);

            yield return new WaitForFixedUpdate();

            float animLength = PlayerController.Instance._animator.GetCurrentAnimatorStateInfo(0).length;

            yield return new WaitForSeconds(animLength);
        }

        
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

    #region End Day

    public void OnEndDayBtn()
    {
        EndDayBtn.TryGetComponent(out RectTransform btnRT);
        btnRT.DOAnchorPos(new Vector2(0f, 0f), 1f).SetEase(Ease.OutCubic);
    }

    #endregion

    #region Set Arrow

    private void SetArrow()
    {
        if(GameManager.Instance.mainInfo.CurrentActivity > 0)
        {
            foreach (SpriteRenderer SR in abilitySRs)
            {
                Color abilityClr = SR.color;
                abilityClr.a = 1f;
                SR.color = abilityClr;   
            }

            Color doorClr = doorSR.color;
            doorClr.a = 0f;
            doorSR.color = doorClr;
        }
        else
        {
            foreach (SpriteRenderer SR in abilitySRs)
            {
                Color abilityClr = SR.color;
                abilityClr.a = 0f;
                SR.color = abilityClr;
            }

            Color doorClr = doorSR.color;
            doorClr.a = 1f;
            doorSR.color = doorClr;
        }
    }

    #endregion
}

public class QuestionWindowConfig
{
    public string QuestionContent;
    public string ActivityKind;
    public AnimationClip AnimClip;

    public int DecActivity = 1;
    public int IncAbility = 1;

    public QuestionWindowConfig(string homeObjectID, int incAbility, int decActivity, AnimationClip AC)
    {
        QuestionContent = DataManager.Instance.Get_HomeObjectReconfirm(homeObjectID);
        ActivityKind = DataManager.Instance.Get_HomeObjectExtra(homeObjectID);
        AnimClip = AC;
        IncAbility = incAbility;
        DecActivity = decActivity;
    }

}
