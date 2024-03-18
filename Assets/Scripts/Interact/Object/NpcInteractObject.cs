using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NpcInteractObject : InteractObject
{
    [Header("*Description")]
    [SerializeField] protected ConversationBase ConversationBase_SO;

    public override void Interact()
    {
        if (base.CanInteract)
        {
            GameSystem.NpcDescriptionOn(ConversationBase_SO);
            base.Interact();
        }
    }
}
