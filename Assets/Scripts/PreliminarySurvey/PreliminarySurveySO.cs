using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Preliminary Survey SO", menuName = "Scriptable Object/Preliminary Survey SO", order = int.MaxValue)]
public class PreliminarySurveySO : ScriptableObject
{
    [SerializeField] public string answerNum;
    [SerializeField] public GameObject[] clues = new GameObject[8];
    [SerializeField] public cutsceneSO cutsceneSO;
    [SerializeField] public string getID;
}
