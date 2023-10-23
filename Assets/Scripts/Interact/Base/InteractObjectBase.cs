using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New InteractObject", menuName = "Prototype/Create New InteractObject")]
public class InteractObjectBase : ScriptableObject
{
    [SerializeField] string interactObjectName;
    [TextArea]
    [SerializeField] string interactObjectInfo;

    public string InteractObjectName
    {
        get { return interactObjectName; }
    }
    public string InteractObjectInfo
    {
        get { return interactObjectInfo; }
    }
}
