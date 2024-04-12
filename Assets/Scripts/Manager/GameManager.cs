using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

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

    private void Awake()
    {
        currentMainInfo.NewGame = false;
    }



}

[Serializable]
public class MainInfo
{
    public bool NewGame = true;
    public int day = 1;
    public string TodayOfTheWeek = "Monday";
    public int money = 100;

    public int stressGage = 0;
    public int angerGage = 0;
    public int riskGage = 0;
    public int overloadGage = 0;
}


