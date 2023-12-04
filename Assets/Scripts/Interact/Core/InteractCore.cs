using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractCore : MonoBehaviour, IInteract, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] OutlineObject Outline;
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("click!");
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        Outline.enabled = true;
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        Outline.enabled = false;
    }

    public virtual void Interact()
    {
        
    }
}
