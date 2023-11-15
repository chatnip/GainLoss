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
    [SerializeField] GameManager GameManager;
    [SerializeField] ObjectPooling ObjectPooling;

    [Space(10)]
    [SerializeField] TodoSpawner todoWordBtnSpawner;
     
    [Header("*View")]
    [SerializeField] TMP_Text inViewWordAction;
    [SerializeField] TMP_Text outViewWordAction;
    [SerializeField] Button doNotingBtn;
    StringReactiveProperty currentWordActiionStr = new();


    // ���� �ܾ� �� �ܾ��� �׼� ��ư ���
    [HideInInspector] public List<WordBtn> enableWordBtnList = new();
    [HideInInspector] public List<WordBtn> enableWordActionBtnList = new();

    // ������ �ܾ� �� �ܾ��� �׼� ���
    [HideInInspector] public List<WordBase> currentWordList = new();
    [HideInInspector] public List<WordActionData> currentWordActionDataList = new();

    // ������ �ܾ� �� �ܾ��� �׼�
    [HideInInspector] public string currentWordName;
    WordActionData currentWordActionData = new();

    private void Start()
    {
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
                currentWordActionData = null;
                todoWordBtnSpawner.PickWordAction();
                inViewWordAction.text = "�ƹ��͵� ���� �ʴ´�";
                outViewWordAction.text = "�ƹ��͵� ���� �ʴ´�";
            });
    }

    #region ButtonListSeting
    public void WordBtnListSet()
    {
        foreach (WordBtn wordBtn in enableWordBtnList)
        {
            wordBtn.button
                .OnClickAsObservable()
                .Select(buttonNum => wordBtn.transform.GetSiblingIndex())
                .Subscribe(buttonNum =>
                {
                    if (enableWordBtnList.Count != 0)
                    {
                        currentWordName = enableWordBtnList[buttonNum].wordBtnTextStr;
                        currentWordActionDataList = FindWordActions(FindWord());
                        todoWordBtnSpawner.SpawnWordActionBtn();
                        WordActionBtnListSet();
                    }
                });
        }
    }

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
                        currentWordActiionStr.Value = currentWordActionData.actionSentence;
                    }
                });
        }
    }
    #endregion

    #region FindData

    private WordData FindWord()
    {
        foreach (WordData data in GameManager.wordDatas)
        {
            if (currentWordName == data.wordName)
            {
                return data;
            }
        }
        return null;
    }

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
    #endregion
}
