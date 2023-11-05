using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class WordJson : MonoBehaviour
{
    string path;
    public List<WordBase> myWords = new List<WordBase>();

    private void Start()
    {
        path = Path.Combine(Application.dataPath, "wordDatabase.json");
        JsonLoadTest();
    }
    private void JsonLoadTest()
    {
        WordList wordList = new WordList();
        string loadJson = File.ReadAllText(path);
        wordList = JsonUtility.FromJson<WordList>(loadJson);
        myWords = wordList.words;

    }

    private void JsonSaveTest()
    {
        WordList wordList = new WordList();

        WordBase word1 = new WordBase
        {
            wordName = "test1",
            wordPlace = 1,
            writeTroll = true,
            sendMail = false,
            sendPackage = false
        };
        wordList.words.Add(word1);

        WordBase word2 = new WordBase
        {
            wordName = "test2",
            wordPlace = 1,
            writeTroll = true,
            sendMail = true,
            sendPackage = false
        };
        wordList.words.Add(word2);

        WordBase word3 = new WordBase
        {
            wordName = "test3",
            wordPlace = 1,
            writeTroll = true,
            sendMail = true,
            sendPackage = true
        };
        wordList.words.Add(word3);

        string json = JsonUtility.ToJson(wordList, true);
        File.WriteAllText(path, json);
    }
}

[Serializable]
public class WordList
{
    public List<WordBase> words = new List<WordBase>();
}



[Serializable]
public class WordBase
{
    public string wordName;
    public int wordPlace;
    public bool writeTroll;
    public bool sendMail;
    public bool sendPackage;
}

