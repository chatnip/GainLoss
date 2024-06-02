using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class PlayerController : Singleton<PlayerController>
{
    #region Value

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

    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool Grounded = true;

    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = 0.8f;

    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.8f;

    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    /*
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
    */

    [SerializeField] CharacterController _controller;
    [SerializeField] Transform _mainCamera;
    [SerializeField] Transform _npcInteractCamera;
    [SerializeField] public bool isTalking = false;
    [SerializeField] public Animator _animator;

    // [SerializeField] GameObject interactCanvas;
    // [SerializeField] private InputAction interactInput,cancelInput;

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
            return PlayerInputController.Instance._input.currentControlScheme == "KeyboardMouse";
        }
    }

    #endregion

    #region Main

    protected override void Awake()
    {
        base.Awake();
        AssignAnimationIDs();
        ft_resetPlayerSpot();
    }

    private void Start()
    {
        //AssignAnimationIDs();
        //ft_resetPlayerSpot();
    }

    private void FixedUpdate()
    {
        
        GroundedCheck();
        if (!GameManager.Instance.canInput) { return; }

        if(PlayerInputController.Instance.CanMove && _controller.enabled) Move();
        if(isTalking) setOriginalAnimation();

        if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f &&
            !_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle Walk Run Blend"))
        {
            resetAnime();
        }
    }

    #endregion


    public void ft_setPlayerSpot(Vector3 pos)
    {
        _controller.enabled = false;
        this.gameObject.transform.position = pos;
        _controller.enabled = true;
    }
    public void ft_resetPlayerSpot()
    {
        ft_setPlayerSpot(Vector3.zero);
    }
    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y + GroundedOffset,
            transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);
        if (!Grounded)
        { this.gameObject.transform.position += Vector3.down * 0.2f * Time.deltaTime; }
        // update animator if using character
        /*
        if (_hasAnimator)
        {
            _animator.SetBool(_animIDGrounded, Grounded);
        }
        */
    }
    void OnDrawGizmosSelected()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y + GroundedOffset,
            transform.position.z);
        // Draw a yellow sphere at the transform's position
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawSphere(spherePosition, GroundedRadius);
    }

    public virtual void Move()
    {
        float targetSpeed = MoveSpeed;

        // �Է��� ������ ��ǥ �ӵ��� 0���� ����
        if (PlayerInputController.Instance.move == Vector2.zero) targetSpeed = 0.0f;

        // ��ȣ�ۿ� ���̸� ��ǥ �ӵ��� 0���� ����
        // if (_input.interactMode.Value == true) targetSpeed = 0.0f;

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
        Vector3 inputDirection = new Vector3(PlayerInputController.Instance.move.x, 0.0f, PlayerInputController.Instance.move.y).normalized;

        // �̵� �Է��� ���� ��� �÷��̾ ������ �� �÷��̾� ȸ��
        if (PlayerInputController.Instance.move != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

            // ī�޶� ��ġ�� �������� �Է¹����� ���ϵ��� ȸ��
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        if (!Grounded)
        { targetDirection.y += -9.81f * Time.deltaTime; }

        // �÷��̾� �̵�
        _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

        

        // �ִϸ����� ������Ʈ
        _animator.SetFloat(_animIDSpeed, _animationBlend);
        _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
    }

    public void resetAnime()
    {
        if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle Walk Run Blend"))
        {
            _animator.SetTrigger("Return");
        }
        if(_animationBlend != 0)
        {
            _animator.SetFloat(_animIDSpeed, 0);
            _animationBlend = 0;
        }
        
    }


    private void setOriginalAnimation() // NPC�� ��ȣ�ۿ� ���� ��, �ִϸ��̼� �ѹ� ���� �� ��
    {
        if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle Walk Run Blend"))
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                _animator.SetTrigger("Return");
            }
        }
    }

    public void setOnNpcInteractCamera(GameObject targetGO)
    {
        _npcInteractCamera.gameObject.SetActive(true);
        this.transform.LookAt(new Vector3(targetGO.transform.position.x, this.transform.position.y, targetGO.transform.position.z));
        _npcInteractCamera.LookAt(targetGO.transform.position + new Vector3(0, 1.6f, 0));
    }
    public void setOffNpcInteractCamera()
    {
        _npcInteractCamera.gameObject.SetActive(false);
    }

    

    /*
    private void CameraRotation()
    {
        // ��ȣ�ۿ� ���̸� ī�޶� ���
        LockCameraPosition = _input.interactMode.Value;

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
    */

    /*
    private void OnTriggerEnter(Collider other)
    {
        other.TryGetComponent(out IInteract interactContianer);
        if (interactContianer != null)
        {
            PlayerInputController.Instance.SetInteract(interactContianer);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        other.TryGetComponent(out IInteract interactContianer);
        if (PlayerInputController.Instance.AlreadyHaveInteract(interactContianer))
        {
            PlayerInputController.Instance.SetInteract(null);
        }
    }
    */
}