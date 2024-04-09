using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BasicInteractObject : InteractObject
{
    #region Value

    [Header("*Description")]
    [TextArea] [SerializeField] public List<string> description;
    [TextArea] [SerializeField] public string basicDescroption;

    #endregion

    #region Interact

    public override void Interact()
    {
        if(base.CanInteract)
        {
            if (description.Count > 0 &&
                description[Convert.ToInt32(PlaceManager.currentPlaceID_Dict[PlaceManager.currentPlace.ID]) - 1] != null &&
                description[Convert.ToInt32(PlaceManager.currentPlaceID_Dict[PlaceManager.currentPlace.ID]) - 1] != "" &&
                description.Count >= Convert.ToInt32(PlaceManager.currentPlaceID_Dict[PlaceManager.currentPlace.ID]))
            { GameSystem.ObjectDescriptionOn(description[Convert.ToInt32(PlaceManager.currentPlaceID_Dict[PlaceManager.currentPlace.ID]) - 1]); }
            else
            { GameSystem.ObjectDescriptionOn(basicDescroption); }

            base.Interact();

        }
    }

    #endregion
}
