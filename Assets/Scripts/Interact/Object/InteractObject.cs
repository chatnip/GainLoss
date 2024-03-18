using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractObject : InteractCore
{
    #region Value

    [Header("*IDs")]
    [SerializeField] string objectID;
    [Tooltip("if this Object can't get something, you have to this string empty!")]
    [SerializeField] public string getWordID;
    [Tooltip("if this Object can't get something, you have to this string empty!")]
    [SerializeField] public string getWordActionID;
    [Tooltip("if this Object can't get something, you have to this string empty!")]
    [SerializeField] public string getPlaceID;

    [Header("*Norification Object")]
    [Tooltip("It's a factor that changes every time.")]
    [SerializeField] public GameObject CurrentNorificationObject;
    [SerializeField] public Vector3 NOP_Scale;
    [SerializeField] private float NOP_Distance;

    [HideInInspector] public bool CanInteract = true;

    protected GameSystem GameSystem;
    ObjectPooling ObjectPooling;
    CheckGetAllDatas CheckGetAllDatas;
    GameObject GetSomething;
    GetDataWithID getSomethingWithID;
    WordManager WordManager;
    PlaceManager PlaceManager;

    #endregion

    #region Main

    private void Awake()
    {
        GameSystem = GameObject.Find("GameSystem").GetComponent<GameSystem>(); ;
        WordManager = GameObject.Find("WordManager").GetComponent<WordManager>();
        GetSomething = GameObject.Find("GetSomething");
        getSomethingWithID = GetSomething.GetComponent<GetDataWithID>();
        ObjectPooling = GameObject.Find("ObjectPooling").GetComponent<ObjectPooling>();
        PlaceManager = GameObject.Find("PlaceManager").GetComponent <PlaceManager>();

        CanInteract = true;
    }

    private void OnEnable()
    {
        CanInteract = true;
    }

    #endregion

    #region Pointer

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

    #endregion

    #region Interact

    public override void Interact()
    {
        if (this.getWordID != "") { GetWordID(); }
        if (this.getWordActionID != "") { GetWordActionID(); }
        if (this.getPlaceID != "") { GetPlaceID(); }

        if (GameObject.Find("TerminatePart") != null)
        {
            CheckGetAllDatas = GameObject.Find("TerminatePart").GetComponent<CheckGetAllDatas>();
            CheckGetAllDatas.ApplyTerminateBtnAndText();
            ft_setOffNOP();
        }

        //if(CanInteract) { CanInteract = false; }

        /*if (GameObject.Find("InteractiveCanvas").TryGetComponent(out ObjectInteractionButtonGenerator objectInteractionButtonGenerator))
        {
            objectInteractionButtonGenerator.SetOffAllBtns();
        }*/


    }

    #endregion

    #region Get ID

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
    protected void GetPlaceID()
    {
        List<string> PlaceIDs = PlaceManager.currentPlaceIDList;
        foreach (string PlaceID in PlaceIDs)
        {
            if (PlaceID == getPlaceID)
            {
                getPlaceID = "";
                return;
            }
        }
        getSomethingWithID.SetData(getPlaceID);
        getPlaceID = "";
    }

    #endregion

    #region Norification Object

    public void ft_setOnNOP()
    {
        if ((getWordID == "" || getWordID == null) && 
            (getWordActionID == "" || getWordActionID == null) &&
            (getPlaceID == "" || getPlaceID == null)) { return; }

        bool canGettingWord = true, canGettingWordAction = true, canGettingPlace = true;
        if (WordManager.currentWordIDList.Contains(getWordID)) { canGettingWord = false; }
        if (WordManager.currentWordActionIDList.Contains(getWordActionID)) { canGettingWordAction = false; }
        if (PlaceManager.currentPlaceIDList.Contains(getPlaceID)) {  canGettingPlace = false; }

        if(canGettingWord || canGettingWordAction || canGettingPlace)
        {
            CurrentNorificationObject = ObjectPooling.ft_getUsableNOP();
            CurrentNorificationObject.SetActive(true);
            CurrentNorificationObject.transform.localScale = NOP_Scale;
            CurrentNorificationObject.transform.position = this.gameObject.transform.position + (Vector3.up * NOP_Distance);
            CurrentNorificationObject.transform.DORotate(new Vector3(0, 360, 0), 2.5f, RotateMode.FastBeyond360)
                     .SetEase(Ease.Linear)
                     .SetLoops(-1);
        }
    }
    private void ft_setOffNOP()
    {
        if(CurrentNorificationObject != null)
        {
            CurrentNorificationObject.transform.DOKill();
            CurrentNorificationObject.SetActive(false);
            CurrentNorificationObject = null;
        }
        
    }

    #endregion
}