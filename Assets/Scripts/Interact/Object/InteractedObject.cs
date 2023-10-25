using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;

public class InteractedObject : MonoBehaviour
{
    [Header("*Property")]
    [SerializeField] WordManager WordManager;

    [Header("*Mesh")]
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] Mesh baseMesh;
    [SerializeField] MeshCollider meshCollider;

    [Header("*Material")]
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Material baseMaterial;
    [SerializeField] Material wordMaterial;

    [Header("*Base")] 

    [SerializeField] private int maxZoomValue;
    [SerializeField] private int minZoomValue;

    [SerializeField] private InputAction pressed, axis, wheel;
    [SerializeField] InputAction wordClick;

    [HideInInspector] public InteractObjectBase interactObjectBase;

    // [SerializeField] bool isWordFind = false;

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
        int layerMask = 1 << LayerMask.NameToLayer("WordObject");
        Debug.DrawRay(ray.origin, ray.direction * 20, Color.red);
        if (Physics.Raycast(ray, out RaycastHit hit, 100, layerMask))
        {
            
            hit.transform.TryGetComponent(out MeshRenderer rend);
            Texture2D tex = rend.materials[1].GetTexture("_WordTexture") as Texture2D;       
            Vector2 pixelUV = hit.textureCoord;
            pixelUV.x *= tex.width;
            pixelUV.y *= tex.height;
            Color color = tex.GetPixel((int)pixelUV.x, (int)pixelUV.y);
            if (color.r > 0 || color.g > 0 || color.b > 0)
            {
                Debug.Log("단어를 찾음");
                Debug.Log("laycastname" + hit.transform.name);
                string name = interactObjectBase.InteractObjectName;
                // WordManager.wordList.Add(name);
                WordManager.wordText.text = name;
            }        
        }

        /*
        Ray ray = Camera.main.ScreenPointToRay(cursorVector);

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
        */
    }
}