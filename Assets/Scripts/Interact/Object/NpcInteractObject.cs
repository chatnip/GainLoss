using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NpcInteractObject : InteractObject
{
    #region Value

    [Header("*Description")]
    [SerializeField] protected ConversationBase ConversationBase_SO;
    [SerializeField] public Animator Animator;
    [Tooltip("playerCharacter will place that this NPC Postion + this Value")]
    [SerializeField] private Transform setPlayerPos;

    #endregion

    #region Interact

    public override void Interact()
    {
        if (base.CanInteract)
        {
            if (GameSystem.objPanel.gameObject.activeSelf) { GameSystem.objPanel.gameObject.SetActive(false); }
            if (GameSystem.NpcPanel.gameObject.activeSelf) { GameSystem.NpcPanel.gameObject.SetActive(false); }

            GameSystem.PlayerController.ft_setPlayerSpot(setPlayerPos.transform.position);
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
    #endregion
}
