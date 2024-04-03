using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PSSO_E", menuName = "Scriptable Object/PSSO/Extract", order = int.MaxValue)]
[System.Serializable]
public class PreliminarySurveySO_Extract : PreliminarySurveySO
{
    [SerializeField] public int[,] array = new int[8, 12];
    [SerializeField] public int GoalPoint;
    [SerializeField] public int GetPoint_OnceTime;
}