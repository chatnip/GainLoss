//Refactoring v1.0
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractObject : InteractCore
{
    #region Value

    [Header("==== ID")]
    [SerializeField] public string ID;
    [HideInInspector] public string endDialogID;

    [Header("=== Interact")]
    [SerializeField] public bool IsInteracted = false;

    #endregion

    #region Pointer

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        if (GameManager.Instance.canInput &&
            GameManager.Instance.canInteractObject)
        { Interact(); }
        
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
    }

    #endregion

    #region Interact

    public override void Interact()
    {
        if (!Outline.enabled) { return; }

        SetOff_colorAni();
    }

    #endregion

    #region Outline Object

    public void SetOn_outlineAni()
    {
        Outline.enabled = true;
        Outline.OutlineMode = OutlineObject.Mode.OutlineVisible;
        Outline.OutlineWidth = 1.0f;
        DOTween.To(() => Outline.OutlineWidth, x => Outline.OutlineWidth = x, 3f, 1f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetId(this.gameObject.name + "_OL_Width");
    }
    public void SetOff_outlineAni()
    {
        DOTween.Kill(this.gameObject.name + "_OL_Width");
        Outline.OutlineWidth = 1.0f;
        Outline.enabled = false;
    }
    public void SetOn_colorAni()
    {
        if (IsInteracted)
        { return; }

        Outline.OutlineColor = Color.white;
        DOTween.To(() => Outline.OutlineColor, x => Outline.OutlineColor = x, new Color(1, 0.5f, 0.5f, 1), 1f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetId(this.gameObject.name + "_OL_Color");

    }
    public void SetOff_colorAni()
    {
        DOTween.Kill(this.gameObject.name + "_OL_Color");
        Outline.OutlineColor = Color.white;
    }

    #endregion
}