using R3;
using UnityEngine;

public class GroundMovementState : MovementBaseState
{
    public override string GetStateName()
    {
        return "GroundMovement";
    }
    public GroundMovementState(SensorEnabledMovementStateMachine currentContext)
    :base(currentContext, currentContext._groundMovementSettings) 
    {
        
    }

    protected override void EnterConcreteState()
    {

        AddSubscription(SensorID.InsideDeepWater, SwitchToWater);

        AddParcourSubscription();

        AddClimbableSubscription();

        SEnSe.SetMovementSettingsAndBlendTree(MovementSettingsID.GroundMovement);
    }

    public void SwitchToWater(bool touchingWater)
    {
        if (touchingWater) { SwitchState(new SwimmingState(SEnSe)); }
    }

    public void DoParcour(float obstacleHeight)
    {
        if (obstacleHeight > 2.5f)
        {
            Debug.LogWarning("ParcourSensor fired but obstacle is too high");
            return;
        }
        AnimationID animation = AnimationID.Parcour_Low;

        if (obstacleHeight > 0.95) animation = AnimationID.Parcour_High;
        if (obstacleHeight < 0.2) animation = AnimationID.Parcour_Slide;
        
        Debug.Log("Parcour! ObstacleHeight: " + obstacleHeight + "  Animation: " + animation.ToString());
        SwitchState(new ParcourState(SEnSe, animation, (n) => { Debug.Log("I really should be switching back now!"); SwitchState(new GroundMovementState(SEnSe)); } ));
    }

    public void StartClimbing(bool touchingClimbable)
    {
        if (touchingClimbable) { SwitchState(new ClimbingState(SEnSe)); }
    }

    private void AddParcourSubscription()
    {
        // If we steer at a parkour object while sprinting, we want to trigger DoParcour(float height) with the 
        // height of the encountered object.
        if (SEnSe._sensorsByKey.TryGetValue(SensorID.Sprint, out ReactiveSensor sprintSensor) &&
            SEnSe._sensorsByKey.TryGetValue(SensorID.ParkourObject, out ReactiveSensor parkourSensor) &&
            SEnSe._sensorsByKey.TryGetValue(SensorID.PlayerVelocity, out ReactiveSensor playerVelocitySensor)
            )
        {

            Observable<bool> speedOkayStream = sprintSensor.ExposeBoolObservable()
                .CombineLatest(playerVelocitySensor.ExposeVector3Observable(),
                    (sprinting, velocity) => sprinting && new Vector3(velocity.x, 0, velocity.z).magnitude > _settings.sprintSpeed - 1)
                .Where(x => x);

            Observable<float> validParcourMove = parkourSensor.ExposeFloatObservable()
                .Select(worldObstacleHeight => worldObstacleHeight - SEnSe.transform.position.y);

            AddManualSubscription(speedOkayStream.WithLatestFrom(validParcourMove, (validSpeed, height) => height).Subscribe(DoParcour));

        }
        else
        {
            Debug.LogError("Either sprint-, velocity- or parcour Sensor could not be found.");
        }
    }

    private void AddClimbableSubscription()
    {
        if (SEnSe._sensorsByKey.TryGetValue(SensorID.HorizontalInput, out ReactiveSensor horizInput) &&
            SEnSe._sensorsByKey.TryGetValue(SensorID.TouchingClimbable, out ReactiveSensor climbableSensor))
        {
            AddManualSubscription(horizInput.ExposeVector3Observable()
                .Select(n => { return n.z > 0.5f; })
                .CombineLatest(climbableSensor.ExposeBoolObservable(),
                (input, climbable) => { return input && climbable; })
                .Where(x => x)
                .Subscribe(StartClimbing));
        }
    }

    protected override void HandleJumpInput(bool jumping)
    {
        base.HandleJumpInput(jumping);
    }

    

    protected override void ExitConcreteState()
    {
        // TODO
    }

    public override void InitializeSubState()
    {
        // TODO
    }

    protected override void UpdateConcreteState()
    {
        // TODO
    }

    protected override void UpdateGravity()
    {
        base.ApplyBasicGravity();
    }
}