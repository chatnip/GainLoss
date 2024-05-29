using DG.Tweening;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TitleInputController : Singleton<TitleInputController>
{
    #region Value

    [Header("*Property")]
    [SerializeField] Title Title;
    [SerializeField] Option Option;

    [Header("*Input Setting")]
    [SerializeField] public PlayerInput _input;
    [SerializeField] public List<List<Button>> SectionBtns;
    [SerializeField] public Button SelectBtn;

    public IInteract interact;

    #endregion

    #region Main

    protected override void Awake()
    {
        TryGetComponent(out _input);

        _input.ObserveEveryValueChanged(x => x.currentControlScheme)
            .Subscribe(OnControlSchemeChanged);
    }
    public void OnControlSchemeChanged(string _controlScheme)
    { }

    private void OnEnable()
    {
        EnableTitleInput();
    }
    private void OnDisable()
    {
        DisableTitleInput();
    }


    private void EnableTitleInput()
    {
        var TitleInput = _input.actions.FindActionMap("Title");

        TitleInput["SelectUp"].started += UpSelectedBtn;
        TitleInput["SelectDown"].started += DownSelectedBtn;
        TitleInput["SelectLeft"].started += LeftSelectedBtn;
        TitleInput["SelectRight"].started += RightSelectedBtn;

        TitleInput["Select"].started += select;

        TitleInput["Apply"].started += ApplyButton;

        TitleInput["Back"].started += back;
    }

    private void DisableTitleInput()
    {
        _input.actions["SelectUp"].started -= UpSelectedBtn;
        _input.actions["SelectDown"].started -= DownSelectedBtn;
        _input.actions["SelectLeft"].started -= LeftSelectedBtn;
        _input.actions["SelectRight"].started -= RightSelectedBtn;

        _input.actions["Select"].started -= select;

        _input.actions["Apply"].started -= ApplyButton;

        _input.actions["Back"].started -= back;
    }

    #endregion

    #region Section

    private void RightSelectedBtn(InputAction.CallbackContext obj)
    {
        if (!GameManager.Instance.canInput) { return; }

        if (SelectBtn != null && SelectBtn.TryGetComponent(out ArrowLRInteractBtn AIB))
        { AIB.SetEnumValue(true); return; }
        else if (SelectBtn != null && SelectBtn.TryGetComponent(out SliderInteractBtn SIB))
        { SIB.SetSliderUI_ByPad(0.1f); return; }


        if (SectionBtns != null && SectionBtns.Count >= 1)
        {
            List<Button> currentBtnList = new List<Button>();
            foreach (List<Button> BtnList in SectionBtns)
            {
                if (BtnList.Contains(SelectBtn))
                {
                    currentBtnList = BtnList;
                }
            }

            int index = currentBtnList.IndexOf(SelectBtn);
            if (index >= currentBtnList.Count - 1)
            {
                SelectBtn = currentBtnList[0];
            }
            else
            {
                SelectBtn = currentBtnList[(index + 1)];
            }
            OnOffSelectedBtn(SelectBtn);
        }

    }
    private void LeftSelectedBtn(InputAction.CallbackContext obj)
    {
        if (!GameManager.Instance.canInput) { return; }

        if (SelectBtn != null && SelectBtn.TryGetComponent(out ArrowLRInteractBtn AIB))
        { AIB.SetEnumValue(false); return; }
        else if (SelectBtn != null && SelectBtn.TryGetComponent(out SliderInteractBtn SIB))
        { SIB.SetSliderUI_ByPad(-0.1f); return; }

        if (SectionBtns != null && SectionBtns.Count >= 1)
        {
            List<Button> currentBtnList = new List<Button>();
            foreach (List<Button> BtnList in SectionBtns)
            {
                if (BtnList.Contains(SelectBtn))
                {
                    currentBtnList = BtnList;
                }
            }

            int index = currentBtnList.IndexOf(SelectBtn);
            if (index <= 0)
            {
                SelectBtn = currentBtnList[currentBtnList.Count - 1];
            }
            else
            {
                SelectBtn = currentBtnList[(index - 1)];
            }
            OnOffSelectedBtn(SelectBtn);
        }
    }
    private void DownSelectedBtn(InputAction.CallbackContext obj)
    {
        if (!GameManager.Instance.canInput) { return; }

        if (SectionBtns != null && SectionBtns.Count > 1)
        {
            List<Button> currentBtnList = new List<Button>();
            foreach (List<Button> BtnList in SectionBtns)
            {
                if (BtnList.Contains(SelectBtn))
                {
                    currentBtnList = BtnList;
                }
            }

            int lineIndex = SectionBtns.IndexOf(currentBtnList);
            int index = currentBtnList.IndexOf(SelectBtn);

            if (lineIndex >= SectionBtns.Count - 1)
            {
                if (index > SectionBtns[0].Count - 1)
                {
                    SelectBtn = SectionBtns[0][0];
                }
                else
                {
                    SelectBtn = SectionBtns[0][index];
                }
            }
            else
            {
                if (index > SectionBtns[lineIndex + 1].Count - 1)
                {
                    SelectBtn = SectionBtns[lineIndex + 1][0];
                }
                else
                {
                    SelectBtn = SectionBtns[lineIndex + 1][index];
                }
            }
            OnOffSelectedBtn(SelectBtn);
        }
    }
    private void UpSelectedBtn(InputAction.CallbackContext obj)
    {
        if (!GameManager.Instance.canInput) { return; }

        if (SectionBtns != null && SectionBtns.Count > 1)
        {
            List<Button> currentBtnList = new List<Button>();
            foreach (List<Button> BtnList in SectionBtns)
            {
                if (BtnList.Contains(SelectBtn))
                {
                    currentBtnList = BtnList;
                }
            }

            int lineIndex = SectionBtns.IndexOf(currentBtnList);
            int index = currentBtnList.IndexOf(SelectBtn);

            if (lineIndex <= 0)
            {
                SelectBtn = SectionBtns[SectionBtns.Count - 1][index];
            }
            else
            {
                SelectBtn = SectionBtns[lineIndex - 1][index];
            }
            OnOffSelectedBtn(SelectBtn);
        }
    }

    // Y Button
    private void ApplyButton(InputAction.CallbackContext obj)
    {
        if (!GameManager.Instance.canInput) { return; }

        if (Option.CheckIsOnOptionDetail() != null)
        {
            Option.Apply_OptionDetail();
        }
    }

    // A Button
    private void select(InputAction.CallbackContext obj)
    {
        if (!GameManager.Instance.canInput) { return; }

        if (SelectBtn != null)
        {
            SelectBtn.TryGetComponent(out Button btn);
            if (interact != null && btn.interactable)
            {
                interact.Interact();
            }
        }
    }
    
    // X Button
    private void back(InputAction.CallbackContext obj)
    {
        if (!GameManager.Instance.canInput) { return; }

        if (!Option.Cancel_OptionDetail())
        { return; }
         
    }


    public void SetSectionBtns(List<List<Button>> btns, IInteract inter)
    {
        if (btns == null || inter == null)
        {
            ClearSeletedBtns();
        }
        else
        {
            SectionBtns = btns;
            SelectBtn = btns[0][0];
            OnOffSelectedBtn(SelectBtn);
            this.interact = inter;
        }

    }

    public void OnOffSelectedBtn(Button btn)
    {
        if (SectionBtns == null || btn == null) { return; }

        List<Button> allSectionList = new List<Button>();
        foreach (List<Button> btns in SectionBtns)
        {
            allSectionList.AddRange(btns);
        }

        foreach (Button SectionBtn in allSectionList)
        {

            if (SectionBtn == btn) // 선택 버튼
            {
                if (SectionBtn.gameObject.TryGetComponent(out Outline outline))
                { outline.enabled = true; }
                if (SectionBtn.gameObject.TryGetComponent(out RectTransform RT))
                {
                    DOTween.Kill(RT.localScale);
                    RT.DOScale(Vector3.one * 1.1f, 0.1f);
                }


            }
            else // 비선택 버튼들
            {
                if (SectionBtn.gameObject.TryGetComponent(out Outline outline))
                { outline.enabled = false; }
                if (SectionBtn.gameObject.TryGetComponent(out RectTransform RT))
                {
                    DOTween.Kill(RT.localScale);
                    RT.DOScale(Vector3.one * 1.0f, 0.1f);
                }
            }
        }
        /*if (SectionBtns == null || btn == null) { return; }

        List<Button> allBtn = new List<Button>();
        foreach(List<Button> SectionBtn in SectionBtns)
        {
            allBtn.AddRange(SectionBtn);
        }


        foreach (Button SectionBtn in allBtn)
        {
            if (SectionBtn == btn)
            {
                if (SectionBtn.gameObject.TryGetComponent(out UnityEngine.UI.Outline outline))
                {
                    outline.enabled = true;
                }
            }
            else
            {
                if (SectionBtn.gameObject.TryGetComponent(out UnityEngine.UI.Outline outline))
                {
                    outline.enabled = false;
                }

            }
        }*/
    }

    public void ClearSeletedBtns()
    {
        SectionBtns = null;
        SelectBtn = null;
        interact = null;
    }

    public List<Button> AllSectionBtns()
    {
        List<Button> Btnlist = new List<Button>();
        foreach(List<Button> Btns in SectionBtns)
        {
            Btnlist.AddRange(Btns);
        }
        return Btnlist;
    }

    #endregion
}
