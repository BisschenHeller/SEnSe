using UnityEngine;

public class ControlledDescend : MovementBaseState
{
    public ControlledDescend(SensorEnabledMovementStateMachine sense) : base(sense, sense._fallingSettings)
    {

    }

    public override string GetStateName()
    {
        return "Controlled Descend";
    }

    protected override void UpdateGravity()
    {
        base.ApplyBasicGravity();
    }

    public override void InitializeSubState()
    {
        
    }

    private void TransitionToGroundMovement(bool doIt)
    {
        if (doIt) SwitchState(new GroundMovementState(SEnSe));
    }

    protected override void EnterConcreteState()
    {
        // When in water transition to swim

        // When grounded transition to groundmovement
        AddSubscription(SensorID.Grounded, TransitionToGroundMovement);
        // When touching climbable transition to climbable
        
    }

    protected override void ExitConcreteState()
    {
        // TODO
    }

    protected override void UpdateConcreteState()
    {
        // TODO
    }
}