public class PlayerStateFactory
{
    PlayerStateMachine _context;

    public PlayerStateFactory(PlayerStateMachine currentContext)
    {
        _context = currentContext;
    }

    public PlayerState GroundMovement()
    {
        return new GroundMovementState(_context, this);
    }

    public PlayerState Swimming()
    {
        return new SwimmingState(_context, this);
    }

    public PlayerState ScriptedAnimation(AnimationID animationID)
    {
        return new ScriptedAnimationState(_context, this, animationID);
    }

    public PlayerState Climbing()
    {
        return new ClimbingState(_context, this);
    }
}