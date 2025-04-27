using UnityEngine;

public class ControlledAscend : MovementBaseState
{
    private Vector3 _jumpTrajectory = Vector3.zero;
    public ControlledAscend(SensorEnabledMovementStateMachine fsm, Vector3 jumpTrajectory) : base(fsm, fsm._fallingSettings)
    {
        _jumpTrajectory = jumpTrajectory;
    }

    public override string GetStateName()
    {
        return "Controlled Ascend";
    }

    public override void InitializeSubState()
    {
        throw new System.NotImplementedException();
    }

    protected override void EnterConcreteState()
    {
        SEnSe.verticalVelocity = _jumpTrajectory.y;

        // TODO Calculate this hash beforehand
        SEnSe.PlayAnimation(Animator.StringToHash("Blend Tree Jump Start"));
    }

    public override void HandleMoveInput(Vector3 desiredVelocity)
    {
        base.HandleMoveInput(desiredVelocity * 0.1f);
    }

    protected override void HandleJumpInput(bool jumping)
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
        Debug.Log("Vert Velocity: " + SEnSe.verticalVelocity);
        if (SEnSe.verticalVelocity <= 0)
        {
            SwitchState(new ControlledDescend(SEnSe));
        }
    }

    protected override void UpdateGravity()
    {
        base.ApplyBasicGravity();
    }
}