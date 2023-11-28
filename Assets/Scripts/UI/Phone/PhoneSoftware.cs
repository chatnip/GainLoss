using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class PhoneSoftware : MonoBehaviour
{
    [Header("*Software")]
    [SerializeField] Button lockScreen;
    [SerializeField] public Button mapBtn;
    [SerializeField] GameObject map;
    [SerializeField] Button wordpadBtn;
    [SerializeField] GameObject wordpad;
    [SerializeField] Button backBtn;

    private void Awake()
    {
        lockScreen
            .OnClickAsObservable()
            .Subscribe(btn =>
            {
                lockScreen.gameObject.SetActive(false);
            });

        mapBtn
            .OnClickAsObservable()
            .Subscribe(btn =>
            {
                map.SetActive(true);
            });

        wordpadBtn
            .OnClickAsObservable()
            .Subscribe(btn =>
            {
                wordpad.SetActive(true);
            });
        
        backBtn
            .OnClickAsObservable()
            .Subscribe(btn =>
            {
                map.SetActive(false);
                wordpad.SetActive(false);
            });       
    }

    private void OnEnable()
    {
        lockScreen.gameObject.SetActive(true);
    }
}
