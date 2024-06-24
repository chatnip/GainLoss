using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class PlayerInputController : Singleton<PlayerInputController>
{
    #region Value

    [Header("=== Base UI")]
    [SerializeField] public TMP_Text isRunModeTxt;

    [Header("=== Input Setting")]
    [SerializeField] public PlayerInput _input;
    [SerializeField] public List<List<Button>> SectionBtns;
    [SerializeField] public Button SelectBtn;
    [SerializeField] InputAction settingOn;

    [Header("=== Character Input Values")]
    public bool CanMove = false;
    public bool isRunMode = false;
    public Vector2 move;
    public Vector2 look;

    public IInteract interact;

    #endregion

    #region Framework & Base Set

    public void Offset()
    {
        isRunModeTxt.text = "OFF";

        TryGetComponent(out _input);
        _input.ObserveEveryValueChanged(x => x.currentControlScheme);
    }

    protected override void Awake()
    {
        base.Awake();
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

    #endregion

    #region Enable & Disable

    public void EnablePlayerInput()
    {
        var playerInput = _input.actions.FindActionMap("Player");

        // Move
        playerInput["Move"].performed += MoveOn;
        playerInput["Move"].canceled += MoveOn_Stop;
        playerInput["ToggleRun"].started += SetRunModeChange;

        // Back
        playerInput["BackBtn"].started += BackBtn;
        
        // Apply
        playerInput["SeleteBtn"].started += ApplySeleteBtn;

        // Cross
        playerInput["ChangeSeletedBtn_Right"].started += RightSelectedBtn;
        playerInput["ChangeSeletedBtn_Left"].started += LeftSelectedBtn;
        playerInput["ChangeSeletedBtn_Down"].started += DownSelectedBtn;
        playerInput["ChangeSeletedBtn_Up"].started += UpSelectedBtn;

        //Wheel
        playerInput["Zoom"].performed += Wheel;
        playerInput["Zoom"].canceled += Wheel_Stop;

        //playerInput["PointerMove"].performed += OnPointerMove;
        //playerInput["PointerMove"].canceled += OnPointerMoveStop;


    }

    public void DisablePlayerInput()
    {
        // Move
        _input.actions["Move"].performed -= MoveOn;
        _input.actions["Move"].canceled -= MoveOn_Stop;
        _input.actions["ToggleRun"].started -= SetRunModeChange;

        // Back
        _input.actions["BackBtn"].started -= BackBtn;

        // Apply
        _input.actions["SeleteBtn"].started -= ApplySeleteBtn;

        // Cross
        _input.actions["ChangeSeletedBtn_Right"].started -= RightSelectedBtn;
        _input.actions["ChangeSeletedBtn_Left"].started -= LeftSelectedBtn;
        _input.actions["ChangeSeletedBtn_Down"].started -= DownSelectedBtn;
        _input.actions["ChangeSeletedBtn_Up"].started -= UpSelectedBtn;

        //Wheel
        _input.actions["Zoom"].performed -= Wheel;
        _input.actions["Zoom"].canceled -= Wheel_Stop;

        //_input.actions["PointerMove"].performed -= OnPointerMove;
        //_input.actions["PointerMove"].canceled -= OnPointerMoveStop;

    }

    #endregion

    #region Physic

    // Move
    private void MoveOn(InputAction.CallbackContext obj)
    {
        MoveInput(obj.ReadValue<Vector2>());
    }

    private void MoveOn_Stop(InputAction.CallbackContext obj)
    {
        MoveInput(Vector2.zero);
    }

    public void MoveInput(Vector2 newMoveDirection)
    {
        if (GameSystem.Instance.canInput && CanMove)
        { move = newMoveDirection; }
    }

    public void MoveStop()
    {
        CanMove = false;
        move = Vector2.zero;
    }

    // Run
    private void SetRunModeChange(InputAction.CallbackContext obj)
    {
        if (!GameSystem.Instance.canInput || !CanMove) { return; }

        if (isRunMode)
        {
            isRunMode = false;
            isRunModeTxt.text = "OFF";

            PlayerController.Instance.MoveSpeed = 6f;
        }
        else
        {
            isRunMode = true;
            isRunModeTxt.text = "ON";

            PlayerController.Instance.MoveSpeed = 12f;
        }
    }

    // Wheel
    private void Wheel(InputAction.CallbackContext obj)
    {
        
        ReasoningController.Instance.ZoomInOut(obj.ReadValue<float>());
    }
    private void Wheel_Stop(InputAction.CallbackContext obj)
    {
        ReasoningController.Instance.ZoomInOut(0f);
    }

    #endregion

    #region Unique Func

    private void BackBtn(InputAction.CallbackContext obj)
    {
        if(!GameSystem.Instance.canInput) { return; }

        // 불가능 상황
        if (DialogManager.Instance.objPanelBtn.gameObject.activeSelf ||
            StreamController.Instance.gameObject.activeSelf) 
        { Debug.Log("can't Input"); return; }

        // 기타
        ActivityController.Instance.QuestionWindow_ActiveOff(0f);


        // 컴퓨터이 켜져 있을 시
        if (DesktopController.Instance.DesktopCamera.gameObject.activeSelf)
        {
            DesktopController.Instance.TurnOff(); Debug.Log("Desktop Off");
        }
        // 추리 코르크 보드가 켜져있을 때
        else if (ReasoningController.Instance.gameObject.activeSelf)
        {
            ReasoningController.Instance.ActiveOff(0.5f);
        }
        // 휴대폰이 켜져있을 경우
        else if (PhoneHardware.Instance.gameObject.activeSelf)
        {
            PhoneSoftware.Instance.ClosePopup(0f);
            PhoneHardware.Instance.PhoneOff();
            CanMove = true; Debug.Log("Phone Off");
        }
        else
        {
            StartCoroutine(PhoneHardware.Instance.Start_PhoneOn(PhoneHardware.e_phoneStateExtra.option)); Debug.Log("Phone On");
        }
    }

    #endregion

    #region Apply

    // A Btn
    public void ApplySeleteBtn(InputAction.CallbackContext obj)
    {
        if (!GameSystem.Instance.canInput) { return; }

        if (SelectBtn != null)
        {
            SelectBtn.TryGetComponent(out Button btn);
            if (interact != null && btn.interactable && btn.gameObject.activeSelf)
            {
                interact.Interact();
            }
        }

    }


    #endregion

    #region Cross

    private void RightSelectedBtn(InputAction.CallbackContext obj)
    {
        if (!GameSystem.Instance.canInput) { return; }

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
        if (!GameSystem.Instance.canInput) { return; }

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
        if (!GameSystem.Instance.canInput) { return; }

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
        if (!GameSystem.Instance.canInput) { return; }

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
    }

    // Set
    public void SetSectionBtns(List<List<Button>> btns, IInteract inter)
    {
        if (btns == null || inter == null || btns.Count == 0)
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
                    RT.DOScale(Vector3.one * 1.1f, 0.1f).SetUpdate(true);
                }
            }
            else // 비선택 버튼들
            {
                if (SectionBtn.gameObject.TryGetComponent(out Outline outline))
                { outline.enabled = false; }
                if (SectionBtn.gameObject.TryGetComponent(out RectTransform RT))
                {
                    DOTween.Kill(RT.localScale);
                    RT.DOScale(Vector3.one * 1.0f, 0.1f).SetUpdate(true);
                }
            }
        }
    }
    private void ClearSeletedBtns()
    {
        SectionBtns = null;
        SelectBtn = null;
        interact = null;
    }

    #endregion

}