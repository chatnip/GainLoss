//Refactoring v1.0
using UnityEngine;

public class NpcInteractObject : InteractObject
{
    #region Value

    [Header("=== Description")]
    [SerializeField] protected ConversationBase ConversationBase_SO;

    [Header("=== Component")]
    [SerializeField] public Animator Animator;
    [Tooltip("playerCharacter will place that this NPC Postion + this Value")]
    [SerializeField] private Transform setPlayerPos;

    #endregion

    #region Framework

    public void FixedUpdate()
    {
        if (!Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            if (Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                Animator.SetTrigger("Return");
            }
        }
    }

    #endregion

    #region Interact

    public override void Interact()
    {
        base.Interact();

        if (GameSystem.Instance.objPanel.gameObject.activeSelf) { GameSystem.Instance.objPanel.gameObject.SetActive(false); }
        if (GameSystem.Instance.NpcPanel.gameObject.activeSelf) { GameSystem.Instance.NpcPanel.gameObject.SetActive(false); }

        PlayerController.Instance.ft_setPlayerSpot(setPlayerPos.transform.position);
        GameSystem.Instance.NpcDescriptionOn(ConversationBase_SO, Animator);
    }
    #endregion
}
