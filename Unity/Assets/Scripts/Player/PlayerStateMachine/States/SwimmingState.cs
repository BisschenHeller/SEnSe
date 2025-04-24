using R3;
using UnityEngine;

public class SwimmingState : MovementBaseState
{
    private float surfaceLevel = -1.2f;

    private float waterHeight = 0.8f;
    public SwimmingState(SensorEnabledMovementStateMachine currentContext)
    : base(currentContext, currentContext._swimmingSettings) { }

    public override string GetStateName()
    {
        return "Swimming";
    }

    public void AdaptToWaterLevel(float waterLevel)
    {
        Debug.Log("New WaterLevel: " + waterLevel);
        surfaceLevel = waterLevel;
    }

    protected override void EnterConcreteState()
    {
        // If feet touch the ground and hip is no longer under water we can start walking again.
        AddManualSubscription(SEnSe.CombineBoolSensors(SensorID.Grounded, SensorID.InsideDeepWater, (a, b) => { return a && !b; }).Subscribe(GoOnLand));

        AddSubscription(SensorID.InsideDeepWater, AdaptToWaterLevel);

        SEnSe.SetMovementSettingsAndBlendTree(MovementSettingsID.Swimming);
    }

    private void GoOnLand(bool groundedAndOut)
    {
        if (groundedAndOut) { SwitchState(new GroundMovementState(SEnSe)); }
    }

    protected override void ExitConcreteState()
    {
        
    }

    public override void InitializeSubState()
    {
        
    }

    protected override void UpdateConcreteGravity()
    {
        /*if (SEnSe.grounded)
            SEnSe.transform.position = new Vector3(SEnSe.transform.position.x, Mathf.Max(SEnSe.transform.position.y, surfaceLevel - SEnSe.height / 2), SEnSe.transform.position.z);
        else
            SEnSe.transform.position = new Vector3(SEnSe.transform.position.x, surfaceLevel - SEnSe.height / 2, SEnSe.transform.position.z);*/
        //if (SEnSe.grounded) base.UpdateGeneralGravity();

        SEnSe.verticalVelocity = Mathf.SmoothStep(-0.01f, 0.01f, surfaceLevel - waterHeight - SEnSe.transform.position.y);

        
    }

    protected override void UpdateConcreteState()
    {
        // TODO
    }
}