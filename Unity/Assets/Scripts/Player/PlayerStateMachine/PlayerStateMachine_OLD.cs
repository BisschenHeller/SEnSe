using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerStateMachine_OLD : MonoBehaviour
{
    public PlayerState_OLD currentActiveState;

    [HideInInspector]
    private CharacterController _characterController;
    public float radius { get { return _characterController.radius; } }
    public float height { get { return _characterController.height; } }
    public Vector3 center { get { return _characterController.center; } }
    public Vector3 velocity { get { return _characterController.velocity; } }

    public CapsuleCollider triggerCollider;


    public void Move(Vector3 movement)
    {
        _characterController.Move(movement);
    }

    private void OnDrawGizmosSelected()
    {
        currentActiveState?.OnDrawGizmosSelected();
    }

    private void OnDrawGizmos()
    {
        currentActiveState?.OnDrawGizmos();
    }

    [HideInInspector]
    public PlayerInput _input;

    public Camera _mainCamera;

    private IDisposable updateSubscription;

    public bool isCurrentDeviceMouse { get { return _input.currentControlScheme == "KeyboardMouse"; } }

    [Header("Camera")]
    public Transform CinemachineTarget;
    public float cameraClampTop;
    public float cameraClampBottom;
    public bool lockCamera = false;

    [Header("Animation")]
    public Animator _animator;
    public float animationBlend = 0.1f;

    [Header("Movement Details")]
    public float walkingAccelleration = 0.1f;
    public float walkingSpeed = 2.0f;
    public float runningSpeed = 5.0f;
    public float swimmingSpeed = 1.5f;
    public float gravity = 15.0f;
    [Range(0.1f, 1.0f)]
    public float harshRotation = 0.2f;
    [HideInInspector]
    public float rotationVelocity = 0.0f;

    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool Grounded = true;

    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = -0.14f;

    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.28f;

    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    [SerializeField]
    private SensedInput detectedInput;

    // Sensors
    public Observable<Vector2> _movement;
    public Observable<bool> _sprinting;
    public Observable<Vector2> _looking;

    public void OnMove(InputValue input)
    {
        detectedInput.move = input.Get<Vector2>();
    }

    public void OnSprint(InputValue input)
    {
        detectedInput.sprinting = input.Get<float>() != 0;
    }

    public void OnJump(InputValue input)
    {
        
    }


    public void OnLook(InputValue input)
    {
        detectedInput.look = input.Get<Vector2>();
    }

    public void ChangeState(PlayerState_OLD newState)
    {
        currentActiveState?.ExitState();
        currentActiveState = newState;
        newState.InitState(this);
    }

    public void Start()
    {
        this._characterController = GetComponent<CharacterController>();
        this._input = GetComponent<PlayerInput>();
        
        triggerCollider.radius = radius;
        triggerCollider.center = center;
        triggerCollider.height = height;
        triggerCollider.isTrigger = true;

        _movement = Observable.EveryUpdate().Select(n => detectedInput.move);
        _sprinting = Observable.EveryUpdate().Select(n => detectedInput.sprinting);
        _looking = Observable.EveryUpdate().Select(n => detectedInput.look);

        updateSubscription = Observable.EveryUpdate().Subscribe(n => currentActiveState.Update());

        ChangeState(new GroundMovementState_OLD());
    }

    public string ToDebugString()
    {
        return currentActiveState.ToDebugString();
    }

    public void OnDestroy()
    {
        updateSubscription.Dispose();
        currentActiveState.ExitState();
    }
}

[Serializable]
public class SensedInput
{
    public Vector2 move;
    public Vector2 look;
    public bool sprinting;
    public bool jump;
}