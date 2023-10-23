using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractCore : MonoBehaviour, IInteract
{
    [SerializeField] GameObject interactUI;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactUI.SetActive(false);
        }
    }

    public virtual void interact()
    {
        
    }

    public virtual void interactCancel()
    {

    }
}
