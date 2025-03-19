using System;
using UnityEngine;

[Serializable]
public abstract class PlayerState_OLD
{
    protected PlayerStateMachine_OLD player;

    protected int _animIDGrounded;

    // State Machine, innit?
    public void InitState(PlayerStateMachine_OLD machine)
    {
        this.player = machine;
        Debug.Log("Entering State " + this.ToDebugString());
        _animIDGrounded = Animator.StringToHash("Grounded");
        OnEnter();
    }

    public void ExitState()
    {
        Debug.Log("Exiting State " + this.ToDebugString());
        OnExit();
    }
    
    public void Update()
    {
        UpdateGravity();
        UpdateCamera();
        UpdateArbitraryValues();
        UpdateGrounded();
    }
    
    public virtual void OnDrawGizmosSelected() {  }

    public virtual void OnDrawGizmos() {  }

    // Set variables once at the beginning of the state
    protected virtual void OnEnter() { Debug.Log("Empty OnExit."); return; }

    // Clear out variables or transforms that need to be reset before another state is entered
    protected virtual void OnExit() { Debug.Log("Empty OnExit."); return; }

    // smoothly position the camera where it needs to be, should be called
    protected abstract void UpdateCamera();

    // Make the character fall if necessary
    protected abstract void UpdateGravity();

    // update the check wheter or not the player is grounded
    protected abstract void UpdateGrounded();

    // Handle things that need to happen regardeless of input
    protected abstract void UpdateArbitraryValues();

    // Return String for Debug UI
    public abstract string ToDebugString();
}