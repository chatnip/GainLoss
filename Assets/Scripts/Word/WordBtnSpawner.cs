using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class WordBtnSpawner : MonoBehaviour
{
    [SerializeField] ObjectPooling ObjectPooling;
    [SerializeField] RectTransform wordPool;
    [SerializeField] WordManager WordManager;
    [SerializeField] TodoManager TodoManager;
    [SerializeField] bool isTODO;
    [SerializeField] RectTransform scrollViewContent;
    [HideInInspector] public List<WordBtn> enableWordBtnList = new List<WordBtn>();
    [HideInInspector] public List<WordBtn> enableWordActionBtnList = new List<WordBtn>();

    private void OnEnable()
    {
        SpawnWord(scrollViewContent, isTODO);
    }

    public void SpawnWord(RectTransform parentObj, bool isTODO)
    {
        for (int i = 0; i < WordManager.currentWordList.Count; i++)
        {
            WordBtn wordBtn = ObjectPooling.WordBtnObjectPool();
            enableWordBtnList.Add(wordBtn);
            wordBtn.transform.SetParent(parentObj);
            wordBtn.isTODO = isTODO;
            wordBtn.wordBtnTextStr = WordManager.currentWordList[i].wordName;
            wordBtn.gameObject.SetActive(true);
        }
    }

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

    public void PickWord()
    {   
        for (int i = enableWordBtnList.Count - 1; i >= 0; i--)
        {
            enableWordBtnList[i].transform.SetParent(wordPool);
            ObjectPooling.ObjectPick(enableWordBtnList[i]);
        }
        enableWordBtnList.Clear();
    }

    public void PickWordAction()
    {
        for (int i = enableWordActionBtnList.Count - 1; i >= 0; i--)
        {
            enableWordActionBtnList[i].transform.SetParent(wordPool);
            ObjectPooling.ObjectPick(enableWordActionBtnList[i]);
        }
        enableWordActionBtnList.Clear();
    }

    private void OnDisable()
    {
        PickWordAction();
        PickWord();       
    }
}
