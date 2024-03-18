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
                cutsceneSO.currentCSSO = this.cutsceneSO;
                cutsceneSO.cutsceneSeq = cutsceneSO.makeCutscene(GameSystem.cutsceneImg, GameSystem.cutsceneTxt);
                cutsceneSO.cutsceneSeq.OnComplete(() =>
                {
                    GameSystem.cutsceneImg.color = new Color(0, 0, 0, 0);
                    GameSystem.cutsceneImg.gameObject.SetActive(false);
                    GameSystem.cutsceneTxt.text = "";
                    cutsceneSO.currentCSSO = null;
                    base.Interact();
                });
            }
            else
            {
                GameSystem.ObjectDescriptionOn(description);
                base.Interact();
            }
            
        }
    }
}
