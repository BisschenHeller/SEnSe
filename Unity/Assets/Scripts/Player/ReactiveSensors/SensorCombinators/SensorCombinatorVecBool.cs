using System;
using UnityEngine;
using R3;
using System.ComponentModel;

public class SensorCombinatorVecBool : ReactiveSensor
{
    [Description("Emits the Vector if the boolean is true.")]

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

    public SensorID sensorID;
    public override SensorID GetSensorID() => sensorID;

    public ReactiveSensor vector3Sensor;

    public ReactiveSensor boolSensor;

    

#if UNITY_EDITOR
    [SerializeField]
    private Vector3 lastVec3;
    [SerializeField]
    private bool lastBool;
    [SerializeField]
    private Vector3 lastValue;
#endif

    protected Observable<Vector3> ConstructObservable()
    {
        return boolSensor.ExposeBoolObservable().Where(n => n).CombineLatest(vector3Sensor.ExposeVec3Observable(),
            (boolean, vec3) => { return vec3; });
    }

    public override string ToInspectorString()
    {
        return string.Format("(C)[<bool>{0} x <Vector3>{1}]", boolSensor.ToInspectorString(), vector3Sensor.ToInspectorString());
    }

    public override IDisposable SubscribeB(Action<bool> boolAction)
    {
        throw new IllegalSensorSubscriptionException("bool", "Vector3-bool");
    }

    public override IDisposable SubscribeV(Action<Vector3> vec3Action)
    {
        return observable.Subscribe(vec3Action).AddTo(this);
    }

    public override IDisposable SubscribeF(Action<float> floatAction)
    {
        throw new IllegalSensorSubscriptionException("float", "Vector3-bool");
    }

    public override IDisposable SubscribeC(Action<Collider> colliderAction)
    {
        throw new IllegalSensorSubscriptionException("Collider", "Vector3-bool");
    }

    public override Observable<bool> ExposeBoolObservable()
    {
        throw new IllegalSensorExpositionException("bool", "Vector3-bool");
    }

    public override Observable<Vector3> ExposeVec3Observable() => observable;

    public override Observable<float> ExposeFloatObservable()
    {
        throw new IllegalSensorExpositionException("float", "Vector3-bool");
    }
}