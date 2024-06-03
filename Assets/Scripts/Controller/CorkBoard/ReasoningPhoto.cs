//Refactoring v1.0
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ReasoningPhoto : MonoBehaviour
{
    #region Value

    [Header("=== Relation")]
    [SerializeField] public string relationObejctID;
    [SerializeField] public bool isVisible = false;

    [Header("=== Component")]
    [SerializeField] public Image actualPhoto;

    #endregion

    #region Active On

    public void CheckVisible(float time)
    {
        if (!actualPhoto.gameObject.activeSelf && isVisible)
        {
            actualPhoto.gameObject.SetActive(true);

            Color color = actualPhoto.color;
            color.a = 0f;
            actualPhoto.color = color;

            actualPhoto.DOFade(1f, time);
        }    
    }

    #endregion
}
