//Refactoring v1.0
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SetInteractionObjects : Singleton<SetInteractionObjects>
{
    #region Value

    [Header("=== Component")]
    [SerializeField] SphereCollider sphereCollider;
    [HideInInspector] public List<GameObject> activeInteractionGOs = new List<GameObject>();

    #endregion

    #region Framework & Base Set

    public void Offset()
    {
        sphereCollider.enabled = true;
    }

    protected override void Awake()
    {
        base.Awake();
    }

    #endregion

    #region Trigger

    private void OnTriggerEnter(Collider OB)
    {
        if(OB.TryGetComponent(out InteractObject interactObject))
        {
            if(interactObject is HomeInteractObject && PlaceManager.Instance.isStreamingTime)
            { return; }
            interactObject.SetOn_outlineAni();
            interactObject.SetOn_colorAni();

            activeInteractionGOs.Add(OB.gameObject);

            ObjectInteractionButtonGenerator.Instance.ObPooling(OB.gameObject, activeInteractionGOs);

            if (ObjectInteractionButtonGenerator.Instance.SectionIsThis)
            { 
                PlayerInputController.Instance.SetSectionBtns(
                    ObjectInteractionButtonGenerator.Instance.SetSectionBtns(), 
                    ObjectInteractionButtonGenerator.Instance); 
            }

        }
    }

    private void OnTriggerExit(Collider OB)
    {
        if (OB.TryGetComponent(out InteractObject interactObject))
        {
            interactObject.SetOff_outlineAni();
            interactObject.SetOff_colorAni();

            activeInteractionGOs.Remove(OB.gameObject);

            ObjectInteractionButtonGenerator.Instance.SetActiveBtns(activeInteractionGOs);

            if (ObjectInteractionButtonGenerator.Instance.SectionIsThis)
            {
                PlayerInputController.Instance.SetSectionBtns(
                    ObjectInteractionButtonGenerator.Instance.SetSectionBtns(), 
                    ObjectInteractionButtonGenerator.Instance); 
            }
        }
    }

    #endregion

    #region Set Interactive Object

    public void SetOff_InteractiveOB()
    {
        sphereCollider.enabled = false;
        activeInteractionGOs.Clear();
        ObjectInteractionButtonGenerator.Instance.SetActiveBtns(activeInteractionGOs);
    }
    public void SetOn_InteractiveOB()
    {
        sphereCollider.enabled = true;
    }

    #endregion
}
