using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    public void PhoneOn()
    {
        this.gameObject.SetActive(true);

        phone2DCamera.SetActive(true);
        phoneScreen.SetActive(true);
        phoneViewCamera.SetActive(true);
        quarterViewCamera.SetActive(false);

        phoneSoftware.ResetUI();

        InteractionUI3D.SetActive(false);
    }

    public void PhoneOff()
    {
        phone2DCamera.SetActive(false);
        phoneScreen.SetActive(false);
        phoneViewCamera.SetActive(false);
        quarterViewCamera.SetActive(true);

        this.gameObject.SetActive(false);

        InteractionUI3D.SetActive(true);
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
