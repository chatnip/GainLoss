//Refactoring v1.0

public class BasicInteractObject : InteractObject
{
    #region Interact

    public override void Interact()
    {
        if (!Outline.enabled) { return; }

        if (GameSystem.Instance.objPanelBtn.gameObject.activeSelf) { GameSystem.Instance.objPanelBtn.gameObject.SetActive(false); }

        if (!this.IsInteracted)
        {
            base.Interact();
            string startDialogID = DataManager.Instance.Get_DialogID(this.ID);
            GameSystem.Instance.ObjDescOn(this, startDialogID);
            ReasoningController.Instance.SetVisibleReasoning(DataManager.Instance.Get_VisibleReasoningID(ID));
        }
        else
        {
            GameSystem.Instance.ObjDescOn(this, endDialogID);
        }
        
    }

    #endregion
}
