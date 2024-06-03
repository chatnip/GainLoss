//Refactoring v1.0
using DG.Tweening;
using UnityEngine;

public class ReasoningArrow : MonoBehaviour
{
    #region Value

    [Header("=== Relation")]
    [SerializeField] ReasoningPhoto[] relationPhotos;
    [SerializeField] bool isVisible = false;

    [Header("=== Component")]
    [SerializeField] CanvasGroup thisCG;

    #endregion

    #region Active On

    public void CheckVisible()
    {
        if (relationPhotos[0].isVisible && relationPhotos[1].isVisible) { this.isVisible = true; }

        if(!this.gameObject.activeSelf && this.isVisible) 
        {
            this.gameObject.SetActive(true);

            thisCG.alpha = 0f;

            thisCG.DOFade(1f, 1f);
        }
    }

    #endregion
}
