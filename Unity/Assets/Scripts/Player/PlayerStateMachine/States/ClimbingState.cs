using UnityEngine;

public class ClimbingState : PlayerState
{
    public override string GetStateName()
    {
        return "Climbing";
    }

    public ClimbingState(PlayerStateMachine context, PlayerStateFactory factory) : base(context, factory)
    {

    }

    public override void InitializeSubState()
    {
        throw new System.NotImplementedException();
    }

    protected override void EnterConcreteState()
    {
        int climbingHash = Animator.StringToHash("Climbing");
        player.CrossFadeAnimation(climbingHash);
        player.SetKinematic(true);
    }

    public bool canStop;

    public Vector3 climbingVelocity = Vector3.zero;

    public float maxClimbingSpeed = 1.5f;

    public override void HandleMoveInput(Vector3 premultipliedMovement)
    {
        Ray ray = new Ray(player.hipsPosition, player.transform.forward);
        LayerMask lm = LayerMask.NameToLayer("Climbable");
        if (Physics.Raycast(ray, out RaycastHit hit, 1.5f))
        {
            player.transform.Translate(ray.direction * (hit.distance - 0.35f) * 5 * Time.deltaTime, Space.World);

            player.transform.forward = Vector3.RotateTowards(player.transform.forward, -hit.normal, Time.deltaTime * 1, 0);

            Vector3 translatedToClimbing = new Vector3(premultipliedMovement.x, premultipliedMovement.z, 0);

            
            climbingVelocity = translatedToClimbing;
            

            if (climbingVelocity.magnitude > maxClimbingSpeed) climbingVelocity = climbingVelocity.normalized * maxClimbingSpeed;
            player.transform.localPosition += player.transform.rotation * climbingVelocity * Time.deltaTime;
        } else
        {
            int animHash = Animator.StringToHash("HangToTop");
            //player.CrossFadeAnimation(animHash);
            player.PlayAnimation(animHash);
            //SwitchState(_factory.GroundMovement());
        }
    }

    protected override void SetAnimationMotionSpeed(Vector3 input)
    {
        player.SetAnimatedMotionSpeed(climbingVelocity.y);
    }

    protected override void ExitConcreteState()
    {
        throw new System.NotImplementedException();
    }
}