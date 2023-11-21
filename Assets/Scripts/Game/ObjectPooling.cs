using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    [SerializeField] List<WordBtn> WordBtnObjectPrefabs = new List<WordBtn>();
    [SerializeField] Queue<WordBtn> WordBtnObjectesQueue = new Queue<WordBtn>();
    [SerializeField] RectTransform wordPool;

    private void Awake()
    {
        SetupQueue();
    }

    private void SetupQueue()
    {
        for (int i = 0; i < WordBtnObjectPrefabs.Count; i++)
        {
            WordBtnObjectesQueue.Enqueue(WordBtnObjectPrefabs[i]);
            WordBtnObjectPrefabs[i].gameObject.SetActive(false);
        }
    }

    public WordBtn WordBtnObjectPool()
    {
        var wordBtnObject = WordBtnObjectesQueue.Dequeue();
        return wordBtnObject;
    }
    public void ObjectPick(WordBtn wordBtnObject)
    {
        WordBtnObjectesQueue.Enqueue(wordBtnObject);
        wordBtnObject.transform.SetParent(wordPool);
        wordBtnObject.gameObject.SetActive(false);
    }
}