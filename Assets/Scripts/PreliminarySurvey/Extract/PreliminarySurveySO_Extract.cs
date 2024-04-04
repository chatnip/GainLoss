using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PSSO_E", menuName = "Scriptable Object/PSSO/Extract", order = int.MaxValue)]
[System.Serializable]
public class PreliminarySurveySO_Extract : PreliminarySurveySO
{
    [SerializeField] public int[,] tempArray = new int[8, 12];
    [SerializeField] public TileLine[] tileArray;
    [SerializeField] public int GoalPoint;
    [SerializeField] public int GetPoint_OnceTime;

    [Serializable]
    public class TileLine
    {
        public int[] LineIndex = new int[12];
    }
}

