using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;
using UnityEngine.UI;
using static Beautify.Universal.Beautify;
using System;

public class PlayerInputController : Manager<PlayerInputController>
{
    #region Value

    [Header("*Property")]
    [SerializeField] GameSystem GameSystem;
    [SerializeField] Pause pause;
    [SerializeField] PhoneHardware PhoneHardware;
    [SerializeField] PhoneSoftware PhoneSoftware;
    [SerializeField] ObjectInteractionButtonGenerator ObjectInteractionButtonGenerator;
    [SerializeField] SchedulePrograss SchedulePrograss;
    [SerializeField] CheckGetAllDatas CheckGetAllDatas;
    [SerializeField] GameObject Panel_Object;
    [SerializeField] GameObject Panel_Npc;
    List<GameObject> Panels = new List<GameObject>();

    [Header("*GameObject")]
    [SerializeField] List<GameObject> Pads;
    
    [Header("*Input Setting")]
    [SerializeField] public PlayerInput _input;
    [SerializeField] public List<Button> SectionBtns;
    [SerializeField] public Button SelectBtn;
    [SerializeField] InputAction settingOn;

    [Header("*Character Input Values")]
    public bool CanMove;
    public Vector2 move;
    public Vector2 look;

    [Header("*Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    [Header("*Event Settings")]
    // public bool interactableMode;

    [Tooltip("��ȣ�ۿ������� ����")]
    // public BoolReactiveProperty interactMode = new BoolReactiveProperty();

    public IInteract interact;

    #endregion

    #region Main

    protected override void Awake()
    {
        //base.Awake();
        TryGetComponent(out _input);

        _input.ObserveEveryValueChanged(x => x.currentControlScheme)
            .Subscribe(OnControlSchemeChanged);

        Panels = new List<GameObject>() { Panel_Object, Panel_Npc };

        /*
        interactMode
            .Subscribe(x =>
            {
                if (x)
                {
                    DisablePlayerInput();
                }
                else
                {
                    EnablePlayerInput();
                }
            });
        */
    }

    public void OnControlSchemeChanged(string _controlScheme)
    {
        // Debug.Log($"OnControlSchemeChanged : {_controlScheme}");
        /*
        if(_controlScheme != "KeyboardMouse")
        {
            SetCursorState(false);
        }
        else
        {
            Debug.Log("Ŀ�������");
            SetCursorState(true);
        }
        */
    }

    private void OnEnable()
    {
        EnablePlayerInput();
        settingOn.Enable();
    }
    private void OnDisable()
    {
        DisablePlayerInput();
        settingOn.Disable();
    }

    public void EnablePlayerInput()
    {
        var playerInput = _input.actions.FindActionMap("Player");
        playerInput["Move"].performed += OnMove;
        playerInput["Move"].canceled += OnMoveStop;
        playerInput["Look"].performed += OnLook;
        playerInput["Pause"].started += OnPause;

        playerInput["ChangeSeletedBtn_Down"].started += DownSelectedBtn;
        playerInput["ChangeSeletedBtn_Up"].started += UpSelectedBtn;
        playerInput["SeleteBtn"].started += ApplySeleteBtn;

        playerInput["Phone"].started += OnOffPhone;
        playerInput["InteractObjectBtn"].started += OnOffInteractObject;
        playerInput["ShowScheduleDetailBtn"].started += OnOffShowScheduleDetailBtn;
        playerInput["TerminatePart"].started += TerminatePart;

        playerInput["BackBtn"].started += BackBtn;

        //playerInput["Interact"].started += OnInteract;
        // playerInput["InteractCancel"].started += OnInteractCancel;

    }

    

    public void DisablePlayerInput()
    {
        _input.actions["Move"].performed -= OnMove;
        _input.actions["Move"].canceled -= OnMoveStop;
        _input.actions["Look"].performed -= OnLook;
        _input.actions["Pause"].started -= OnPause;

        _input.actions["ChangeSeletedBtn_Down"].started -= DownSelectedBtn;
        _input.actions["ChangeSeletedBtn_Up"].started -= UpSelectedBtn;
        _input.actions["SeleteBtn"].started -= ApplySeleteBtn;

        _input.actions["Phone"].started -= OnOffPhone;
        _input.actions["InteractObjectBtn"].started -= OnOffInteractObject;
        _input.actions["ShowScheduleDetailBtn"].started -= OnOffShowScheduleDetailBtn;
        _input.actions["TerminatePart"].started -= TerminatePart;

        _input.actions["BackBtn"].started -= BackBtn;

        //_input.actions["Interact"].started -= OnInteract;
        // _input.actions["InteractCancel"].started -= OnInteractCancel;
    }
    #endregion

    #region Player
    private void OnMove(InputAction.CallbackContext obj)
    {
        MoveInput(obj.ReadValue<Vector2>());
    }

    private void OnMoveStop(InputAction.CallbackContext obj)
    {
        MoveInput(Vector2.zero);
    }

    private void OnLook(InputAction.CallbackContext obj)
    {
        LookInput(obj.ReadValue<Vector2>());
    }


    /*private void OnInteract(InputAction.CallbackContext obj)
    {
        if (interact != null)
        {
            interact.Interact();
        }
    }

    
    private void OnInteractCancel(InputAction.CallbackContext obj)
    {
        interact.InteractCancel();
    }

    
    public void SetInteract(IInteract interact)
    {
        this.interact = interact;
    }

    public bool AlreadyHaveInteract(IInteract interact)
    {
        if (this.interact == interact)
        {
            return true;
        }
        return false;
    }
    

    
    private void OnClick(InputAction.CallbackContext obj)
    {
        if (interactableMode && LookForGameObject(out RaycastHit hit))
        {
            if (hit.collider.gameObject.CompareTag("ClickableInteract"))
            {
                // �ܾ��忡 �ܾ� �߰�
            }
        }
    }
    */


    public void MoveInput(Vector2 newMoveDirection)
    {
        if (CanMove) { move = newMoveDirection; }
        
    }

    public void LookInput(Vector2 newLookDirection)
    {
        if (CanMove) { look = newLookDirection; }
    }

    #endregion

    #region Unique Func

    //�Ͻ�����
    private void OnPause(InputAction.CallbackContext obj)
    {
        if (pause.gameObject.activeSelf)
        {
            pause.closePausePopup();
        }
        else
        {
            pause.gameObject.SetActive(true);
        }
    }

    private void OnOffPhone(InputAction.CallbackContext obj)
    {
        foreach(GameObject Go in Panels)
        {
            if (Go.activeSelf) { return; }
        }

        if (ObjectInteractionButtonGenerator.SectionIsThis)
        { ObjectInteractionButtonGenerator.SetOnOffInteractObjectBtn(); }

        PhoneHardware.SetOnOffPhoneBtn();
    }

    private void OnOffInteractObject(InputAction.CallbackContext obj)
    {
        foreach (GameObject Go in Panels)
        {
            if (Go.activeSelf) { return; }
        }

        if (PhoneHardware.sectionIsThis)
        { PhoneHardware.SetOnOffPhoneBtn(); }

        ObjectInteractionButtonGenerator.SetOnOffInteractObjectBtn();

    }

    private void OnOffShowScheduleDetailBtn(InputAction.CallbackContext obj)
    {
        SchedulePrograss.OnOffExlanation();
    }

    private void TerminatePart(InputAction.CallbackContext context)
    {
        if (CheckGetAllDatas.TerminateBtn.gameObject.activeSelf)
        {
            CheckGetAllDatas.TerminatePlaceAndGoHome();
        }
    }

    private void BackBtn(InputAction.CallbackContext obj)
    {

        if (Panel_Object.activeSelf)
        { GameSystem.ObjectDescriptionOff(); return; }
        else if (Panel_Npc.activeSelf)
        { GameSystem.NpcDescriptionOff(); return; }


        for (int i = 0; i < Pads.Count; i++)
        {
            if (Pads[i].gameObject.activeSelf) 
            { 
                Pads[i].gameObject.SetActive(false);
                return;
            }
        }

        if (PhoneSoftware.transform.parent.gameObject.activeSelf)
        {
            Debug.Log("dd");
            PhoneHardware.PhoneOff();
            ClearSeletedBtns();
            return;
        }

        if (PhoneHardware.sectionIsThis)
        { PhoneHardware.SetOnOffPhoneBtn(); }
        if (ObjectInteractionButtonGenerator.SectionIsThis)
        { ObjectInteractionButtonGenerator.SetOnOffInteractObjectBtn(); }

    }

    #endregion

    #region Selected Btns

    private void ApplySeleteBtn(InputAction.CallbackContext obj)
    {
        if (Panel_Object.activeSelf)
        { GameSystem.ObjectDescriptionSkip(); return; }
        else if (Panel_Npc.activeSelf)
        { GameSystem.NpcDescriptionSkip(); return; }

        if (SelectBtn != null)
        {
            SelectBtn.TryGetComponent(out Button btn);
            if (interact != null && btn.interactable)
            {
                interact.Interact();
            }
        }
        
    }

    private void DownSelectedBtn(InputAction.CallbackContext obj)
    {
        if(SectionBtns != null && SectionBtns.Count > 1)
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

    private void UpSelectedBtn(InputAction.CallbackContext obj)
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



    public void SetSectionBtns(List<Button> btns, IInteract inter)
    {
        if(btns == null || inter == null)
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
        foreach (Button SectionBtn in SectionBtns)
        {
            if(SectionBtn == btn)
            {
                if(SectionBtn.gameObject.TryGetComponent(out UnityEngine.UI.Outline outline))
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


    /*
    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
    

    private bool LookForGameObject(out RaycastHit hit)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out hit);
    }
    */
}