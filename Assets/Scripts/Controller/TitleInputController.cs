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
    [SerializeField] public List<Button> SectionBtns;
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
    }

    private void DisableTitleInput()
    {
        _input.actions["SelectUp"].started -= selectUp;
        _input.actions["SelectDown"].started -= selectDown;
        _input.actions["Select"].started -= select;
    }

    #endregion

    #region Section

    private void selectUp(InputAction.CallbackContext obj)
    {
        if (SectionBtns != null && SectionBtns.Count > 1)
        {
            int index = SectionBtns.IndexOf(SelectBtn);
            if (index <= 0)
            {
                SelectBtn = SectionBtns[SectionBtns.Count - 1];
            }
            else
            {
                SelectBtn = SectionBtns[(index - 1)];
            }
            OnOffSelectedBtn(SelectBtn);
        }
    }
    private void selectDown(InputAction.CallbackContext obj)
    {
        if (SectionBtns != null && SectionBtns.Count > 1)
        {
            int index = SectionBtns.IndexOf(SelectBtn);
            if (index >= SectionBtns.Count - 1)
            {
                SelectBtn = SectionBtns[0];
            }
            else
            {
                SelectBtn = SectionBtns[(index + 1)];
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


    public void SetSectionBtns(List<Button> btns, IInteract inter)
    {
        if (btns == null || inter == null)
        {
            ClearSeletedBtns();
        }
        else
        {
            SectionBtns = btns;
            SelectBtn = btns[0];
            OnOffSelectedBtn(SelectBtn);
            this.interact = inter;
        }

    }

    public void OnOffSelectedBtn(Button btn)
    {
        if (SectionBtns == null || btn == null) { return; }
        foreach (Button SectionBtn in SectionBtns)
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
