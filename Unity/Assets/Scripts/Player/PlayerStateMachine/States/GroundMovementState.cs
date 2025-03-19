using JetBrains.Annotations;
using UnityEngine;

public class GroundMovementState : PlayerState
{
    public override string GetStateName()
    {
        return "GroundMovement";
    }
    public GroundMovementState(PlayerStateMachine currentContext, PlayerStateFactory factory)
    :base(currentContext, factory) 
    {
        
    }

    public void SwitchToWater(bool touchingWater)
    {
        if (touchingWater) { SwitchState(_factory.Swimming()); }
    }

    public void DoParcour(Vector3 obstacleHeight)
    {

        float relativeHeight = obstacleHeight.y - player.transform.position.y;

        AnimationID animation = AnimationID.Parcour_Low;
        if (relativeHeight > 1.0) animation = AnimationID.Parcour_High;
        if (relativeHeight < 0.2) animation = AnimationID.Parcour_Slide;
        
        if (obstacleHeight.y != 0 && player.currentSpeed > 6) {
            Debug.Log("Parcour! Animation: " + animation.ToString());
            SwitchState(_factory.ScriptedAnimation(animation)); 
        }
    }

    public void StartClimbing(bool touchingClimbable)
    {
        if (touchingClimbable) { SwitchState(_factory.Climbing()); }
    }

    protected override void EnterConcreteState()
    {
        AddSubscription(SensorID.Jump, Jump);

        AddSubscription(SensorID.TouchingWater, SwitchToWater);

        AddSubscription(SensorID.ParkourAndSprinting, DoParcour);

        AddSubscription(SensorID.Climbable, StartClimbing);

        player.SetMovementSettingsAndBlendTree(MovementSettingsID.GroundMovement);
    }

    protected override void ExitConcreteState()
    {
        
    }

    public override void InitializeSubState()
    {
        
    }
}