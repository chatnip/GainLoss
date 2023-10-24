using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TestMCode : MonoBehaviour
{
    [Header("*Mesh")]
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] Mesh baseMesh;

    [Header("*Material")]
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Material baseMaterial;
    [SerializeField] Material wordMaterial;

    private void Start()
    {
        MaterialSetting();
    }

    void MaterialSetting()
    {
        meshFilter.mesh = baseMesh;
        meshRenderer.materials = new Material[2] { baseMaterial, wordMaterial };
    }
}
