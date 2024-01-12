using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PhoneHardware : MonoBehaviour
{
    [Header("*Hardware")]
    [SerializeField] GameObject phoneScreen;
    [SerializeField] GameObject phone2DCamera;
    [SerializeField] PhoneSoftware phoneSoftware;
    [SerializeField] GameObject phoneViewCamera;
    [SerializeField] GameObject quarterViewCamera;
    [SerializeField] GameObject lockScreen;

    [Header("*UICanvas")]
    [SerializeField] GameObject InteractionUI3D;
    [SerializeField] CanvasGroup Schedules;
    [SerializeField] GameObject ScheduleUIs;

    [Header("*On/Off Btns")]
    [SerializeField] Button PhoneOnOffBtn;

    public void Awake()
    {
        PhoneOnOffBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                if(!this.gameObject.activeSelf)
                {
                    PhoneOn();
                }
            });

    }

    public void PhoneOn()
    {
        this.gameObject.SetActive(true);

        phone2DCamera.SetActive(true);
        phoneScreen.SetActive(true);
        phoneViewCamera.SetActive(true);
        quarterViewCamera.SetActive(false);

        phoneSoftware.ResetUI();

        InteractionUI3D.SetActive(false);

        Schedules.alpha = 0f;
        PhoneOnOffBtn.gameObject.SetActive(false);
    }

    public void PhoneOff()
    {
        phone2DCamera.SetActive(false);
        phoneScreen.SetActive(false);
        phoneViewCamera.SetActive(false);
        quarterViewCamera.SetActive(true);

        this.gameObject.SetActive(false);

        InteractionUI3D.SetActive(true);

        Schedules.alpha = 1f;
        PhoneOnOffBtn.gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        PhoneOn();
    }

    private void OnDisable()
    {
        PhoneOff();
    }
}
