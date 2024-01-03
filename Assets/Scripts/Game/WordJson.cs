using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordJson : MonoBehaviour
{
    [HideInInspector] public string json_filePath;
    [HideInInspector] public string json_wordFileName;
    [HideInInspector] public string json_wordActionFileName;

    // public List<WordBase> myWords = new List<WordBase>();
    [SerializeField] WordManager WordManager;

    private void Awake()
    {
        json_filePath = "Assets/Resources/Json/";
        json_wordFileName = "wordDatabase.json";
        json_wordActionFileName = "wordActionDatabase.json";

        WordManager.currentWordIDList = JsonLoadTest(json_filePath, json_wordFileName);
        WordManager.currentWordActionIDList = JsonLoadTest(json_filePath, json_wordActionFileName);
    }

    
    public List<string> JsonLoadTest(string jsonPath, string jsonName)
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

    public void JsonSaveTest(string jsonPath, string jsonName, List<string> currentWordIDs)
    {
        WordIDs wordIDs = new WordIDs();
        foreach(string currentWordID in currentWordIDs)
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