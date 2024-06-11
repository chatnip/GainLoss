//Refactoring v1.0
using UnityEngine;

public class ReasoningArrow : ReasoningModule
{
    #region Value

    [Header("=== Relation")]
    [SerializeField] ReasoningPhoto[] relationPhotos;


    #endregion

    #region OnEnable

    public override void SetEachTime(float time)
    {
        // Set Visible
        if (relationPhotos[0].isActive && relationPhotos[1].isActive) 
        { this.isActive = true; }

        base.SetEachTime(time);
    }

    #endregion
}
