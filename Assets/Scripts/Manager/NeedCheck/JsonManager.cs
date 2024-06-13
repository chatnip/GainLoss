using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JsonManager : Singleton<JsonManager>
{
    #region Value

    [HideInInspector] public string json_filePath;

    [HideInInspector] public string json_tutorialFileName;
    [HideInInspector] public string json_mainInfoFileName;
    [HideInInspector] public string json_wordFileName;
    [HideInInspector] public string json_wordActionFileName;
    [HideInInspector] public string json_sentenceFileName;
    [HideInInspector] public string json_ScheduleFileName;
    [HideInInspector] public string json_PlaceFileName;
    [HideInInspector] public string json_PSFileName;

    [HideInInspector] public string json_SettingFileName;

    // public List<WordBase> myWords = new List<WordBase>();

    [SerializeField] TutorialManager TutorialManager;

    [Header("*Setting")]
    [SerializeField] GameSettingManager GameSettingManager;

    #endregion

    #region Main

    protected override void Awake()
    {
        base.Awake();
        if(SceneManager.GetActiveScene().name == "Title")
        {
            Debug.Log("Title Json");
            SetSettingJsonPath();
        }
        else if(SceneManager.GetActiveScene().name == "Main")
        {
            Debug.Log("Main Json");
            SetAllJsonPath();
            LoadAllGameDatas();

            /*if (GameManager.currentMainInfo.NewGame)
            {
                Debug.Log("Reset!!!");
                ResetMainJson();
                LoadAllGameDatas();
            }*/
        }
    }

    private void SetSettingJsonPath()
    {
        json_filePath = "Json/";

        json_SettingFileName = "SettingDatabase";
    }

    private void SetAllJsonPath()
    {
        json_filePath = "Json/";

        json_tutorialFileName = "tutorialInfoDatabase";
        json_mainInfoFileName = "mainInfoDatabase";
        json_wordFileName = "wordDatabase";
        json_wordActionFileName = "wordActionDatabase";
        json_sentenceFileName = "sentenceDatabase";
        json_ScheduleFileName = "scheduleDatabase";
        json_PlaceFileName = "placeDatabase";
        json_PSFileName = "gotPSDatabase";
        SetSettingJsonPath();
    }

    #endregion

    #region Load Json
    
    // Main
    /*public TutorialInfo JsonLoad_TR(string jsonPath, string jsonName)
    {
        string jsonString;
        TutorialInfo tutorialInfo;

#if UNITY_EDITOR
        jsonString = File.ReadAllText(Application.dataPath + "/Resources/" + jsonPath + jsonName + ".json");
        tutorialInfo = JsonUtility.FromJson<TutorialInfo>(jsonString);
#else
        string jsonString = File.ReadAllText(Application.persistentDataPath + "/" + jsonName + ".json");
        TutorialInfo tutorialInfo = JsonUtility.FromJson<TutorialInfo>(jsonString);
#endif

        return tutorialInfo;
    }*/
    public MainInfo JsonLoad_MI(string jsonPath, string jsonName)
    {
        string jsonString;
        MainInfo mainInfo;

#if UNITY_EDITOR
        jsonString = File.ReadAllText(Application.dataPath + "/Resources/" + jsonPath + jsonName + ".json");
        mainInfo = JsonUtility.FromJson<MainInfo>(jsonString);
#else
        string jsonString = File.ReadAllText(Application.persistentDataPath + "/" + jsonName + ".json");
        MainInfo mainInfo = JsonUtility.FromJson<MainInfo>(jsonString);
#endif

        return mainInfo;
    }
    public List<string> JsonLoad_L(string jsonPath, string jsonName)
    {
        string jsonString;
        IDs ids;

#if UNITY_EDITOR
        jsonString = File.ReadAllText(Application.dataPath + "/Resources/" + jsonPath + jsonName + ".json");
        ids = JsonUtility.FromJson<IDs>(jsonString);
#else
        string jsonString = File.ReadAllText(Application.persistentDataPath + "/" + jsonName + ".json");
        IDs ids = JsonUtility.FromJson<IDs>(jsonString);
#endif
        return ids.dataIDList;
    }
    public Dictionary<string, int> JsonLoad_D(string jsonPath, string jsonName)
    {
        List<string> LineDatas = JsonLoad_L(jsonPath, jsonName);
        Dictionary<string, int> DicData = new Dictionary<string, int>();
        foreach(string LineData in LineDatas)
        {
            string[] s = LineData.Split(',');
            DicData.Add(s[0], Convert.ToInt32(s[1]));
        }
        return DicData;


    }
    private List<Dictionary<string, object>> JsonLoad_LD(string jsonPath, string jsonName)
    {
        List<string> LineDatas = JsonLoad_L(jsonPath, jsonName);
        string[] s = LineDatas[0].Split(',');
        int LineLength = s.Length;

        string[] Lines = new string[LineLength];
        for(int i = 0; i < Lines.Length; i++)
        {
            Lines[i] = "EMPTY,";
        }

        foreach (string i in LineDatas)
        {
            string[] Elements = i.Split(",");
            
            for(int j = 0; j < LineLength; j++)
            {
                Lines[j] += Elements[j] + ",";
            }
        }
        string ReaderString = "";
        for(int i = 0; i < LineLength; i++)
        {
            Lines[i] = Lines[i].TrimEnd(',');
            if (i != LineLength) { ReaderString += Lines[i] + "\n"; }
            else { ReaderString += Lines[i]; }
        }

        List<Dictionary<string, object>> FinalResult = CSVReader.Read(ReaderString);

        return FinalResult;
    }
    public List<List<string>> JsonLoad_LL(string jsonPath, string jsonName)
    {
        string jsonString;
        PSBase ids;

#if UNITY_EDITOR
        jsonString = File.ReadAllText(Application.dataPath + "/Resources/" + jsonPath + jsonName + ".json");
        ids = JsonUtility.FromJson<PSBase>(jsonString);
#else
        string jsonString = File.ReadAllText(Application.persistentDataPath + "/" + jsonName + ".json");
        PSBase ids = JsonUtility.FromJson<PSBase>(jsonString);
#endif
        return ids.Data();
    }

    // Setting
    private GameSetting JsonLoad_S(string jsonPath, string jsonName)
    {
        string jsonString;
        GameSetting GS;

#if UNITY_EDITOR
        jsonString = File.ReadAllText(Application.dataPath + "/Resources/" + jsonPath + jsonName + ".json");
        GS = JsonUtility.FromJson<GameSetting>(jsonString);
#else
        string jsonString = File.ReadAllText(Application.persistentDataPath + "/" + jsonName + ".json");
        GameSetting GS = JsonUtility.FromJson<GameSetting>(jsonString);
#endif
        return GS;

    }
    
    // Load All
    public void LoadAllGameDatas()
    {
        // Load -> TutorialInfo Json
        //TutorialManager.currentTutorialInfo =
        //    JsonLoad_TR(json_filePath, json_tutorialFileName);

        // Load -> MainInfo Json
        GameManager.Instance.mainInfo =
            JsonLoad_MI(json_filePath, json_mainInfoFileName);

        // Load -> Word Json
        //WordManager.currentWordIDList =
        //    JsonLoad_L(json_filePath, json_wordFileName);

        // Load -> WordAction Json
        //WordManager.currentWordActionIDList =
        //    JsonLoad_L(json_filePath, json_wordActionFileName);

        // Load -> Sentence Json
        //StreamManager.currentStreamEventDatas =
        //    JsonLoad_LD(json_filePath, json_sentenceFileName);

        // Load -> schedule Json
        //ScheduleManager.currentHaveScheduleID =
        //    JsonLoad_L(json_filePath, json_ScheduleFileName);
        //ScheduleManager.currentPrograssScheduleID = "S00";

        // Load -> place Json
        //PlaceManager.currentPlaceID_Dict =
        //    JsonLoad_D(json_filePath, json_PlaceFileName);

        // Load -> got PreliminarySurvey Json
        //PreliminarySurveyManager.PSSO_FindClue_ExceptionIDs = JsonLoad_LL(json_filePath, json_PSFileName)[0];
        //PreliminarySurveyManager.PSSO_Extract_ExceptionIDs = JsonLoad_LL(json_filePath, json_PSFileName)[1];

        // Load -> Setting
        GameSettingManager.GameSetting = JsonLoad_S(json_filePath, json_SettingFileName);

    }

#endregion

    #region Save Json

   /* public void JsonSave(string jsonName, TutorialInfo tutorialInfo)
    {
        string saveJson = JsonUtility.ToJson(tutorialInfo, true);
        string saveFilePath = Application.persistentDataPath + "/" + jsonName + ".json";

#if UNITY_EDITOR
        saveFilePath = "Assets/Resources/Json/" + jsonName + ".json";
#endif
        System.IO.File.WriteAllText(saveFilePath, saveJson);
        Debug.Log(jsonName);
    }
*/
    public void JsonSave(string jsonName, MainInfo mainDatas)
    {
        string saveJson = JsonUtility.ToJson(mainDatas, true);
        string saveFilePath = Application.persistentDataPath + "/" + jsonName + ".json";
        Debug.Log(saveFilePath);

#if UNITY_EDITOR
        saveFilePath = "Assets/Resources/Json/" + jsonName + ".json";
        Debug.Log(saveFilePath);
#endif


        System.IO.File.WriteAllText(saveFilePath, saveJson);
        Debug.Log(jsonName);
    }
    public void JsonSave(string jsonName, List<string> currentWordIDs)
    {
        IDs wordIDs = new IDs();
        foreach (string currentWordID in currentWordIDs)
        {
            wordIDs.dataIDList.Add(currentWordID);
        }

        string saveJson = JsonUtility.ToJson(wordIDs, true);
        string saveFilePath = Application.persistentDataPath + "/" + jsonName + ".json";

#if UNITY_EDITOR
        saveFilePath = "Assets/Resources/Json/" + jsonName + ".json";
#endif

        System.IO.File.WriteAllText(saveFilePath, saveJson);
        Debug.Log(jsonName);
    }
    public void JsonSave(string jsonName,  Dictionary<string, int> Dict)
    {
        List<string> ListData = new List<string>();
        foreach(KeyValuePair<string, int> KVP in Dict)
        {
            string Data = KVP.Key + "," + KVP.Value;
            ListData.Add(Data);
        }
        JsonSave(jsonName, ListData);
    }
    public void JsonSave(string jsonName, List<Dictionary<string, object>> Datas)
    {
        List<string> Elements = new List<string>();
        
        string keys = "";
        foreach (string Key in Datas[0].Keys)
        {
            keys += Key + ",";
        }

        string[] Lines = new string[Datas.Count];
        int Num = 0;
        foreach (var Datas_D in Datas)
        {
            foreach (var Value in Datas_D.Values)
            {
                Lines[Num] += Convert.ToString(Value) + ",";
            }
            Num++;
        }

        for (int i = 0; i < Datas[0].Count; i++)
        {
            string s = keys.Split(',')[i] + ",";
            for (int j = 0; j < Lines.Length; j++)
            {
                s += Lines[j].Split(',')[i] + ",";
            }
            s = s.TrimEnd(',');
            Elements.Add(s);
        }

        JsonSave(jsonName, Elements);
    }
    public void JsonSave(string jsonName, PSBase Data)
    {
        string saveJson = JsonUtility.ToJson(Data, true);
        string saveFilePath = Application.persistentDataPath + "/" + jsonName + ".json";

#if UNITY_EDITOR
        saveFilePath = "Assets/Resources/Json/" + jsonName + ".json";
#endif

        System.IO.File.WriteAllText(saveFilePath, saveJson);
        Debug.Log(jsonName);
    }
    public void JsonSave(string jsonName, GameSetting gameSetting)
    {
        string saveJson = JsonUtility.ToJson(gameSetting, true);
        string saveFilePath = Application.persistentDataPath + "/" + jsonName + ".json";

#if UNITY_EDITOR
        saveFilePath = "Assets/Resources/Json/" + jsonName + ".json";
#endif

        System.IO.File.WriteAllText(saveFilePath, saveJson);
        Debug.Log(jsonName);
    }

    // Save ALL
    public void SaveAllMainGameDatas()
    {
        // Save -> Tutorial Json
        //JsonSave(json_tutorialFileName, TutorialManager.currentTutorialInfo);

        // Save -> MainInfo Json
        JsonSave(json_mainInfoFileName, GameManager.Instance.mainInfo);

        // Save -> Word Json
        //JsonSave(json_wordFileName, WordManager.currentWordIDList);

        // Save -> WordAction Json
        //JsonSave(json_wordActionFileName, WordManager.currentWordActionIDList);

        // Save -> Sentence Json
        //JsonSave(json_sentenceFileName, StreamManager.currentStreamEventDatas);

        // Save -> Schedule Json
        //JsonSave(json_ScheduleFileName, ScheduleManager.currentHaveScheduleID);

        // Save -> Place Json
        //JsonSave(json_PlaceFileName, PlaceManager.currentPlaceID_Dict);

        // Save -> Got PreliminarySurvey Json
        /*JsonSave(json_PSFileName, new PSBase(
            PreliminarySurveyManager.PSSO_FindClue_ExceptionIDs,
            PreliminarySurveyManager.PSSO_Extract_ExceptionIDs
        ));*/

        // Save -> Setting
        JsonSave(json_SettingFileName, GameSettingManager.GameSetting);

    }
    

#endregion

    #region StartSet (Reset)

    // Reset SentenceSheet - Json
    [ContextMenu("ResetJson_MainData")]
    public void ResetMainJson()
    {
        SetAllJsonPath();
        //JsonSave(json_tutorialFileName, new TutorialInfo());
        JsonSave(json_mainInfoFileName, new MainInfo());
        JsonSave(json_wordFileName, new List<string>() { "W001" });
        JsonSave(json_wordActionFileName, new List<string>() { "WA01", "WA02" });
        JsonSave(json_ScheduleFileName, new List<string>() { "S01", "S02", "S03", "S04" });
        JsonSave(json_PlaceFileName, new Dictionary<string, int>() { { "P00", 1 }, { "P01", 0 } });
        JsonSave(json_PSFileName, new PSBase(new List<string> { }, new List<string> { }));
        Set_StartSentence(); // Sentence IDs
    }


    void Set_StartSentence()
    {
        string path = "Sheet/SentenceSheet";
        string[] saveFiles = Resources.Load<TextAsset>(path).ToString().Split('\n');

        string[] a = saveFiles[0].Replace("\r", "").Split(",");
        string[] b = saveFiles[1].Replace("\r", "").Split(",");
        string[] c = saveFiles[2].Replace("\r", "").Split(",");

        List<string> result = new List<string>();

        for (int i = 0; i < a.Length; i++)
        {
            string data = a[i] + "," + b[i] + "," + c[i];
            result.Add(data);
        }
        result.RemoveAt(0);

        JsonSave(json_sentenceFileName, result);
    }

    [ContextMenu("ResetJson_SettingData")]
    public void ResetSettingJson()
    {
        SetSettingJsonPath();
        JsonSave(json_SettingFileName, new GameSetting());
    }

    #endregion

}

[Serializable]
public class IDs
{
    public List<string> dataIDList = new();
}

//PS Base
[Serializable]
public class PSBase
{
    public List<string> got_FindClue_ID;
    public List<string> got_Extract_ID;

    public PSBase(List<string> list_00, List<string> list_01)
    {
        got_FindClue_ID = list_00;
        got_Extract_ID = list_01;
    }
    public List<List<string>> Data()
    {
        return new List<List<string>> { got_FindClue_ID, got_Extract_ID };
    }
}

