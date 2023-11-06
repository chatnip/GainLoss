using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 디버깅용 보여주기 나중에 hideInspactor
    [SerializeField] public WordData[] wordDatas;

    private void Start()
    {
        wordDatas = WordCSVReader.Parse("TestCVSFile2");
    }
}
