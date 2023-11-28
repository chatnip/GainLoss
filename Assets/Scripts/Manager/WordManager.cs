using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;

public class WordManager : Manager<WordManager>
{
    [Header("*Property")]
    [SerializeField] StreamManager StreamManager;
    [SerializeField] ObjectPooling ObjectPooling;

    [Space(10)]
    [SerializeField] TodoSpawner todoWordBtnSpawner;
     
    [Header("*View")]
    [SerializeField] TMP_Text viewWordAction;
    [SerializeField] Button resetBtn;
    StringReactiveProperty currentWordActiionStr = new();

    [Header("*Test")]
    [SerializeField] WordJson WordJson;


    // ���� �ܾ� �� �ܾ��� �׼� ��ư ���
    [HideInInspector] public List<IDBtn> enableWordBtnList = new();
    [HideInInspector] public List<IDBtn> enableWordActionBtnList = new();

    // ������ �ܾ� �� �ܾ��� �׼� ���
    [HideInInspector] public List<string> currentWordIDList = new();
    [HideInInspector] public List<ButtonValue> currentWordList = new();
    [HideInInspector] private List<string> currentWordActionIDList = new();
    [HideInInspector] public List<ButtonValue> currentWordActionList = new();
    
    // ������ �ܾ� �� �ܾ��� �׼�
    private ButtonValue currentWord;
    private ButtonValue currentWordAction;
    

    private void Start()
    {
        TodoReset();
        InitWord();

        currentWordActiionStr
            .Subscribe(x =>
            {
                viewWordAction.text = x;
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
        todoWordBtnSpawner.PickWordActionBtn();
        currentWord = null;
        currentWordAction = null;
        currentWordActiionStr.Value = "�ƹ��͵� ���� �ʴ´�";
    }

    #region ButtonListSeting
    public void WordBtnListSet()
    {
        foreach (IDBtn wordBtn in enableWordBtnList)
        {
            wordBtn.button
                .OnClickAsObservable()
                .Select(word => wordBtn.buttonValue)
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
                .Select(action => wordActionBtn.buttonValue)
                .Subscribe(action =>
                {
                    currentWordAction = action;                   
                    string text = string.Format("<#D40047><b>{0}</b></color> �� ���� <#D40047><b>{1}</b></color> ��(��) �Ѵ�.", currentWord.Name, currentWordAction.Name);
                    currentWordActiionStr.Value = text;
                    StreamManager.currentStreamEventID = currentWord.ID + currentWordAction.ID;
                });
        }
    }
    #endregion

    #region Init
    // JsonLoadTest() == InitWordID()
    // ���߿� �Ϸ� ���������� ����ǵ��� �ؾ���
    public void InitWord()
    {
        currentWordList.Clear(); // �ʱ�ȭ
        foreach (string id in currentWordIDList) // ID ��ȸ
        {
            ButtonValue word = new(id, (string)DataManager.WordDatas[1][id]);
            currentWordList.Add(word);
        }
    }

    private void InitWordActionID(string id)
    {
        currentWordActionIDList.Clear(); // �ʱ�ȭ
        foreach (var data in DataManager.StreamEventDatas[0]) // ��Ʈ�� �̺�Ʈ ��ȸ
        {
            if (data.Key.Contains(id))
            {
                string key = data.Key.ToString().Substring(4, 4);
                currentWordActionIDList.Add(key);
            }
        }
        InitWordAction();
    }

    private void InitWordAction()
    {
        currentWordActionList.Clear(); // �ʱ�ȭ
        foreach (string id in currentWordActionIDList) // ID ��ȸ
        {
            ButtonValue word = new(id, (string)DataManager.WordActionDatas[1][id]);
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