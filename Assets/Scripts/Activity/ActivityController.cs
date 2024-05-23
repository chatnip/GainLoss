//Refactoring v1.0
using DG.Tweening;
using System.Collections;
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
            { e_HomeInteractType.Observational, new QuestionWindowConfig("사진 더미를 보며 시간 좀 때워볼까?", "관찰력", "observationalAnim") },
            { e_HomeInteractType.Persuasive, new QuestionWindowConfig("책을 하나 읽을까?", "설득력", "persuasiveAnim") },
            { e_HomeInteractType.MentalStrength, new QuestionWindowConfig("침대에서 조금 잘까?", "정신력", "mentalStrengthAnim") },
            { e_HomeInteractType.GoOutside, new QuestionWindowConfig("외출할까?", "", "goOutsideAnim") }
        };
        questionWindowAbilitiyDict = new Dictionary<e_HomeInteractType, int>
        { 
            { e_HomeInteractType.Observational, GameManager.Instance.MainInfo.ObservationalAbility },
            { e_HomeInteractType.Persuasive, GameManager.Instance.MainInfo.PersuasiveAbility },
            { e_HomeInteractType.MentalStrength, GameManager.Instance.MainInfo.MentalStrengthAbility }
        };

        // Set UI
        SetActivityGageUI();

        activityQuestionWindowRT.anchoredPosition = new Vector2(1200, activityQuestionWindowRT.anchoredPosition.y);
        noBtn.interactable = false;
        yesBtn.interactable = false;

        // Set Btn
        noBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                if (!GameManager.Instance.CanInput) { return; }

                SetActivityGageUI();
                QuestionWindow_ActiveOff(0.25f);
            });
        yesBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                if (!GameManager.Instance.CanInput) { return; }

                if (currentQuestionWindowType == e_HomeInteractType.GoOutside)
                { 
                    GoOutside(); 
                }
                else
                {
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

    private void SetActivityGageUI()
    {
        // Set Fill Gage
        float value = (float)GameManager.Instance.MainInfo.currentActivity / GameManager.Instance.MainInfo.maxActivity;
        foreach(Image img in gageActualImgs)
        { img.fillAmount = value; }
        foreach (Image img in gageUsePreviewImgs)
        { img.fillAmount = 0; }


        // Set Num
        amountNumTxt.text = GameManager.Instance.MainInfo.currentActivity.ToString();

        // Set Triangle Pos
        float RT_X = activityGageWindowRT.rect.width;
        RT_X /= GameManager.Instance.MainInfo.maxActivity;
        if (markImg.TryGetComponent(out RectTransform markRT)) 
        { markRT.anchoredPosition = new Vector2(RT_X * GameManager.Instance.MainInfo.currentActivity, 0); }
    }

    private void SetActivityGageUI_Use(e_HomeInteractType previewHI_Type)
    {
        SetActivityGageUI();
        int previewActivity = GameManager.Instance.MainInfo.currentActivity - questionWindowConfigDict[previewHI_Type].DecActivity;
        float value = 1 - ((float)previewActivity / GameManager.Instance.MainInfo.maxActivity);
        foreach (Image img in gageUsePreviewImgs)
        { img.fillAmount = value; }
    }

    #endregion

    #region Question Window

    public void QuestionWindow_ActiveOn(e_HomeInteractType HI_Type, float time)
    {
        PlayerInputController.Instance.StopMove();
        GameManager.Instance.CanInput = false;
        GameManager.Instance.CanInteractObject = false;

        // Exp
        if (ObjectInteractionButtonGenerator.Instance.SectionIsThis)
        { ObjectInteractionButtonGenerator.Instance.SetOnOffInteractObjectBtn(); }

        PlayerInputController.Instance.SetSectionBtns(new List<List<Button>> { new List<Button> { noBtn, yesBtn } }, this);
        
        // set Txt
        questionContentTxt.text = 
            questionWindowConfigDict[HI_Type].QuestionContent;
        
        kindOfGageByActivityTxt.text =
            questionWindowConfigDict[HI_Type].ActivityKind + "+" + questionWindowConfigDict[HI_Type].IncAbility.ToString();
        
        amountNumInWindowTxt.text = 
            GameManager.Instance.MainInfo.currentActivity.ToString() + "/" + GameManager.Instance.MainInfo.maxActivity.ToString();

        // Set Fill Gage
        SetActivityGageUI_Use(currentQuestionWindowType);

        if (questionWindowConfigDict[HI_Type].ActivityKind != null && questionWindowConfigDict[HI_Type].ActivityKind != "")
        { aboutAbilityPanelRT.gameObject.SetActive(true); }
        else
        { aboutAbilityPanelRT.gameObject.SetActive(false); }

        activityQuestionWindowRT.DOAnchorPos(new Vector2(720, activityQuestionWindowRT.anchoredPosition.y), time)
            .OnComplete(() =>
            {
                GameManager.Instance.CanInput = true;
                yesBtn.interactable = true;
                noBtn.interactable = true; 
            });
    }
    public void QuestionWindow_ActiveOff(float time)
    {
        PlayerInputController.Instance.CanMove = false;
        GameManager.Instance.CanInput = false;

        yesBtn.interactable = false;
        noBtn.interactable = false;

        activityQuestionWindowRT.DOAnchorPos(new Vector2(1200, activityQuestionWindowRT.anchoredPosition.y), time)
            .OnComplete(() =>
            {
                GameManager.Instance.CanInput = true;
                GameManager.Instance.CanInteractObject = true;
                PlayerInputController.Instance.CanMove = true;           
            });
    }

    #endregion

    #region Do Interact

    private void GoOutside()
    {

    }


    private void GetAbility_Start(e_HomeInteractType HI_Type)
    {
        GameManager.Instance.CanInput = false;

        QuestionWindowConfig QWC = questionWindowConfigDict[HI_Type];
        StartCoroutine(PlayAnim(QWC.AnimString));
    }

    public IEnumerator PlayAnim(string animName)
    {
        PlayerController.Instance._animator.Play(animName);

        yield return new WaitForFixedUpdate();

        float animLength = PlayerController.Instance._animator.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(animLength);

        GameManager.Instance.CanInput = true;
        GetAbility_End(currentQuestionWindowType);
    }

    private void GetAbility_End(e_HomeInteractType HI_Type)
    {
        GameManager.Instance.MainInfo.IncAbility(
            HI_Type, questionWindowConfigDict[HI_Type].IncAbility,
            questionWindowConfigDict[HI_Type].DecActivity);

        SetActivityGageUI();

        Debug.Log(
            $"\n행동력{GameManager.Instance.MainInfo.currentActivity}" +
            $"\n관찰력{GameManager.Instance.MainInfo.ObservationalAbility}" +
            $"\n설득력{GameManager.Instance.MainInfo.PersuasiveAbility}" +
            $"\n정신력{GameManager.Instance.MainInfo.MentalStrengthAbility}"
            );
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

    public QuestionWindowConfig(string questionContent, string activityKind, string animString)
    {
        QuestionContent = questionContent;
        ActivityKind = activityKind;
        AnimString = animString;
    }

}
