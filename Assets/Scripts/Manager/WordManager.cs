using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;

public class WordManager : Manager<WordManager>
{
    [Header("*Property")]
    [SerializeField] GameManager GameManager;
    [SerializeField] ObjectPooling ObjectPooling;
    [SerializeField] WordBtnSpawner todoWordBtnSpawner;

    [Header("*Word")]
    public List<WordBase> currentWordList = new List<WordBase>();
    [SerializeField] TMP_Text viewWordAction;

    // 행동 선택 버튼
    public List<Button> wordBtns = new();
    public List<Button> wordActionBtns = new();

    public Button button;

    // 선택한 단어의 이름
    string currentWordName;

    // 선택한 단어의 액션 목록
    public List<WordActionData> currentWordActionDataList = new();
    public WordActionData currentWordActionData;

    public void WordBtnListSet()
    {
        foreach (Button wordBtn in wordBtns)
        {
            wordBtn
                .OnClickAsObservable()
                .Select(buttonNum => wordBtn.transform.GetSiblingIndex())
                .Subscribe(buttonNum =>
                {
                    Debug.Log("Click!");
                    if (todoWordBtnSpawner.enableWordBtnList.Count != 0)
                    {
                        Debug.Log("Click!!");
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
                        viewWordAction.text = currentWordActionData.actionSentence;
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

    // 이 아래에다가 단어 선택시 행동 선택지 생성되는 함수 만들기
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
