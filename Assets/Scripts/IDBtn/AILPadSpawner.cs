using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AILpadSpawner : IDBtnSpawner
{
    [SerializeField] WordManager WordManager;

    [Header("*WordParentObj")]
    [SerializeField] RectTransform wordParentObject;

    protected override void SpawnIDBtn()
    {
        for (int i = 0; i < WordManager.currentWordIDList.Count; i++)
        {
            IDBtn wordBtn = CreateIDBtn(WordManager.currentWordList[i]);
            wordBtn.transform.SetParent(wordParentObject);
            wordBtn.buttonType = ButtonType.WordPadType;
            WordManager.enableWordBtnList.Add(wordBtn);
            wordBtn.gameObject.SetActive(true);
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
                ObjectPooling.WordObjectPick(WordManager.enableWordBtnList[i]);
            }
            WordManager.enableWordBtnList.Clear();
        }
    }

    protected override IDBtn CreateIDBtn(ButtonValue word)
    {
        IDBtn wordBtn = ObjectPooling.WordBtnObjectPool();
        wordBtn.isButton = false;
        wordBtn.buttonValue = word;       
        return wordBtn;
    }
}
