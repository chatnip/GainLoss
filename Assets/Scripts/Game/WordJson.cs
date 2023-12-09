using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class WordJson : MonoBehaviour
{
    string path;
    // public List<WordBase> myWords = new List<WordBase>();
    [SerializeField] WordManager WordManager;

    private void Awake()
    {
        path = "Json/wordDatabase";
        // JsonSaveTest();
        JsonLoadTest();
    }
    public void JsonLoadTest()
    {
        var jsonTextFile = Resources.Load<TextAsset>(path);
        WordIDs wordIDs = JsonUtility.FromJson<WordIDs>(jsonTextFile.ToString());
        WordManager.currentWordIDList = wordIDs.wordIDList;
    }

    private void JsonSaveTest()
    {
        WordIDs wordIDs = new WordIDs();

        wordIDs.wordIDList.Add("W01");
        wordIDs.wordIDList.Add("W02");
        wordIDs.wordIDList.Add("W03");

        string json = JsonUtility.ToJson(wordIDs, true);
        File.WriteAllText(path, json);
    }
}

[Serializable]
public class WordIDs
{
    public List<string> wordIDList = new();
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