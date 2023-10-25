using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;

public class PlayerInputController : Manager<PlayerInputController>
{
    [Header("Input Setting")]
    [SerializeField] public PlayerInput _input;

    [Header("Character Input Values")]
    public Vector2 move;
    public Vector2 look;

    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    [Header("Event Settings")]
    public bool interactableMode;
    

    protected override void Awake()
    {
        base.Awake();
        TryGetComponent(out _input);

        _input.ObserveEveryValueChanged(x => x.currentControlScheme)
            .Subscribe(OnControlSchemeChanged);
    }

    public void OnControlSchemeChanged(string _controlScheme)
    {
        Debug.Log($"OnControlSchemeChanged : {_controlScheme}");
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
    }
    private void OnDisable()
    {
        DisablePlayerInput();
    }

    public void EnablePlayerInput()
    {
        var playerInput = _input.actions.FindActionMap("Player");
        playerInput["Move"].performed += OnMove;
        playerInput["Move"].canceled += OnMoveStop;
        playerInput["Look"].performed += OnLook;
    }

    public void DisablePlayerInput()
    {
        _input.actions["Move"].performed -= OnMove;
        _input.actions["Move"].canceled -= OnMoveStop;
        _input.actions["Look"].performed -= OnLook;
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

    #endregion

    public void MoveInput(Vector2 newMoveDirection)
    {
        move = newMoveDirection;
    }

    public void LookInput(Vector2 newLookDirection)
    {
        look = newLookDirection;
    }
    /*
    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }
    */

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
    

    private bool LookForGameObject(out RaycastHit hit)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out hit);
    }
}