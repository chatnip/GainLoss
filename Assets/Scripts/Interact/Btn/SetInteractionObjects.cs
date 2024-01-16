using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetInteractionObjects : MonoBehaviour
{
    [Header("*Collider")]
    [SerializeField] SphereCollider sphereCollider;
    [Tooltip("상호작용 가능 반경")] 
    [SerializeField] float colliderRadius;
    [Tooltip("기본 위치")]
    [SerializeField] Vector3 ColliderPos;

    [Header("*Generator")]
    [SerializeField] ObjectInteractionButtonGenerator objectInteractionButtonGenerator;

    [HideInInspector] public List<GameObject> activeInteractionGOs = new List<GameObject>();

    private void Awake()
    {
        sphereCollider.radius = this.colliderRadius;
        sphereCollider.center = this.ColliderPos;
    }

    private void OnTriggerEnter(Collider OB)
    {
        if(OB.TryGetComponent<InteractObject>(out InteractObject interactObject))
        {
            if (interactObject.CanInteract)
            {
                OB.gameObject.GetComponent<OutlineObject>().enabled = true;
                activeInteractionGOs.Add(OB.gameObject);

                objectInteractionButtonGenerator.ObPooling(OB.gameObject, activeInteractionGOs);
            }
        }
        if (OB.TryGetComponent<InteractNpc>(out InteractNpc interactNpc))
        {
            if (interactNpc.CanInteract)
            {
                OB.gameObject.GetComponent<OutlineObject>().enabled = true;
                activeInteractionGOs.Add(OB.gameObject);

                objectInteractionButtonGenerator.ObPooling(OB.gameObject, activeInteractionGOs);
            }
        }
    }
    private void OnTriggerExit(Collider OB)
    {
        if (OB.TryGetComponent<InteractObject>(out InteractObject interactObject))
        {
            OB.gameObject.GetComponent<OutlineObject>().enabled = false;
            activeInteractionGOs.Remove(OB.gameObject);

            objectInteractionButtonGenerator.SetActiveBtns(activeInteractionGOs);
        }
        if (OB.TryGetComponent<InteractNpc>(out InteractNpc interactNpc))
        {
            OB.gameObject.GetComponent<OutlineObject>().enabled = false;
            activeInteractionGOs.Remove(OB.gameObject);

            objectInteractionButtonGenerator.SetActiveBtns(activeInteractionGOs);
        }

    }
    private void OnTriggerStay(Collider OB)
    {
        if (OB.TryGetComponent<InteractObject>(out InteractObject interactObject))
        {
            if (!interactObject.CanInteract)
            {
                OB.gameObject.GetComponent<OutlineObject>().enabled = false;
                activeInteractionGOs.Remove(OB.gameObject);

                objectInteractionButtonGenerator.SetActiveBtns(activeInteractionGOs);
            }
        }
        if (OB.TryGetComponent<InteractNpc>(out InteractNpc interactNpc))
        {
            if (!interactNpc.CanInteract)
            {
                OB.gameObject.GetComponent<OutlineObject>().enabled = false;
                activeInteractionGOs.Remove(OB.gameObject);

                objectInteractionButtonGenerator.SetActiveBtns(activeInteractionGOs);
            }
        }
    }

    public void OffInteractiveOB()
    {
        sphereCollider.radius = 0;
    }
    public void OnInteractiveOB()
    {
        sphereCollider.radius = this.colliderRadius;
    }
    public void ClearInteractiveOB()
    {
        activeInteractionGOs.Clear();
    }

}
