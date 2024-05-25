using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DG.Tweening;
using UniRx;
using TMPro;
using Spine.Unity;
using System;

public class DialogManager : Singleton<DialogManager>, IInteract
{
    #region Value

    [Header("Property")]
    [SerializeField] GameManager GameManager;
    [SerializeField] GameSystem GameSystem;
    [SerializeField] PlayerInputController PlayerInputController;
    [SerializeField] Desktop Desktop;

    [Header("*Dialog")]
    [SerializeField] public TMP_Text streamURLText;
    [SerializeField] TMP_Text subscriberAmountText;
    [SerializeField] TMP_Text streamScriptText;
    [SerializeField] public Button dialogNextBtn;
    [SerializeField] InputAction click;
    [SerializeField] public ReactiveProperty<ScenarioBase> ScenarioBase = new();

    [Header("*UI Component")]
    [SerializeField] public Button allSkipBtn;

    [Header("*Animation")]
    [SerializeField] SkeletonGraphic skeletonGraphic;

    [Header("*Result")]
    [SerializeField] float showTime = 0.8f; //Dotween 시간값
    [SerializeField] GameObject resultWindow;
    [SerializeField] Color baseColor;

    [Header("*Gage")]
    [SerializeField] Slider stressSlider;
    [SerializeField] TMP_Text stressText;
    [SerializeField] Color stressBarColor;

    [SerializeField] Slider angerSlider;
    [SerializeField] TMP_Text angerText;
    [SerializeField] Color angerBarColor;

    [SerializeField] Slider riskSlider;
    [SerializeField] TMP_Text riskText;
    [SerializeField] Color riskBarColor;

    [SerializeField] Slider overloadSilder;
    [SerializeField] TMP_Text overloadText;
    [SerializeField] Color overloadBarColor;

    [Header("*Other")]
    [SerializeField] Button EndBtn;
    [HideInInspector] public bool turnOver = false;

    //[HideInInspector] public StreamEvent currentStreamEvent = new StreamEvent();



    #endregion

    #region Main

    private void OnEnable()
    {
        click.Enable();
    }

    private void OnDisable()
    {
        click.Disable();
    }

    private void Start()
    {
        ScenarioBase
            .Where(Base => Base != null)
            .Subscribe(texting =>
            {
                StartCoroutine(DialogTexting(texting));
            });

        allSkipBtn.OnClickAsObservable()
                .Subscribe(btn =>
                {
                    ft_allSkip();
                });


        /*
        ScenarioBase
            .Where(Base => Base == null)
            .Subscribe(_ =>
            {
                // 여긴 예외처리 안해도 될듯
            });
        */
    }
    public void ft_allSkip()
    {
        StopAllCoroutines();
        StartCoroutine(ResultWindowOn());
    }

    public void Interact()
    {
        if (PlayerInputController.SelectBtn == dialogNextBtn)
        { turnOver = true; return; }
        else if (PlayerInputController.SelectBtn == EndBtn)
        { 
            Desktop.EndScheduleThis();
            PlayerInputController.SetSectionBtns(null, null);
            Desktop.streamWindow.SetActive(false);
            Desktop.resultWindow.SetActive(false);
        }

    }

    #endregion

    #region Animation

    private void AnimationSetup(SpineAniState state)
    {
        // 애니메이션 출력
        switch (state)
        {
            case SpineAniState.A01: // 인사 후, 기본
                skeletonGraphic.AnimationState.SetAnimation(trackIndex: 0, "Konnichiwa", loop: false);
                skeletonGraphic.AnimationState.SetAnimation(trackIndex: 1, "idle", loop: true);
                break;
            case SpineAniState.A02: // 기본
                skeletonGraphic.AnimationState.SetAnimation(trackIndex: 0, "idle", loop: true);
                break;
            case SpineAniState.A03: // 화난 표정
                skeletonGraphic.AnimationState.SetAnimation(trackIndex: 0, "angry look", loop: true);
                break;
            case SpineAniState.A04: // 당황한 표정
                skeletonGraphic.AnimationState.SetAnimation(trackIndex: 0, "bewildered look", loop: true);
                break;
            case SpineAniState.A05: // 차분한 화남 A
                skeletonGraphic.AnimationState.SetAnimation(trackIndex: 0, "calm anger_A", loop: true);
                break;
            case SpineAniState.A06: // 차분한 화남 B
                skeletonGraphic.AnimationState.SetAnimation(trackIndex: 0, "calm anger_B", loop: true);
                break;
            case SpineAniState.A07: // 눈 감기
                skeletonGraphic.AnimationState.SetAnimation(trackIndex: 0, "closed-eyed", loop: true);
                break;
            case SpineAniState.A08: // 교활(?)
                skeletonGraphic.AnimationState.SetAnimation(trackIndex: 0, "crafty", loop: true);
                break;
            case SpineAniState.A09: // 들뜬 표정
                skeletonGraphic.AnimationState.SetAnimation(trackIndex: 0, "excited look", loop: true);
                break;
            case SpineAniState.A10: // 자신감 넘치는
                skeletonGraphic.AnimationState.SetAnimation(trackIndex: 0, "full of confidence", loop: true);
                break;
            case SpineAniState.A11: // 행복한 표정
                skeletonGraphic.AnimationState.SetAnimation(trackIndex: 0, "happy look", loop: true);
                break;
            case SpineAniState.A12: // 홤봑 움음
                skeletonGraphic.AnimationState.SetAnimation(trackIndex: 0, "laugh", loop: true);
                break;
            case SpineAniState.A13: // 콧수염 기본
                skeletonGraphic.AnimationState.SetAnimation(trackIndex: 0, "Mustache idle", loop: true);
                break;
            case SpineAniState.A14: // 콧수염 시작(뿅하면서 나타남)
                skeletonGraphic.AnimationState.SetAnimation(trackIndex: 0, "Mustache start", loop: false);
                skeletonGraphic.AnimationState.SetAnimation(trackIndex: 1, "Mustache idle", loop: true);
                break;

        }

        
    }

    public enum SpineAniState
    {
        A01, A02, A03, A04, A05, A06, A07, A08, A09, A10, 
        A11, A12, A13, A14
    }

    #endregion

    #region Dialog

    public IEnumerator DialogTexting(ScenarioBase scenarioBase)
    {
        streamScriptText.text = null;

        for (int i = 0; i < scenarioBase.Fragments.Count; i++)
        {
            int temp = i;
            Sequence sequence = DOTween.Sequence();
            streamScriptText.text = null;

            skeletonGraphic.AnimationState.SetEmptyAnimations(0);
            AnimationSetup((SpineAniState)System.Enum.Parse(typeof(SpineAniState), scenarioBase.Fragments[temp].animationID));

            Fragment newFragment = scenarioBase.Fragments[temp];
            sequence.Append(streamScriptText.DOText(newFragment.Script, newFragment.Script.Length / 10)
                    .SetEase(Ease.Linear)
                    .OnUpdate(() =>
                    {
                        if (click.triggered || turnOver)
                        {
                            sequence.Complete();
                            turnOver = false;
                        }
                    }));

            yield return new WaitUntil(() =>
            {
                if (streamScriptText.text == newFragment.Script)
                {
                    return true;
                }
                return false;
            });

            yield return new WaitForSeconds(0.2f);

            yield return new WaitUntil(() =>
            {
                if (click.triggered || turnOver)
                {
                    turnOver = false;
                    return true;
                }
                return false;
            });
            turnOver = false;
            continue;
        }

        // 행동 있는지 확인
        // 행동 있으면 검은화면 껐다 켜지고 행동지문 출력
        // 행동 없으면 검은화면 껐다 켜지고 결과값 보여주기

        // dialog.SetActive(false);
        // GameSystem.StartGame();
        // 다이얼로그가 끝나면 게이지 결과값 보여주고 다음 날 시작

        turnOver = false;

        StartCoroutine(ResultWindowOn());
    }

    #endregion

    #region ResultWindow

    IEnumerator ResultWindowOn()
    {
        DOTween.Kill("UpdateSubscriberAmountText");

        PlayerInputController.SetSectionBtns(null, null);
        ClearGageAndText();

        CanvasGroup CG = resultWindow.GetComponent<CanvasGroup>();
        CG.alpha = 0.0f;
        CG.DOFade(1, showTime);
        yield return new WaitForSeconds(showTime);

        // 다음날로 가는 버튼 출력
        EndBtn.gameObject.SetActive(true);
        EndBtn.image.DOFade(1, showTime).SetEase(Ease.OutSine);
        PlayerInputController.SetSectionBtns(new List<List<Button>> { new List<Button> { EndBtn } }, this);

        void ClearGageAndText()
        {
            EndBtn.gameObject.SetActive(false);
            EndBtn.image.color = new Color(0, 0, 0, 0);
            stressText.alpha = 0;
            angerText.alpha = 0;
            riskText.alpha = 0;
            overloadText.alpha = 0;
            resultWindow.SetActive(true); //결과창
        }
/*
        void EffectGage(Slider slider, int appliedGage, float time, Color color)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(DOTween.To(() => slider.value, x => slider.value = x, (appliedGage * 0.01f), time)).SetEase(Ease.OutSine);
            Image img = slider.transform.GetChild(1).GetChild(0).GetComponent<Image>();
            var sequence2 = DOTween.Sequence();
            img.color = baseColor;
            Color LastColor = Color.Lerp(baseColor, color, appliedGage * 0.01f);
            sequence2.Append(img.DOColor(LastColor, time)).SetEase(Ease.OutSine);
        }
        void IncOpacityText(TMP_Text text, int appliedGage, int incGage, float time)
        {
            string gageAmount = "";
            if (incGage >= 0) { gageAmount = appliedGage + " ( +" + incGage + " )"; }
            else { gageAmount = appliedGage + " ( " + incGage + " )"; }
            
            text.text = gageAmount;
            text.DOFade(1, time).SetEase(Ease.OutSine);
        }
*/
    }

    #endregion

    #region Other

    public void SetUpdateSubscriberAmountText()
    {
        Sequence seq = DOTween.Sequence();
        int i = UnityEngine.Random.Range(4000, 5000 + 1);
        subscriberAmountText.text = i.ToString();

        seq.Append(subscriberAmountText.DOFade(1f, 1f)
            .OnComplete(() =>
            {
                int j = UnityEngine.Random.Range(-10, 10 + 1);
                int currentAmount = Convert.ToInt32(subscriberAmountText.text); 
                Debug.Log(currentAmount);
                int result = currentAmount + j;

                if (result < 4000) { result = 4000; }
                else if (result > 5000) { result = 5000; }

                subscriberAmountText.text = result.ToString();
            }));
        seq.AppendInterval(1f);

        seq.SetLoops(-1, LoopType.Restart)
            .SetId("UpdateSubscriberAmountText");
    }


    #endregion
}