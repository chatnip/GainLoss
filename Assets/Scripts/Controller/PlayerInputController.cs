using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class PlayerInputController : Manager<PlayerInputController>
{
    #region Value

    [Header("*Property")]
    [SerializeField] GameSystem GameSystem;
    [SerializeField] ActionEventManager ActionEventManager;
    [SerializeField] PartTimeJobManager PartTimeJobManager;
    [SerializeField] Pause pause;
    [HideInInspector] public bool isPause = false;

    [Header("*Phone")]
    [SerializeField] PhoneHardware PhoneHardware;
    [SerializeField] PhoneSoftware PhoneSoftware;
    [SerializeField] GameObject PhoneCamera2D;
    [SerializeField] List<GameObject> Pads;

    [Header("*Computer")]
    [SerializeField] ComputerInteract ComputerInteract;
    [SerializeField] Desktop Desktop;
    [SerializeField] PreliminarySurveyWindow PreliminarySurveyWindow;
    [SerializeField] GameObject ComputerCamera2D;

    [Header("*PartTimeJob")]
    [SerializeField] Button PartTimeJobLoadingBtn;

    [Header("*Interact Object")]
    [SerializeField] ObjectInteractionButtonGenerator ObjectInteractionButtonGenerator;
    [SerializeField] CheckGetAllDatas CheckGetAllDatas;
    [SerializeField] GameObject Panel_Object;
    [SerializeField] GameObject Panel_Npc;
    List<GameObject> Panels = new List<GameObject>();

    [Header("*Schedule")]
    [SerializeField] SchedulePrograss SchedulePrograss;
    [SerializeField] ScheduleManager ScheduleManager;

    [Header("*Camera")]
    [SerializeField] GameObject QuarterViewCamera; 
    
    [Header("*Input Setting")]
    [SerializeField] public PlayerInput _input;
    [SerializeField] public List<List<Button>> SectionBtns;
    [SerializeField] public Button SelectBtn;
    [SerializeField] InputAction settingOn;

    [Header("*Character Input Values")]
    public static bool CanMove = false;
    public Vector2 pointerMove;
    public Vector2 move;
    public Vector2 look;

    [Header("*Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    [Header("*Event Settings")]
    // public bool interactableMode;

    [Tooltip("상호작용중인지 여부")]
    // public BoolReactiveProperty interactMode = new BoolReactiveProperty();

    public IInteract interact;
    private List<List<Button>> TempSectionBtns;
    private Button TempSelectedBtn;
    private IInteract TempInteract;

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
            Debug.Log("커서안잠금");
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

        playerInput["ChangeSeletedBtn_Right"].started += RightSelectedBtn;
        playerInput["ChangeSeletedBtn_Left"].started += LeftSelectedBtn;
        playerInput["ChangeSeletedBtn_Down"].started += DownSelectedBtn;
        playerInput["ChangeSeletedBtn_Up"].started += UpSelectedBtn;
        playerInput["SeleteBtn"].started += ApplySeleteBtn;

        playerInput["Phone"].started += OnOffPhone;
        playerInput["InteractObjectBtn"].started += OnOffInteractObject;
        playerInput["ShowScheduleDetailBtn"].started += OnOffShowScheduleDetailBtn;
        playerInput["TerminatePart"].started += TerminatePart;
        playerInput["SetSomething"].started += SetSomething;

        playerInput["PointerMove"].performed += OnPointerMove;
        playerInput["PointerMove"].canceled += OnPointerMoveStop;

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

        _input.actions["ChangeSeletedBtn_Right"].started -= RightSelectedBtn;
        _input.actions["ChangeSeletedBtn_Left"].started -= LeftSelectedBtn;
        _input.actions["ChangeSeletedBtn_Down"].started -= DownSelectedBtn;
        _input.actions["ChangeSeletedBtn_Up"].started -= UpSelectedBtn;

        _input.actions["SeleteBtn"].started -= ApplySeleteBtn;

        _input.actions["Phone"].started -= OnOffPhone;
        _input.actions["InteractObjectBtn"].started -= OnOffInteractObject;
        _input.actions["ShowScheduleDetailBtn"].started -= OnOffShowScheduleDetailBtn;
        _input.actions["TerminatePart"].started -= TerminatePart;
        _input.actions["SetSomething"].started -= SetSomething;

        _input.actions["PointerMove"].performed -= OnPointerMove;
        _input.actions["PointerMove"].canceled -= OnPointerMoveStop;

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
                // 단어장에 단어 추가
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

    public void StopMove()
    {
        CanMove = false;
        move = Vector2.zero;
    }

    #endregion

    #region Unique Func

    //일시정지
    private void OnPause(InputAction.CallbackContext obj)
    {
        if (pause.gameObject.activeSelf)
        {
            pause.ft_closePausePopup();

            SetSectionBtns(TempSectionBtns, TempInteract);
            OnOffSelectedBtn(TempSelectedBtn);
        }
        else
        {
            TempSectionBtns = this.SectionBtns;
            TempInteract = this.interact;
            TempSelectedBtn = this.SelectBtn;

            pause.ft_openPausePopup();
        }
    }

    private void OnOffPhone(InputAction.CallbackContext obj)
    {
        if(isPause || !QuarterViewCamera.activeSelf || GameSystem.cutsceneImg.gameObject.activeSelf) { return; }

        foreach(GameObject Go in Panels)
        {
            if (Go.activeSelf) { return; }
        }

        if (ObjectInteractionButtonGenerator.SectionIsThis)
        { ObjectInteractionButtonGenerator.SetOnOffInteractObjectBtn(); }
        if (SchedulePrograss.OnExplanation) 
        { SchedulePrograss.OnOffExlanation(); }

        PhoneHardware.SetOnOffPhoneBtn();
    }

    private void OnOffInteractObject(InputAction.CallbackContext obj)
    {
        if (isPause || !QuarterViewCamera.activeSelf || GameSystem.cutsceneImg.gameObject.activeSelf) { return; }

        foreach (GameObject Go in Panels)
        {
            if (Go.activeSelf) { return; }
        }

        if (PhoneHardware.sectionIsThis)
        { PhoneHardware.SetOnOffPhoneBtn(); }
        if (SchedulePrograss.OnExplanation)
        { SchedulePrograss.OnOffExlanation(); }

        ObjectInteractionButtonGenerator.SetOnOffInteractObjectBtn();

    }

    private void OnOffShowScheduleDetailBtn(InputAction.CallbackContext obj)
    {
        if (isPause || GameSystem.cutsceneImg.gameObject.activeSelf) { return; }

        if (PhoneHardware.sectionIsThis)
        { PhoneHardware.SetOnOffPhoneBtn(); }
        if (ObjectInteractionButtonGenerator.SectionIsThis)
        { ObjectInteractionButtonGenerator.SetOnOffInteractObjectBtn(); }

        SchedulePrograss.OnOffExlanation();
    }

    private void SetSomething(InputAction.CallbackContext obj)

    {
        if (Desktop.PSWindow.activeSelf && !PreliminarySurveyWindow.resultWindowParentGO.activeSelf || GameSystem.cutsceneImg.gameObject.activeSelf)
        {
            PreliminarySurveyWindow.ft_setChooseClue(SelectBtn);
        }
    }
    private void TerminatePart(InputAction.CallbackContext context)
    {
        if (isPause || GameSystem.cutsceneImg.gameObject.activeSelf) { return; }

        if (Desktop.PSWindow.activeSelf && !PreliminarySurveyWindow.resultWindowParentGO.activeSelf) 
        { PreliminarySurveyWindow.ft_tryToCombine(); }

        if (PhoneHardware.sectionIsThis)
        { PhoneHardware.SetOnOffPhoneBtn(); }
        if (ObjectInteractionButtonGenerator.SectionIsThis)
        { ObjectInteractionButtonGenerator.SetOnOffInteractObjectBtn(); }
        if (SchedulePrograss.OnExplanation)
        { SchedulePrograss.OnOffExlanation(); }

        if (PartTimeJobLoadingBtn.gameObject.activeSelf)
        {
            StartCoroutine(PartTimeJobManager.StartPartTimeJob());
        }
        if (CheckGetAllDatas.TerminateBtn.gameObject.activeSelf)
        {
            CheckGetAllDatas.TerminatePlaceAndGoHome();
        }
        else if (ScheduleManager.EndDayBtn.gameObject.activeSelf)
        {
            ActionEventManager.TurnOnLoading();
        }
    }

    private void BackBtn(InputAction.CallbackContext obj)
    {
        //Pause
        if (pause.gameObject.activeSelf) 
        { pause.ft_closePausePopup(); return; }

        //Cutscene
        if (GameSystem.cutsceneImg.gameObject.activeSelf)
        { return; }

        //Panels
        if (Panel_Object.activeSelf) 
        { GameSystem.ObjectDescriptionOff(); return; }
        else if (Panel_Npc.activeSelf) 
        { GameSystem.NpcDescriptionOff(); return; }

        //Computer
        if (ComputerOffWindow(Desktop.confirmPopup)) 
        { return; }
        else if (ComputerOffWindow(Desktop.todoWindow)) 
        { return; }
        else if (Desktop.PSWindow.activeSelf)
        {
            if (!PreliminarySurveyWindow.resultWindowParentGO.activeSelf)
            {
                PreliminarySurveyWindow.ft_clearChooseClue();
            }
            return;
        }
        else if (Desktop.streamWindow.activeSelf) 
        { return; }
        else if (ComputerCamera2D.activeSelf) //Computer Screen
        {
            StartCoroutine(ComputerInteract.ScreenZoomOut());
            ClearSeletedBtns();
            return;
        }

        bool ComputerOffWindow(GameObject Window)
        {
            if (Window.activeSelf)
            {
                Desktop.DisappearPopup(Window);
                Desktop.setDesktopSectionBtns();
                return true;
            }
            return false;
        }

        //Phone
        for (int i = 0; i < Pads.Count; i++) //Pads
        {
            if (Pads[i].gameObject.activeSelf) 
            { 
                Pads[i].gameObject.SetActive(false);
                return;
            }
        }
        if (PhoneCamera2D.activeSelf) //PhoneScreen
        {
            PhoneHardware.PhoneOff();
            ClearSeletedBtns();
            return;
        }

        //Section
        if (PhoneHardware.sectionIsThis)
        { PhoneHardware.SetOnOffPhoneBtn(); }
        if (ObjectInteractionButtonGenerator.SectionIsThis)
        { ObjectInteractionButtonGenerator.SetOnOffInteractObjectBtn(); }

    }

    private void OnPointerMove(InputAction.CallbackContext obj)
    {
        PointerMoveInput(obj.ReadValue<Vector2>());
    }
    public void OnPointerMoveStop(InputAction.CallbackContext obj)
    {
        PointerMoveInput(Vector2.zero);
    }
    public void PointerMoveInput(Vector2 newPointerMoveDirection)
    {
        if (Desktop.PSWindow.activeSelf) { pointerMove = newPointerMoveDirection; }
    }
    

    #endregion

    #region Selected Btns

    private void ApplySeleteBtn(InputAction.CallbackContext obj)
    {
        if (GameSystem.cutsceneImg.gameObject.activeSelf)
        { cutsceneSO.skipOrCompleteSeq(GameSystem.cutsceneImg); return; }

        if (Panel_Object.activeSelf)
        { GameSystem.ObjectDescriptionSkip(); return; }
        else if (Panel_Npc.activeSelf)
        { GameSystem.NpcDescriptionSkip(); return; }

        if (Desktop.PSWindow.activeSelf)
        {
            interact.Interact();
        }

        if (SelectBtn != null)
        {
            SelectBtn.TryGetComponent(out Button btn);
            if (interact != null && btn.interactable && btn.gameObject.activeSelf)
            {
                interact.Interact();
            }
        }
        
    }

    private void RightSelectedBtn(InputAction.CallbackContext obj)
    {
        if(SectionBtns != null && SectionBtns.Count >= 1)
        {
            List<Button> currentBtnList = new List<Button>();
            foreach(List<Button> BtnList in SectionBtns)
            {
                if(BtnList.Contains(SelectBtn))
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

        if (Desktop.PSWindow.activeSelf)
        {
            PreliminarySurveyWindow.ft_setClueImg(SelectBtn);
        }

    }

    private void LeftSelectedBtn(InputAction.CallbackContext obj)
    {
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
        if (Desktop.PSWindow.activeSelf)
        {
            PreliminarySurveyWindow.ft_setClueImg(SelectBtn);
        }
    }


    private void DownSelectedBtn(InputAction.CallbackContext obj)
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

            if(lineIndex >= SectionBtns.Count - 1)
            {
                SelectBtn = SectionBtns[0][index];
            }
            else
            {
                SelectBtn = SectionBtns[lineIndex + 1][index];
            }
            OnOffSelectedBtn(SelectBtn);
        }

        if (Desktop.PSWindow.activeSelf)
        {
            PreliminarySurveyWindow.ft_setClueImg(SelectBtn);
        }
    }
    private void UpSelectedBtn(InputAction.CallbackContext obj)
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

            if(lineIndex <= 0)
            {
                SelectBtn = SectionBtns[SectionBtns.Count - 1][index];
            }
            else
            {
                SelectBtn = SectionBtns[lineIndex - 1][index];
            }
            OnOffSelectedBtn(SelectBtn);
        }
        if (Desktop.PSWindow.activeSelf)
        {
            PreliminarySurveyWindow.ft_setClueImg(SelectBtn);
        }
    }


    public void SetSectionBtns(List<List<Button>> btns, IInteract inter)
    {
        if(btns == null || inter == null || btns.Count == 0)
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
        if(SectionBtns == null || btn == null) { return; }

        List<Button> allSectionList = new List<Button>();
        foreach(List<Button> btns in SectionBtns)
        {
            allSectionList.AddRange(btns);
        }

        foreach (Button SectionBtn in allSectionList)
        {
            if(SectionBtn == btn)
            {
                if(SectionBtn.gameObject.TryGetComponent(out UnityEngine.UI.Outline outline))
                { outline.enabled = true; }
            }
            else
            {
                if (SectionBtn.gameObject.TryGetComponent(out UnityEngine.UI.Outline outline))
                { outline.enabled = false; }
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