//Refactoring v1.0
public class BasicInteractObject : InteractObject
{
    #region Interact

    public override void Interact()
    {
        if (!Outline.enabled) { return; }

        base.Interact();
        if (GameSystem.Instance.objPanelBtn.gameObject.activeSelf) { GameSystem.Instance.objPanelBtn.gameObject.SetActive(false); }
        GameSystem.Instance.ObjDescOn(this, null);
        ReasoningController.Instance.SetPhotoVisible(this.ID);
    }

    #endregion
}
