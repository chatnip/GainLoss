using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DataManager : Manager<DataManager>
{
    [Header("*All_CSV_File")]
    [SerializeField] TextAsset ActionEventsFile;
    [SerializeField] TextAsset StreamEventsFile;
    [SerializeField] TextAsset WordsFile;
    [SerializeField] TextAsset WordActionsFile;
    [SerializeField] TextAsset BehaviorActionsFile;

    [Header("*AllData")]
    public static List<Dictionary<string, object>> ActionEventDatas = new();
    public static List<Dictionary<string, object>> StreamEventDatas = new();
    public static List<Dictionary<string, object>> WordDatas = new();
    public static List<Dictionary<string, object>> WordActionDatas = new();
    public static List<Dictionary<string, object>> BehaviorActionDatas = new();
    void Start()
    {
        InitData();
        // Debug.Log(WordDatas[0]["W01"]);
    }

    public void InitData()
    {
        ActionEventDatas = CSVReader.Read(this.ActionEventsFile);
        // [0]:MinVisitValue | [1]:GetWordID | [2]:GetBehaviorActionID
        StreamEventDatas = CSVReader.Read(this.StreamEventsFile);
        // [0]:MinVisitValue | [1]:GetWordID | [2]:GetBehaviorActionID
        WordDatas = CSVReader.Read(this.WordsFile);
        // [0]:WordName | [1]:WordNameKor
        WordActionDatas = CSVReader.Read(this.WordActionsFile);
        // [0]:WordActionName | [1]:WordActionNameKor
        BehaviorActionDatas = CSVReader.Read(this.BehaviorActionsFile);
        // [0]:BehaviorActionName | [1]:BehaviorActionNameKor
    }
}
