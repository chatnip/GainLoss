using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BasicInteractObject : InteractObject
{
    [SerializeField] GameSystem GameSystem;
    [TextArea]
    [SerializeField] string description;

    private void OnEnable()
    {
        if(GameSystem == null)
        {
            GameObject game = GameObject.Find("GameSystem");
            game.TryGetComponent(out GameSystem);
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        GameSystem.ObjectDescriptionOn(description);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
    }
}
