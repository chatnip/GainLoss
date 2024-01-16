using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NpcInteractObject : InteractNpc
{
    [SerializeField] GameSystem GameSystem;


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
        GameSystem.NpcDescriptionOn(base.ConversationBase_SO);

    }
}
