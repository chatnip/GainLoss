using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // ������ �����ֱ� ���߿� hideInspactor
    [SerializeField] public WordData[] wordDatas;

    private void Start()
    {
        wordDatas = WordCSVReader.Parse("TestCVSFile2");
    }
}
