//Refactoring v1.0
using DG.Tweening;
using UnityEngine;

public class ReasoningArrow : ReasoningModule
{
    #region Value

    [Header("=== Relation")]
    [SerializeField] ReasoningPhoto[] relationPhotos;
    [SerializeField] public bool isVisible = false;

    [Header("=== Component")]
    [SerializeField] CanvasGroup thisCG;

    #endregion

    #region Active On

    public void CheckVisible(float time)
    {
        if (relationPhotos[0].isVisible && relationPhotos[1].isVisible) { this.isVisible = true; }

        if(!this.gameObject.activeSelf && this.isVisible) 
        {
            this.gameObject.SetActive(true);

            thisCG.alpha = 0f;

            thisCG.DOFade(1f, time);
        }
    }

    #endregion
}
