using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractObject : InteractCore
{
    [SerializeField] string objectID;
    [Tooltip("if this Object can't get something, you have to this string empty!")]
    [SerializeField] string getWordID;
    [Tooltip("if this Object can't get something, you have to this string empty!")]
    [SerializeField] string getWordActionID;

    GameObject GetSomething;
    GetDataWithID getSomethingWithID;
    WordManager WordManager;

    private void Awake()
    {
        WordManager = GameObject.Find("WordManager").GetComponent<WordManager>();
        GetSomething = GameObject.Find("MainCanvas").transform.Find("GetSomething").gameObject;
        getSomethingWithID = GetSomething.GetComponent<GetDataWithID>();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
    }
    public override void Interact()
    {
        if(this.getWordID != "")
        {
            GetWordID();
        }
        if (this.getWordActionID != "")
        {
            GetWordActionID();
        }
    }

    protected void GetWordID()
    {
        
    }
    protected void GetWordActionID()
    {
        
    }
}
