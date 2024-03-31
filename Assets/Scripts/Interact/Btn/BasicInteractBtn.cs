using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BasicInteractBtn : InteractCore
{
    #region Value

    [Header("*Manager")]
    [SerializeField] PlayerInputController PlayerInputController;

    [Header("*Compnenet")]
    [SerializeField] public Button thisBtn;

    #endregion

    #region Main

    public void Awake()
    {
        //InteractiveBtn_comp = GetComponent<Button>();
        PlayerInputController = GameObject.Find("PlayerInputController").GetComponent<PlayerInputController>();
        thisBtn = this.gameObject.GetComponent<Button>();
    }

    #endregion

    #region Pointer

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        // 이중 List -> List로 변경
        List<Button> Section = new List<Button>();
        if (PlayerInputController.SectionBtns != null)
        {
            foreach (List<Button> BtnList in PlayerInputController.SectionBtns)
            {
                Section.AddRange(BtnList);
            }
            if (Section.Contains(thisBtn)) // PlayerInputController 버튼 리스트에 포함되어있는지
            {
                PlayerInputController.SelectBtn = thisBtn;
                PlayerInputController.OnOffSelectedBtn(PlayerInputController.SelectBtn);
            }
        }

        

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

    #endregion
}
