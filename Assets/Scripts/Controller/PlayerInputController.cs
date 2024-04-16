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
    [SerializeField] public PlayerController PlayerController;
    [SerializeField] ActionEventManager ActionEventManager;
    [SerializeField] PartTimeJobManager PartTimeJobManager;
    [SerializeField] DialogManager DialogManager;
    [SerializeField] Pause pause;
    [HideInInspector] public bool isPause = false;
    [SerializeField] GameObject allLoadingGO;

    [Header("*Tutorial")]
    [SerializeField] GameObject TutorialScreenGO;

    [Header("*Phone")]
    [SerializeField] PhoneHardware PhoneHardware;
    [SerializeField] PhoneSoftware PhoneSoftware;
    [SerializeField] GameObject PhoneCamera2D;
    [SerializeField] List<GameObject> Pads;

    [Header("*Computer")]
    [SerializeField] ComputerInteract ComputerInteract;
    [SerializeField] Desktop Desktop;
    [SerializeField] GameObject PSWindow_choose;
    [SerializeField] PreliminarySurveyWindow_FindClue PSWindow_FC;
    [SerializeField] PreliminarySurveyWindow_Extract PSWindow_E;
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
        playerInput["Pause"].started += OnOffPause;

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
        _input.actions["Pause"].started -= OnOffPause;

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
        if (PSWindow_E.gameObject.activeSelf)
        { PSWindow_E.boardMoveDir = newMoveDirection; }

        if (CanMove)
        { move = newMoveDirection; }

    }

    public void LookInput(Vector2 newLookDirection)
    {
        if (CanMove) { look = newLookDirection; }
    }

    public void StopMove()
    {
        CanMove = false;
        move = Vector2.zero;
        PlayerController.resetAnime();
    }

    #endregion

    #region Unique Func

    // 일시정지
    private void OnOffPause(InputAction.CallbackContext obj)
    {
        OnOffPause();
        /*if (pause.gameObject.activeSelf)
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
        }*/
    }
    public void OnOffPause()
    {
        if (pause.gameObject.activeSelf)
        {
            Debug.Log("Pause 끄기");
            OffPause();

            pause.ft_closePausePopup();
        }
        else
        {
            Debug.Log("Pause 켜기");
            OnPause();

            pause.ft_openPausePopup();

        }
    }
    public void OnPause()
    {
        if (this.SectionBtns != null) { TempSectionBtns = new List<List<Button>>(this.SectionBtns); }
        if (interact != null) { TempInteract = interact; }
        if (SelectBtn != null) { TempSelectedBtn = SelectBtn; }

    }
    public void OffPause()
    {
        SetSectionBtns(TempSectionBtns, TempInteract);
        OnOffSelectedBtn(TempSelectedBtn);

    }

    // 폰 끄고 키기
    private void OnOffPhone(InputAction.CallbackContext obj)
    {
        OnOffPhone();
        /*if(isPause || !QuarterViewCamera.activeSelf || GameSystem.cutsceneImg.gameObject.activeSelf) { return; }

        foreach(GameObject Go in Panels)
        {
            if (Go.activeSelf) { return; }
        }

        if (ObjectInteractionButtonGenerator.SectionIsThis)
        { ObjectInteractionButtonGenerator.SetOnOffInteractObjectBtn(); }
        if (SchedulePrograss.OnExplanation) 
        { SchedulePrograss.OnOffVisibleSchedule(); }

        PhoneHardware.SetOnOffPhoneBtn();*/
    }
    public void OnOffPhone()
    {
        if (isPause ||
            !QuarterViewCamera.activeSelf ||
            GameSystem.cutsceneImg.gameObject.activeSelf ||
            TutorialScreenGO.activeSelf ||
            allLoadingGO.activeSelf) 
        { return; }

        foreach (GameObject Go in Panels)
        {
            if (Go.activeSelf) { return; }
        }

        if (ObjectInteractionButtonGenerator.SectionIsThis)
        { ObjectInteractionButtonGenerator.SetOnOffInteractObjectBtn(); }
        //if (SchedulePrograss.OnExplanation)
        //{ SchedulePrograss.OnOffVisibleSchedule(); }

        PhoneHardware.SetOnOffPhoneBtn();
    }

    // 상호작용 오브젝트 UI 활성화 / 비활성화
    private void OnOffInteractObject(InputAction.CallbackContext obj)
    {
        OnOffInteractObject();
        /*if (isPause || !QuarterViewCamera.activeSelf || GameSystem.cutsceneImg.gameObject.activeSelf) { return; }

        foreach (GameObject Go in Panels)
        {
            if (Go.activeSelf) { return; }
        }

        if (PhoneHardware.sectionIsThis)
        { PhoneHardware.SetOnOffPhoneBtn(); }
        if (SchedulePrograss.OnExplanation)
        { SchedulePrograss.OnOffVisibleSchedule(); }

        ObjectInteractionButtonGenerator.SetOnOffInteractObjectBtn();*/
    }
    public void OnOffInteractObject()
    {
        if (isPause || 
            !QuarterViewCamera.activeSelf || 
            GameSystem.cutsceneImg.gameObject.activeSelf ||
            TutorialScreenGO.activeSelf ||
            allLoadingGO.activeSelf) 
        { return; }

        foreach (GameObject Go in Panels)
        {
            if (Go.activeSelf) { return; }
        }

        if (PhoneHardware.sectionIsThis)
        { PhoneHardware.SetOnOffPhoneBtn(); }
        //if (SchedulePrograss.OnExplanation)
        //{ SchedulePrograss.OnOffVisibleSchedule(); }

        ObjectInteractionButtonGenerator.SetOnOffInteractObjectBtn();
    }

    // 스케쥴 표 보이기
    private void OnOffShowScheduleDetailBtn(InputAction.CallbackContext obj)
    {
        //OnOffShowScheduleDetailBtn();
       /* if (isPause || GameSystem.cutsceneImg.gameObject.activeSelf) { return; }

        if (PhoneHardware.sectionIsThis)
        { PhoneHardware.SetOnOffPhoneBtn(); }
        if (ObjectInteractionButtonGenerator.SectionIsThis)
        { ObjectInteractionButtonGenerator.SetOnOffInteractObjectBtn(); }

        SchedulePrograss.OnOffVisibleSchedule();*/
    }
    /*public void OnOffShowScheduleDetailBtn()
    {
        if (isPause || GameSystem.cutsceneImg.gameObject.activeSelf) { return; }

        if (PhoneHardware.sectionIsThis)
        { PhoneHardware.SetOnOffPhoneBtn(); }
        if (ObjectInteractionButtonGenerator.SectionIsThis)
        { ObjectInteractionButtonGenerator.SetOnOffInteractObjectBtn(); }

        SchedulePrograss.OnOffVisibleSchedule();
    }*/

    // 무언가를 선택 (X키)
    private void SetSomething(InputAction.CallbackContext obj)
    {
        if (Desktop.PSWindow.activeSelf && !PSWindow_FC.resultWindowParentGO.activeSelf || GameSystem.cutsceneImg.gameObject.activeSelf)
        {
            PSWindow_FC.ft_setChooseClue(SelectBtn);
        }
    }

    // 파트 넘기기 (Y키)
    private void TerminatePart(InputAction.CallbackContext context)
    {
        if (isPause || 
            GameSystem.cutsceneImg.gameObject.activeSelf ||
            allLoadingGO.activeSelf) 
        { return; }

        if(Desktop.streamWindow.activeSelf) 
        { DialogManager.ft_allSkip(); }

        if (Desktop.PSWindow.activeSelf && !PSWindow_FC.resultWindowParentGO.activeSelf) 
        { PSWindow_FC.ft_tryToCombine(); }

        if (ScheduleManager.PassNextScheduleBtn.gameObject.activeSelf)
        { ScheduleManager.PassNextSchedule(); }

        if (PhoneHardware.sectionIsThis)
        { PhoneHardware.SetOnOffPhoneBtn(); }
        if (ObjectInteractionButtonGenerator.SectionIsThis)
        { ObjectInteractionButtonGenerator.SetOnOffInteractObjectBtn(); }
        //if (SchedulePrograss.OnExplanation)
        //{ SchedulePrograss.OnOffVisibleSchedule(); }

        if (PartTimeJobLoadingBtn.gameObject.activeSelf)
        {
            StartCoroutine(PartTimeJobManager.StartPartTimeJob(5.0f, PartTimeJobManager.selectCSSO()));
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

    // 뒤로가기(B키)
    private void BackBtn(InputAction.CallbackContext obj)
    {
        //Pause
        if (pause.gameObject.activeSelf) 
        { pause.ft_closePausePopup(); return; }

        // 본 오브젝트가 켜져있을 때, 이 키는 기능 X
        if (GameSystem.cutsceneImg.gameObject.activeSelf ||
            Desktop.streamWindow.activeSelf || 
            PSWindow_E.gameObject.activeSelf ||
            TutorialScreenGO.activeSelf ||
            allLoadingGO.activeSelf)
        { return; }

        //Panels
        if (Panel_Object.activeSelf) 
        { GameSystem.ObjectDescriptionOff(); return; }
        else if (Panel_Npc.activeSelf) 
        { GameSystem.NpcDescriptionOff(); return; }

        //Computer
        if (ComputerOffWindow(Desktop.confirmPopup)) // Confirm 팝업창이 있다면 끄기
        { return; }
        else if (ComputerOffWindow(Desktop.todoWindow)) // Todo 팝업창이 있다면 끄기
        { return; }
        else if (ComputerOffWindow(PSWindow_choose)) // 사전 조사 결정창이 있다면 끄기
        { return; }
        else if (PSWindow_FC.gameObject.activeSelf)
        {
            if (!PSWindow_FC.resultWindowParentGO.activeSelf)
            {
                PSWindow_FC.ft_clearChooseClue(); // FindClue 상태일 때, 모든 선택 Num 제거
            }
            return;
        }
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

    // 포인트 이동 (L Stick)
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

    public void ApplySeleteBtn(InputAction.CallbackContext obj)
    {
        if (GameSystem.cutsceneImg.gameObject.activeSelf)
        { cutsceneSO.skipOrCompleteSeq(GameSystem.cutsceneImg, GameSystem.cutsceneTxt); return; }

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
        foreach(GameObject pad in Pads)
        { if (pad.gameObject.activeSelf) { return; } }

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
            PSWindow_FC.ft_setClueImg(SelectBtn);
        }

    }
    private void LeftSelectedBtn(InputAction.CallbackContext obj)
    {
        foreach (GameObject pad in Pads)
        { if (pad.gameObject.activeSelf) { return; } }

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
            PSWindow_FC.ft_setClueImg(SelectBtn);
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
                if(index > SectionBtns[0].Count - 1)
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

        if (Desktop.PSWindow.activeSelf)
        {
            PSWindow_FC.ft_setClueImg(SelectBtn);
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
            PSWindow_FC.ft_setClueImg(SelectBtn);
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