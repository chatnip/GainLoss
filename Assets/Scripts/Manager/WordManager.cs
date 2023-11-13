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
    public List<WordBtn> enableWordBtnList = new List<WordBtn>();
    public List<WordBtn> enableWordActionBtnList = new List<WordBtn>();

    // ������ �ܾ� �� �ܾ��� �׼� ���
    [HideInInspector] public List<WordBase> currentWordList = new();
    [HideInInspector] public List<WordActionData> currentWordActionDataList = new();

    // ������ �ܾ� �� �ܾ��� �׼�
    public string currentWordName;
    WordActionData currentWordActionData = new();

    private void Start()
    {
        currentWordActiionStr
            .Subscribe(x =>
            {
                inViewWordAction.text = x;
                outViewWordAction.text = x;
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

    // ���߿� �Ϸ簡 ����۵Ǹ� ���� �������� �ʾҴ�... �߰� �ϱ�

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
}
