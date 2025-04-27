using System;
using UnityEngine;

public class ScriptedAnimationState : MovementBaseState 
{
    private AnimationID _animation;

    private Action<bool> _onComplete;

    private Vector3 _totalDisplacement;

    private Vector3 _initialPosition;

    public ScriptedAnimationState(SensorEnabledMovementStateMachine stateMachine, AnimationID animation, Vector3 totalDisplacement, Action<bool> onComplete) 
        : base(stateMachine, stateMachine._transitionSettings) 
    { 
        _animation = animation;
        _onComplete = onComplete;
        _totalDisplacement = totalDisplacement;
        _initialPosition = SEnSe.transform.position;
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
        SEnSe.SetKinematic(true);
        
        AddSubscription(SensorID.AnimationFinished, TransitionToGroundMovement);

        bool exists = SEnSe.animationHashes.TryGetValue(_animation, out int animationHash);
        if (!exists) Debug.LogError("Animation " + _animation.ToString() + " does not exist.");
        //Debug.Log("Playing animation " + _animation.ToString() + " (Hash=" + animationHash + ")");
        SEnSe.CrossFadeAnimation(animationHash, 0.5f); 
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
        float normalizedTime = SEnSe._animator.GetFloat("Progress");
        SEnSe.transform.position = _initialPosition + SEnSe.transform.rotation * _totalDisplacement * normalizedTime;
    }
}

public enum AnimationID
{
    UNASSIGNED = 0,
    Parcour_Low = 1,
    Parcour_High = 2,
    Parcour_Slide = 3,
    Parcour_StepUp = 4,
    Parcour_StepDown = 5,
    Parcour_WallClimb = 6,

    Climbing_TopOut = 100
}