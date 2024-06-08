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
        Debug.Log("Act");
        if (!Outline.enabled) { return; }

        base.Interact();
        if (GameManager.Instance.currentActPart != GameManager.e_currentActPart.UseActivity &&
            thisAbilityType != ActivityController.e_HomeInteractType.Reasoning)
        {
            GameSystem.Instance.ObjDescOn(this, null);
        }
        else
        {
            
            ActivityController.Instance.currentQuestionWindowType = thisAbilityType;
            ActivityController.Instance.QuestionWindow_ActiveOn(thisAbilityType, 0.25f);
        }
        
    }

    #endregion
}
