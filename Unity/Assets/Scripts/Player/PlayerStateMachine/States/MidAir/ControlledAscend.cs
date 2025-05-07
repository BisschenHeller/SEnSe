using UnityEngine;

public class ControlledAscend : MidAirState
{
    private Vector3 _jumpTrajectory = Vector3.zero;
    public ControlledAscend(SensorEnabledMovementStateMachine fsm, Vector3 jumpTrajectory) : base(fsm, fsm._jumpingSettings)
    {
        _jumpTrajectory = jumpTrajectory;
    }

    public override void InitializeSubState()
    {
        throw new System.NotImplementedException();
    }

    protected override void EnterConcreteState()
    {
        base.EnterConcreteState();

        SEnSe.verticalVelocity = _jumpTrajectory.y;

        // TODO Calculate this hash beforehand
        SEnSe.PlayAnimation(Animator.StringToHash("Blend Tree Jump Start"));
    }

    protected override void ExitConcreteState()
    {
       // TODO
    }

    protected override void UpdateConcreteState()
    {
        //Debug.Log("Vert Velocity: " + SEnSe.verticalVelocity);
        if (SEnSe.verticalVelocity <= 0)
        {
            SwitchState(new ControlledDescend(SEnSe));
        } else
        {
            //Debug.Log("Vertical Velocity: " + SEnSe.verticalVelocity);
        }
    }

    protected override void UpdateGravity()
    {
        base.ApplyBasicGravity();
    }
}