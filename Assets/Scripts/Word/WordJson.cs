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
        JsonLoadTest();
    }
    private void JsonLoadTest()
    {
        WordList wordList = new WordList();
        string loadJson = File.ReadAllText(path);
        wordList = JsonUtility.FromJson<WordList>(loadJson);
        WordManager.currentWordList = wordList.words;

    }

    /*
    private void JsonSaveTest()
    {
        WordList wordList = new WordList();

        WordBase word1 = new WordBase
        {
            wordName = "test1"
        };
        wordList.words.Add(word1);

        WordBase word2 = new WordBase
        {
            wordName = "test2"
        };
        wordList.words.Add(word2);

        WordBase word3 = new WordBase
        {
            wordName = "test3"
        };
        wordList.words.Add(word3);

        string json = JsonUtility.ToJson(wordList, true);
        File.WriteAllText(path, json);
    }
    */
}

[Serializable]
public class WordList
{
    public List<WordBase> words = new List<WordBase>();
}



[Serializable]
public class WordBase
{
    public int wordID;
    public string wordName;
}

