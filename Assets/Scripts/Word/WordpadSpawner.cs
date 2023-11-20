using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordpadSpawner : WordBtnSpawner
{
    public override void SpawnWordBtn()
    {
        for (int i = 0; i < WordManager.currentWordIDList.Count; i++)
        {
            WordBtn wordBtn = CreateWordBtn(WordManager.currentWordList[i]);
            wordBtn.transform.SetParent(wordParentObject);
            WordManager.enableWordBtnList.Add(wordBtn);
            wordBtn.gameObject.SetActive(true);
        }
    }

    protected override WordBtn CreateWordBtn(Word word)
    {
        WordBtn wordBtn = ObjectPooling.WordBtnObjectPool();
        wordBtn.isButton = false;
        wordBtn.word = word;       
        return wordBtn;
    }
}
