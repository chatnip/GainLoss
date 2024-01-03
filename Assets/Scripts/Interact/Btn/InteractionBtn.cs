using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractionBtn : InteractCore
{
    [Header("*Manager")]
    [SerializeField] GameSystem GameSystem;

    [Header("*Set")]
    [SerializeField] public TMP_Text txt_name_left;
    [SerializeField] public TMP_Text txt_name_right;

    [SerializeField] public GameObject TargetGO;
    //[SerializeField] Button InteractiveBtn_comp;

    public void Awake()
    {
        //InteractiveBtn_comp = GetComponent<Button>();
        GameSystem = GameObject.Find("GameSystem").GetComponent<GameSystem>();
    }
    
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        TargetGO.GetComponent<InteractObject>().Interact();
    }
}
