using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class Desktop : MonoBehaviour
{
    [Header("*SNS")]
    [SerializeField] Button snsOpenBtn;
    [SerializeField] Button snsExitBtn;
    [SerializeField] GameObject snsWindow;

    [Header("*FanCafe")]
    [SerializeField] Button fancafeOpenBtn;
    [SerializeField] Button fancafeExitBtn;
    [SerializeField] GameObject fancafeWindow;

    [Header("*Stream")]
    [SerializeField] Button streamOpenBtn;
    [SerializeField] Button streamPopupExitBtn;
    [SerializeField] Button streamConfirmBtn;
    [SerializeField] GameObject streamPopup;

    [Header("*Todo")]
    [SerializeField] Button todoOpenBtn;
    [SerializeField] Button todoExitBtn;
    [SerializeField] GameObject todoWindow;

    private void Awake()
    {
        /*
        snsOpenBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {

            });

        fancafeOpenBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {

            });
        */

        streamOpenBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                streamPopup.SetActive(true);
            });

        streamPopupExitBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                streamPopup.SetActive(false);
            });

        
        todoOpenBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                todoWindow.SetActive(true);
            });

        todoExitBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                todoWindow.SetActive(false);
            });
    }
}
