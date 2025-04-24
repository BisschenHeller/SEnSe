using R3;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class KeySensor : ReactiveBoolSensor
{
    [SerializeField]
    protected bool input = false;

    public SensorID _sensorID = SensorID.UNASSIGNED;

    public override Observable<bool> ExposeBoolObservable() => observable;

    protected override bool Check() {  return input; }

    public override SensorID GetSensorID() => _sensorID;

    public override string ToInspectorString() => _sensorID.ToString();

    protected override Observable<bool> ConstructObservable()
    {
        return Observable.EveryUpdate().Select(n => input).DistinctUntilChanged();
    }

    public abstract void OnAction(InputAction.CallbackContext context);
}