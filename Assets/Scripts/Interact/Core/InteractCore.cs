//Refactoring v1.0
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractCore : MonoBehaviour, IInteract, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    #region Value

    [Header("=== CoreThings")]
    [SerializeField] protected OutlineObject Outline;

    #endregion

    #region Pointer

    public virtual void OnPointerDown(PointerEventData eventData)
    { Debug.Log("click!"); }

    public virtual void OnPointerEnter(PointerEventData eventData)
    { }

    public virtual void OnPointerExit(PointerEventData eventData)
    { }

    #endregion

    #region Interact

    public virtual void Interact()
    { }

    #endregion

}
