using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Option : MonoBehaviour
{
    [Header("*Window")]
    [SerializeField] GameObject GameSetting;
    [SerializeField] GameObject AudioSetting;
    [SerializeField] GameObject VideoSetting;
    [SerializeField] GameObject CreditSetting;

    [Header("*OptionBtn")]
    [SerializeField] Button GameBtn;
    [SerializeField] Button AudioBtn;
    [SerializeField] Button VideoBtn;
    [SerializeField] Button CreditBtn;

    [Header("*SetBtn")]
    [SerializeField] Button ApplyBtn;
    [SerializeField] Button CancelBtn;




    private void Awake()
    {
        GameBtn.OnClickAsObservable()
            .Subscribe(x =>
            {

            });
        AudioBtn.OnClickAsObservable()
            .Subscribe(x =>
            {

            });
        VideoBtn.OnClickAsObservable()
            .Subscribe(x =>
            {

            });
        CreditBtn.OnClickAsObservable()
            .Subscribe(x =>
            {

            });


        ApplyBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                //옵션 내용 저장
                EffectfulWindow.DisappearEffectful(this.GetComponent<RectTransform>(), 0.2f, 0.0f, Ease.InOutBack);
            });
        CancelBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                EffectfulWindow.DisappearEffectful(this.GetComponent<RectTransform>(), 0.2f, 0.0f, Ease.InOutBack);
            });
    }

}
