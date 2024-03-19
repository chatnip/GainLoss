using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BasicInteractObject : InteractObject
{
    [Header("*Description")]
    [TextArea] [SerializeField] public List<string> description;
    [TextArea] [SerializeField] public string basicDescroption;
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
                    if (description[Convert.ToInt32(PlaceManager.currentPlaceID_Dict[PlaceManager.currentPlace.ID]) - 1] != null &&
                        description[Convert.ToInt32(PlaceManager.currentPlaceID_Dict[PlaceManager.currentPlace.ID]) - 1] != "" &&
                        description.Count >= Convert.ToInt32(PlaceManager.currentPlaceID_Dict[PlaceManager.currentPlace.ID]))
                    { GameSystem.ObjectDescriptionOn(description[Convert.ToInt32(PlaceManager.currentPlaceID_Dict[PlaceManager.currentPlace.ID]) - 1]); }
                    else 
                    { GameSystem.ObjectDescriptionOn(basicDescroption); }

                    base.Interact();
                });
            }
            else
            {
                if (description[Convert.ToInt32(PlaceManager.currentPlaceID_Dict[PlaceManager.currentPlace.ID]) - 1] != null &&
                    description[Convert.ToInt32(PlaceManager.currentPlaceID_Dict[PlaceManager.currentPlace.ID]) - 1] != "" &&
                    description.Count >= Convert.ToInt32(PlaceManager.currentPlaceID_Dict[PlaceManager.currentPlace.ID]))
                { GameSystem.ObjectDescriptionOn(description[Convert.ToInt32(PlaceManager.currentPlaceID_Dict[PlaceManager.currentPlace.ID]) - 1]); }
                else 
                { GameSystem.ObjectDescriptionOn(basicDescroption); }

                base.Interact();
            }
            
        }
    }
}
