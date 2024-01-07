using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonManager : MonoBehaviour
{
    [HideInInspector] public string json_filePath;

    [HideInInspector] public string json_wordFileName;
    [HideInInspector] public string json_wordActionFileName;
    [HideInInspector] public string json_sentenceFileName;

    // public List<WordBase> myWords = new List<WordBase>();
    [SerializeField] WordManager WordManager;
    [SerializeField] StreamManager StreamManager;

    private void Awake()
    {
        json_filePath = "Assets/Resources/Json/";

        json_wordFileName = "wordDatabase.json";
        json_wordActionFileName = "sentenceDatabase.json";
        json_sentenceFileName = "wordActionDatabase.json";



        WordManager.currentWordIDList = 
            JsonLoad_L(json_filePath, json_wordFileName);
        WordManager.currentWordActionIDList = 
            JsonLoad_L(json_filePath, json_wordActionFileName);
        StreamManager.currentStreamEventDatas = 
            JsonLoad_LD(json_filePath, json_sentenceFileName);

        // SentenceSheet의 값을 받아와 Json파일에 저장하는 함수 / 즉, 데이터 초기화
        CSVToJson_SentenceSheet();

        JsonSave(json_filePath, json_sentenceFileName, StreamManager.currentStreamEventDatas);
    }


    // Load Json
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
        WordIDs saveData = JsonUtility.FromJson<WordIDs>(saveFile);
        return saveData.dataIDList;
    }
    private List<Dictionary<string, object>> JsonLoad_LD(string jsonPath, string jsonName)
    {
        List<Dictionary<string, object>> FinalResult = new();

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
        FinalResult = CSVReader.Read(ReaderString);

        return FinalResult;
    }

    // Save Json
    public void JsonSave(string jsonPath, string jsonName, List<string> currentWordIDs)
    {
        WordIDs wordIDs = new WordIDs();
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
    
    // Reset Json
    void CSVToJson_SentenceSheet()
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

    // Save ALL
    public void SaveAllGameDatas()
    {
        // Save -> Word Json
        JsonSave(json_filePath, json_wordFileName, WordManager.currentWordIDList);

        // Save -> WordAction Json
        JsonSave(json_filePath, json_wordActionFileName, WordManager.currentWordActionIDList);

        // Save -> Sentence Json
        JsonSave(json_filePath, json_sentenceFileName, StreamManager.currentStreamEventDatas);
    }
}

[Serializable]
public class WordIDs
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