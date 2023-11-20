using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using NaughtyAttributes;

public class WordManager : Manager<WordManager>
{
    [Header("*Property")]
    [SerializeField] DataManager DataManager;
    [SerializeField] GameManager GameManager;
    [SerializeField] ObjectPooling ObjectPooling;

    [Space(10)]
    [SerializeField] TodoSpawner todoWordBtnSpawner;
     
    [Header("*View")]
    [SerializeField] TMP_Text inViewWordAction;
    [SerializeField] TMP_Text outViewWordAction;
    [SerializeField] Button doNotingBtn;
    StringReactiveProperty currentWordActiionStr = new();

    [Header("*Test")]
    [SerializeField] WordJson WordJson;


    // 켜진 단어 및 단어의 액션 버튼 목록
    [HideInInspector] public List<WordBtn> enableWordBtnList = new();
    [HideInInspector] public List<WordBtn> enableWordActionBtnList = new();

    // 선택한 단어 및 단어의 액션 목록
    [SerializeField] public List<string> currentWordIDList = new();
    [SerializeField] public List<Word> currentWordList = new();
    [SerializeField] public List<string> currentWordActionIDList = new();
    [SerializeField] public List<Word> currentWordActionList = new();

    // 선택한 단어 및 단어의 액션
    [HideInInspector] public string currentWordName;
    // WordAction currentWordActionData = new();

    private void Start()
    {
        InitWord();

        currentWordActiionStr
            .Subscribe(x =>
            {
                if(x == null)
                {
                    inViewWordAction.text = "아직 정해지지 않았다...";
                    outViewWordAction.text = "아직 정해지지 않았다...";
                }
                else
                {
                    inViewWordAction.text = x;
                    outViewWordAction.text = x;
                }
            });

        doNotingBtn
            .OnClickAsObservable()
            .Subscribe(x =>
            {
                currentWordName = null;
                // currentWordActionData = null;
                todoWordBtnSpawner.PickWordAction();
                inViewWordAction.text = "아무것도 하지 않는다";
                outViewWordAction.text = "아무것도 하지 않는다";
            });
    }

    #region ButtonListSeting
    public void WordBtnListSet()
    {
        foreach (WordBtn wordBtn in enableWordBtnList) // 버튼 순회
        {
            wordBtn.button
                .OnClickAsObservable()
                .Select(id => wordBtn.word.ID)
                .Subscribe(id =>
                {
                    foreach (var data in DataManager.StreamEventDatas[0]) // 스트림 이벤트 순회
                    {
                        if (data.Key.Contains(id)) // 만약 단어 ID가 들어간 스트림 이벤트를 찾으면
                        {
                            //string key = data.Key.ToString()
                            //currentWordActionIDList.Add()
                            // 스트림 이벤트 리스트로 모으기
                            //Word word = new(data.Key, (string)DataManager.WordActionDatas[0][id]);
                        }
                    }
                    // currentWordName = enableWordBtnList[buttonNum].wordBtnName;
                    // currentWordActionDataList = FindWordActions(FindWord());
                    // todoWordBtnSpawner.SpawnWordActionBtn();
                    // WordActionBtnListSet();
                });
        }
    }

    /*
    public void WordActionBtnListSet()
    {
        foreach (WordBtn wordActionBtn in enableWordActionBtnList)
        {
            wordActionBtn.button
                .OnClickAsObservable()
                .Select(buttonNum => wordActionBtn.transform.GetSiblingIndex())
                .Subscribe(buttonNum =>
                {
                    if (enableWordActionBtnList.Count != 0)
                    {
                        currentWordActionData = currentWordActionDataList[buttonNum];
                        //currentWordActiionStr.Value = currentWordActionData.actionSentence;
                    }
                });
        }
    }
    */
    #endregion

    #region FindData

    private Word FindWord()
    {
        foreach (Word data in GameManager.wordDatas)
        {
            if (currentWordName == data.Name)
            {
                return data;
            }
        }
        return null;
    }



    /*
    private List<WordActionData> FindWordActions(WordData wordData)
    {
        List<WordActionData> datas = new();
        foreach (WordActionData data in wordData.wordActionDatas)
        {
            if (data.wordActionBool == true)
            {
                datas.Add(data);
            }
        }
        return datas;
    }
    */
    #endregion

    public void InitWord()
    {
        foreach(string id in currentWordIDList)
        {
            Word word = new(id, (string)DataManager.WordDatas[0][id]);
            currentWordList.Add(word);
        }
    }
}


[System.Serializable]
public class Word
{
    public string ID;
    public string Name;

    public Word(string id, string name)
    {
        ID = id;
        Name = name;
    }
}