using R3;
using System;
using UnityEngine;

public class WaitForAnimationSensor : ReactiveSensor
{
    [SerializeField]
    private bool animFinished = false;

    [SerializeField]
    private bool canStop;

    private Observable<bool> _boolObservable = null;
    private Observable<bool> boolObservable {
        get { if (_boolObservable == null) _boolObservable = Observable.EveryUpdate().Select(n => { if (animFinished) { animFinished = false; return true; } return false; }).DistinctUntilChanged(); return _boolObservable; } 
    }

    public override SensorID GetSensorID()
    {
        return SensorID.AnimationFinished;
    }

    public void OnAnimationFinished()
    {
        animFinished = true;
    }

    public override string ToInspectorString()
    {
        return "WaitForAnimation";
    }

    public override IDisposable SubscribeB(Action<bool> boolAction)
    {   
        return boolObservable.Subscribe(boolAction);
    }

    public override Observable<Vector3> ExposeVec3Observable()
    {
        throw new IllegalSensorExpositionException("Vector3", "Animation");
    }

    public override IDisposable SubscribeV(Action<Vector3> vec3Action)
    {
        throw new IllegalSensorSubscriptionException("Vector3", "Animation");
    }

    public override Observable<bool> ExposeBoolObservable()
    {
        return boolObservable;
    }

    public override IDisposable SubscribeF(Action<float> floatAction)
    {
        throw new IllegalSensorSubscriptionException("float", "Animation");
    }

    public override IDisposable SubscribeC(Action<Collider> colliderAction)
    {
        throw new IllegalSensorSubscriptionException("Collider", "Animation");
    }

    public override Observable<float> ExposeFloatObservable()
    {
        throw new IllegalSensorExpositionException("float", "Animation");
    }
}