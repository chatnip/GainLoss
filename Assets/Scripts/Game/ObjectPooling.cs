using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    [Header("*Word Btn")]
    [SerializeField] List<IDBtn> WordBtnObjectPrefabs = new List<IDBtn>();
    [SerializeField] Queue<IDBtn> WordBtnObjectsQueue = new Queue<IDBtn>();
    [SerializeField] RectTransform wordPool;

    /*[Header("*Norification Object")]
    [SerializeField] public List<GameObject> NorificationObjectPrefabs = new List<GameObject>();*/

    private void Awake()
    {
        Setup();
    }

    private void Setup()
    {
        for (int i = 0; i < WordBtnObjectPrefabs.Count; i++)
        {
            WordBtnObjectsQueue.Enqueue(WordBtnObjectPrefabs[i]);
            WordBtnObjectPrefabs[i].gameObject.SetActive(false);
        }
        /*for (int i = 0; i < NorificationObjectPrefabs.Count; i++)
        {
            NorificationObjectPrefabs[i].gameObject.SetActive(false);
        }*/
    }

    public IDBtn WordBtnObjectPool()
    {
        var wordBtnObject = WordBtnObjectsQueue.Dequeue();
        return wordBtnObject;
    }

    public void WordObjectPick(IDBtn wordBtnObject)
    {
        WordBtnObjectsQueue.Enqueue(wordBtnObject);
        wordBtnObject.transform.SetParent(wordPool);
        wordBtnObject.gameObject.SetActive(false);
    }

   /* public GameObject ft_getUsableNOP()
    {
        for(int i = 0; i < NorificationObjectPrefabs.Count; i++)
        {
            if (!NorificationObjectPrefabs[i].gameObject.activeSelf)
            {
                return NorificationObjectPrefabs[i];
            }
        }
        return null;
    }*/
}