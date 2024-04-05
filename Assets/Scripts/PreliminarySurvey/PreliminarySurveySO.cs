using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PreliminarySurveySO : ScriptableObject
{
    [SerializeField] public cutsceneSO cutsceneSO;
    [SerializeField] public string getID;
    [SerializeField] public int conditionToOverload;
}
