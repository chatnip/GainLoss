using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class GameManager : MonoBehaviour
{
    [Header("*Gage")]
    [ProgressBar("StressGage", 100, EColor.Red)]
    public int stressGage = 0;
    [ProgressBar("AngerGage", 100, EColor.Green)]
    public int angerGage = 0;
    [ProgressBar("RiskGage", 100, EColor.Blue)]
    public int riskGage = 0;

    [Header("*Day")]
    public int CurrentDay = 1;

    private void Start()
    {
        // wordDatas = WordCSVReader.Parse("TestCVSFile2");
    }
}
