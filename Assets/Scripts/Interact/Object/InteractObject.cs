using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractObject : InteractCore
{
    [SerializeField] ObjectPooling ObjectPooling;
    [SerializeField] InteractObjectBase interactObjectBase;
    [SerializeField] RectTransform interactCanvas;
    [SerializeField] TMP_Text interactText;
    InteractedObject interactedObject;

    public override void Interact()
    {
        base.Interact();

        InteractSetting();
    }

    private void InteractSetting()
    {
        interactCanvas.gameObject.SetActive(true);
        interactedObject = ObjectPooling.InteractedObjectPool();
        interactedObject.TryGetComponent(out RectTransform rect);
        interactedObject.transform.SetParent(interactCanvas);
        ObjectPosSetting(rect);
        interactText.text = interactObjectBase.InteractObjectInfo;
        interactedObject.interactObjectBase = this.interactObjectBase;
        interactedObject.gameObject.SetActive(true);
    }

    private void ObjectPosSetting(RectTransform rect)
    {
        rect.localPosition = new Vector3(0, 0, -1430);
        rect.rotation = Quaternion.identity;
        int val = interactObjectBase.MinZoomValue;
        rect.localScale = new Vector3(val, val, val);
    }

    public override void InteractCancel()
    {
        interactCanvas.gameObject.SetActive(false);
        interactedObject.gameObject.SetActive(false);
        interactedObject.transform.SetParent(ObjectPooling.transform);
        ObjectPooling.ObjectPick(interactedObject);
    }
}