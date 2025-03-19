using R3;
using System;
using System.ComponentModel;
using UnityEngine;

public class SensorCombinatorVecFloat : ReactiveSensor
{
    [Header("Emits the Vector Multiplied by the Float")]

    private Observable<Vector3> _observable;

    public SensorID sensorID;

    [SerializeField]
    private ReactiveSensor vec3Sensor;
    [SerializeField]
    private ReactiveSensor floatSensor;

    private Observable<Vector3> observable
    {
        get { _observable = ConstructObservable(); return _observable; }
    }

    private Observable<Vector3> ConstructObservable()
    {
        return vec3Sensor.ExposeVec3Observable().CombineLatest(floatSensor.ExposeFloatObservable(), (vec3S, floatS) =>
        {
            return vec3S * floatS;
        });
    }

    public override Observable<bool> ExposeBoolObservable()
    {
        throw new IllegalSensorExpositionException("bool", "Vector3-Float");
    }

    public override Observable<Vector3> ExposeVec3Observable()
    {
        return observable;
    }

    public override Observable<float> ExposeFloatObservable()
    {
        return observable.Select(n => n.magnitude);
    }

    public override SensorID GetSensorID() => sensorID;

    /// <summary>
    /// Subscribes an Action to the Vec3 Stream.
    /// </summary>
    public override IDisposable SubscribeV(Action<Vector3> vec3Action)
    {
        return observable.Subscribe(vec3Action).AddTo(this);
    }

    /// <summary>
    /// Subscribes an Action to the case that the magnitude of the combined vector exceeds zero.
    /// </summary>
    public override IDisposable SubscribeB(Action<bool> boolAction)
    {
        return observable.Select(n => n.magnitude != 0).Subscribe(boolAction).AddTo(this);
    }

    /// <summary>
    /// Subscribes an Action to the magnitude of the combined vector.
    /// </summary>
    public override IDisposable SubscribeF(Action<float> floatAction)
    {
        return observable.Select(n => n.magnitude).Subscribe(floatAction).AddTo(this);
    }

    public override IDisposable SubscribeC(Action<Collider> colliderAction)
    {
        throw new IllegalSensorSubscriptionException("Collider", "Vector3-float");
    }

    public override string ToInspectorString()
    {
        return string.Format("(C)[<Vector3>{0} x <float>{1}]", vec3Sensor.ToInspectorString(), floatSensor.ToInspectorString());
    }
}