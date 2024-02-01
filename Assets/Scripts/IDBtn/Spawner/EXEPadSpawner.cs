using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EXEpadSpawner : IDBtnSpawner
{
    [SerializeField] WordManager WordManager;

    [Header("*WordParentObj")]
    [SerializeField] RectTransform wordActionParentObject;

    protected override void SpawnIDBtn()
    {
        for (int i = 0; i < WordManager.currentWordActionIDList.Count; i++)
        {
            Debug.Log(WordManager.currentWordActionIDList[i]);
            IDBtn wordActionBtn = CreateIDBtn(new ButtonValue(WordManager.currentWordActionIDList[i], DataManager.WordActionDatas[3][WordManager.currentWordActionIDList[i]].ToString()));
            wordActionBtn.transform.SetParent(wordActionParentObject);
            wordActionBtn.buttonType = ButtonType.WordPadType;
            WordManager.enableWordActionBtnList.Add(wordActionBtn);
            wordActionBtn.gameObject.SetActive(true);
        }
    }

    protected override void PickIDBtn()
    {
        PickWordActionBtn();
    }

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

    protected override IDBtn CreateIDBtn(ButtonValue word)
    {
        IDBtn wordBtn = ObjectPooling.WordBtnObjectPool();
        wordBtn.isButton = false;
        wordBtn.buttonValue = word;
        return wordBtn;
    }
}
