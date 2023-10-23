using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardFX : MonoBehaviour
{
    [SerializeField] Transform camTransform;
    Quaternion originalRotation;

    private void Start()
    {
        originalRotation = transform.rotation;
    }

    private void Update()
    {
        transform.rotation = camTransform.rotation * originalRotation;
    }
}
