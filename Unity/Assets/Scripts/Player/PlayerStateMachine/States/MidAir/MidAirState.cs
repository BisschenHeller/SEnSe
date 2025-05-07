using UnityEngine;

public abstract class MidAirState : MovementBaseState
{
    public MidAirState(SensorEnabledMovementStateMachine context, MovementStateSettings settings) 
    : base(context, settings) {
        
    }

    protected override void EnterConcreteState()
    {
        //AddSubscription(SensorID.TouchingClimbable, TransitionToClimbing);
    }

    protected void TransitionToClimbing(bool climbing)
    {
        SwitchState(new ClimbingState(SEnSe));
    }

    protected override void UpdateGravity()
    {
        base.ApplyBasicGravity();
    }

    public override string GetStateName()
    {
        return "MidAir";
    }

    public override void HandleMoveInput(Vector3 desiredVelocity)
    {
        // move the player along current trajectory
        SEnSe.Move(SEnSe.transform.forward * (SEnSe.currentSpeed * Time.deltaTime) + SEnSe.verticalVelocity * Vector3.up);
    }
}