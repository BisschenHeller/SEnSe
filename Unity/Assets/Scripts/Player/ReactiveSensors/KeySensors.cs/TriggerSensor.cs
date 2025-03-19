using R3;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class TriggerSensor : KeySensor
{
    public override void OnAction(InputAction.CallbackContext context)
    {
        input = context.started;
    }
}