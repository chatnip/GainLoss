using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TodoSpawner : IDBtnSpawner
{
    [SerializeField] WordManager WordManager;

    [Header("*WordParentObj")]
    [SerializeField] RectTransform wordParentObject;
    [SerializeField] RectTransform wordActionParentObject;


    protected override IDBtn CreateIDBtn(ButtonValue word)
    {
        IDBtn wordBtn = ObjectPooling.WordBtnObjectPool();
        wordBtn.isButton = true;
        wordBtn.buttonValue = word;
        //wordBtn.Rate = ;
        return wordBtn;
    }

    protected override void SpawnIDBtn()
    {
        SpawnWordBtn();
    }

    private void SpawnWordBtn()
    {
        WordManager.enableWordBtnList.Clear();
        for (int i = 0; i < WordManager.currentWordList.Count; i++)
        {
            IDBtn wordBtn = CreateIDBtn(WordManager.currentWordList[i]); // 생성
            wordBtn.transform.SetParent(wordParentObject); // 부모 설정
            wordBtn.AddVisiableWordRate((string)DataManager.WordDatas[1][WordManager.currentWordList[i].ID]); // 등급 표시
            bool checkCanUseMalicious = CheckCanUseMalicious((string)DataManager.WordDatas[1][WordManager.currentWordList[i].ID]);
            if (!checkCanUseMalicious)
            { wordBtn.CannotUse(false, "Use up to the maximum(1)"); }
            else
            { wordBtn.CannotUse(true, ""); }
            wordBtn.buttonType = ButtonType.WordType; // 정렬
            WordManager.enableWordBtnList.Add(wordBtn); // 활성화 리스트에 삽입
            // WordManager.WordBtnListSet(); // 데이터 삽입
            wordBtn.gameObject.SetActive(true); // 활성화
        }
        WordManager.WordBtnListSet(); // 데이터 삽입
    }

    private bool CheckCanUseMalicious(string rate)
    {
        if(rate == "Malicious")
        {
            //판별
            return true;
        }
        else
        {
            return true;
        }
    }

    public void SpawnWordActionBtn()
    {
        PickWordActionBtn(); // 버튼 초기화
        WordManager.enableWordActionBtnList.Clear();

        for (int i = 0; i < WordManager.currentWordActionList.Count; i++)
        {

            IDBtn actionBtn = CreateIDBtn(WordManager.currentWordActionList[i]); // 생성
            actionBtn.transform.SetParent(wordActionParentObject); // 부모 설정
            if ((int)(DataManager.StreamEventDatas[1][WordManager.currentWord.ID + WordManager.currentWordActionList[i].ID]) >= 3)
            { actionBtn.CannotUse(false, "Use up to the maximum(3)"); }
            else 
            { actionBtn.CannotUse(true, ""); }
            actionBtn.AddVisiableWordRate((string)DataManager.WordActionDatas[1][WordManager.currentWordActionList[i].ID]); // 등급 표시
            actionBtn.buttonType = ButtonType.WordActionType; // 정렬
            WordManager.enableWordActionBtnList.Add(actionBtn); // 활성화 리스트에 삽입
                                                                // WordManager.WordActionBtnListSet(); // 데이터 삽입
            actionBtn.gameObject.SetActive(true); // 활성화

            

        }
        WordManager.WordActionBtnListSet(); // 데이터 삽입
    }

    protected override void PickIDBtn()
    {
        PickWordBtn();
    }

    public void PickWordBtn()
    {
        if (WordManager.enableWordBtnList.Count != 0)
        {
            for (int i = WordManager.enableWordBtnList.Count - 1; i >= 0; i--)
            {
                ObjectPooling.ObjectPick(WordManager.enableWordBtnList[i]);
            }
            WordManager.enableWordBtnList.Clear();
        }
    }

    /// <summary>
    /// </summary>
    public void PickWordActionBtn()
    {
        if (WordManager.enableWordActionBtnList.Count != 0)
        {
            for (int i = WordManager.enableWordActionBtnList.Count - 1; i >= 0; i--)
            {
                ObjectPooling.ObjectPick(WordManager.enableWordActionBtnList[i]);
            }
            WordManager.enableWordActionBtnList.Clear();
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        PickWordActionBtn();
    }

    
}
