using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class Desktop : MonoBehaviour
{
    [SerializeField] StreamManager StreamManager;

    [Header("*SNS")]
    [SerializeField] Button snsOpenBtn;
    [SerializeField] Button snsExitBtn;
    [SerializeField] GameObject snsWindow;

    [Header("*FanCafe")]
    [SerializeField] Button fancafeOpenBtn;
    [SerializeField] Button fancafeExitBtn;
    [SerializeField] GameObject fancafeWindow;

    [Header("*Stream")]
    [SerializeField] public Button streamOpenBtn;
    [SerializeField] Button streamPopupExitBtn;
    [SerializeField] Button streamConfirmBtn;
    [SerializeField] GameObject streamPopup;
    [SerializeField] GameObject streamWindow;

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
        streamConfirmBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                streamWindow.SetActive(true);
                streamPopup.SetActive(false);
                StreamManager.StartDialog(StreamManager.currentStreamEventID);
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
