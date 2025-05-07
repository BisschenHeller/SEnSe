using UnityEngine;

public class ControlledDescend : MidAirState
{
    public ControlledDescend(SensorEnabledMovementStateMachine sense) : base(sense, sense._midAirSettings)
    {

    }

    protected override void EnterConcreteState()
    {
        base.EnterConcreteState();

        AddSubscription(SensorID.Grounded, TransitionToGrounded);

        AddSubscription(SensorID.InsideDeepWater, TransitionToSwimming);
    }

    protected void TransitionToGrounded(bool grounded)
    {
        if (grounded) SwitchState(new GroundMovementState(SEnSe));
    }

    protected void TransitionToSwimming(bool swimming)
    {
        if (swimming) SwitchState(new SwimmingState(SEnSe));
    }

    public override string GetStateName()
    {
        return "Controlled Descend";
    }

    public override void InitializeSubState()
    {
        
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