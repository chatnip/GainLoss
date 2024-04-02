using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractObject : InteractCore
{
    #region Value
    [Header("*This Things")]
    [SerializeField] public string thisName;

    [Header("*IDs")]
    [SerializeField] public string objectID;
    [Tooltip("if this Object can't get something, you have to this string empty!")]
    [SerializeField] public string getWordID;
    [Tooltip("if this Object can't get something, you have to this string empty!")]
    [SerializeField] public string getWordActionID;
    [Tooltip("if this Object can't get something, you have to this string empty!")]
    [SerializeField] public string getPlaceID;

    /*[Header("*Norification Object")]
    [Tooltip("It's a factor that changes every time.")]
    [SerializeField] public GameObject CurrentNorificationObject;
    [SerializeField] private Vector3 NOP_Scale;
    [SerializeField] private Vector3 NOP_Distance;*/
    
    [HideInInspector] public bool CanInteract = true;

    protected GameSystem GameSystem;
    ObjectPooling ObjectPooling;
    CheckGetAllDatas CheckGetAllDatas;
    GameObject GetSomething;
    GetDataWithID getSomethingWithID;
    WordManager WordManager;
    protected PlaceManager PlaceManager;

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
        if (this.getWordID != "" && this.getWordID != null) { GetWordID(); }
        if (this.getWordActionID != "" && this.getWordActionID != null) { GetWordActionID(); }
        if (this.getPlaceID != "" && this.getPlaceID != null) { GetPlaceID(); }

        ft_setOff_colorAni();

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
        if(getWordID != null && getWordID != "")
        {
            getSomethingWithID.SetData(getWordID);
            getWordID = "";
        }
        
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
        List<string> PlaceIDs = PlaceManager.currentPlaceID_Dict.Keys.ToList();
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

    #region OutLine Object

    public void ft_setOn_outlineAni()
    {
        Outline.enabled = true;
        Outline.OutlineMode = OutlineObject.Mode.OutlineVisible;
        Outline.OutlineWidth = 1.0f;
        DOTween.To(() => Outline.OutlineWidth, x => Outline.OutlineWidth = x, 3f, 1f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetId(this.gameObject.name + "_OL_Width");
    }
    public void ft_setOff_outlineAni()
    {
        DOTween.Kill(this.gameObject.name + "_OL_Width");
        Outline.OutlineWidth = 1.0f;
        Outline.enabled = false;
    }
    public void ft_setOn_colorAni()
    {
        if ((getWordID == "" || getWordID == null) && 
            (getWordActionID == "" || getWordActionID == null) &&
            (getPlaceID == "" || getPlaceID == null)) { return; }

        bool canGettingWord = true, canGettingWordAction = true, canGettingPlace = true;
        if (WordManager.currentWordIDList.Contains(getWordID)) { canGettingWord = false; }
        if (WordManager.currentWordActionIDList.Contains(getWordActionID)) { canGettingWordAction = false; }
        if (PlaceManager.currentPlaceID_Dict.Keys.ToList().Contains(getPlaceID)) {  canGettingPlace = false; }

        if(canGettingWord && canGettingWordAction && canGettingPlace)
        {
            Outline.OutlineColor = Color.white;
            DOTween.To(() => Outline.OutlineColor, x => Outline.OutlineColor = x, new Color(1, 0.5f, 0.5f, 1), 1f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetId(this.gameObject.name + "_OL_Color");
        }
    }
    public void ft_setOff_colorAni()
    {
        DOTween.Kill(this.gameObject.name + "_OL_Color");
        Outline.OutlineColor = Color.white;
    }

    #endregion
}