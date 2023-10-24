using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractedObject : MonoBehaviour
{
    [Header("*Mesh")]
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] Mesh baseMesh;
    [SerializeField] MeshCollider meshCollider;

    [Header("*Material")]
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Material baseMaterial;
    [SerializeField] Material wordMaterial;

    [Header("*Base")] 
    [SerializeField] Rigidbody rigid;
    [SerializeField] BoxCollider wordCollider;

    [SerializeField] private int maxZoomValue;
    [SerializeField] private int minZoomValue;

    [SerializeField] private InputAction pressed, axis, wheel;
    [SerializeField] InputAction wordClick;

    [HideInInspector] public InteractObjectBase interactObjectBase;

    private Vector2 cursorVector;
    private bool rotateAllowed;  
    private float rotateSpeed = 100f;
    private float zoomValue;

    private void OnEnable()
    {
        ObjectSetting();
        EnableObjectInput();
    }

    private void OnDisable()
    {
        DisableObjectInput();
    }

    private void ObjectSetting()
    {
        this.baseMesh = interactObjectBase.InteractBaseMesh;
        this.baseMaterial = interactObjectBase.InteractBaseMaterial;
        this.wordMaterial = interactObjectBase.InteractWordMaterial;
        this.minZoomValue = interactObjectBase.MinZoomValue;
        this.maxZoomValue = interactObjectBase.MaxZoomValue;
        meshFilter.mesh = baseMesh;
        meshRenderer.materials = new Material[2] { baseMaterial, wordMaterial };
        meshCollider.sharedMesh = baseMesh;
        wordCollider.gameObject.transform.localPosition = interactObjectBase.WordColPos;
        wordCollider.gameObject.transform.rotation = Quaternion.identity;
        wordCollider.gameObject.transform.localScale = Vector3.one;
        wordCollider.size = interactObjectBase.WordColSize;
    }

    private void EnableObjectInput()
    {
        pressed.Enable();
        axis.Enable();
        wheel.Enable();
        wordClick.Enable();
        pressed.performed += _ => { rotateAllowed = true; };
        pressed.canceled += _ => { rotateAllowed = false; };
        axis.performed += context =>
        {
            cursorVector = context.ReadValue<Vector2>();
        };
        wheel.performed += context =>
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
        wordClick.started += _ =>
        {
            RaycastCursor();
        };
    }

    private void DisableObjectInput()
    {
        pressed.Disable();
        axis.Disable();
        wheel.Disable();
        pressed.performed -= _ => { Rotate(); };
        pressed.canceled -= _ => { rotateAllowed = false; };
        axis.performed -= context => { cursorVector = context.ReadValue<Vector2>(); };
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
        wordClick.started -= _ =>
        {
            RaycastCursor();
        };
    }

    private void Rotate()
    {
        if(rotateAllowed)
        {
            float x = cursorVector.x * rotateSpeed * Time.deltaTime;
            float y = cursorVector.y * rotateSpeed * Time.deltaTime;

            transform.Rotate(Vector3.down * x);
            transform.Rotate(Vector3.right * y);
        }
    }

    private void FixedUpdate()
    {
        Rotate();
    }

    private void Zoom(float value)
    {
        transform.localScale = new Vector3(
            transform.localScale.x + value,
            transform.localScale.y + value,
            transform.localScale.z + value);
    }

    private void RaycastCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Debug.DrawRay(ray.origin, ray.direction * 1000, Color.blue);
        int layerMask = 1 << LayerMask.NameToLayer("WordObject");
        if (Physics.Raycast(ray, out RaycastHit hit, 1000, layerMask))
        {
            Debug.Log("d" + hit.collider.gameObject.name);

            if (hit.collider != null)
            {
                if (hit.collider.gameObject.CompareTag("ClickableInteract"))
                {
                    Debug.Log("hit" + hit.collider.gameObject.name);
                }
            }
            if (hit.rigidbody != null)
            {
                if (hit.rigidbody.gameObject.CompareTag("ClickableInteract"))
                {
                    Debug.Log("col" + hit.rigidbody.gameObject.name);
                }
            }
        }


    }

}