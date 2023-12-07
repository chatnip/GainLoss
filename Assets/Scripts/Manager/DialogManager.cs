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
    [SerializeField] GameManager GameManager;
    [SerializeField] GameSystem GameSystem;

    [Header("*Dialog")]
    [SerializeField] public TMP_Text streamTitleText;
    [SerializeField] TMP_Text streamScriptText;
    [SerializeField] Button dialogNextBtn;
    [SerializeField] InputAction click;
    [SerializeField] public ReactiveProperty<ScenarioBase> ScenarioBase = new();

    [Header("*Result")]
    [SerializeField] float showTime = 0.8f; //Dotween �ð���
    [SerializeField] GameObject resultWindow;
    [SerializeField] Color baseColor;

    [SerializeField] Slider stressSlider;
    [SerializeField] TMP_Text stressText;
    [SerializeField] Color stressBarColor;
    
    [SerializeField] Slider angerSlider;
    [SerializeField] TMP_Text angerText;
    [SerializeField] Color angerBarColor;

    [SerializeField] Slider riskSlider;
    [SerializeField] TMP_Text riskText;
    [SerializeField] Color riskBarColor;

    [SerializeField] Button nextDayBtn;
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

        StartCoroutine(ResultWindowOn());
    }

    IEnumerator ResultWindowOn()
    {
        ClearGageAndText();

        resultWindow.SetActive(true); //���â

        // ������ ���ʷ� ���
        EffectGage(stressSlider, GameManager.stressGage, showTime, stressBarColor);
        yield return new WaitForSeconds(showTime);
        EffectGage(angerSlider, GameManager.angerGage, showTime, angerBarColor);
        yield return new WaitForSeconds(showTime);
        EffectGage(riskSlider, GameManager.riskGage, showTime, riskBarColor);
        yield return new WaitForSeconds(showTime);

        // ��ġ(text) ���
        IncOpacityText(stressText, GameManager.stressGage, streamEvent.stressValue, showTime);
        IncOpacityText(angerText, GameManager.angerGage, streamEvent.angerValue, showTime);
        IncOpacityText(riskText, GameManager.riskGage, streamEvent.riskValue, showTime);
        yield return new WaitForSeconds(showTime);

        // �������� ���� ��ư ���
        nextDayBtn.gameObject.SetActive(true);
        nextDayBtn.image.DOFade(1, showTime).SetEase(Ease.OutSine);

    }
    void ClearGageAndText()
    {
        stressSlider.value = 0.0f;
        angerSlider.value = 0.0f;
        riskSlider.value = 0.0f;
        stressText.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        angerText.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        riskText.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
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
        string gageAmount = appliedGage + " ( +" + incGage + " )";
        text.text = gageAmount;
        text.DOFade(1, time).SetEase(Ease.OutSine);
    }
    
}