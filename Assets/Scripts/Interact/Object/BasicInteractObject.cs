//Refactoring v1.0
using UnityEngine;

public class BasicInteractObject : InteractObject
{
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
}
