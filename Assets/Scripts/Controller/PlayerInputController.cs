using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;

public class PlayerInputController : Manager<PlayerInputController>
{
    [Header("Popup")]
    [SerializeField] Pause pause;
    
    [Header("Input Setting")]
    [SerializeField] public PlayerInput _input;
    [SerializeField] InputAction settingOn;

    [Header("Character Input Values")]
    public Vector2 move;
    public Vector2 look;

    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    [Header("Event Settings")]
    // public bool interactableMode;

    [Tooltip("상호작용중인지 여부")]
    // public BoolReactiveProperty interactMode = new BoolReactiveProperty();

    private IInteract interact;


    protected override void Awake()
    {
        base.Awake();
        TryGetComponent(out _input);

        _input.ObserveEveryValueChanged(x => x.currentControlScheme)
            .Subscribe(OnControlSchemeChanged);

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
        // playerInput["Interact"].started += OnInteract;
        // playerInput["InteractCancel"].started += OnInteractCancel;
    }

    public void DisablePlayerInput()
    {
        _input.actions["Move"].performed -= OnMove;
        _input.actions["Move"].canceled -= OnMoveStop;
        _input.actions["Look"].performed -= OnLook;
        _input.actions["Pause"].started -= OnPause;
        // _input.actions["Interact"].started -= OnInteract;
        // _input.actions["InteractCancel"].started -= OnInteractCancel;
    }


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

    /*
    private void OnInteract(InputAction.CallbackContext obj)
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
    */

    /*
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

    #endregion

    public void MoveInput(Vector2 newMoveDirection)
    {
        move = newMoveDirection;
    }

    public void LookInput(Vector2 newLookDirection)
    {
        look = newLookDirection;
    }

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