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
    [SerializeField] WordBtnSpawner todoWordBtnSpawner;
     
    [Header("*View")]
    [SerializeField] TMP_Text inViewWordAction;
    [SerializeField] TMP_Text outViewWordAction;
    [SerializeField] Button doNotingBtn;
    StringReactiveProperty currentWordActiionStr = new();


    // 행동 선택 버튼
    List<Button> wordBtns = new();
    List<Button> wordActionBtns = new();

    // 선택한 단어 및 단어의 액션 목록
    [HideInInspector] public List<WordBase> currentWordList = new();
    [HideInInspector] public List<WordActionData> currentWordActionDataList = new();

    // 선택한 단어 및 단어의 액션
    string currentWordName;
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
                inViewWordAction.text = "아무것도 하지 않는다";
                outViewWordAction.text = "아무것도 하지 않는다";
            });
    }

    // 나중에 하루가 재시작되면 아직 정해지지 않았다... 뜨게 하기

    public void WordBtnListSet()
    {
        foreach (Button wordBtn in wordBtns)
        {
            wordBtn
                .OnClickAsObservable()
                .Select(buttonNum => wordBtn.transform.GetSiblingIndex())
                .Subscribe(buttonNum =>
                {
                    if (todoWordBtnSpawner.enableWordBtnList.Count != 0)
                    {
                        currentWordName = todoWordBtnSpawner.enableWordBtnList[buttonNum].wordBtnTextStr;
                        currentWordActionDataList = FindWordActions(FindWord());
                        todoWordBtnSpawner.SpawnWordActionBtn();
                        GetActionButtonList();
                    }
                });
        }
    }

    public void WordActionBtnListSet()
    {
        foreach (Button wordActionBtn in wordActionBtns)
        {
            wordActionBtn
                .OnClickAsObservable()
                .Select(buttonNum => wordActionBtn.transform.GetSiblingIndex())
                .Subscribe(buttonNum =>
                {
                    if (todoWordBtnSpawner.enableWordActionBtnList.Count != 0)
                    {
                        currentWordActionData = currentWordActionDataList[buttonNum];
                        currentWordActiionStr.Value = currentWordActionData.actionSentence;
                    }
                });
        }
    }

    public void GetWordButtonList()
    {
        wordBtns.Clear();

        for (int i = 0; i < todoWordBtnSpawner.enableWordBtnList.Count; i++)
        {
            wordBtns.Add(todoWordBtnSpawner.enableWordBtnList[i].button);
        }

        WordBtnListSet();
    }

    public void GetActionButtonList()
    {
        wordActionBtns.Clear();

        for (int i = 0; i < todoWordBtnSpawner.enableWordActionBtnList.Count; i++)
        {
            wordActionBtns.Add(todoWordBtnSpawner.enableWordActionBtnList[i].button);
        }

        WordActionBtnListSet();
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
