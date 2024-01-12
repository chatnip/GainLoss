using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TodoSpawner : IDBtnSpawner
{
    [SerializeField] WordManager WordManager;
    [SerializeField] StreamManager StreamManager;

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
            IDBtn wordBtn = CreateIDBtn(WordManager.currentWordList[i]); // ����
            wordBtn.transform.SetParent(wordParentObject); // �θ� ����
            wordBtn.AddVisiableWordRate((string)DataManager.WordDatas[1][WordManager.currentWordList[i].ID]); // ��� ǥ��
            bool checkCanUseMalicious = CheckCanUseMalicious(
                (string)DataManager.WordDatas[1][WordManager.currentWordList[i].ID],
                WordManager.currentWordIDList[i]); // Malicious��� ail������ ����ߴ��� �Ǵ�
            if (!checkCanUseMalicious)
            { wordBtn.CannotUse(false, "Use up to the maximum(1)"); }
            else
            { wordBtn.CannotUse(true, ""); }
            wordBtn.buttonType = ButtonType.WordType; // ����
            WordManager.enableWordBtnList.Add(wordBtn); // Ȱ��ȭ ����Ʈ�� ����
            // WordManager.WordBtnListSet(); // ������ ����
            wordBtn.gameObject.SetActive(true); // Ȱ��ȭ
        }
        WordManager.WordBtnListSet(); // ������ ����
    }

    
    private bool CheckCanUseMalicious(string rate, string wordID)
    {
        if (rate == "Malicious")
        {
            List<string> streamEventIDs = new List<string>();
            for (int i = 0; i < (DataManager.WordActionDatas[0].Count - 1); i++)
            {
                if (i.ToString().Length == 1) { streamEventIDs.Add(wordID + "WA0" + (i + 1)); }
                else if (i.ToString().Length == 2) { streamEventIDs.Add(wordID + "WA" + (i + 1)); }
            }

            foreach (string s in streamEventIDs)
            {
                if (Convert.ToBoolean(StreamManager.currentStreamEventDatas[0][s]))
                { return false; }
            }
            return true;
        }
        else
        { return true; }
    }


    public void SpawnWordActionBtn()
    {
        PickWordActionBtn(); // ��ư �ʱ�ȭ
        WordManager.enableWordActionBtnList.Clear();

        for (int i = 0; i < WordManager.currentWordActionList.Count; i++)
        {

            IDBtn actionBtn = CreateIDBtn(WordManager.currentWordActionList[i]); // ����
            actionBtn.transform.SetParent(wordActionParentObject); // �θ� ����
            if (Convert.ToInt32(StreamManager.currentStreamEventDatas[1][WordManager.currentWord.ID + WordManager.currentWordActionList[i].ID]) >= 3)
            { actionBtn.CannotUse(false, "Use up to the maximum(3)"); }
            else 
            { actionBtn.CannotUse(true, ""); }
            actionBtn.AddVisiableWordRate((string)DataManager.WordActionDatas[1][WordManager.currentWordActionList[i].ID]); // ��� ǥ��
            actionBtn.buttonType = ButtonType.WordActionType; // ����
            WordManager.enableWordActionBtnList.Add(actionBtn); // Ȱ��ȭ ����Ʈ�� ����
                                                                // WordManager.WordActionBtnListSet(); // ������ ����
            actionBtn.gameObject.SetActive(true); // Ȱ��ȭ

            

        }
        WordManager.WordActionBtnListSet(); // ������ ����
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
                ObjectPooling.WordObjectPick(WordManager.enableWordBtnList[i]);
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
                ObjectPooling.WordObjectPick(WordManager.enableWordActionBtnList[i]);
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
