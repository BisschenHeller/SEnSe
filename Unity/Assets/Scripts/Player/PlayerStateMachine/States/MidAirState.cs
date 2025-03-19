public class MidAirState : PlayerState
{
    public MidAirState(PlayerStateMachine context, PlayerStateFactory factory) 
    : base(context, factory) { }


    protected override void EnterConcreteState()
    {
        throw new System.NotImplementedException();
    }

    protected override void ExitConcreteState()
    {
        throw new System.NotImplementedException();
    }

    public override void InitializeSubState()
    {
        
    }

    protected override void UpdateGravity()
    {
        
    }
}