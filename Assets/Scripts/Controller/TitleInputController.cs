using System.Collections.Generic;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TitleInputController : Manager<TitleInputController>
{
    #region Value

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
        TitleInput["SelectUp"].started += selectUp;
        TitleInput["SelectDown"].started += selectDown;
        TitleInput["Select"].started += select;
        TitleInput["Back"].started += back;
    }

    private void DisableTitleInput()
    {
        _input.actions["SelectUp"].started -= selectUp;
        _input.actions["SelectDown"].started -= selectDown;
        _input.actions["Select"].started -= select;
        _input.actions["Back"].started -= back;
    }

    #endregion

    #region Section

    private void selectUp(InputAction.CallbackContext obj)
    {
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
    private void selectDown(InputAction.CallbackContext obj)
    {
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
    private void select(InputAction.CallbackContext obj)
    {
        if (SelectBtn != null)
        {
            SelectBtn.TryGetComponent(out Button btn);
            if (interact != null && btn.interactable)
            {
                interact.Interact();
            }
        }
    }
    private void back(InputAction.CallbackContext obj)
    {

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
        }
    }

    public void ClearSeletedBtns()
    {
        SectionBtns = null;
        SelectBtn = null;
        interact = null;
    }

    #endregion
}
