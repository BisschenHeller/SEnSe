using System;
using UnityEngine;

public class ParcourState : MovementBaseState 
{
    private AnimationID _animation;

    private Action<bool> _onComplete;

    public ParcourState(SensorEnabledMovementStateMachine stateMachine, AnimationID animation, Action<bool> onComplete) 
        : base(stateMachine, stateMachine._transitionSettings) 
    { 
        _animation = animation;
        _onComplete = onComplete;
    }

    public override string GetStateName()
    {
        return "Parcour";
    }

    public override void InitializeSubState()
    {
        
    }

    private void TransitionToGroundMovement(bool doIt)
    {
        if (doIt)
        {
            SwitchState(new GroundMovementState(SEnSe));
        }
    }

    protected override void EnterConcreteState()
    {
        // Add Sensor that will tell us whe the animation is finished and subscribe to that sensor.
        //_player.AddSensor(SensorID.WaitOnAnimationFinished);
        

        SEnSe.SetKinematic(true);
        
        AddSubscription(SensorID.AnimationFinished, TransitionToGroundMovement);

        bool exists = SEnSe.animationHashes.TryGetValue(_animation, out int animationHash);
        if (!exists) Debug.LogError("Animation " + _animation.ToString() + " does not exist.");
        //Debug.Log("Playing animation " + _animation.ToString() + " (Hash=" + animationHash + ")");
        SEnSe.PlayAnimation(animationHash); 
    }

    protected override void ExitConcreteState()
    {
        SEnSe.SetKinematic(false);
    }

    public override void HandleMoveInput(Vector3 desiredVelocity)
    {
        // Cannot interrupt these
        return;
    }

    protected override void UpdateGravity()
    {
        return;
        // No Gravity
    }

    protected override void UpdateConcreteState()
    {
        SEnSe.transform.position += SEnSe.transform.forward * Time.deltaTime * SEnSe.currentSpeed;
    }
}