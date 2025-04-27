public class MidAirState : MovementBaseState
{
    public MidAirState(SensorEnabledMovementStateMachine context) 
    : base(context, context._fallingSettings) {
        
    }

    protected override void EnterConcreteState()
    {
        AddSubscription(SensorID.Grounded, TransitionToGrounded);

        AddSubscription(SensorID.InsideDeepWater, TransitionToSwimming);

        AddSubscription(SensorID.TouchingClimbable, TransitionToClimbing);
    }

    private void TransitionToClimbing(bool climbing)
    {
        SwitchState(new ClimbingState(SEnSe));
    }

    private void TransitionToGrounded(bool grounded)
    {
        SwitchState(new GroundMovementState(SEnSe));
    }

    private void TransitionToSwimming(bool swimming)
    {
        SwitchState(new SwimmingState(SEnSe));
    }

    protected override void ExitConcreteState()
    {
        // TODO
    }

    public override void InitializeSubState()
    {
        // TODO
    }

    protected override void UpdateGravity()
    {
        base.ApplyBasicGravity();
    }

    public override string GetStateName()
    {
        return "MidAir";
    }

    protected override void UpdateConcreteState()
    {
        
    }
}