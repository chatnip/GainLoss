using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    [SerializeField] List<InteractedObject> InteractedObjectPrefabs = new List<InteractedObject>();
    [SerializeField] Queue<InteractedObject> InteractedObjectesQueue = new Queue<InteractedObject>();
    [SerializeField] List<WordBtn> WordBtnObjectPrefabs = new List<WordBtn>();
    [SerializeField] Queue<WordBtn> WordBtnObjectesQueue = new Queue<WordBtn>();
    [SerializeField] RectTransform wordPool;

    private void Awake()
    {
        SetupQueue();
    }

    private void SetupQueue()
    {
        for (int i = 0; i < InteractedObjectPrefabs.Count; i++)
        {
            InteractedObjectesQueue.Enqueue(InteractedObjectPrefabs[i]);
            InteractedObjectPrefabs[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < WordBtnObjectPrefabs.Count; i++)
        {
            WordBtnObjectesQueue.Enqueue(WordBtnObjectPrefabs[i]);
            WordBtnObjectPrefabs[i].gameObject.SetActive(false);
        }
    }

    public InteractedObject InteractedObjectPool()
    {
        var interactedObject = InteractedObjectesQueue.Dequeue();
        return interactedObject;
    }
    public void ObjectPick(InteractedObject interactedObject)
    {
        InteractedObjectesQueue.Enqueue(interactedObject);
        interactedObject.gameObject.SetActive(false);
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