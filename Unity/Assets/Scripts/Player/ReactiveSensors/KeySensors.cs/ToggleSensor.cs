using R3;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class ToggleSensor : KeySensor
{
    // The Observable will hold true while toggled on.
    public override void OnAction(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            input = !input;
        }
    }
}