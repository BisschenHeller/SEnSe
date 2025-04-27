using R3;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class ClimbingState : MovementBaseState
{
    public override string GetStateName()
    {
        return "Climbing";
    }

    public ClimbingState(SensorEnabledMovementStateMachine context) : base(context, context._climbingSettings)
    {

    }

    public override void InitializeSubState()
    {
        throw new System.NotImplementedException();
    }

    private void GoBackToGround(bool doIt)
    {
        SwitchState(new GroundMovementState(SEnSe));
    }

    private void ClimbOutOnTop(bool doIt)
    {
        // Start Transition Animation
        SwitchState(new ScriptedAnimationState(SEnSe, AnimationID.Climbing_TopOut, 
            // In future versions this needs to be handled in the animation import or through root motion
            new Vector3(0, 1.772f, -0.482f),
            // When that is done, go to ground movement
            (n) => SwitchState(new GroundMovementState(SEnSe))));
    }

    protected override void EnterConcreteState()
    {
        int climbingHash = Animator.StringToHash("Climbing");
        SEnSe.CrossFadeAnimation(climbingHash);
        SEnSe.SetKinematic(true);

        bool horizInputSensor = SEnSe._sensorsByKey.TryGetValue(SensorID.HorizontalInput, out ReactiveSensor horizontalInputSensor);

        if (SEnSe._sensorsByKey.TryGetValue(SensorID.Grounded, out ReactiveSensor groundedSensor) &&
            horizInputSensor)
        {
            // Go back down from the wall if ground is being touched and the player is pressing down keys.
            AddManualSubscription(horizontalInputSensor.ExposeVector3Observable()
                .Select<Vector3, bool>(n => { return n.z < -0.5f; } )
                .Where(x => x)
                .CombineLatest(groundedSensor.ExposeBoolObservable(), (inputDown, grounded) => { return grounded; })
                .Where(x => x)
                .Subscribe(GoBackToGround)
                );
        } else
        {
            Debug.LogError("Either Grounded- or HorizintalInput Sensor were not found.");
        }

        if (SEnSe._sensorsByKey.TryGetValue(SensorID.TouchingClimbable, out ReactiveSensor climbableSurfaceSensor) &&
            horizInputSensor)
        {
            AddManualSubscription(horizontalInputSensor.ExposeVector3Observable()
                .Select<Vector3, bool>(n => { return n.z > 0.0f; })
                .WithLatestFrom(climbableSurfaceSensor.ExposeBoolObservable(), (inputUp, climbable) => { /*Debug.Log("InputUp: " + inputUp + "  climbable: " + climbable); */ return inputUp && !climbable; })
                .Where(x => x)
                .Subscribe(ClimbOutOnTop)
                );
        }
    }

    public Vector3 climbingVelocity = Vector3.zero;

    public override void HandleMoveInput(Vector3 desiredVelocity)
    {
        Ray ray = new Ray(SEnSe.hipsPosition, SEnSe.transform.forward);
        LayerMask lm = LayerMask.NameToLayer("Climbable");
        if (Physics.Raycast(ray, out RaycastHit hit, 1.5f))
        {
            SEnSe.transform.Translate(ray.direction * (hit.distance - 0.35f) * 5 * Time.deltaTime, Space.World);

            SEnSe.transform.forward = Vector3.RotateTowards(SEnSe.transform.forward, -hit.normal, Time.deltaTime * 1, 0);

            Vector3 translatedToClimbing = new Vector3(desiredVelocity.x, desiredVelocity.z, 0);
            
            climbingVelocity = translatedToClimbing;
            

            //if (climbingVelocity.magnitude > _settings.regularSpeed) climbingVelocity = climbingVelocity.normalized * _settings.regularSpeed;
            SEnSe.transform.localPosition += SEnSe.transform.rotation * climbingVelocity * Time.deltaTime;
        } else
        {
            int animHash = Animator.StringToHash("HangToTop");
            //player.CrossFadeAnimation(animHash);
            SEnSe.PlayAnimation(animHash);
            //SwitchState(_factory.GroundMovement());
        }
    }

    protected override void SubscribeAnimationSpeed()
    {
        AddManualSubscription(Observable.EveryUpdate().Select(n => climbingVelocity).Subscribe(n => SEnSe.SetAnimationSpeed(climbingVelocity.y)));
    }

    protected override void ExitConcreteState()
    {
        SEnSe.SetKinematic(false);
    }

    protected override void UpdateConcreteState()
    {
        // TODO
    }

    protected override void UpdateGravity()
    {
        // No gravity
        return;
    }
}