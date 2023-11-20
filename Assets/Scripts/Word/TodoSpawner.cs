using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TodoSpawner : WordBtnSpawner
{
    [SerializeField] RectTransform wordActionParentObject;

    public override void SpawnWordBtn()
    {
        for (int i = 0; i < WordManager.currentWordList.Count; i++)
        {
            WordBtn wordBtn = CreateWordBtn(WordManager.currentWordList[i]); // ����
            wordBtn.transform.SetParent(wordParentObject); // �θ� ����
            WordManager.enableWordBtnList.Add(wordBtn); // Ȱ��ȭ ����Ʈ�� ����
            WordManager.WordBtnListSet(); // ������ ����
            wordBtn.gameObject.SetActive(true); // Ȱ��ȭ
        }
    }

    public void SpawnWordActionBtn()
    {
        PickWordAction(); // ��ư �ʱ�ȭ

        for (int i = 0; i < WordManager.currentWordActionList.Count; i++)
        {
            WordBtn wordBtn = CreateWordBtn(WordManager.currentWordActionList[i]); // ����
            wordBtn.transform.SetParent(wordActionParentObject); // �θ� ����
            WordManager.enableWordActionBtnList.Add(wordBtn); // Ȱ��ȭ ����Ʈ�� ����
            WordManager.WordActionBtnListSet(); // ������ ����
            wordBtn.gameObject.SetActive(true); // Ȱ��ȭ
        }
    }

    protected override WordBtn CreateWordBtn(Word word)
    {
        WordBtn wordBtn = ObjectPooling.WordBtnObjectPool();
        wordBtn.isButton = true;
        wordBtn.word = word;
        return wordBtn;
    }

    public override void PickWordAction()
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
}
