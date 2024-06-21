//Refactoring v1.0
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class BasicInteractObject : InteractObject
{
    #region Value

    [Header("=== NPC Things")]
    [Tooltip("필요없으면 삽입 X")][SerializeField] public AnimatorController _AC;
    [Tooltip("필요없으면 삽입 X")]
    [SerializeField] List<AnimationClip> _Acs_ByIntetact;

    #endregion

    #region Interact

    public override void Interact()
    {
        if (!Outline.enabled) { return; }

        if (DialogManager.Instance.objPanelBtn.gameObject.activeSelf) { DialogManager.Instance.objPanelBtn.gameObject.SetActive(false); }

        if (!this.IsInteracted)
        {
            base.Interact();
            string startDialogID = DataManager.Instance.Get_DialogID(this.ID);
            DialogManager.Instance.ObjDescOn(this, startDialogID);

            string getReasoningID = DataManager.Instance.Get_VisibleReasoningID(ID);
            if(getReasoningID != "")
            {
                Debug.Log(getReasoningID);
                ReasoningController.Instance.SetVisibleReasoning(getReasoningID);
            }
        }
        else
        {
            DialogManager.Instance.ObjDescOn(this, endDialogID);
        }
        
    }

    #endregion

    #region Animation

    public AnimationClip GetAnimtionClip(string animationClipName)
    {
        foreach (AnimationClip Ac in _Acs_ByIntetact)
        {
            if (Ac.name == animationClipName)
            {
                return Ac;
            }
        }
        return null;
    }

    #endregion
}
