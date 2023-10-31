using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class Phone : MonoBehaviour
{
    [SerializeField] Button lockScreen;
    [SerializeField] Button todoBtn;
    [SerializeField] GameObject calender;
    [SerializeField] Button backBtn;

    private void Awake()
    {
        lockScreen
            .OnClickAsObservable()
            .Subscribe(btn =>
            {
                lockScreen.gameObject.SetActive(false);
            });

        todoBtn
            .OnClickAsObservable()
            .Subscribe(btn =>
            {
                calender.SetActive(true);
            });

        backBtn
            .OnClickAsObservable()
            .Subscribe(btn =>
            {
                calender.SetActive(false);
            });
    }
}
