//Refactoring v1.0
using TMPro;
using UnityEngine;

public class ReasoningPhoto : ReasoningModule
{
    #region Value

    [Header("=== Name")]
    [SerializeField] bool isVisibleName = false;
    [SerializeField] TMP_Text nameTxt;

    #endregion

    #region OnEnable

    public override void SetEachTime(float time)
    {
        // Set Visible
        if (!this.gameObject.activeSelf && base.isActive)
        { base.isActive = true; }

        base.SetEachTime(time);

        if (!isVisibleName)
        { nameTxt.text = "???"; }
        else
        { nameTxt.text = DataManager.Instance.Get_ReasoningName(thisID); }
    }

    #endregion
}
