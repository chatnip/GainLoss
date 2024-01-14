using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractObject : InteractCore
{
    [SerializeField] string objectID;
    [Tooltip("if this Object can't get something, you have to this string empty!")]
    [SerializeField] public string getWordID;
    [Tooltip("if this Object can't get something, you have to this string empty!")]
    [SerializeField] public string getWordActionID;

    [HideInInspector] public bool CanInteract = true;

    CheckGetAllDatas CheckGetAllDatas;
    GameObject GetSomething;
    GetDataWithID getSomethingWithID;
    WordManager WordManager;

    private void Awake()
    {
        WordManager = GameObject.Find("WordManager").GetComponent<WordManager>();
        GetSomething = GameObject.Find("GetSomething");
        getSomethingWithID = GetSomething.GetComponent<GetDataWithID>();

        CanInteract = true;
    }
    private void OnEnable()
    {
        CanInteract = true;
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
        if (this.getWordID != "") { GetWordID(); }
        if (this.getWordActionID != "") { GetWordActionID(); }

        if (GameObject.Find("TerminatePart") != null)
        {
            CheckGetAllDatas = GameObject.Find("TerminatePart").GetComponent<CheckGetAllDatas>();
            CheckGetAllDatas.ApplyTerminateBtnAndText();
        }

        //if(CanInteract) { CanInteract = false; }

        /*if (GameObject.Find("InteractiveCanvas").TryGetComponent(out ObjectInteractionButtonGenerator objectInteractionButtonGenerator))
        {
            objectInteractionButtonGenerator.SetOffAllBtns();
        }*/


    }

    protected void GetWordID()
    {
        List<string> wordIDs = WordManager.currentWordIDList;
        foreach (string wordID in wordIDs)
        {
            if (wordID == getWordID)
            {
                getWordID = "";
                return;
            }
        }
        getSomethingWithID.SetData(getWordID);
        getWordID = "";
    }
    protected void GetWordActionID()
    {
        List<string> wordActionIDs = WordManager.currentWordActionIDList;
        foreach (string wordActionID in wordActionIDs)
        {
            if (wordActionID == getWordActionID)
            {
                getWordActionID = "";
                return;
            }
        }
        getSomethingWithID.SetData(getWordActionID);
        getWordActionID = "";
    }
}