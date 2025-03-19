using R3;
using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public abstract class ReactiveVec3Sensor : ReactiveSensor
{
    private Observable<Vector3> _observable = null;
    protected Observable<Vector3> observable
    {
        get
        {
            if (_observable == null)
            {
                _observable = ConstructObservable();
            }
            return _observable;
        }
    }
    protected abstract Observable<Vector3> ConstructObservable();

    

    public override IDisposable SubscribeV(Action<Vector3> vec3Action)
    {
        return observable.Subscribe(vec3Action).AddTo(this);
    }

    

    /// <summary>
    ///    Observes the magnitude of the Vector3 Sensor
    /// </summary>
    /// <returns></returns>
    public override Observable<float> ExposeFloatObservable()
    {
        return observable.Select(n => n.magnitude);
    }

    public override Observable<Vector3> ExposeVec3Observable() => observable;

    public override Observable<bool> ExposeBoolObservable()
    {
        throw new IllegalSensorExpositionException("bool", "Vector3");
    }
    public override IDisposable SubscribeB(Action<bool> boolAction)
    {
        throw new IllegalSensorSubscriptionException("bool", "Vector3");
    }
    public override IDisposable SubscribeF(Action<float> floatAction)
    {
        throw new IllegalSensorSubscriptionException("float", "Vector3");
    }
    public override IDisposable SubscribeC(Action<Collider> colliderAction)
    {
        throw new IllegalSensorSubscriptionException("Collider", "Vector3");
    }
}