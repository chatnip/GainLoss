using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class WordBtnSpawner : MonoBehaviour
{
    [Header("*Property")]
    [SerializeField] ObjectPooling ObjectPooling;
    [SerializeField] WordManager WordManager;

    [Header("*WordParentObj")]
    [SerializeField] RectTransform wordPool;
    [SerializeField] RectTransform scrollViewContent;
    [SerializeField] RectTransform scrollViewContent_action;

    [Header("*Word")]
    [SerializeField] WordBtnType wordBtnType;

    public List<WordBtn> enableWordBtnList = new List<WordBtn>();
    public List<WordBtn> enableWordActionBtnList = new List<WordBtn>();

    private void OnEnable()
    {
        SelectBtnType();
    }

    private void SelectBtnType()
    {
        switch (wordBtnType)
        {
            case WordBtnType.WordPad_Btn:
                SpawnWordBtn(false);
                break;
            case WordBtnType.Word_Btn:
                SpawnWordBtn(true);
                WordManager.GetWordButtonList();
                break;
        }
    }

    public void SpawnWordBtn(bool isButton)
    {
        for (int i = 0; i < WordManager.currentWordList.Count; i++)
        {
            enableWordBtnList.Add(CreateWordBtn(scrollViewContent, isButton, WordManager.currentWordList[i].wordName));
        }
    }

    public void SpawnWordActionBtn()
    {
        PickWordAction();

        for (int i = 0; i < WordManager.currentWordActionDataList.Count; i++)
        {
            enableWordActionBtnList.Add(CreateWordBtn(scrollViewContent_action, true, WordManager.currentWordActionDataList[i].wordActionName));
        }
    }

    public WordBtn CreateWordBtn(RectTransform parentObj, bool isButton, string btnText)
    {
        WordBtn wordBtn = ObjectPooling.WordBtnObjectPool();      
        wordBtn.transform.SetParent(parentObj);
        wordBtn.isButton = isButton;
        wordBtn.wordBtnTextStr = btnText;
        wordBtn.gameObject.SetActive(true);
        return wordBtn;
    }

    public void PickWord()
    {
        if (enableWordBtnList.Count != 0)
        {
            for (int i = enableWordBtnList.Count - 1; i >= 0; i--)
            {
                enableWordBtnList[i].transform.SetParent(wordPool);
                ObjectPooling.ObjectPick(enableWordBtnList[i]);
            }
            enableWordBtnList.Clear();
        }         
    }

    public void PickWordAction()
    {
        if (enableWordActionBtnList.Count != 0)
        {
            for (int i = enableWordActionBtnList.Count - 1; i >= 0; i--)
            {
                enableWordActionBtnList[i].transform.SetParent(wordPool);
                ObjectPooling.ObjectPick(enableWordActionBtnList[i]);
            }
            enableWordActionBtnList.Clear();
        }
    }

    private void OnDisable()
    {
        PickWord();
        PickWordAction();
    }


    /*
    public void SpawnWordAction(RectTransform parentObj)
    {
        for(int i = 0; i < TodoManager.currentWordActionData.Count; i++)
        {
            WordBtn wordBtn = ObjectPooling.WordBtnObjectPool();
            enableWordActionBtnList.Add(wordBtn);
            wordBtn.transform.SetParent(parentObj);
            wordBtn.isTODO = true;
            wordBtn.wordBtnTextStr = TodoManager.currentWordActionData[i].wordActionName;
            wordBtn.gameObject.SetActive(true);
        }
    }
    */
}

[System.Serializable]
public enum WordBtnType
{ 
    WordPad_Btn,
    Word_Btn
}
