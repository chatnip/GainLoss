//Refactoring v1.0

public class BasicInteractObject : InteractObject
{
    #region Interact

    public override void Interact()
    {
        base.Interact();

        if (GameSystem.Instance.objPanelBtn.gameObject.activeSelf) { GameSystem.Instance.objPanelBtn.gameObject.SetActive(false); }
        if (GameSystem.Instance.npcPanelBtn.gameObject.activeSelf) { GameSystem.Instance.npcPanelBtn.gameObject.SetActive(false); }

        GameSystem.Instance.ObjDescOn(this);


    }

    #endregion
}
