using R3;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class HoldingSensor : KeySensor
{
    // The observable will hold true as long as the button is pressed.
    public override void OnAction(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            input = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            input = false;
        }
    }
}