//Refactoring v1.0
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractObjectBtn : InteractCore
{
    #region Value

    [Header("=== Set")]
    [SerializeField] public TMP_Text txt_name_left;
    [SerializeField] public GameObject TargetGO;
    [SerializeField] public Button thisBtn;

    #endregion

    #region Framework

    public void Awake()
    {
        thisBtn = this.gameObject.GetComponent<Button>();
    }

    #endregion

    #region Interact

    public void interactObject()
    {
        if (TargetGO.TryGetComponent(out InteractObject interactObject)) { interactObject.Interact(); }
    }

    #endregion
}
