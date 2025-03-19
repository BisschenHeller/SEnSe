using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.VisualScripting;
using JetBrains.Annotations;

public abstract class PlayerState
{
    protected PlayerStateMachine player;

    protected PlayerStateFactory _factory;

    private List<IDisposable> subscriptions = new List<IDisposable>();

    public virtual string GetStateName() { return "UNNAMED_STATE"; }

    public PlayerState(PlayerStateMachine context, PlayerStateFactory factory)
    {
        player = context;
        _factory = factory;
    }

    protected abstract void EnterConcreteState();

    public void UpdateState()
    {
        UpdateGravity();
    }

    public void EnterState()
    {
        AddSubscription(SensorID.Movement, HandleMoveInput);
        AddSubscription(SensorID.LookAround, LookAround);
        AddSubscription(SensorID.Movement, SetAnimationMotionSpeed);
        AddSubscription(SensorID.Grounded, SetGrounded);

        EnterConcreteState();
    }

    public void ExitState()
    {
        subscriptions.ForEach(n => n.Dispose());

        ExitConcreteState();
    }

    protected abstract void ExitConcreteState();

    public abstract void InitializeSubState();

    protected void SwitchState(PlayerState newState)
    {
        Debug.Log("Transition: " + GetStateName() + "  -->  " + newState.GetStateName());
        ExitState();

        newState.EnterState();

        player._currentState = newState;
    }

    protected void SetSuperState()
    {

    }

    protected void SetSubState()
    {

    }

    protected virtual void SetAnimationMotionSpeed(Vector3 movement)
    {
        player.desiredMotionSpeed = player.currentSpeed / player.currentMovementSettings.sprintSpeed;
        if (movement.z < 0) player.desiredMotionSpeed = -player.currentMovementSettings.regularSpeed;
        
        player.SetAnimatedMotionSpeed(player.desiredMotionSpeed);
    }

    protected void SetGrounded(bool newGrounded)
    {
        //Debug.Log("SetGrounded(" + newGrounded + ")");
        player.grounded = newGrounded;
    }

    public virtual void HandleMoveInput(Vector3 premultipliedMovement)
    {
        float targetSpeed = premultipliedMovement.magnitude > 1.1? player.currentMovementSettings.sprintSpeed : player.currentMovementSettings.regularSpeed;

        if (premultipliedMovement.z < 0) targetSpeed = Mathf.Clamp(-player.currentSpeed, -player.currentMovementSettings.regularSpeed, 0);

        if (premultipliedMovement.magnitude == 0) targetSpeed = 0;
        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        if (Mathf.Abs(player.currentSpeed - targetSpeed) < 0.1)
        {
            // Do Nothing
        }
        else if (player.currentSpeed > targetSpeed)
        {
            player.currentSpeed -= Time.deltaTime * 10;
        } else
        {
            player.currentSpeed += Time.deltaTime * 10;
        }
        //_player.currentSpeed = targetSpeed;

        //Debug.Log("Premultiplied = "  +  premultipliedMovement+  "   TargetSpeed = " + targetSpeed + "   Speed = " + _player.currentSpeed);

        // normalise input direction
        Vector3 inputDirection = new Vector3(premultipliedMovement.x, 0.0f, premultipliedMovement.y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (premultipliedMovement != Vector3.zero)
        {
            float targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                              player.cameraOrientation.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(player.transform.eulerAngles.y, targetRotation, ref player.rotationVelocity,
                player.currentMovementSettings.directionChangeRadius);

            // rotate to face input direction relative to camera position
            player.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        // move the player
        player.Move(player.transform.forward * (player.currentSpeed * Time.deltaTime) + player.verticalVelocity * Vector3.up);
    }

    protected void AddSubscription(SensorID id, Action<Vector3> vec3Action)
    {
        IDisposable sub = player.TrySubscribe(id, vec3Action);
        subscriptions.Add(sub);
    }

    protected void AddSubscription(SensorID id, Action<bool> boolAction)
    {
        IDisposable sub = player.TrySubscribe(id, boolAction);
        subscriptions.Add(sub);
    }

    public void LookAround(Vector3 mouseMovement)
    {
        //Don't multiply mouse input by Time.deltaTime;
        float deltaTimeMultiplier = player.isCurrentDeviceMouse ? 1.0f : Time.deltaTime;

        player._cinemachineTargetYaw += mouseMovement.x * deltaTimeMultiplier * player.cameraRotationSpeed;
        player._cinemachineTargetPitch += mouseMovement.z * deltaTimeMultiplier * player.cameraRotationSpeed;

        // clamp our rotations so our values are limited 360 degrees
        player._cinemachineTargetYaw = ClampAngle(player._cinemachineTargetYaw, float.MinValue, float.MaxValue);
        player._cinemachineTargetPitch = ClampAngle(player._cinemachineTargetPitch, player.cameraClampBottom, player.cameraClampTop);

        // Cinemachine will follow this target
        player.cameraOrientation.transform.rotation = Quaternion.Euler(player._cinemachineTargetPitch, player._cinemachineTargetYaw, 0.0f);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    protected void Jump(bool jump)
    {
        if (jump && player.grounded)
        {
            Debug.Log("Jump!");
            //_player.verticalVelocity += 10.0f;
        }
    }

    protected virtual void UpdateGravity()
    {
        if (!player.grounded) player.verticalVelocity -= player.currentMovementSettings.gravity * Time.deltaTime;
    }
}