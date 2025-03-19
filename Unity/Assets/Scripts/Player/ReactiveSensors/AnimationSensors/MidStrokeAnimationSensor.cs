using R3;
using System;
using UnityEngine;

public class MidStrokeAnimationSensor : ReactiveSensor
{
    private bool midStroke = false;
    public void StartStroke()
    {
        midStroke = true;
    }

    public void EndStroke()
    {
        midStroke = false;
    }

    private Observable<bool> midStrokeObservable;
    public override Observable<bool> ExposeBoolObservable()
    {
        return midStrokeObservable;
    }

    public override Observable<float> ExposeFloatObservable()
    {
        throw new NotImplementedException();
    }

    public override Observable<Vector3> ExposeVec3Observable()
    {
        throw new NotImplementedException();
    }

    public override SensorID GetSensorID()
    {
        throw new NotImplementedException();
    }

    public override IDisposable SubscribeB(Action<bool> boolAction)
    {
        throw new NotImplementedException();
    }

    public override IDisposable SubscribeC(Action<Collider> colliderAction)
    {
        throw new NotImplementedException();
    }

    public override IDisposable SubscribeF(Action<float> floatAction)
    {
        throw new NotImplementedException();
    }

    public override IDisposable SubscribeV(Action<Vector3> vec3Action)
    {
        throw new NotImplementedException();
    }

    public override string ToInspectorString()
    {
        throw new NotImplementedException();
    }
}