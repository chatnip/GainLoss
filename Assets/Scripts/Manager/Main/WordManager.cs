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


    // 켜진 단어 및 단어의 액션 버튼 목록
    [HideInInspector] public List<WordBtn> enableWordBtnList = new();
    [HideInInspector] public List<WordBtn> enableWordActionBtnList = new();

    // 선택한 단어 및 단어의 액션 목록
    [HideInInspector] public List<string> currentWordList = new();
    [HideInInspector] public List<WordActionData> currentWordActionDataList = new();

    // 선택한 단어 및 단어의 액션
    [HideInInspector] public string currentWordName;
    WordActionData currentWordActionData = new();

    private void Start()
    {
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
                currentWordActionData = null;
                todoWordBtnSpawner.PickWordAction();
                inViewWordAction.text = "아무것도 하지 않는다";
                outViewWordAction.text = "아무것도 하지 않는다";
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
                        // currentWordActionDataList = FindWordActions(FindWord());
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

    private Word FindWord()
    {
        foreach (Word data in GameManager.wordDatas)
        {
            if (currentWordName == data.wordName)
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

    public void SetupWord() // 단어 세팅부분 제작하기
    {

        Word word = new Word();
        // DataManager.WordDatas
    }
}


[System.Serializable]
public class Word
{
    public string wordID;
    public string wordName;
}

[System.Serializable]
public class WordActionData
{
    public string wordActionName;
    public string actionSentence;
    public bool wordActionBool;
    public int stressGage;
    public int angerGage;
    public int riskGage;
    public int streamEventID;
}