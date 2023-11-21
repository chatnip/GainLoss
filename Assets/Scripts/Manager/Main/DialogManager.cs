using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DG.Tweening;
using UniRx;
using TMPro;

public class DialogManager : MonoBehaviour
{
    [Header("*Dialog")]
    [SerializeField] public TMP_Text streamTitleText;
    [SerializeField] TMP_Text streamScriptText;
    [SerializeField] Button dialogNextBtn;
    [SerializeField] InputAction click;
    [SerializeField] public ReactiveProperty<ScenarioBase> ScenarioBase = new();

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

        ScenarioBase
            .Where(Base => Base == null)
            .Subscribe(_ =>
            {
                // ���̾�αװ� ������ ������ ����� �����ְ� ���� �� ����
            });
    }

    private void TitleSetup()
    {
        // ��� ���� �ٲٱ�
    }

    private void AnimationSetup()
    {
        // �ִϸ��̼� ���
    }

    public IEnumerator DialogTexting(ScenarioBase scenarioBase)
    {
        Debug.Log("Texting");
        streamScriptText.text = null;
        // TitleSetup();
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
        // dialog.SetActive(false);
        // GameSystem.StartGame();
        // ���̾�αװ� ������ ������ ����� �����ְ� ���� �� ����
    }
}