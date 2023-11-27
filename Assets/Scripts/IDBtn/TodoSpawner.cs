using System.Collections;
using System.Collections.Generic;
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
        return wordBtn;
    }

    protected override void SpawnIDBtn()
    {
        SpawnWordBtn();
    }

    private void SpawnWordBtn()
    {
        for (int i = 0; i < WordManager.currentWordList.Count; i++)
        {
            IDBtn wordBtn = CreateIDBtn(WordManager.currentWordList[i]); // ����
            wordBtn.transform.SetParent(wordParentObject); // �θ� ����
            wordBtn.buttonType = ButtonType.WordType; // ����
            WordManager.enableWordBtnList.Add(wordBtn); // Ȱ��ȭ ����Ʈ�� ����
            WordManager.WordBtnListSet(); // ������ ����
            wordBtn.gameObject.SetActive(true); // Ȱ��ȭ
        }
    }

    public void SpawnWordActionBtn()
    {
        PickWordActionBtn(); // ��ư �ʱ�ȭ

        for (int i = 0; i < WordManager.currentWordActionList.Count; i++)
        {
            IDBtn actionBtn = CreateIDBtn(WordManager.currentWordActionList[i]); // ����
            actionBtn.transform.SetParent(wordActionParentObject); // �θ� ����
            actionBtn.buttonType = ButtonType.WordActionType; // ����
            WordManager.enableWordActionBtnList.Add(actionBtn); // Ȱ��ȭ ����Ʈ�� ����
            WordManager.WordActionBtnListSet(); // ������ ����
            actionBtn.gameObject.SetActive(true); // Ȱ��ȭ
        }
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
