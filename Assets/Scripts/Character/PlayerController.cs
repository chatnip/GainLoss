using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;

public class PlayerController : MonoBehaviour
{
    #region 변수
    [Header("Player")]
    [Tooltip("캐릭터의 속도")]
    [SerializeField] float MoveSpeed = 2.0f;

    [Tooltip("캐릭터가 이동 방향을 향하여 회전하는 속도")]
    [Range(0.0f, 0.3f)]
    [SerializeField] float RotationSmoothTime = 0.12f;

    [Tooltip("가속 및 감속")]
    public float SpeedChangeRate = 10.0f;

    [Tooltip("캐릭터의 좌표계")]
    [SerializeField] protected Vector3 direction;

    [Header("Cinemachine")]
    [Tooltip("카메라가 따라갈 시네머신 가상 카메라에 설정된 추적 대상")]
    public GameObject CinemachineCameraTarget;

    [Tooltip("카메라를 얼마나 위로 올릴 수 있는지")]
    public float TopClamp = 10.0f;

    [Tooltip("카메라를 얼마나 아래로 내릴 수 있는지")]
    public float BottomClamp = -10.0f;

    [Tooltip("카메라 위치 미세 조정")]
    public float CameraAngleOverride = 0.0f;

    [Tooltip("카메라 위치 고정 여부")]
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
    [Tooltip("상호작용 가능한지 여부")]
    private BoolReactiveProperty interactableMode = new BoolReactiveProperty();

    [Tooltip("상호작용중인지 여부")]
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

        // 입력이 없으면 목표 속도를 0으로 설정
        if (_input.move == Vector2.zero) targetSpeed = 0.0f;

        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;   
        float speedOffset = 0.1f;
        float inputMagnitude = 1f;

        // 목표 속도까지 가속 또는 감속
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

        // 입력 방향 정규화
        Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

        // 이동 입력이 있을 경우 플레이어가 움직일 때 플레이어 회전
        if (_input.move != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

            // 카메라 위치를 기준으로 입력방향을 향하도록 회전
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        // 플레이어 이동
        _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

        // 애니메이터 업데이트
        _animator.SetFloat(_animIDSpeed, _animationBlend);
        _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
    }
    private void CameraRotation()
    {
        // 입력이 있고 카메라 위치가 고정되지 않은 경우
        if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
            _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
        }

        // 회전을 고정하여 값이 360도로 제한되도록 함
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
            Debug.Log("닿음");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        interactableMode.Value = false;
        interactObject = null;
        Debug.Log("떨어짐");
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