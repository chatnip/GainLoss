using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordpadSpawner : WordBtnSpawner
{
    public override void SpawnWordBtn()
    {
        for (int i = 0; i < WordManager.currentWordList.Count; i++)
        {
            WordBtn wordBtn = CreateWordBtn(WordManager.currentWordList[i].wordName);
            wordBtn.transform.SetParent(wordParentObject);
            WordManager.enableWordBtnList.Add(wordBtn);
            wordBtn.gameObject.SetActive(true);
        }
    }

    protected override WordBtn CreateWordBtn(string btnText)
    {
        WordBtn wordBtn = ObjectPooling.WordBtnObjectPool();
        wordBtn.isButton = false;
        wordBtn.wordBtnTextStr = btnText;       
        return wordBtn;
    }
}
