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


    // ���� �ܾ� �� �ܾ��� �׼� ��ư ���
    [HideInInspector] public List<WordBtn> enableWordBtnList = new();
    [HideInInspector] public List<WordBtn> enableWordActionBtnList = new();

    // ������ �ܾ� �� �ܾ��� �׼� ���
    [SerializeField] public List<string> currentWordIDList = new();
    [SerializeField] public List<Word> currentWordList = new();
    [SerializeField] public List<string> currentWordActionIDList = new();
    [SerializeField] public List<Word> currentWordActionList = new();

    // ������ �ܾ� �� �ܾ��� �׼�
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
                    inViewWordAction.text = "���� �������� �ʾҴ�...";
                    outViewWordAction.text = "���� �������� �ʾҴ�...";
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
                inViewWordAction.text = "�ƹ��͵� ���� �ʴ´�";
                outViewWordAction.text = "�ƹ��͵� ���� �ʴ´�";
            });
    }

    #region ButtonListSeting
    public void WordBtnListSet()
    {
        foreach (WordBtn wordBtn in enableWordBtnList) // ��ư ��ȸ
        {
            wordBtn.button
                .OnClickAsObservable()
                .Select(id => wordBtn.word.ID)
                .Subscribe(id =>
                {
                    foreach (var data in DataManager.StreamEventDatas[0]) // ��Ʈ�� �̺�Ʈ ��ȸ
                    {
                        if (data.Key.Contains(id)) // ���� �ܾ� ID�� �� ��Ʈ�� �̺�Ʈ�� ã����
                        {
                            //string key = data.Key.ToString()
                            //currentWordActionIDList.Add()
                            // ��Ʈ�� �̺�Ʈ ����Ʈ�� ������
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