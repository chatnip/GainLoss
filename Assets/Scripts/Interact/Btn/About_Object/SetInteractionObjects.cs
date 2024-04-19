using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SetInteractionObjects : MonoBehaviour
{
    #region Value

    [Header("*Input")]
    [SerializeField] EventSystem EventSystem;
    [SerializeField] PlayerInputController PlayerInputController;

    [Header("*Collider")]
    [SerializeField] SphereCollider sphereCollider;
    [Tooltip("��ȣ�ۿ� ���� �ݰ�")] 
    [SerializeField] float colliderRadius;
    [Tooltip("�⺻ ��ġ")]
    [SerializeField] Vector3 ColliderPos;

    [Header("*Generator")]
    [SerializeField] ObjectInteractionButtonGenerator objectInteractionButtonGenerator;

    [HideInInspector] public List<GameObject> activeInteractionGOs = new List<GameObject>();

    #endregion

    #region Main

    private void Awake()
    {
        sphereCollider.radius = this.colliderRadius;
        sphereCollider.center = this.ColliderPos;
    }

    #endregion

    #region Trigger

    private void OnTriggerEnter(Collider OB)
    {
        if(OB.TryGetComponent(out InteractObject interactObject))
        {
            if (interactObject.CanInteract)
            {
                interactObject.ft_setOn_outlineAni();
                interactObject.ft_setOn_colorAni();
                //OB.gameObject.GetComponent<OutlineObject>().enabled = true;
                activeInteractionGOs.Add(OB.gameObject);

                objectInteractionButtonGenerator.ObPooling(OB.gameObject, activeInteractionGOs);

                if (objectInteractionButtonGenerator.SectionIsThis)
                { PlayerInputController.SetSectionBtns(objectInteractionButtonGenerator.SetSectionBtns(), objectInteractionButtonGenerator); }
            }
        }
    }
    private void OnTriggerExit(Collider OB)
    {
        if (OB.TryGetComponent(out InteractObject interactObject))
        {
            interactObject.ft_setOff_outlineAni();
            //OB.gameObject.GetComponent<OutlineObject>().enabled = false;
            activeInteractionGOs.Remove(OB.gameObject);

            objectInteractionButtonGenerator.SetActiveBtns(activeInteractionGOs);

            if (objectInteractionButtonGenerator.SectionIsThis)
            { PlayerInputController.SetSectionBtns(objectInteractionButtonGenerator.SetSectionBtns(), objectInteractionButtonGenerator); }
        }

    }
    private void OnTriggerStay(Collider OB)
    {
        if (OB.TryGetComponent(out InteractObject interactObject))
        {
            if (!interactObject.CanInteract)
            {
                interactObject.ft_setOff_outlineAni();
                //OB.gameObject.GetComponent<OutlineObject>().enabled = false;
                activeInteractionGOs.Remove(OB.gameObject);

                objectInteractionButtonGenerator.SetActiveBtns(activeInteractionGOs);
            }
        }
    }


    #endregion

    #region Func

    public void OffInteractiveOB()
    {
        //sphereCollider.radius = 0;
        sphereCollider.enabled = false;
        activeInteractionGOs.Clear();
        objectInteractionButtonGenerator.SetActiveBtns(activeInteractionGOs);
    }
    public void OnInteractiveOB()
    {
        //sphereCollider.radius = this.colliderRadius;
        sphereCollider.enabled = true;
    }

    #endregion
}
