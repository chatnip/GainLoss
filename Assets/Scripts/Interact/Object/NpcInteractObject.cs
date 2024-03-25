using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcInteractObject : InteractObject
{
    [Header("*Description")]
    [SerializeField] protected ConversationBase ConversationBase_SO;
    [SerializeField] public Animator Animator;

    public override void Interact()
    {
        if (base.CanInteract)
        {
            GameSystem.NpcDescriptionOn(ConversationBase_SO, Animator);
            base.Interact();
        }
    }
    public void FixedUpdate()
    {
        if(!Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            if (Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                Animator.SetTrigger("Return");
            }
        }
    }
}
