using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneHardware : MonoBehaviour
{
    [Header("*Hardware")]
    [SerializeField] GameObject phoneScreen;
    [SerializeField] GameObject phone2DCamera;
    [SerializeField] GameObject phoneViewCamera;
    [SerializeField] GameObject quarterViewCamera;


    private void OnEnable()
    {
        phone2DCamera.SetActive(true);
        phoneViewCamera.SetActive(true);
        quarterViewCamera.SetActive(false);
    }

    private void OnDisable()
    {
        phone2DCamera.SetActive(false);
        phoneViewCamera.SetActive(false);
        quarterViewCamera.SetActive(true);
    }
}
