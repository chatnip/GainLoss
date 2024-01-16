using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractNpc : InteractCore
{
    [SerializeField] string NpcID;
    [SerializeField] protected ConversationBase ConversationBase_SO;
    [HideInInspector] public bool CanInteract = true;

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

    }
}


