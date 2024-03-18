using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BasicInteractObject : InteractObject
{
    [Header("*Description")]
    [TextArea] [SerializeField] public string description;
    [SerializeField] cutsceneSO cutsceneSO;

    public override void Interact()
    {
        if(base.CanInteract)
        {
            if(cutsceneSO != null)
            {
                /*Sequence sequence = cutsceneSO.makeCutscene(cutsceneSO, cutsceneImg, cutsceneTxt, cutsceneIsPlaying);
                sequence.OnComplete(() =>
                { 
                    cutsceneImg.color = new Color(0, 0, 0, 0);
                    cutsceneImg.gameObject.SetActive(false);
                    cutsceneTxt.text = "";
                });*/
            }
            else
            {
                GameSystem.ObjectDescriptionOn(description);
            }
            base.Interact();
        }
    }
}
