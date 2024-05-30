using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerInputController : Singleton<PlayerInputController>
{
    #region Value

    [Header("*Property")]
    [HideInInspector] public bool isPause = false;
    [SerializeField] GameObject allLoadingGO;

    [Header("*Tutorial")]
    [SerializeField] GameObject TutorialScreenGO;

    [Header("*Phone")]
    [SerializeField] PhoneHardware PhoneHardware;
    [SerializeField] PhoneSoftware PhoneSoftware;
    [SerializeField] GameObject PhoneCamera2D;

    [Header("*Computer")]
    [SerializeField] ComputerInteract ComputerInteract;
    [SerializeField] DesktopController Desktop;
    [SerializeField] GameObject PSWindow_choose;
    [SerializeField] PreliminarySurveyWindow_FindClue PSWindow_FC;
    [SerializeField] PreliminarySurveyWindow_Extract PSWindow_E;
    [SerializeField] GameObject ComputerCamera2D;

    [Header("*Interact Object")]
    [SerializeField] ObjectInteractionButtonGenerator ObjectInteractionButtonGenerator;
    [SerializeField] GameObject Panel_Object;
    [SerializeField] GameObject Panel_Npc;
    List<GameObject> Panels = new List<GameObject>();

    [Header("*Camera")]
    [SerializeField] GameObject QuarterViewCamera;

    [Header("*Input Setting")]
    [SerializeField] public PlayerInput _input;
    [SerializeField] public List<List<Button>> SectionBtns;
    [SerializeField] public Button SelectBtn;
    [SerializeField] InputAction settingOn;

    [Header("*Character Input Values")]
    public bool CanMove = false;
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
        base.Awake();
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
        PlayerController.Instance.resetAnime();
    }

    #endregion

    #region Unique Func

    // 일시정지
    private void OnOffPause(InputAction.CallbackContext obj)
    {
        if (!GameManager.Instance.canInput) { return; }

        // 폰 켜져있을 시 끄기
        if (PhoneHardware.Instance.gameObject.activeSelf)
        {
            PhoneHardware.PhoneOff();
            return;
        }
        // 컴퓨터 켜져있을 시 끄기
        else if (ComputerCamera2D.activeSelf)
        {
            return;
        }
        else if (GameSystem.Instance.objPanelBtn.gameObject.activeSelf)
        {
            GameSystem.Instance.ObjDescOff();
            return;
        }

        OnOffPause();
    }
    public void OnOffPause()
    {
        if (!GameManager.Instance.canInput) { return; }

        /*if (pause.gameObject.activeSelf)
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

        }*/
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
    }
    public void OnOffPhone()
    {
        if (!GameManager.Instance.canInput) { return; }

        if (isPause ||
            !QuarterViewCamera.activeSelf ||
            GameSystem.Instance.cutsceneImg.gameObject.activeSelf ||
            TutorialScreenGO.activeSelf ||
            allLoadingGO.activeSelf) 
        { return; }

        foreach (GameObject Go in Panels)
        {
            if (Go.activeSelf) { return; }
        }

        if (ObjectInteractionButtonGenerator.SectionIsThis)
        { ObjectInteractionButtonGenerator.SetOnOffInteractObjectBtn(); }

    }

    // 상호작용 오브젝트 UI 활성화 / 비활성화
    private void OnOffInteractObject(InputAction.CallbackContext obj)
    {
        OnOffInteractObject();
    }
    public void OnOffInteractObject()
    {
        if (!GameManager.Instance.canInput) { return; }

        if (isPause || 
            !QuarterViewCamera.activeSelf || 
            GameSystem.Instance.cutsceneImg.gameObject.activeSelf ||
            TutorialScreenGO.activeSelf ||
            allLoadingGO.activeSelf) 
        { return; }

        foreach (GameObject Go in Panels)
        {
            if (Go.activeSelf) { return; }
        }

        //if (SchedulePrograss.OnExplanation)
        //{ SchedulePrograss.OnOffVisibleSchedule(); }

        ObjectInteractionButtonGenerator.SetOnOffInteractObjectBtn();
    }

    // 스케쥴 표 보이기
    private void OnOffShowScheduleDetailBtn(InputAction.CallbackContext obj)
    {
       
    }
    

    // 무언가를 선택 (X키)
    private void SetSomething(InputAction.CallbackContext obj)
    {
        if (!GameManager.Instance.canInput) { return; }

        if (PSWindow_FC.gameObject.activeSelf && !PSWindow_FC.resultWindowParentGO.activeSelf || GameSystem.Instance.cutsceneImg.gameObject.activeSelf)
        {
            PSWindow_FC.ft_setChooseClue(SelectBtn);
        }
    }

    // 파트 넘기기 (Y키)
    private void TerminatePart(InputAction.CallbackContext context)
    {
        if (!GameManager.Instance.canInput) { return; }

        if (isPause || 
            GameSystem.Instance.cutsceneImg.gameObject.activeSelf ||
            allLoadingGO.activeSelf) 
        { return; }

        if (PSWindow_FC.gameObject.activeSelf && !PSWindow_FC.resultWindowParentGO.activeSelf) 
        { PSWindow_FC.ft_tryToCombine(); }

        if (ObjectInteractionButtonGenerator.SectionIsThis)
        { ObjectInteractionButtonGenerator.SetOnOffInteractObjectBtn(); }

    }

    // 뒤로가기(B키)
    private void BackBtn(InputAction.CallbackContext obj)
    {
        if (!GameManager.Instance.canInput) { return; }

        // 본 오브젝트가 켜져있을 때, 이 키는 기능 X
        if (GameSystem.Instance.cutsceneImg.gameObject.activeSelf ||
            Desktop.streamWindow.activeSelf || 
            PSWindow_E.gameObject.activeSelf ||
            TutorialScreenGO.activeSelf ||
            allLoadingGO.activeSelf)
        { return; }

        //Panels
        if (Panel_Object.activeSelf) 
        { GameSystem.Instance.ObjDescOff(); return; }

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
            ClearSeletedBtns();
            return;
        }

        bool ComputerOffWindow(GameObject Window)
        {
            if (Window.activeSelf)
            {
                //Desktop.DisappearPopup(Window);
                return true;
            }
            return false;
        }

        if (PhoneCamera2D.activeSelf) //PhoneScreen
        {
            PhoneHardware.PhoneOff();
            ClearSeletedBtns();
            return;
        }

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
        if (!GameManager.Instance.canInput) { return; }

        if (PSWindow_FC.gameObject.activeSelf) { pointerMove = newPointerMoveDirection; }
    }
    

    #endregion

    #region Selected Btns

    public void ApplySeleteBtn(InputAction.CallbackContext obj)
    {
        if (!GameManager.Instance.canInput) { return; }

        if (GameSystem.Instance.cutsceneImg.gameObject.activeSelf)
        { cutsceneSO.skipOrCompleteSeq(GameSystem.Instance.cutsceneImg, GameSystem.Instance.cutsceneTxt); return; }

        if (Panel_Object.activeSelf)
        { GameSystem.Instance.ObjDescSkip(); return; }

        if (PSWindow_FC.gameObject.activeSelf)
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
        if (!GameManager.Instance.canInput) { return; }

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

        if (PSWindow_FC.gameObject.activeSelf)
        {
            PSWindow_FC.ft_setClueImg(SelectBtn);
        }

    }
    private void LeftSelectedBtn(InputAction.CallbackContext obj)
    {
        if (!GameManager.Instance.canInput) { return; }

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
        if (PSWindow_FC.gameObject.activeSelf)
        {
            PSWindow_FC.ft_setClueImg(SelectBtn);
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

        if (PSWindow_FC.gameObject.activeSelf)
        {
            PSWindow_FC.ft_setClueImg(SelectBtn);
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
                if (index > SectionBtns[SectionBtns.Count - 1].Count - 1)
                {
                    SelectBtn = SectionBtns[SectionBtns.Count - 1][0];
                }
                else
                {
                    SelectBtn = SectionBtns[SectionBtns.Count - 1][index];
                }
            }
            else
            {
                if (index > SectionBtns[lineIndex - 1].Count - 1)
                {
                    SelectBtn = SectionBtns[lineIndex - 1][0];
                }
                else
                {
                    SelectBtn = SectionBtns[lineIndex - 1][index];
                }
            }
            OnOffSelectedBtn(SelectBtn);
        }
        if (PSWindow_FC.gameObject.activeSelf)
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

}