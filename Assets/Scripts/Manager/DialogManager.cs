using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DG.Tweening;
using UniRx;
using TMPro;
using Spine.Unity;

public class DialogManager : Manager<DialogManager>
{
    [SerializeField] GameManager GameManager;
    [SerializeField] GameSystem GameSystem;

    [Header("*Dialog")]
    [SerializeField] public TMP_Text streamTitleText;
    [SerializeField] TMP_Text streamScriptText;
    [SerializeField] Button dialogNextBtn;
    [SerializeField] InputAction click;
    [SerializeField] public ReactiveProperty<ScenarioBase> ScenarioBase = new();

    [Header("*Animation")]
    [SerializeField] SkeletonGraphic skeletonGraphic;

    [Header("*Result")]
    [SerializeField] float showTime = 0.8f; //Dotween 시간값
    [SerializeField] GameObject resultWindow;
    [SerializeField] GameObject resultWindowGage;
    [SerializeField] GameObject resultWindowOverloadGage;
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


    [Header("*Overload")]
    [SerializeField] Slider overloadSilder;
    [SerializeField] TMP_Text overloadText;
    [SerializeField] Color overloadBarColor;

    [Header("*Other")]
    [SerializeField] Button nextDayBtn;

    [HideInInspector] public StreamEvent currentStreamEvent = new StreamEvent();


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


        /*
        ScenarioBase
            .Where(Base => Base == null)
            .Subscribe(_ =>
            {
                // 여긴 예외처리 안해도 될듯
            });
        */
    }

    private void AnimationSetup(SpineAniState state)
    {
        // 애니메이션 출력
        switch (state)
        {
            case SpineAniState.A01: // 인사하고 Idle
                skeletonGraphic.AnimationState.SetAnimation(trackIndex: 0, "안녕", loop: false);
                skeletonGraphic.AnimationState.SetAnimation(trackIndex: 1, "ible", loop: true);
                break;
            case SpineAniState.A02: // Idle
                skeletonGraphic.AnimationState.SetAnimation(trackIndex: 0, "ible", loop: true);
                break;
            case SpineAniState.A03: // 깜찍한 표정
                skeletonGraphic.AnimationState.SetAnimation(trackIndex: 0, "><", loop: true);
                break;
            case SpineAniState.A04: // 웃는 표정
                skeletonGraphic.AnimationState.SetAnimation(trackIndex: 0, "웃음!", loop: true);
                break;
            case SpineAniState.A05: // 화난 표정
                skeletonGraphic.AnimationState.SetAnimation(trackIndex: 0, "음!", loop: false);
                skeletonGraphic.AnimationState.SetAnimation(trackIndex: 1, "음! ible", loop: true);
                break;
            case SpineAniState.A06: // 화난 표정 Idle
                skeletonGraphic.AnimationState.SetAnimation(trackIndex: 0, "음! ible", loop: true);
                break;
            case SpineAniState.A07: // 콧수염
                skeletonGraphic.AnimationState.SetAnimation(trackIndex: 0, "콧수염", loop: false);
                skeletonGraphic.AnimationState.SetAnimation(trackIndex: 1, "콧수염 ible", loop: true);
                break;
            case SpineAniState.A08: // 콧수염 Idle
                skeletonGraphic.AnimationState.SetAnimation(trackIndex: 1, "콧수염 ible", loop: true);
                break;
        }
    }

    public IEnumerator DialogTexting(ScenarioBase scenarioBase)
    {
        streamScriptText.text = null;

        for (int i = 0; i < scenarioBase.Fragments.Count; i++)
        {
            int temp = i;
            var sequence = DOTween.Sequence();
            streamScriptText.text = null;

            skeletonGraphic.AnimationState.SetEmptyAnimations(0);
            AnimationSetup((SpineAniState)System.Enum.Parse(typeof(SpineAniState), scenarioBase.Fragments[temp].animationID));

            Fragment newFragment = scenarioBase.Fragments[temp];
            sequence.Append(streamScriptText.DOText(newFragment.Script, newFragment.Script.Length / 10)
                    .SetEase(Ease.Linear)
                    .OnUpdate(() =>
                    {
                        if (click.triggered)
                        {
                            sequence.Complete();
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
                if (click.triggered)
                {
                    return true;
                }
                return false;
            });
            continue;
        }

        // 행동 있는지 확인
        // 행동 있으면 검은화면 껐다 켜지고 행동지문 출력
        // 행동 없으면 검은화면 껐다 켜지고 결과값 보여주기

        // dialog.SetActive(false);
        // GameSystem.StartGame();
        // 다이얼로그가 끝나면 게이지 결과값 보여주고 다음 날 시작

        StartCoroutine(ResultWindowOn());
    }

    IEnumerator ResultWindowOn()
    {
        ClearGageAndText();

        CanvasGroup CG = resultWindow.GetComponent<CanvasGroup>();
        CG.alpha = 0.0f;
        CG.DOFade(1, showTime);
        yield return new WaitForSeconds(showTime);

        // 게이지 출력
        EffectGage(stressSlider, GameManager.currentMainInfo.stressGage, showTime, stressBarColor);
        EffectGage(angerSlider, GameManager.currentMainInfo.angerGage, showTime, angerBarColor);
        EffectGage(riskSlider, GameManager.currentMainInfo.riskGage, showTime, riskBarColor);
        yield return new WaitForSeconds(showTime);

        // 수치(text) 출력
        IncOpacityText(stressText, GameManager.currentMainInfo.stressGage, currentStreamEvent.stressValue, showTime);
        IncOpacityText(angerText, GameManager.currentMainInfo.angerGage, currentStreamEvent.angerValue, showTime);
        IncOpacityText(riskText, GameManager.currentMainInfo.riskGage, currentStreamEvent.riskValue, showTime);
        yield return new WaitForSeconds(showTime);

        // 과부하 출력
        resultWindowOverloadGage.transform.DOLocalMoveY(-375f, showTime);
        resultWindowGage.transform.DOLocalMoveY(60f, showTime);
        yield return new WaitForSeconds(showTime);

        EffectGage(overloadSilder, GameManager.currentMainInfo.overloadGage, showTime, overloadBarColor);
        yield return new WaitForSeconds(showTime);
        IncOpacityText(overloadText, GameManager.currentMainInfo.overloadGage, currentStreamEvent.OverloadValue, showTime);
        yield return new WaitForSeconds(showTime);

        // 다음날로 가는 버튼 출력
        nextDayBtn.gameObject.SetActive(true);
        nextDayBtn.image.DOFade(1, showTime).SetEase(Ease.OutSine);

        void ClearGageAndText()
        {
            nextDayBtn.gameObject.SetActive(false);
            nextDayBtn.image.color = new Color(0, 0, 0, 0);
            stressText.alpha = 0;
            angerText.alpha = 0;
            riskText.alpha = 0;
            overloadText.alpha = 0;
            resultWindowGage.GetComponent<RectTransform>().localPosition = Vector3.zero;
            resultWindowOverloadGage.GetComponent<RectTransform>().localPosition = Vector3.zero;
            resultWindow.SetActive(true); //결과창
        }
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
    }
    

    public enum SpineAniState
    {
        A01,
        A02,
        A03,
        A04,
        A05,
        A06,
        A07,
        A08
    }

}