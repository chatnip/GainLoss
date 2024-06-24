//Refactoring v1.0
using UnityEngine;

public class HomeInteractObject : InteractObject
{
    #region Value

    [Header("=== Type")]
    [SerializeField] ActivityController.e_HomeInteractType thisAbilityType;

    #endregion

    #region Interact

    public override void Interact()
    {
        if (!Outline.enabled) { return; }

        base.Interact();
        if (GameSystem.Instance.currentActPart != GameSystem.e_currentActPart.UseActivity &&
            thisAbilityType != ActivityController.e_HomeInteractType.Reasoning)
        {
            DialogManager.Instance.ObjDescOn(this, "-1");
        }
        else
        {
            ActivityController.Instance.currentQuestionWindowType = thisAbilityType;
            ActivityController.Instance.QuestionWindow_ActiveOn(thisAbilityType, 0.25f);
        }
        
    }

    #endregion
}
