using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    [SerializeField] List<InteractedObject> InteractedObjectPrefabs = new List<InteractedObject>();
    [SerializeField] Queue<InteractedObject> InteractedObjectesQueue = new Queue<InteractedObject>();

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
}