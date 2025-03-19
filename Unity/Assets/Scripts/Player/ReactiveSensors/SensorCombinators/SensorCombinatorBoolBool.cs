using R3;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public enum BinaryOperator
{
    AND, OR, XOR, AND_NOT
}

public class SensorCombinatorBoolBool : ReactiveSensor
{
    public SensorID sensorID;
    [Space]
    public ReactiveSensor bool1Sensor;
    public BinaryOperator operation;
    public ReactiveSensor bool2Sensor;

    [SerializeField]
    private float falseValue;
    [SerializeField]
    private float trueValue;

    private Observable<bool> _observable = null;
    protected Observable<bool> observable
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

    protected Observable<bool> ConstructObservable()
    {
        return bool1Sensor.ExposeBoolObservable().CombineLatest(bool2Sensor.ExposeBoolObservable(), (bool1, bool2) =>
        {
            switch (operation)
            {
                case BinaryOperator.AND: return bool1 && bool2;
                case BinaryOperator.OR: return bool1 || bool2;
                case BinaryOperator.XOR: return bool1 ^ bool2;
                case BinaryOperator.AND_NOT: return bool1 && !bool2;
                default: return false;
            }
        });
    }

    public override Observable<bool> ExposeBoolObservable() => observable;

    

    public override Observable<float> ExposeFloatObservable()
    {
        return observable.Select(n => n ? trueValue : falseValue);
    }

    public override SensorID GetSensorID() => sensorID;
    public override string ToInspectorString()
    {
        return string.Format("[C] <bool>{0} {1} <bool>{2}", bool1Sensor.ToInspectorString(), operation.ToString(), bool2Sensor.ToInspectorString());
    }
    public override IDisposable SubscribeB(Action<bool> boolAction) => observable.Subscribe(boolAction).AddTo(this);

    public override IDisposable SubscribeV(Action<Vector3> vec3Action)
    {
        throw new IllegalSensorSubscriptionException("Vector3", "bool-bool");
    }
    public override IDisposable SubscribeF(Action<float> floatAction)
    {
        throw new IllegalSensorSubscriptionException("float", "bool-bool");
    }
    public override IDisposable SubscribeC(Action<Collider> colliderAction) { 
        throw new IllegalSensorSubscriptionException("Collider", "bool-bool"); 
    }
    public override Observable<Vector3> ExposeVec3Observable() { 
        throw new IllegalSensorExpositionException("Vector3", "bool-bool"); 
    }
    
}