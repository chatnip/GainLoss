using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class WordJson : MonoBehaviour
{
    string path;
    public List<WordBase> myWords = new List<WordBase>();
    [SerializeField] WordManager WordManager;

    private void Start()
    {
        path = Path.Combine(Application.dataPath, "wordDatabase.json");
        // JsonSaveTest();
        JsonLoadTest();
    }
    private void JsonLoadTest()
    {
        WordList wordList = new WordList();
        string loadJson = File.ReadAllText(path);
        wordList = JsonUtility.FromJson<WordList>(loadJson);
        WordManager.currentWordList = wordList.wordIDs;

    }

    private void JsonSaveTest()
    {
        WordList wordList = new WordList();

        wordList.wordIDs.Add("W01");
        wordList.wordIDs.Add("W02");
        wordList.wordIDs.Add("W03");

        string json = JsonUtility.ToJson(wordList, true);
        File.WriteAllText(path, json);
    }
}

[Serializable]
public class WordList
{
    public List<string> wordIDs = new List<string>();
}



[Serializable]
public class WordBase
{
    public int wordID;
    public string wordName;
    public bool isUsed;
}

