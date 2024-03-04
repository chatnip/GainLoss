using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening.Plugins.Core.PathCore;
using static UnityEditor.PlayerSettings;

public class JsonManager : MonoBehaviour
{
    [HideInInspector] public string json_filePath;

    [HideInInspector] public string json_mainInfoFileName;
    [HideInInspector] public string json_wordFileName;
    [HideInInspector] public string json_wordActionFileName;
    [HideInInspector] public string json_sentenceFileName;
    [HideInInspector] public string json_ScheduleFileName;
    [HideInInspector] public string json_PlaceFileName;
    [HideInInspector] public string json_PSFileName;

    // public List<WordBase> myWords = new List<WordBase>();
    [SerializeField] WordManager WordManager;
    [SerializeField] StreamManager StreamManager;
    [SerializeField] GameManager GameManager;
    [SerializeField] ScheduleManager ScheduleManager;
    [SerializeField] PlaceManager PlaceManager;
    [SerializeField] PreliminarySurveyManager PreliminarySurveyManager;

    private void Awake()
    {
        SetPath();
        LoadAllGameDatas();
        if (GameManager.currentMainInfo.NewGame)
        {
            ResetJson();
            LoadAllGameDatas();
        }
    }

    private void SetPath()
    {
        json_filePath = "Assets/Resources/Json/";

        json_mainInfoFileName = "mainInfoDatabase.json";
        json_wordFileName = "wordDatabase.json";
        json_wordActionFileName = "wordActionDatabase.json";
        json_sentenceFileName = "sentenceDatabase.json";
        json_ScheduleFileName = "scheduleDatabase.json";
        json_PlaceFileName = "placeDatabase.json";
        json_PSFileName = "gotPSDatabase.json";
    }

    #region Load Json

    public MainInfo JsonLoad_MI(string jsonPath, string jsonName)
    {
        if (!File.Exists(jsonPath + jsonName))
        {
            Debug.LogError("None File this Path");
            return null;
        }

        string path = jsonPath + jsonName;
        string loadJson = File.ReadAllText(path);
        MainInfo mainInfo = JsonUtility.FromJson<MainInfo>(loadJson);

        return mainInfo;
    }
    public List<string> JsonLoad_L(string jsonPath, string jsonName)
    {
        /*string loadJson = File.ReadAllText(jsonPath + jsonName);
        
        WordIDs wordIDs = JsonUtility.FromJson<WordIDs>(loadJson.ToString());
        WordManager.currentWordIDList = wordIDs.wordIDList;*/

        if (!File.Exists(jsonPath + jsonName))
        {
            Debug.LogError("None File this Path");
            return null;
        }

        string saveFile = File.ReadAllText(jsonPath + jsonName);
        IDs saveData = JsonUtility.FromJson<IDs>(saveFile);
        return saveData.dataIDList;
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
    
    // Load All
    public void LoadAllGameDatas()
    {
        // Load -> MainInfo Json
        GameManager.currentMainInfo =
            JsonLoad_MI(json_filePath, json_mainInfoFileName);

        // Load -> Word Json
        WordManager.currentWordIDList =
            JsonLoad_L(json_filePath, json_wordFileName);

        // Load -> WordAction Json
        WordManager.currentWordActionIDList =
            JsonLoad_L(json_filePath, json_wordActionFileName);

        // Load -> Sentence Json
        StreamManager.currentStreamEventDatas =
            JsonLoad_LD(json_filePath, json_sentenceFileName);

        // Load -> schedule Json
        ScheduleManager.currentHaveScheduleID =
            JsonLoad_L(json_filePath, json_ScheduleFileName);
        ScheduleManager.currentPrograssScheduleID = "S00";

        // Load -> place Json
        PlaceManager.currentPlaceIDList =
            JsonLoad_L(json_filePath, json_PlaceFileName);

        // Load -> got PreliminarySurvey Json
        PreliminarySurveyManager.CPSSO_IDs =
            JsonLoad_L(json_filePath, json_PSFileName);


    }

    #endregion

    #region Save Json

    public void JsonSave(string jsonPath, string jsonName, MainInfo mainDatas)
    {
        if (!Directory.Exists(jsonPath))
        {
            Directory.CreateDirectory(jsonPath);
        }

        string saveJson = JsonUtility.ToJson(mainDatas, true);

        string saveFilePath = jsonPath + jsonName;
        File.WriteAllText(saveFilePath, saveJson);
        Debug.Log("Save Success: " + saveFilePath);
    }
    public void JsonSave(string jsonPath, string jsonName, List<string> currentWordIDs)
    {
        IDs wordIDs = new IDs();
        foreach (string currentWordID in currentWordIDs)
        {
            wordIDs.dataIDList.Add(currentWordID);
        }

        if (!Directory.Exists(jsonPath))
        {
            Directory.CreateDirectory(jsonPath);
        }

        string saveJson = JsonUtility.ToJson(wordIDs, true);

        string saveFilePath = jsonPath + jsonName;
        File.WriteAllText(saveFilePath, saveJson);
        Debug.Log("Save Success: " + saveFilePath);
    }
    public void JsonSave(string jsonPath, string jsonName, List<Dictionary<string, object>> Datas)
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

        JsonSave(jsonPath, jsonName, Elements);
    }

    // Save ALL
    public void SaveAllGameDatas()
    {
        // Save -> MainInfo Json
        JsonSave(json_filePath, json_mainInfoFileName, GameManager.currentMainInfo);

        // Save -> Word Json
        JsonSave(json_filePath, json_wordFileName, WordManager.currentWordIDList);

        // Save -> WordAction Json
        JsonSave(json_filePath, json_wordActionFileName, WordManager.currentWordActionIDList);

        // Save -> Sentence Json
        JsonSave(json_filePath, json_sentenceFileName, StreamManager.currentStreamEventDatas);

        // Save -> Schedule Json
        JsonSave(json_filePath, json_ScheduleFileName, ScheduleManager.currentHaveScheduleID);

        // Save -> Place Json
        JsonSave(json_filePath, json_PlaceFileName, PlaceManager.currentPlaceIDList);

        // Save -> Got PreliminarySurvey Json
        JsonSave(json_filePath, json_PSFileName, PreliminarySurveyManager.CPSSO_IDs);
    }

    #endregion

    #region StartSet (Reset)

    // Reset SentenceSheet - Json
    [ContextMenu("ResetJson")]
    public void ResetJson()
    {
        SetPath();

        JsonSave(json_filePath, json_mainInfoFileName, new MainInfo());
        JsonSave(json_filePath, json_wordFileName, new List<string>() { "W001", "W004" });
        JsonSave(json_filePath, json_wordActionFileName, new List<string>() { "WA01", "WA02" });
        JsonSave(json_filePath, json_ScheduleFileName, new List<string>() { "S01", "S02", "S03", "S04" });
        JsonSave(json_filePath, json_PlaceFileName, new List<string>() { "P01", "P02"});
        JsonSave(json_filePath, json_PSFileName, new List<string>() { });

        Set_StartSentence(); // Sentence IDs
    }

    void Set_StartSentence()
    {
        string path = "Assets/Resources/Sheet/SentenceSheet.csv";
        string[] saveFiles = File.ReadAllText(path).Split('\n');
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

        JsonSave(json_filePath, json_sentenceFileName, result);
    }

    #endregion

}

[Serializable]
public class IDs
{
    public List<string> dataIDList = new();
}



/*
[Serializable]
public class WordBase
{
    public int wordID;
    public string wordName;
    public bool isUsed;
}

*/