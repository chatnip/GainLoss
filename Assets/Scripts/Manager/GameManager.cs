using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;

[Serializable]
public class GameManager : MonoBehaviour
{
    /*[Header("*Gage")]
    [ProgressBar("StressGage", 100, EColor.Red)]
    public int stressGage = 0;
    [ProgressBar("AngerGage", 100, EColor.Green)]
    public int angerGage = 0;
    [ProgressBar("RiskGage", 100, EColor.Blue)]
    public int riskGage = 0;
    [ProgressBar("OverloadGage", 100, EColor.Gray)]
    public int OverloadGage = 0;

    [Header("*Day")]
    public int CurrentDay = 1;*/

    [HideInInspector] public MainInfo currentMainInfo = new MainInfo();

    private void Start()
    {
        // wordDatas = WordCSVReader.Parse("TestCVSFile2");
    }

}

[Serializable]
public class MainInfo
{
    public int day;
    public int stressGage;
    public int angerGage;
    public int riskGage;
    public int overloadGage;
}

