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
            WordBtn wordBtn = CreateWordBtn(WordManager.currentWordList[i].wordName);
            wordBtn.transform.SetParent(wordParentObject);
            WordManager.enableWordBtnList.Add(wordBtn);
            WordManager.WordBtnListSet();
            wordBtn.gameObject.SetActive(true);
        }
    }

    public void SpawnWordActionBtn()
    {
        PickWordAction();

        for (int i = 0; i < WordManager.currentWordActionDataList.Count; i++)
        {
            WordBtn wordBtn = CreateWordBtn(WordManager.currentWordActionDataList[i].wordActionName);
            wordBtn.transform.SetParent(wordActionParentObject);
            WordManager.enableWordActionBtnList.Add(wordBtn);
            wordBtn.gameObject.SetActive(true);
        }
    }

    protected override WordBtn CreateWordBtn(string btnText)
    {
        WordBtn wordBtn = ObjectPooling.WordBtnObjectPool();
        wordBtn.isButton = true;
        wordBtn.wordBtnTextStr = btnText;
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
