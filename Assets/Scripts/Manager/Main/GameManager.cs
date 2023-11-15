using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class GameManager : MonoBehaviour
{
    // 디버깅용 보여주기 나중에 hideInspactor
    [Header("*Word")]
    [SerializeField] public WordData[] wordDatas;

    [Header("*Gage")]
    [ProgressBar("StressGage", 100, EColor.Red)]
    public int stressGage = 50;
    [ProgressBar("AngerGage", 100, EColor.Green)]
    public int angerGage = 50;
    [ProgressBar("RiskGage", 100, EColor.Blue)]
    public int riskGage = 50;

    private void Start()
    {
        wordDatas = WordCSVReader.Parse("TestCVSFile2");
    }
}
