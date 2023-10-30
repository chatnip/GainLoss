using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;

public class PlayerController : MonoBehaviour
{
    #region ����
    [Header("Player")]
    [Tooltip("ĳ������ �ӵ�")]
    [SerializeField] float MoveSpeed = 2.0f;

    [Tooltip("ĳ���Ͱ� �̵� ������ ���Ͽ� ȸ���ϴ� �ӵ�")]
    [Range(0.0f, 0.3f)]
    [SerializeField] float RotationSmoothTime = 0.12f;

    [Tooltip("���� �� ����")]
    public float SpeedChangeRate = 10.0f;

    [Tooltip("ĳ������ ��ǥ��")]
    [SerializeField] protected Vector3 direction;

    [Header("Cinemachine")]
    [Tooltip("ī�޶� ���� �ó׸ӽ� ���� ī�޶� ������ ���� ���")]
    public GameObject CinemachineCameraTarget;

    [Tooltip("ī�޶� �󸶳� ���� �ø� �� �ִ���")]
    public float TopClamp = 10.0f;

    [Tooltip("ī�޶� �󸶳� �Ʒ��� ���� �� �ִ���")]
    public float BottomClamp = -10.0f;

    [Tooltip("ī�޶� ��ġ �̼� ����")]
    public float CameraAngleOverride = 0.0f;

    [Tooltip("ī�޶� ��ġ ���� ����")]
    public bool LockCameraPosition = false;

    [SerializeField] CharacterController _controller;
    [SerializeField] PlayerInputController _input;
    [SerializeField] Transform _mainCamera;
    [SerializeField] Animator _animator;

    // [SerializeField] GameObject interactCanvas;
    [SerializeField] private InputAction interactInput,cancelInput;

    private const float _threshold = 0.01f;

    // cinemachine
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    // player
    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    // private float _terminalVelocity = 53.0f;

    // timeout deltatime
    // private float _jumpTimeoutDelta;
    // private float _fallTimeoutDelta;

    // animation IDs
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;

    // Keyboard Mouse
    private bool IsCurrentDeviceMouse
    {
        get
        {
            return _input._input.currentControlScheme == "KeyboardMouse";
        }
    }

    // Event Mode
    [Tooltip("��ȣ�ۿ� �������� ����")]
    private BoolReactiveProperty interactableMode = new BoolReactiveProperty();

    [Tooltip("��ȣ�ۿ������� ����")]
    private BoolReactiveProperty interactMode = new BoolReactiveProperty();

    private IInteract interactObject;
    #endregion

    private void Awake()
    {
        interactableMode
            .Subscribe(x =>
            {
                if(x)
                {
                    EnableInteractInput();
                }
                else
                {
                    DisableInteractInput();
                }
            });

        interactMode
            .Subscribe(x =>
            {
                if (x)
                {
                    _input.DisablePlayerInput();
                }
                else
                {
                    _input.EnablePlayerInput();
                }
            });
    }

    private void Start()
    {
        AssignAnimationIDs();
        
    }

    private void EnableInteractInput()
    {
        interactInput.Enable();
        cancelInput.Enable();
        interactInput.started += _ => { OnInteract(); };
        cancelInput.started += _ => { OnInteractCancel(); };
    }

    private void DisableInteractInput()
    {
        interactInput.Disable();
        cancelInput.Disable();
        interactInput.started -= _ => { OnInteract(); };
        cancelInput.started -= _ => { OnInteractCancel(); };
    }

    private void FixedUpdate()
    {
        if(!interactMode.Value)
        {
            Move();
        }
    }

    private void LateUpdate()
    {
        if (!interactMode.Value)
        {
            CameraRotation();
        }
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    public virtual void Move()
    {
        float targetSpeed = MoveSpeed;

        // �Է��� ������ ��ǥ �ӵ��� 0���� ����
        if (_input.move == Vector2.zero) targetSpeed = 0.0f;

        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;   
        float speedOffset = 0.1f;
        float inputMagnitude = 1f;

        // ��ǥ �ӵ����� ���� �Ǵ� ����
        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        // �Է� ���� ����ȭ
        Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

        // �̵� �Է��� ���� ��� �÷��̾ ������ �� �÷��̾� ȸ��
        if (_input.move != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

            // ī�޶� ��ġ�� �������� �Է¹����� ���ϵ��� ȸ��
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        // �÷��̾� �̵�
        _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

        // �ִϸ����� ������Ʈ
        _animator.SetFloat(_animIDSpeed, _animationBlend);
        _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
    }
    private void CameraRotation()
    {
        // �Է��� �ְ� ī�޶� ��ġ�� �������� ���� ���
        if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
            _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
        }

        // ȸ���� �����Ͽ� ���� 360���� ���ѵǵ��� ��
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("InteractableObject"))
        {
            interactableMode.Value = true;
            other.TryGetComponent(out interactObject);
            Debug.Log("����");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        interactableMode.Value = false;
        interactObject = null;
        Debug.Log("������");
    }

    private void OnInteract()
    {
        if(!interactMode.Value)
        {
            // interactCanvas.SetActive(true);
            interactObject.Interact();
        }     
        interactMode.Value = true;

    }

    private void OnInteractCancel()
    {
        if (interactMode.Value)
        {
            // interactCanvas.SetActive(false);
            interactMode.Value = false;
        }      
        interactObject.InteractCancel();
    }
}