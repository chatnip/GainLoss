using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnlyEffectfulBtn : InteractCore
{
    [Header("*Manager")]
    [SerializeField] PlayerInputController PlayerInputController;

    [Header("*Compnenet")]
    [SerializeField] public Button thisBtn;

    public void Awake()
    {
        //InteractiveBtn_comp = GetComponent<Button>();
        PlayerInputController = GameObject.Find("PlayerInputController").GetComponent<PlayerInputController>();
        thisBtn = this.gameObject.GetComponent<Button>();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        // Base
        thisBtn.TryGetComponent(out RectTransform RT);
        DOTween.Kill(RT.localScale);
        RT.DOScale(Vector3.one * 1.1f, 0.1f);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        // Base
        thisBtn.TryGetComponent(out RectTransform RT);
        DOTween.Kill(RT.localScale);
        RT.DOScale(Vector3.one * 1.0f, 0.1f);
    }
}
