using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BasicInteractObject : InteractObject
{
    [SerializeField] GameSystem GameSystem;
    [TextArea]
    [SerializeField] public string description;

    private void OnEnable()
    {
        if (GameSystem == null)
        {
            GameObject game = GameObject.Find("GameSystem");
            game.TryGetComponent(out GameSystem);
        }
    }

    public override void Interact()
    {
        GameSystem.ObjectDescriptionOn(description);
        base.Interact();

        /*GameSystem.ObjectDescriptionOn(description)
            .OnComplete(() =>
            {
                if (GetWordID() != null)
                {
                    GetWordID()
                        .OnComplete(() =>
                        {
                            AlreadyGotWordID();
                        });
                }
                if (GetWordActionID() != null)
                {
                    Debug.Log("µé¾î°¨");
                    GetWordActionID()
                        .OnComplete(() =>
                        {
                            AlreadyGotWordActionID();
                        });
                }
            });*/

    }

    /*    public override void OnPointerDown(PointerEventData eventData)
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
        }*/
}
