using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Title : MonoBehaviour
{

    [Header("*TitleBtn")]
    [SerializeField] Button newGameBtn;
    [SerializeField] Button continueBtn;
    [SerializeField] Button OptionBtn;
    [SerializeField] Button QuitBtn;

    [Header("*Window")]
    [SerializeField] Image BlackScreenImg;
    [SerializeField] GameObject OptionWindow;

    private void Awake()
    {
        newGameBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                BlackScreenImg.gameObject.SetActive(true);
                BlackScreenImg.DOFade(1.0f, 0.7f)
                    .SetEase(Ease.InOutBack)
                    .OnComplete(() =>
                    {
                        SceneManager.LoadScene("Main");
                    });
            });
        continueBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                Debug.Log("Need Update");
            });
        OptionBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                EffectfulWindow.AppearEffectful(OptionWindow.GetComponent<RectTransform>(), 0.2f, 0.0f, Ease.InOutBack);
            });
        QuitBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                Application.Quit();
            });

    }
    private void OnEnable()
    {
        BlackScreenImg.gameObject.SetActive(true);
        EffectfulWindow.AppearEffectful(this.GetComponent<RectTransform>(), 1f, 0.5f, Ease.InOutBack);
        BlackScreenImg.DOFade(0.0f, 0.5f)
            .SetEase(Ease.InOutBack)
            .OnComplete(() =>
            {
                BlackScreenImg.gameObject.SetActive(false);
            });
    }
}
