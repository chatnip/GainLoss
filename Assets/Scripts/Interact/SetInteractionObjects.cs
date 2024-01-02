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
    

    private void Awake()
    {
        sphereCollider.radius = this.colliderRadius;
        sphereCollider.center = this.ColliderPos;
    }

    private void OnTriggerEnter(Collider OB)
    {
        if(OB.GetComponent<InteractObject>() != null)
        {
            OB.gameObject.GetComponent<OutlineObject>().enabled = true;
            objectInteractionButtonGenerator.ObPooling(OB.gameObject);
        }
    }
    private void OnTriggerExit(Collider OB)
    {
        if(OB.GetComponent<InteractObject>() != null)
        {
            OB.gameObject.GetComponent<OutlineObject>().enabled = false;
            objectInteractionButtonGenerator.SetActiveBtn(OB.gameObject, false);
        }
    }

}
