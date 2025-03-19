using UnityEngine;

public class SwimmingState : PlayerState
{
    private float surfaceLevel = -1.2f;
    public SwimmingState(PlayerStateMachine currentContext, PlayerStateFactory factory)
    : base(currentContext, factory) { }

    public override string GetStateName()
    {
        return "Swimming";
    }

    protected override void EnterConcreteState()
    {
        Debug.Log("Entering Swimming State!");
        AddSubscription(SensorID.GroundedAndNotTouchingWater, GoOnLand);

        player.SetMovementSettingsAndBlendTree(MovementSettingsID.Swimming);
    }

    private void GoOnLand(bool groundedAndOut)
    {
        if (groundedAndOut) { SwitchState(_factory.GroundMovement()); }
    }

    protected override void ExitConcreteState()
    {
        
    }

    public override void InitializeSubState()
    {
        
    }

    protected override void UpdateGravity()
    {
        if (player.grounded)
            player.transform.position = new Vector3(player.transform.position.x, Mathf.Max(player.transform.position.y, surfaceLevel - player.height / 2), player.transform.position.z);
        else
            player.transform.position = new Vector3(player.transform.position.x, surfaceLevel - player.height / 2, player.transform.position.z);

        player.verticalVelocity = 0;
    }
}