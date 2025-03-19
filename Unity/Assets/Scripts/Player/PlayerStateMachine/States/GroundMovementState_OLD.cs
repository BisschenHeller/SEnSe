using R3;
using R3.Triggers;
using System;
using UnityEngine;

public class GroundMovementState_OLD : PlayerState_OLD
{
    [Tooltip("Acceleration and deceleration")]
    public float SpeedChangeRate = 10.0f;

    [Space(10)]
    [Tooltip("The height the player can jump")]
    public float JumpHeight = 1.2f;

    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    public float JumpTimeout = 0.50f;

    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float FallTimeout = 0.15f;

    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = -0.14f;

    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.28f;

    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;

    [Tooltip("For locking the camera position on all axis")]
    public bool LockCameraPosition = false;

    // cinemachine
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    // player
    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    // animation IDs
    private int _animIDSpeed;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;

    protected override void OnEnter()
    {
        _cinemachineTargetYaw = player.CinemachineTarget.transform.rotation.eulerAngles.y;

        AssignAnimationIDs();

        // reset our timeouts on start
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;

        player._looking.Where(n => n.magnitude > 0.01f).Subscribe(LookAround).AddTo(player);

        player._movement.CombineLatest(player._sprinting, (movementInput, sprinting) =>
        {
            return (sprinting && movementInput != Vector2.zero) ? movementInput.normalized * player.runningSpeed : movementInput * player.walkingSpeed;
        }).Subscribe(n => Move(n)).AddTo(player);

        SubscribeTransitions();
    }

    private IDisposable swimSubscription;

    protected void SubscribeTransitions()
    {
        player.gameObject.OnCollisionEnterAsObservable().Subscribe(n =>
        {
            Debug.Log("CollisionEnter");
            if (n.gameObject.layer == LayerMask.NameToLayer("Water"))
            {
                //player.ChangeState(new SwimmingState(n.contacts[0].point.y));
            } 
        }).AddTo(player);
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    private void LookAround(Vector2 look)
    {
        // if there is an input and camera position is not fixed
        if (!LockCameraPosition)
        {
            //Don't multiply mouse input by Time.deltaTime;
            float deltaTimeMultiplier = player.isCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            _cinemachineTargetYaw += look.x * deltaTimeMultiplier;
            _cinemachineTargetPitch += look.y * deltaTimeMultiplier;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, player.cameraClampBottom, player.cameraClampTop);

        // Cinemachine will follow this target
        player.CinemachineTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw, 0.0f);
    }

    private void Move(Vector3 desiredVelocity)
    {
        
        float targetSpeed = desiredVelocity.magnitude;
        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(player.velocity.x, 0.0f, player.velocity.z).magnitude;

        _speed = Mathf.Min(targetSpeed, (currentHorizontalSpeed + targetSpeed) / 2);

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        // normalise input direction
        Vector3 inputDirection = new Vector3(desiredVelocity.x, 0.0f, desiredVelocity.y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (desiredVelocity != Vector3.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                              player._mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(player.transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                player.harshRotation);

            // rotate to face input direction relative to camera position
            player.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }


        // move the player
        player.Move(player.transform.forward * (_speed * Time.deltaTime));
        // update animator if using character


        player._animator.SetFloat(_animIDSpeed, _animationBlend);
        player._animator.SetFloat(_animIDMotionSpeed, desiredVelocity.magnitude);
    }

    private void JumpAndGravity()
    {
        
    }

    protected override void UpdateGravity()
    {
        /*if (player.Grounded)
        {
            _verticalVelocity = 0;
            
        } else if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += player.gravity * Time.deltaTime;
        }
        player.Move(new Vector3(0, -_verticalVelocity, 0));*/
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    public override void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (player.Grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(
            new Vector3(player.transform.position.x, player.transform.position.y - GroundedOffset, player.transform.position.z),
            GroundedRadius);
    }

    protected override void UpdateCamera()
    {
        player.CinemachineTarget.transform.position = player.transform.position + new Vector3(0, player.height, 0);
    }

    protected override void UpdateGrounded()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(player.transform.position.x,
                                             player.transform.position.y - GroundedOffset,
                                             player.transform.position.z);
        player.Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, player.GroundLayers,
            QueryTriggerInteraction.Ignore);

        // if (!player.Grounded) player._animator.SetBool(_animIDFreeFall, true); // TODO Transition to Falling State

        // update animator if using character
        player._animator.SetBool(_animIDGrounded, player.Grounded);
    }

    protected override void UpdateArbitraryValues()
    {
        // Nothing to update
    }

    public override string ToDebugString()
    {
        return "Ground Movement\n" +
            "Velocity: " + player.velocity + "\n" +
            "Speed: " + Math.Round(player.velocity.magnitude, 1);
            
    }
}