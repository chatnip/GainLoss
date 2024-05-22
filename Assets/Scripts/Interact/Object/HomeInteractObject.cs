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
        if (base.CanInteract)
        {
            ActivityController.Instance.currentQuestionWindowType = thisAbilityType;
            ActivityController.Instance.QuestionWindow_ActiveOn(thisAbilityType, 0.25f);



            base.Interact();
        }
    }

    #endregion
}
