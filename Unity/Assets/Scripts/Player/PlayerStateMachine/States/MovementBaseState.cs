using System;
using System.Collections.Generic;
using R3;
using Unity.VisualScripting;
using UnityEngine;

public abstract class MovementBaseState
{
    protected SensorEnabledMovementStateMachine SEnSe;

    public MovementStateSettings _settings { get; protected set; }

    private List<IDisposable> activeSubscriptions = new List<IDisposable>();

    private MovementBaseState currentChildState;

    private MovementBaseState currentParentState;

    // We need this reference because we want to adjust how far away the parkour Sensor is going to be.
    // private ReactiveSensor parkourSensor;

    public abstract string GetStateName();

    public MovementBaseState(SensorEnabledMovementStateMachine context, MovementStateSettings settings)
    {
        SEnSe = context;
        _settings = settings;
    }

    public void EnterState()
    {
        SubscribeMoveInput();
        
        AddSubscription(SensorID.LookAround, LookAround);
        SubscribeAnimationSpeed();
        AddSubscription(SensorID.Grounded, SetGrounded);

        AddSubscription(SensorID.Jump, HandleJumpInput);

        EnterConcreteState();
    }

    public void ExitState()
    {
        activeSubscriptions.ForEach(n => n.Dispose());
        activeSubscriptions.Clear();
        ExitConcreteState();
    }

    

    public abstract void InitializeSubState();

    protected void SwitchState(MovementBaseState newState)
    {
        Debug.Log("Transition: " + GetStateName() + "  -->  " + newState.GetStateName());
        ExitState();

        newState.EnterState();

        SEnSe._currentState = newState;
    }

    protected void SetSuperState()
    {

    }

    protected void SetSubState()
    {

    }

    protected virtual void SubscribeAnimationSpeed()
    {
        AddSubscription(SensorID.PlayerVelocity, SetAnimationSpeedFromHorizVelocity);
    }

    protected virtual void SubscribeMoveInput()
    {
        if (SEnSe._sensorsByKey.TryGetValue(SensorID.HorizontalInput, out ReactiveSensor horizInputSensor) &&
            SEnSe._sensorsByKey.TryGetValue(SensorID.Sprint, out ReactiveSensor sprintSensor))
        {
            AddManualSubscription(horizInputSensor.ExposeVector3Observable().CombineLatest<Vector3, bool, Vector3>(sprintSensor.ExposeBoolObservable(),
                (horiz, sprint) => { return (horiz * (sprint ? _settings.sprintSpeed : _settings.regularSpeed)); }).Subscribe<Vector3>(HandleMoveInput));
        }
    }

    protected void SetAnimationSpeedFromHorizVelocity(Vector3 velocity)
    {
        float normally = new Vector3(velocity.x, 0, velocity.z).magnitude / _settings.sprintSpeed;
        if (Vector3.Angle(velocity, SEnSe.transform.forward) > 100) normally *= -1;
        SEnSe.SetAnimationSpeed(normally);
    }

    protected void SetGrounded(bool newGrounded)
    {
        //Debug.Log("SetGrounded(" + newGrounded + ")");
        SEnSe.grounded = newGrounded;
        /*if (!SEnSe.grounded)
        {
            SwitchState(new ControlledDescend(SEnSe));
        }*/
    }

    public virtual void HandleMoveInput(Vector3 desiredVelocity)
    {
        float targetSpeed = desiredVelocity.magnitude;

        if (desiredVelocity.z < 0) targetSpeed *= -1;

        if (desiredVelocity.magnitude == 0) targetSpeed = 0;
        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        if (Mathf.Abs(SEnSe.currentSpeed - targetSpeed) < 0.1)
        {
            // Do Nothing
        }
        else if (SEnSe.currentSpeed > targetSpeed)
        {
            SEnSe.currentSpeed -= Time.deltaTime * 100 * _settings.drag;
        } else
        {
            SEnSe.currentSpeed += Time.deltaTime * 100 * _settings.accelleration;
        }

        // normalise input direction
        Vector3 inputDirection = new Vector3(desiredVelocity.x, 0.0f, desiredVelocity.y).normalized;

        
        // if there is a move input rotate player when the player is moving
        if (desiredVelocity != Vector3.zero)
        {
            float targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                              SEnSe.cameraOrientation.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(SEnSe.transform.eulerAngles.y, targetRotation, ref SEnSe.rotationVelocity,
                _settings.directionChangeRadius);

            // rotate to face input direction relative to camera position
            SEnSe.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        // move the player
        SEnSe.Move(SEnSe.transform.forward * (SEnSe.currentSpeed * Time.deltaTime) + SEnSe.verticalVelocity * Vector3.up);
    }

    /// <summary>
    /// Use this method if you want to manually handle deboucing, zipping or similar.
    /// </summary>
    /// <param name="subscription">The IDisposable used to end the subscription on State exit</param>
    protected void AddManualSubscription(IDisposable subscription)
    {
        activeSubscriptions.Add(subscription);
    }

    /// <summary>
    /// Use this method if you want to subscribe an action to a sensor
    /// </summary>
    /// <param name="id">Which sensor are you looking to subscribe to</param>
    /// <param name="action">The method you want to subscribe</param>
    protected void AddSubscription(SensorID id, Action<bool> action) 
    {
        activeSubscriptions.Add(SEnSe.TrySubscribe(id, action));
    }

    /// <summary>
    /// Use this method if you want to subscribe an action to a sensor
    /// </summary>
    /// <param name="id">Which sensor are you looking to subscribe to</param>
    /// <param name="action">The method you want to subscribe</param>
    protected void AddSubscription(SensorID id, Action<Vector3> action)
    {
        activeSubscriptions.Add(SEnSe.TrySubscribe(id, action));
    }

    /// <summary>
    /// Use this method if you want to subscribe an action to a sensor
    /// </summary>
    /// <param name="id">Which sensor are you looking to subscribe to</param>
    /// <param name="action">The method you want to subscribe</param>
    protected void AddSubscription(SensorID id, Action<float> action)
    {
        activeSubscriptions.Add(SEnSe.TrySubscribe(id, action));
    }

    public void LookAround(Vector3 mouseMovement)
    {
        //Don't multiply mouse input by Time.deltaTime;
        float deltaTimeMultiplier = SEnSe.isCurrentDeviceMouse ? 1.0f : Time.deltaTime;

        SEnSe._cinemachineTargetYaw += mouseMovement.x * deltaTimeMultiplier * SEnSe.cameraRotationSpeed;
        SEnSe._cinemachineTargetPitch += mouseMovement.z * deltaTimeMultiplier * SEnSe.cameraRotationSpeed;

        // clamp our rotations so our values are limited 360 degrees
        SEnSe._cinemachineTargetYaw = ClampAngle(SEnSe._cinemachineTargetYaw, float.MinValue, float.MaxValue);
        SEnSe._cinemachineTargetPitch = ClampAngle(SEnSe._cinemachineTargetPitch, _settings.pitchClampBottom, _settings.pitchClampTop);

        // Cinemachine will follow this target
        SEnSe.cameraOrientation.transform.rotation = Quaternion.Euler(SEnSe._cinemachineTargetPitch, SEnSe._cinemachineTargetYaw, 0.0f);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    protected virtual void HandleJumpInput(bool jumping)
    {
        Debug.Log("HandleJumpInput()");
        if (SEnSe.grounded)
        {
            Debug.Log("Jump!");
            SwitchState(new ControlledAscend(SEnSe, new Vector3(0, _settings.jumpPower, 0)));
        }
    }

    protected virtual void ApplyBasicGravity()
    {
        if (!SEnSe.grounded) SEnSe.verticalVelocity += SEnSe.gravity * Time.deltaTime * _settings.gravityMultiplier;
    }

    public void UpdateState()
    {
        UpdateGravity();
        UpdateConcreteState();
    }

    protected abstract void EnterConcreteState();

    protected abstract void ExitConcreteState();

    protected abstract void UpdateConcreteState();

    protected abstract void UpdateGravity();
}