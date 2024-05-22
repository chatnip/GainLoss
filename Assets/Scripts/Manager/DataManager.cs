using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*#if UNITY_EDITOR
using UnityEditor;
#endif*/

public class DataManager : Singleton<DataManager>
{
    [Header("*All_CSV_File")]
    [SerializeField] TextAsset StreamEventsFile;
    [SerializeField] TextAsset TitlesFile;
    [SerializeField] TextAsset BasicDialogsFile;
    [SerializeField] TextAsset DialogsFile;
    [SerializeField] TextAsset WordsFile;
    [SerializeField] TextAsset WordActionsFile;
    [SerializeField] TextAsset PlaceFile;
    [SerializeField] TextAsset ScheduleFile;
    [SerializeField] TextAsset ObjectEventFile;

    [Header("*AllData")]
    public static List<Dictionary<string, object>> StreamEventDatas = new();
    public static List<Dictionary<string, object>> TitleDatas = new();
    public static List<Dictionary<string, object>> BasicDialogDatas = new();
    public static List<Dictionary<string, object>> DialogDatas = new();
    public static List<Dictionary<string, object>> WordDatas = new();
    public static List<Dictionary<string, object>> WordActionDatas = new();
    public static List<Dictionary<string, object>> PlaceDatas = new();
    public static List<Dictionary<string, object>> ScheduleDatas = new();
    public static List<Dictionary<string, object>> ObjectEventData = new();


    protected override void Awake()
    {
        base.Awake();
        InitData();
    }

    public void InitData()
    {
        #region original

        StreamEventDatas = CSVReader.Read(this.StreamEventsFile);
        // [0]:IsCreate | [1]:Value

        TitleDatas = CSVReader.Read(this.TitlesFile);
        // [0]:TitleName | [1]:TitleNameKor

        BasicDialogDatas = CSVReader.Read(this.BasicDialogsFile); // WA**T**D**
        // [0]:AnimationID | [1]:Dialog | [2]:DialogKor

        DialogDatas = CSVReader.Read(this.DialogsFile);
        // [0]:AnimationID | [1]:Dialog | [2]:DialogKor

        WordDatas = CSVReader.Read(this.WordsFile);
        // [0]:WordName | [1]:WordRate | [2]:Stress | [3]:Anger | [4]:Risk | [5]:WordNameKor

        WordActionDatas = CSVReader.Read(this.WordActionsFile);
        // [0]:WordActionName | [1]:WordActionRate | [2]:OverloadingGage | [3]:WordActionNameKor

        PlaceDatas = CSVReader.Read(this.PlaceFile);
        // [0]:PlaceName | [1]:PlaceNameKor

        ScheduleDatas = CSVReader.Read(this.ScheduleFile); 
        // [0]:Name | [1]:firstOfAll | [2]:ScheduleExplanation | [3]:KorName | [4]ScheduleExplanationKor

        ObjectEventData = CSVReader.Read(this.ObjectEventFile);
        // [0]:1st | [1]:2nd | [2]:3rd | [3]:Type | [4]:GetID | [5]:IsUseScene | [6]:SceneID 

        #endregion

    }


}
