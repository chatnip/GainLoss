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

    // Dotween
    Sequence SAG_UI;
    Sequence SAG_UI_Use;

    #endregion

    #region Enum

    public enum e_HomeInteractType
    {
        // ������       // �����    // ���ŷ�        //�����ϱ�
        Observational, Persuasive, MentalStrength, GoOutside
    }

    #endregion

    #region Framework & Base Set

    public void Offset()
    {
        // Set Dict
        questionWindowConfigDict = new Dictionary<e_HomeInteractType, QuestionWindowConfig>
        { 
            { e_HomeInteractType.Observational, new QuestionWindowConfig("���� ���̸� ���� �ð� �� ��������?", "������", "observationalAnim", 1, 1) },
            { e_HomeInteractType.Persuasive, new QuestionWindowConfig("å�� �ϳ� ������?", "�����", "persuasiveAnim", 1, 1) },
            { e_HomeInteractType.MentalStrength, new QuestionWindowConfig("ħ�뿡�� ���� �߱�?", "���ŷ�", "mentalStrengthAnim", 1, 1) },
            { e_HomeInteractType.GoOutside, new QuestionWindowConfig("�����ұ�?", "", "goOutsideAnim", 0, 0) }
        };
        questionWindowAbilitiyDict = new Dictionary<e_HomeInteractType, int>
        { 
            { e_HomeInteractType.Observational, GameManager.Instance.MainInfo.ObservationalAbility },
            { e_HomeInteractType.Persuasive, GameManager.Instance.MainInfo.PersuasiveAbility },
            { e_HomeInteractType.MentalStrength, GameManager.Instance.MainInfo.MentalStrengthAbility }
        };

        // Set UI
        SetActivityGageUI(0.25f);

        activityQuestionWindowRT.anchoredPosition = new Vector2(1200, activityQuestionWindowRT.anchoredPosition.y);
        noBtn.interactable = false;
        yesBtn.interactable = false;

        // Set Btn
        noBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                if (!GameManager.Instance.CanInput) { return; }

                SetActivityGageUI(0.25f);
                QuestionWindow_ActiveOff(0.25f);
            });
        yesBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                if (!GameManager.Instance.CanInput) { return; }

                if (currentQuestionWindowType == e_HomeInteractType.GoOutside)
                { 
                    StartCoroutine(GoOutside()); 
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

    private void SetActivityGageUI(float dotweenTime)
    {
        // Dotween
        DOTween.Kill(SAG_UI); 
        DOTween.Kill(SAG_UI_Use); 
        SAG_UI = DOTween.Sequence();
        SAG_UI_Use = DOTween.Sequence();

        // Set Fill Gage
        float value = (float)GameManager.Instance.MainInfo.currentActivity / GameManager.Instance.MainInfo.maxActivity;
        foreach(Image img in gageActualImgs)
        { SAG_UI.Join(DOTween.To(() => img.fillAmount, x => img.fillAmount = x, value, dotweenTime)); }
        foreach (Image img in gageUsePreviewImgs)
        { SAG_UI_Use.Join(DOTween.To(() => img.fillAmount, x => img.fillAmount = x, 0, dotweenTime)); }


        // Set Num
        amountNumTxt.text = GameManager.Instance.MainInfo.currentActivity.ToString();

        // Set Triangle Pos
        float RT_X = activityGageWindowRT.rect.width;
        RT_X /= GameManager.Instance.MainInfo.maxActivity;
        if (markImg.TryGetComponent(out RectTransform markRT)) 
        { SAG_UI.Join(markRT.DOAnchorPos(new Vector2(RT_X * GameManager.Instance.MainInfo.currentActivity, 0), dotweenTime)); }
    }

    private void SetActivityGageUI_Use(e_HomeInteractType previewHI_Type, float dotweenTime)
    {
        // Dotween
        DOTween.Kill(SAG_UI_Use);
        SAG_UI_Use = DOTween.Sequence();

        // Set Gage
        SetActivityGageUI(dotweenTime);
        int previewActivity = GameManager.Instance.MainInfo.currentActivity - questionWindowConfigDict[previewHI_Type].DecActivity;
        float value = 1 - ((float)previewActivity / GameManager.Instance.MainInfo.maxActivity);
        foreach (Image img in gageUsePreviewImgs)
        { SAG_UI_Use.Join(DOTween.To(() => img.fillAmount, x => img.fillAmount = x, value, dotweenTime)); }
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
        if(currentQuestionWindowType == e_HomeInteractType.GoOutside && GameManager.Instance.MainInfo.currentActivity > 0)
        {
            questionContentTxt.text =
                questionWindowConfigDict[HI_Type].QuestionContent + "\n<size=70%><color=red>���� ������� ���� �ൿ���� ����������, ���� ������?</size></color>";
        }
        else
        {
            questionContentTxt.text =
                questionWindowConfigDict[HI_Type].QuestionContent;
        }
        
        kindOfGageByActivityTxt.text =
            questionWindowConfigDict[HI_Type].ActivityKind + "+" + questionWindowConfigDict[HI_Type].IncAbility.ToString();
        
        amountNumInWindowTxt.text = 
            GameManager.Instance.MainInfo.currentActivity.ToString() + "/" + GameManager.Instance.MainInfo.maxActivity.ToString();

        // Set Fill Gage
        SetActivityGageUI_Use(currentQuestionWindowType, 0.25f);

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

    #region Go Outside

    private IEnumerator GoOutside()
    {
        QuestionWindow_ActiveOff(0.25f);
        yield return new WaitForSeconds(0.26f);
        PhoneHardware.Instance.Start_PhoneOn(PhoneHardware.e_phoneStateExtra.visitPlace);
    }

    #endregion

    #region Get Ability

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

        QuestionWindow_ActiveOff(0.25f);
        SetActivityGageUI(0.25f);

#if UNITY_EDITOR
        Debug.Log(
            $"\n�ൿ��{GameManager.Instance.MainInfo.currentActivity}" +
            $"\n������{GameManager.Instance.MainInfo.ObservationalAbility}" +
            $"\n�����{GameManager.Instance.MainInfo.PersuasiveAbility}" +
            $"\n���ŷ�{GameManager.Instance.MainInfo.MentalStrengthAbility}"
            );
#endif
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
