using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New InteractObject", menuName = "Prototype/Create New InteractObject")]
public class InteractObjectBase : ScriptableObject
{
    [Header("*Base")]
    [SerializeField] string interactObjectName;
    [TextArea]
    [SerializeField] string interactObjectInfo;
    [SerializeField] int maxZoomValue;
    [SerializeField] int minZoomValue;
    [SerializeField] Vector3 wordColPos;
    [SerializeField] Vector3 wordColSize;

    [Header("*Mesh")]
    [SerializeField] Mesh interactBaseMesh;
    [SerializeField] Material interactBaseMaterial;
    [SerializeField] Material interactWordMaterial;

    public string InteractObjectName
    {
        get { return interactObjectName; }
    }
    public string InteractObjectInfo
    {
        get { return interactObjectInfo; }
    }
    public Vector3 WordColPos
    {
        get { return wordColPos; }
    }
    public Vector3 WordColSize
    {
        get { return wordColSize; }
    }
    public int MaxZoomValue
    {
        get { return maxZoomValue; }
    }
    public int MinZoomValue
    {
        get { return minZoomValue; }
    }

    public Mesh InteractBaseMesh
    {
        get { return interactBaseMesh; }
    }
    public Material InteractBaseMaterial
    {
        get { return interactBaseMaterial; }
    }
    public Material InteractWordMaterial
    {
        get { return interactWordMaterial; }
    }
}
