using Spine;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    #region Value

    [Header("Player")]
    [Tooltip("캐릭터의 속도")]
    [SerializeField] public float MoveSpeed = 2.0f;

    [Tooltip("캐릭터가 이동 방향을 향하여 회전하는 속도")]
    [Range(0.0f, 0.3f)]
    [SerializeField] float RotationSmoothTime = 0.12f;

    [Tooltip("가속 및 감속")]
    public float SpeedChangeRate = 10.0f;

    [Tooltip("캐릭터의 좌표계")]
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

    [SerializeField] CharacterController _controller;
    [SerializeField] Transform _mainCamera;
    [SerializeField] Transform _npcInteractCamera;
    [SerializeField] public bool isTalking = false;
    [SerializeField] public Animator _animator;

    // player
    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;

    // animation IDs
    private int _animIDSpeed;
    private int _animIDMotionSpeed;

    #endregion

    #region Framework

    private void Offset()
    {
        // Assign Animation IDs
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");

        // Reset Pos
        ft_resetPlayerSpot();


        _animator.SetFloat(_animIDMotionSpeed, 1f);
    }

    protected override void Awake()
    {
        base.Awake();
        Offset();
    }

    private void FixedUpdate()
    {
        GroundedCheck();
        if (!GameManager.Instance.canInput) { return; }

        if(PlayerInputController.Instance.CanMove && _controller.enabled) Move();
        if(isTalking) SetOriginalAnimation();

        if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f &&
            !_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle Walk Run Blend"))
        {
            ResetAnime();
        }
    }

    #endregion

    #region Pos

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

    #endregion

    #region Gounded

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

    #endregion

    #region Move

    public virtual void Move()
    {
        float targetSpeed = MoveSpeed;

        // 입력이 없으면 목표 속도를 0으로 설정
        if (PlayerInputController.Instance.move == Vector2.zero) targetSpeed = 0.0f;

        // 상호작용 중이면 목표 속도를 0으로 설정
        // if (_input.interactMode.Value == true) targetSpeed = 0.0f;

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
        Vector3 inputDirection = new Vector3(PlayerInputController.Instance.move.x, 0.0f, PlayerInputController.Instance.move.y).normalized;

        // 이동 입력이 있을 경우 플레이어가 움직일 때 플레이어 회전
        if (PlayerInputController.Instance.move != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

            // 카메라 위치를 기준으로 입력방향을 향하도록 회전
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        if (!Grounded)
        { targetDirection.y += -9.81f * Time.deltaTime; }

        // 플레이어 이동
        _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

        

        // 애니메이터 업데이트
        _animator.SetFloat(_animIDSpeed, _animationBlend);
        _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
    }

    #endregion

    #region Animation 

    public void ResetAnime()
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

    private void SetOriginalAnimation() // NPC와 상호작용 중일 때, 애니메이션 한번 실행 후 컷
    {
        if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle Walk Run Blend"))
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                _animator.SetTrigger("Return");
            }
        }
    }

    public void PlayInteractAnim(AnimationClip AC)
    {
        AnimatorOverrideController aoc = new AnimatorOverrideController(_animator.runtimeAnimatorController);
        var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        foreach (var a in aoc.animationClips)
            anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(a, AC));
        aoc.ApplyOverrides(anims);
        _animator.runtimeAnimatorController = aoc;
        _animator.Play("Interact");
    }


    #endregion

    #region Gizmos

    void OnDrawGizmosSelected()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y + GroundedOffset,
            transform.position.z);
        // Draw a yellow sphere at the transform's position
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawSphere(spherePosition, GroundedRadius);
    }

    #endregion
}