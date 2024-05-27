//Refactoring v1.0
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class BasicInteractBtn : InteractCore
{
    #region Value

    [Header("=== Compnenet")]
    [SerializeField] public Button thisBtn;

    #endregion

    #region Main

    public virtual void Awake()
    {
        if (this.gameObject.TryGetComponent(out Button btn))
        { thisBtn = btn; }
    }

    #endregion

    #region Pointer

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (TitleInputController.Instance != null)
        {
            // 이중 List -> List로 변경
            List<Button> Section = new List<Button>();
            if (TitleInputController.Instance.SectionBtns != null)
            {
                foreach (List<Button> BtnList in TitleInputController.Instance.SectionBtns)
                {
                    Section.AddRange(BtnList);
                }
                if (Section.Contains(thisBtn)) // PlayerInputController 버튼 리스트에 포함되어있는지
                {
                    TitleInputController.Instance.SelectBtn = thisBtn;
                    TitleInputController.Instance.OnOffSelectedBtn(TitleInputController.Instance.SelectBtn);
                }
            }
        }
        if (PlayerInputController.Instance != null)
        {
            // 이중 List -> List로 변경
            List<Button> Section = new List<Button>();
            if (PlayerInputController.Instance.SectionBtns != null)
            {
                foreach (List<Button> BtnList in PlayerInputController.Instance.SectionBtns)
                {
                    Section.AddRange(BtnList);
                }
                if (Section.Contains(thisBtn)) // PlayerInputController 버튼 리스트에 포함되어있는지
                {
                    PlayerInputController.Instance.SelectBtn = thisBtn;
                    PlayerInputController.Instance.OnOffSelectedBtn(PlayerInputController.Instance.SelectBtn);
                }
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
