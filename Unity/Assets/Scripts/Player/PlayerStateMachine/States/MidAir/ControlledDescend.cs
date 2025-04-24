public class ControlledDescend : MovementBaseState
{
    public ControlledDescend(SensorEnabledMovementStateMachine sense) : base(sense, sense._midAirSettings)
    {

    }

    public override string GetStateName()
    {
        return "Controlled Descend";
    }

    protected override void UpdateConcreteGravity()
    {
        base.UpdateGeneralGravity();
    }

    public override void InitializeSubState()
    {
        
    }

    protected override void EnterConcreteState()
    {
        // When in water transition to swim

        // When grounded transition to groundmovement

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