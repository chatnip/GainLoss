using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;

public class EditorTestManager : Manager<EditorTestManager>
{
    [Header("*Camera")]
    [Tooltip("카메라 변경 치트키")]
    [SerializeField] InputAction switchCamera;
    [SerializeField] GameObject shoulderViewCamera;
    [SerializeField] GameObject quaterViewCamera;

#if UNITY_EDITOR
    protected override void Awake()
    {
        
    }

    private void OnEnable()
    {
        switchCamera.Enable();
        switchCamera.started += _ =>
        {
            CameraMode();
        };
    }

    private void OnDisable()
    {
        switchCamera.Disable();
        switchCamera.started -= _ =>
        {
            CameraMode();
        };
    }

    private void CameraMode()
    {
        if (shoulderViewCamera.activeSelf == true)
        {
            shoulderViewCamera.SetActive(false);
            quaterViewCamera.SetActive(true);
        }
        else
        {
            shoulderViewCamera.SetActive(true);
            quaterViewCamera.SetActive(false);
        }
    }

#endif
}
