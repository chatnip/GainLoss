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

        if (GameSystem.Instance.objPanel.gameObject.activeSelf) { GameSystem.Instance.objPanel.gameObject.SetActive(false); }
        if (GameSystem.Instance.NpcPanel.gameObject.activeSelf) { GameSystem.Instance.NpcPanel.gameObject.SetActive(false); }

        GameSystem.Instance.ObjectDescriptionOn(basicDescroption); 


    }

    #endregion
}
