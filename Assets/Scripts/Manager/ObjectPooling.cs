using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    [SerializeField] List<InteractedObject> InteractedObjectPrefabs = new List<InteractedObject>();
    [SerializeField] Queue<InteractedObject> InteractedObjectesQueue = new Queue<InteractedObject>();
    [SerializeField] List<Word> WordObjectPrefabs = new List<Word>();
    [SerializeField] Queue<Word> WordObjectesQueue = new Queue<Word>();

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
        for (int i = 0; i < WordObjectPrefabs.Count; i++)
        {
            WordObjectesQueue.Enqueue(WordObjectPrefabs[i]);
            WordObjectPrefabs[i].gameObject.SetActive(false);
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

    public Word WordObjectPool()
    {
        var wordObject = WordObjectesQueue.Dequeue();
        return wordObject;
    }
    public void ObjectPick(Word wordObject)
    {
        WordObjectesQueue.Enqueue(wordObject);
        wordObject.gameObject.SetActive(false);
    }
}