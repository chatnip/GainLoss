//Refactoring v1.0
using UnityEngine;

public class BasicInteractObject : InteractObject
{
    #region Value

    [Header("=== Description")]
    [TextArea] [SerializeField] public string basicDescroption;

    #endregion

    #region Interact

    public override void Interact()
    {
        base.Interact();

        if (GameSystem.Instance.objPanelBtn.gameObject.activeSelf) { GameSystem.Instance.objPanelBtn.gameObject.SetActive(false); }
        if (GameSystem.Instance.NpcPanelBtn.gameObject.activeSelf) { GameSystem.Instance.NpcPanelBtn.gameObject.SetActive(false); }

        GameSystem.Instance.ObjDescOn(basicDescroption); 


    }

    #endregion
}
