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
            //IDBtn wordBtn = CreateIDBtn(WordManager.currentWordList[i]);
            IDBtn wordBtn = CreateIDBtn(new ButtonValue(WordManager.currentWordIDList[i], DataManager.WordDatas[5][WordManager.currentWordIDList[i]].ToString()));
            wordBtn.transform.SetParent(wordParentObject);
            wordBtn.buttonType = ButtonType.WordPadType;
            if (wordBtn.TryGetComponent(out RectTransform RT)) { RT.sizeDelta = new Vector2(Screen.width, Screen.height * 0.3f); }
            wordBtn.text.fontSize = 90;
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
