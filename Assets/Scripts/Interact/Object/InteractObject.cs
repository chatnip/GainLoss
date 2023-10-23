using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractObject : InteractCore
{
    [SerializeField] InteractObjectBase interactObjectBase;
    [SerializeField] RectTransform interactCanvas;
    [SerializeField] TMP_Text interactText;
    [SerializeField] GameObject canvasObject;

    public override void interact()
    {
        base.interact();

        canvasObject.TryGetComponent(out RectTransform canvasObjectRect);       
        canvasObject.transform.SetParent(interactCanvas);
        canvasObjectRect.localPosition = new Vector3(0, 0, -1430);
        canvasObjectRect.rotation = Quaternion.identity;
        canvasObjectRect.localScale = Vector3.one;

        canvasObject.TryGetComponent(out InteractedObject interacted);
        interacted.enabled = true;
        canvasObject.layer = 5;

        interactText.text = interactObjectBase.InteractObjectInfo;

        canvasObject.SetActive(true);
    }

    public override void interactCancel()
    {
        canvasObject.SetActive(false);
        canvasObject.transform.SetParent(this.transform);
        canvasObject.TryGetComponent(out InteractedObject interacted);
        interacted.enabled = false;
    }
}