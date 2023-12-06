using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DG.Tweening;
using UniRx;
using TMPro;
using Unity.VisualScripting;

public class DialogManager : Manager<DialogManager>
{
    [SerializeField] GameManager gameManager;

    [Header("*Dialog")]
    [SerializeField] public TMP_Text streamTitleText;
    [SerializeField] TMP_Text streamScriptText;
    [SerializeField] Button dialogNextBtn;
    [SerializeField] InputAction click;
    [SerializeField] public ReactiveProperty<ScenarioBase> ScenarioBase = new();

    [Header("*Result")]
    [SerializeField] GameObject ResultWindow;
    [SerializeField] Color BaseColor;

    [SerializeField] Slider StressSlider;
    [SerializeField] TMP_Text StressText;
    [SerializeField] Color StressBarColor;
    
    [SerializeField] Slider AngerSlider;
    [SerializeField] TMP_Text AngerText;
    [SerializeField] Color AngerBarColor;

    [SerializeField] Slider RiskSlider;
    [SerializeField] TMP_Text RiskText;
    [SerializeField] Color RiskBarColor;

    [SerializeField] Button NextDayBtn;
    [HideInInspector] public StreamEvent streamEvent = new StreamEvent();
    

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

    private void AnimationSetup()
    {
        // 애니메이션 출력
    }

    public IEnumerator DialogTexting(ScenarioBase scenarioBase)
    {
        Debug.Log("Texting");
        streamScriptText.text = null;
        // AnimationSetup(); //scenarioBase.Fragments[0]

        for (int i = 0; i < scenarioBase.Fragments.Count; i++)
        {
            int temp = i;
            var sequence = DOTween.Sequence();
            streamScriptText.text = null;
            // AnimationSetup(); //scenarioBase.fragments[temp]
            Fragment newFragment = scenarioBase.Fragments[temp];
            sequence.Append(streamScriptText.DOText(newFragment.Script, newFragment.Script.Length / 5)
                    .SetEase(Ease.Linear)
                    .OnUpdate(() =>
                    {
                        if(click.triggered)
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

        #region Result Window

        float showGageTime = 0.8f; //Dotween 시간값

        ResultWindow.SetActive(true); //결과창

        // 게이지 차례로 출력
        EffectGage(StressSlider, gameManager.stressGage, showGageTime, StressBarColor);
        yield return new WaitForSeconds(showGageTime);
        EffectGage(AngerSlider, gameManager.angerGage, showGageTime, AngerBarColor);
        yield return new WaitForSeconds(showGageTime);
        EffectGage(RiskSlider, gameManager.riskGage, showGageTime, RiskBarColor);
        yield return new WaitForSeconds(showGageTime);
        
        // 수치(text) 출력
        IncOpacityText(StressText, gameManager.stressGage, streamEvent.stressValue, showGageTime);
        IncOpacityText(AngerText, gameManager.angerGage, streamEvent.angerValue, showGageTime);
        IncOpacityText(RiskText, gameManager.riskGage, streamEvent.riskValue, showGageTime);
        yield return new WaitForSeconds(showGageTime);

        // 다음날로 가는 버튼 출력
        NextDayBtn.gameObject.SetActive(true);
        NextDayBtn.image.DOFade(1, showGageTime).SetEase(Ease.OutSine);

        #endregion
    }

    void EffectGage(Slider slider, int appliedGage, float time, Color color)
    {
        var sequence = DOTween.Sequence();
        sequence.Append(DOTween.To(() => slider.value, x => slider.value = x, (appliedGage * 0.01f), time)).SetEase(Ease.OutSine);
        Image img = slider.transform.GetChild(1).GetChild(0).GetComponent<Image>();
        var sequence2 = DOTween.Sequence();
        img.color = BaseColor;
        Color LastColor = Color.Lerp(BaseColor, color, appliedGage * 0.01f);
        sequence2.Append(img.DOColor(LastColor, time)).SetEase(Ease.OutSine);
    }
    void IncOpacityText(TMP_Text text, int appliedGage, int incGage, float time)
    {
        string gageAmount = appliedGage + " ( +" + incGage + " )";
        text.text = gageAmount;
        text.DOFade(1, time).SetEase(Ease.OutSine);
    }
    
}