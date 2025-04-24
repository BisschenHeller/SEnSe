public class MidAirState : MovementBaseState
{
    public MidAirState(SensorEnabledMovementStateMachine context, bool jump = false) 
    : base(context, context._midAirSettings) {
        if (jump)
        {

        }
    }


    protected override void EnterConcreteState()
    {
        // TODO
    }

    protected override void ExitConcreteState()
    {
        // TODO
    }

    public override void InitializeSubState()
    {
        // TODO
    }

    protected override void UpdateConcreteGravity()
    {
        base.UpdateGeneralGravity();
    }

    public override string GetStateName()
    {
        return "MidAir";
    }

    protected override void UpdateConcreteState()
    {
        throw new System.NotImplementedException();
    }
}