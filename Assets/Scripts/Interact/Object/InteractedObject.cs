using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractedObject : MonoBehaviour
{
    [SerializeField] Rigidbody rigid;

    [SerializeField] private int maxZoomValue;
    [SerializeField] private int minZoomValue;

    [SerializeField] private InputAction pressed, axis, wheel;



    private Vector2 rotation;
    private bool rotateAllowed;  
    private float rotateSpeed = 500f;
    private bool zoomAllowed;
    private float zoomSpeed = 1f;
    private float zoomValue;

    private void OnEnable()
    {
        EnableObjectInput();
    }

    private void OnDisable()
    {
        DisableObjectInput();
    }

    private void EnableObjectInput()
    {
        pressed.Enable();
        axis.Enable();
        wheel.Enable();
        pressed.performed += _ => { rotateAllowed = true; };
        pressed.canceled += _ => { rotateAllowed = false; };
        axis.performed += context => { rotation = context.ReadValue<Vector2>(); };
        wheel.performed += context =>
        {
            zoomValue = context.ReadValue<float>();
            Debug.Log(transform.localScale.x);
            Debug.Log(zoomValue);
            if (zoomValue > 0 && transform.localScale.x < maxZoomValue)
            {
                Zoom(1);
            }
            else if (zoomValue < 0 && transform.localScale.x > minZoomValue)
            {
                Zoom(-1);
            }
        };
    }

    private void DisableObjectInput()
    {
        pressed.Disable();
        axis.Disable();
        wheel.Disable();
        pressed.performed -= _ => { Rotate(); };
        pressed.canceled -= _ => { rotateAllowed = false; };
        axis.performed -= context => { rotation = context.ReadValue<Vector2>(); };
        wheel.performed -= context =>
        {
            zoomValue = context.ReadValue<float>();
            if (zoomValue > 0 && transform.localScale.x < maxZoomValue)
            {
                Zoom(1);
            }
            else if (zoomValue < 0 && transform.localScale.x > minZoomValue)
            {
                Zoom(-1);
            }
        };
    }

    private void Rotate()
    {
        if(rotateAllowed)
        {
            float x = rotation.x * rotateSpeed * Time.deltaTime;
            float y = rotation.y * rotateSpeed * Time.deltaTime;

            rigid.AddTorque(Vector3.down * x);
            rigid.AddTorque(Vector3.right * y);
        }
    }

    private void FixedUpdate()
    {
        Rotate();
    }

    private void Zoom(float value)
    {
        Debug.Log("Ä¿¼­" + Cursor.visible);
        transform.localScale = new Vector3(
            transform.localScale.x + value,
            transform.localScale.y + value,
            transform.localScale.z + value);
    }
}