using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using NaughtyAttributes;


public class WordManager : Manager<WordManager>
{
    [Header("*Property")]
    [SerializeField] StreamManager StreamManager;
    [SerializeField] ObjectPooling ObjectPooling;

    [Space(10)]
    [SerializeField] TodoSpawner todoWordBtnSpawner;
     
    [Header("*View")]
    [SerializeField] TMP_Text inViewWordAction;
    [SerializeField] TMP_Text outViewWordAction;
    [SerializeField] Button resetBtn;
    StringReactiveProperty currentWordActiionStr = new();

    [Header("*Test")]
    [SerializeField] WordJson WordJson;


    // 켜진 단어 및 단어의 액션 버튼 목록
    [HideInInspector] public List<IDBtn> enableWordBtnList = new();
    [HideInInspector] public List<IDBtn> enableWordActionBtnList = new();

    // 선택한 단어 및 단어의 액션 목록
    [HideInInspector] public List<string> currentWordIDList = new();
    [HideInInspector] public List<ButtonValue> currentWordList = new();
    [HideInInspector] private List<string> currentWordActionIDList = new();
    [HideInInspector] public List<ButtonValue> currentWordActionList = new();
    
    // 선택한 단어 및 단어의 액션
    private ButtonValue currentWord;
    private ButtonValue currentWordAction;
    

    private void Start()
    {
        TodoReset();
        InitWord();

        currentWordActiionStr
            .Subscribe(x =>
            {
                inViewWordAction.text = x;
                outViewWordAction.text = x;
            });

        resetBtn
            .OnClickAsObservable()
            .Subscribe(x =>
            {
                TodoReset();
            });
    }

    private void TodoReset()
    {
        todoWordBtnSpawner.PickWordAction();
        currentWord = null;
        currentWordAction = null;
        currentWordActiionStr.Value = "아무것도 하지 않는다";
    }

    #region ButtonListSeting
    public void WordBtnListSet()
    {
        foreach (IDBtn wordBtn in enableWordBtnList)
        {
            wordBtn.button
                .OnClickAsObservable()
                .Select(word => wordBtn.word)
                .Subscribe(word =>
                {
                    currentWord = word;
                    InitWordActionID(currentWord.ID);
                    todoWordBtnSpawner.SpawnWordActionBtn();
                });
        }
    }

    public void WordActionBtnListSet()
    {
        foreach (IDBtn wordActionBtn in enableWordActionBtnList)
        {
            wordActionBtn.button
                .OnClickAsObservable()
                .Select(word => wordActionBtn.word)
                .Subscribe(word =>
                {
                    currentWordAction = word;                   
                    string text = string.Format("{0}에 대한 {1}을(를) 한다.", currentWord.Name, currentWordAction.Name);
                    currentWordActiionStr.Value = text;
                    StreamManager.currentStreamEventID = currentWord.ID + currentWordAction.ID;
                });
        }
    }
    #endregion

    #region Init
    // JsonLoadTest() == InitWordID()
    public void InitWord()
    {
        currentWordList.Clear(); // 초기화
        foreach (string id in currentWordIDList) // ID 순회
        {
            ButtonValue word = new(id, (string)DataManager.WordDatas[0][id]);
            currentWordList.Add(word);
        }
    }

    private void InitWordActionID(string id)
    {
        currentWordActionIDList.Clear(); // 초기화
        foreach (var data in DataManager.StreamEventDatas[0]) // 스트림 이벤트 순회
        {
            if (data.Key.Contains(id))
            {
                string key = Regex.Replace(data.Key.ToString(), id, "");
                currentWordActionIDList.Add(key);
            }
        }
        InitWordAction();
    }

    private void InitWordAction()
    {
        currentWordActionList.Clear(); // 초기화
        foreach (string id in currentWordActionIDList) // ID 순회
        {
            ButtonValue word = new(id, (string)DataManager.WordActionDatas[0][id]);
            currentWordActionList.Add(word);
        }
    }
    #endregion
}


[System.Serializable]
public class ButtonValue
{
    public string ID;
    public string Name;

    public ButtonValue(string id, string name)
    {
        ID = id;
        Name = name;
    }
}