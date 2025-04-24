using UnityEngine;

public class ControlledAscend : MovementBaseState
{
    private float _jumpPower = -1;
    public ControlledAscend(SensorEnabledMovementStateMachine fsm, float jumpPower) : base(fsm, fsm._midAirSettings)
    {
        _jumpPower = jumpPower;
    }

    public override string GetStateName()
    {
        return "Controlled Ascend";
    }

    public override void InitializeSubState()
    {
        throw new System.NotImplementedException();
    }

    private void ReactToVelocity(Vector3 velocity)
    {
        if (velocity.y <= 0) { }
    }

    protected override void EnterConcreteState()
    {
        SEnSe.verticalVelocity = _jumpPower;

        AddSubscription(SensorID.PlayerVelocity, ReactToVelocity);
    }

    public override void HandleMoveInput(Vector3 desiredVelocity)
    {
        // No Control over direction mid-flight
        return;

        // Optional: Designated fraction of control
        // 
        // base.HandleMoveInput(premultipliedMovement * 0.3f);
    }

    protected override void Jump(bool jumping)
    {
        // No Double Jump
        return;
    }

    protected override void ExitConcreteState()
    {
       // TODO
    }

    protected override void UpdateConcreteState()
    {
        // TODO
    }

    protected override void UpdateConcreteGravity()
    {
        // TODO
    }
}