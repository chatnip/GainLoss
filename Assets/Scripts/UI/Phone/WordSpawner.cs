using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class WordSpawner : MonoBehaviour
{
    [SerializeField] ObjectPooling ObjectPooling;
    [SerializeField] RectTransform wordPool;
    [SerializeField] WordManager WordManager;
    [SerializeField] bool isTODO;
    [SerializeField] RectTransform scrollViewContent;
    List<Word> enableWordList = new List<Word>();

    private void OnEnable()
    {
        SpawnWord(scrollViewContent, isTODO);
    }

    public void SpawnWord(RectTransform parentObj, bool isTODO)
    {
        for (int i = 0; i < WordManager.currentWordList.Count; i++)
        {
            Word word = ObjectPooling.WordObjectPool();
            enableWordList.Add(word);
            word.transform.SetParent(parentObj);
            word.isTODO = isTODO;
            word.wordBase = WordManager.currentWordList[i];
            word.gameObject.SetActive(true);
        }
    }

    public void PickWord()
    {   
        for (int i = enableWordList.Count - 1; i >= 0; i--)
        {
            enableWordList[i].transform.SetParent(wordPool);
            ObjectPooling.ObjectPick(enableWordList[i]);
        }
        enableWordList.Clear();
    }

    private void OnDisable()
    {
        PickWord();
    }
}
