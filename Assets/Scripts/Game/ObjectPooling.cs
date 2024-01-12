using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    [SerializeField] List<IDBtn> WordBtnObjectPrefabs = new List<IDBtn>();
    [SerializeField] Queue<IDBtn> WordBtnObjectesQueue = new Queue<IDBtn>();
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

    public IDBtn WordBtnObjectPool()
    {
        var wordBtnObject = WordBtnObjectesQueue.Dequeue();
        return wordBtnObject;
    }


    public void WordObjectPick(IDBtn wordBtnObject)
    {
        WordBtnObjectesQueue.Enqueue(wordBtnObject);
        wordBtnObject.transform.SetParent(wordPool);
        wordBtnObject.gameObject.SetActive(false);
    }
}