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

    public override Observable<Vector3> ExposeVec3Observable()
    {
        // TODO TEMPORARY; JUST FOR DEBUGGING 
        return observable.Select(n => { if (n) return Vector3.forward; else return Vector3.zero; });
    }

    public override Observable<float> ExposeFloatObservable()
    {
        if (continuous)
            return observable.DistinctUntilChanged().Select(n => { return n ? trueValue : falseValue; });
        else
            return observable.Select(n => { return n ? trueValue : falseValue; });
    }

    public override SensorID GetSensorID() => _sensorID;

    public override string ToInspectorString() => _sensorID.ToString();

    protected override Observable<bool> ConstructObservable()
    {
        return Observable.EveryUpdate().Select(n => input).DistinctUntilChanged();
    }

    public abstract void OnAction(InputAction.CallbackContext context);

    public override IDisposable SubscribeB(Action<bool> boolAction)
    {
        return observable.Subscribe(boolAction);
    }

    public override IDisposable SubscribeF(Action<float> floatAction)
    {
        if (continuous)
            return observable.DistinctUntilChanged().Select(n => { return n ? trueValue : falseValue; }).Subscribe(floatAction).AddTo(this);
        else
            return observable.Select(n => { return n ? trueValue : falseValue; }).Subscribe(floatAction).AddTo(this);
    }

    public override IDisposable SubscribeC(Action<Collider> colliderAction)
    {
        throw new IllegalSensorSubscriptionException("Collider", "Key");
    }

    public override IDisposable SubscribeV(Action<Vector3> vec3Action)
    {
        throw new IllegalSensorSubscriptionException("Vector3", "Key");
    }
}