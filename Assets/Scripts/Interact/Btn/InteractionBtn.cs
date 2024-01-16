using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InteractionBtn : InteractCore
{
    [Header("*Manager")]
    [SerializeField] GameSystem GameSystem;

    [Header("*Set")]
    [SerializeField] public TMP_Text txt_name_left;
    [SerializeField] public TMP_Text txt_name_right;

    [SerializeField] public GameObject TargetGO;
    [SerializeField] public Button thisBtn;
    //[SerializeField] Button InteractiveBtn_comp;

    public void Awake()
    {
        //InteractiveBtn_comp = GetComponent<Button>();
        GameSystem = GameObject.Find("GameSystem").GetComponent<GameSystem>();
    }
    
    public override void OnPointerDown(PointerEventData eventData)
    {
        if(thisBtn.interactable)
        {
            base.OnPointerDown(eventData);
            if (TargetGO.TryGetComponent(out InteractObject interactObject)) { interactObject.Interact(); }
            else if (TargetGO.TryGetComponent(out InteractNpc interactNpc)) { interactNpc.Interact(); }
        }
    }
}
