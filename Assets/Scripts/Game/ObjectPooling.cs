//Refactoring v1.0
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : Singleton<ObjectPooling>
{
    #region Value

    [Header("*Word Btn")]
    [SerializeField] List<IDBtn> OP_IDBtn_Prefabs = new List<IDBtn>();
    Queue<IDBtn> OP_IDBtn_Queue = new Queue<IDBtn>();
    [SerializeField] RectTransform OP_IDBtn_ParentRT;

    #endregion

    #region Framework & Base Set
    public void Offset()
    {
        for (int i = 0; i < OP_IDBtn_Prefabs.Count; i++)
        {
            OP_IDBtn_Queue.Enqueue(OP_IDBtn_Prefabs[i]);
            OP_IDBtn_Prefabs[i].gameObject.SetActive(false);
        }
    }

    protected override void Awake()
    {
        base.Awake();
    }

    #endregion

    #region Object Pooling _About Get

    public IDBtn GetIDBtn()
    {
        var IDBtnObject = OP_IDBtn_Queue.Dequeue();
        return IDBtnObject;
    }

    public void GetBackIDBtn(IDBtn wordBtnObject)
    {
        OP_IDBtn_Queue.Enqueue(wordBtnObject);
        wordBtnObject.transform.SetParent(OP_IDBtn_ParentRT);
        wordBtnObject.gameObject.SetActive(false);
    }

    #endregion
}