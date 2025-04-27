using R3;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class TriggerSensor : KeySensor
{
    private bool triggered = false;

    public override void OnAction(InputAction.CallbackContext context)
    {
        if (context.started) triggered = true;
    }

    protected override Observable<bool> ConstructObservable()
    {
        return Observable.EveryUpdate().Select(n => { bool ret = triggered; triggered = false; return ret; } ).Where(x => x);
    }
}