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
    [SerializeField] Slider StressSlider;
    [SerializeField] TMP_Text StressText;
    [SerializeField] Slider AngerSlider;
    [SerializeField] TMP_Text AngerText;
    [SerializeField] Slider RiskSlider;
    [SerializeField] TMP_Text RiskText;
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
                // ���� ����ó�� ���ص� �ɵ�
            });
        */
    }

    private void AnimationSetup()
    {
        // �ִϸ��̼� ���
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

        // �ൿ �ִ��� Ȯ��
        // �ൿ ������ ����ȭ�� ���� ������ �ൿ���� ���
        // �ൿ ������ ����ȭ�� ���� ������ ����� �����ֱ�

        // dialog.SetActive(false);
        // GameSystem.StartGame();
        // ���̾�αװ� ������ ������ ����� �����ְ� ���� �� ����
        float showGageTime = 0.75f;
        ResultWindow.SetActive(true);
        EffectGage(StressSlider, gameManager.stressGage, showGageTime);
        yield return new WaitForSeconds(showGageTime);
        EffectGage(AngerSlider, gameManager.angerGage, showGageTime);
        yield return new WaitForSeconds(showGageTime);
        EffectGage(RiskSlider, gameManager.riskGage, showGageTime);
        yield return new WaitForSeconds(showGageTime);
        IncOpacity(StressText, gameManager.stressGage, streamEvent.stressValue, showGageTime);
        IncOpacity(AngerText, gameManager.angerGage, streamEvent.angerValue, showGageTime);
        IncOpacity(RiskText, gameManager.riskGage, streamEvent.riskValue, showGageTime);
    }
    public void EffectGage(Slider slider, int appliedGage, float time)
    {
        var sequence = DOTween.Sequence();
        sequence.Append(DOTween.To(() => slider.value, x => slider.value = x, (appliedGage * 0.01f), time))
                    .SetEase(Ease.OutSine);
    }
    public void IncOpacity(TMP_Text text, int appliedGage, int incGage, float time)
    {
        string gageAmount = appliedGage + " ( +" + incGage + " )";
        text.text = gageAmount;
        var sequence = DOTween.Sequence();
        sequence.Append(DOTween.To(() => text.color, x => text.color = x, new Color(0, 0, 0, 1), time))
                    .SetEase(Ease.OutSine);
    }
}