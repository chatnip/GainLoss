using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnlyEffectfulBtn : InteractCore
{
    #region Value

    [Header("=== Compnenet")]
    [SerializeField] public Button thisBtn;

    [Header("=== Value")]
    [SerializeField] public float maxSize = 1.1f;
    [SerializeField] public float minSize = 1.0f;

    #endregion

    #region Framework

    public void Awake()
    {
        thisBtn = this.gameObject.GetComponent<Button>();
    }

    #endregion

    #region Pointer

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        // Base
        thisBtn.TryGetComponent(out RectTransform RT);
        DOTween.Kill(RT.localScale);
        RT.DOScale(Vector3.one * maxSize, 0.1f);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        // Base
        thisBtn.TryGetComponent(out RectTransform RT);
        DOTween.Kill(RT.localScale);
        RT.DOScale(Vector3.one * minSize, 0.1f);
    }

    #endregion
}
