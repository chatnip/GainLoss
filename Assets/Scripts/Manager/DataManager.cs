using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*#if UNITY_EDITOR
using UnityEditor;
#endif*/

public class DataManager : Manager<DataManager>
{
    [Header("*All_CSV_File")]
    [SerializeField] TextAsset ActionEventsFile;
    [SerializeField] TextAsset StreamEventsFile;
    [SerializeField] TextAsset TitlesFile;
    [SerializeField] TextAsset BasicDialogsFile;
    [SerializeField] TextAsset DialogsFile;
    [SerializeField] TextAsset WordsFile;
    [SerializeField] TextAsset WordActionsFile;
    [SerializeField] TextAsset BehaviorActionsFile;
    [SerializeField] TextAsset PlaceFile;

    [Header("*AllData")]
    public static List<Dictionary<string, object>> ActionEventDatas = new();
    public static List<Dictionary<string, object>> StreamEventDatas = new();
    public static List<Dictionary<string, object>> TitleDatas = new();
    public static List<Dictionary<string, object>> BasicDialogDatas = new();
    public static List<Dictionary<string, object>> DialogDatas = new();
    public static List<Dictionary<string, object>> WordDatas = new();
    public static List<Dictionary<string, object>> WordActionDatas = new();
    public static List<Dictionary<string, object>> BehaviorActionDatas = new();
    public static List<Dictionary<string, object>> PlaceDatas = new();

    protected override void Awake()
    {
        base.Awake();
        InitData();
    }

    public void InitData()
    {
        #region original

        ActionEventDatas = CSVReader.Read(this.ActionEventsFile);
        // [0]:MinVisitValue | [1]:GetWordID | [2]:GetBehaviorActionID | [3]:GetPlaceID
        StreamEventDatas = CSVReader.Read(this.StreamEventsFile);
        // [0]:Used | [1]:StressGage | [2]:AngerGage | [3]:RiskGage | [4]:StreamTitle | [5]:StreamTitleKor
        // => [0]:IsCreate | [1]:Value
        TitleDatas = CSVReader.Read(this.TitlesFile);
        // [0]:TitleName | [1]:TitleNameKor
        BasicDialogDatas = CSVReader.Read(this.BasicDialogsFile); // WA**T**D**
        // [0]:AnimationID | [1]:Dialog | [2]:DialogKor
        DialogDatas = CSVReader.Read(this.DialogsFile);
        // [0]:AnimationID | [1]:Dialog | [2]:DialogKor
        WordDatas = CSVReader.Read(this.WordsFile);
        // [0]:WordName | [1]:WordNameKor
        // => [0]:WordName | [1]:WordRate | [2]:Stress | [3]:Anger | [4]:Risk | [5]:WordNameKor
        WordActionDatas = CSVReader.Read(this.WordActionsFile);
        // [0]:WordActionName | [1]:WordActionNameKor
        // => [0]:WordActionName | [1]:WordActionRate | [2]:OverloadingGage | [3]:WordActionNameKor
        BehaviorActionDatas = CSVReader.Read(this.BehaviorActionsFile);
        // [0]:BehaviorActionName | [1]:BehaviorActionNameKor
        PlaceDatas = CSVReader.Read(this.PlaceFile);
        // [0]:PlaceName | [1]:PlaceNameKor

        #endregion

    }


}
