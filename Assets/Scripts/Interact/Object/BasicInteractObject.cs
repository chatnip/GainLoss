//Refactoring v1.0

using System.Linq;

public class BasicInteractObject : InteractObject
{
    #region Interact

    public override void Interact()
    {
        base.Interact();

        if (GameSystem.Instance.objPanelBtn.gameObject.activeSelf) { GameSystem.Instance.objPanelBtn.gameObject.SetActive(false); }

        GameSystem.Instance.ObjDescOn(this, false, null);
    }

    #endregion
}
