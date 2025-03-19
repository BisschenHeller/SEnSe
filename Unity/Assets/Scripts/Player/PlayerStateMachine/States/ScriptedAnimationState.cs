using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ScriptedAnimationState : PlayerState 
{
    private AnimationID _animation;

    public ScriptedAnimationState(PlayerStateMachine stateMachine, PlayerStateFactory factory, AnimationID animation) : base(stateMachine, factory) { _animation = animation; }

    public override string GetStateName()
    {
        return "Parcour";
    }

    public override void InitializeSubState()
    {
        
    }

    private void TransitionToGroundMovement(bool doIt)
    {
        if (doIt) SwitchState(_factory.GroundMovement());
    }

    protected override void EnterConcreteState()
    {
        // Add Sensor that will tell us whe the animation is finished and subscribe to that sensor.
        //_player.AddSensor(SensorID.WaitOnAnimationFinished);
        Debug.Log("EnterCOncrete State Parcour!");

        player.SetKinematic(true);
        Debug.Log("SetKinematic()");
        AddSubscription(SensorID.AnimationFinished, TransitionToGroundMovement);
        Debug.Log("AddedSubscription()");

        bool exists = player.animationHashes.TryGetValue(_animation, out int animationHash);
        if (!exists) Debug.LogError("Animation " + _animation.ToString() + " does not exist.");
        Debug.Log("Playing animation " + _animation.ToString() + " (Hash=" + animationHash + ")");
        player.PlayAnimation(animationHash); 
    }

    protected override void ExitConcreteState()
    {
        player.SetKinematic(false);
    }

    public override void HandleMoveInput(Vector3 premultipliedMovement)
    {
        player.transform.position += player.transform.forward * Time.deltaTime * player.currentSpeed;
    }

    protected override void SetAnimationMotionSpeed(Vector3 premultipliedMovement)
    {
        player.SetAnimatedMotionSpeed(player.currentSpeed);
    }

    protected override void UpdateGravity()
    {
        // No Gravity
    }
}

public enum AnimationID
{
    UNASSIGNED = 0,
    Parcour_Low = 1,
    Parcour_High = 2,
    Parcour_Slide = 3,
    Parcour_StepUp = 4,
    Parcour_StepDown = 5,
    Parcour_WallClimb = 6,

    Climbing_TopOut = 100
}