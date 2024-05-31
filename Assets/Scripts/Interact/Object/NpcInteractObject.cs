//Refactoring v1.0
using UnityEngine;

public class NpcInteractObject : InteractObject
{
    #region Value

    [Header("=== Component")]
    [SerializeField] public Animator Animator;

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
        if (!Outline.enabled) { return; }

        base.Interact();
        if (GameSystem.Instance.objPanelBtn.gameObject.activeSelf) { GameSystem.Instance.objPanelBtn.gameObject.SetActive(false); }
        GameSystem.Instance.ObjDescOn(this, null);
    }
    #endregion
}
