using R3;
using System;
using Unity.VisualScripting;
using UnityEngine;

public abstract class ReactiveBoolSensor : ReactiveSensor
{
    private Observable<bool> _observable = null;

    [SerializeField]
    protected float falseValue = 0;
    [SerializeField]
    protected float trueValue = 1;

    [Tooltip("When checked, the Observable will emit values every frame, not just when the value changes.")]
    public bool continuous = false;

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

#if UNITY_EDITOR
    // This is for debugging only, don't fall for the dark side!
    public bool lastValue = false;
#endif

    protected virtual Observable<bool> ConstructObservable()
    {
        if (continuous) return Observable.EveryUpdate().Select(n => { lastValue = Check(); return lastValue; });
        return Observable.EveryUpdate().Select(n => { lastValue = Check(); return lastValue; }).DistinctUntilChanged();
    }

    protected abstract bool Check();

    public override Observable<bool> ExposeBoolObservable() 
        => observable;

    public override Observable<float> ExposeFloatObservable() 
        => observable.Where(n => n).Select(n => n ? trueValue : falseValue);

    public override IDisposable SubscribeB(Action<bool> boolAction)
    {
        return observable.Subscribe(boolAction).AddTo(this);
    }

    public override IDisposable SubscribeF(Action<float> floatAction)
    {
        return observable.Select(n => n? trueValue : falseValue).Subscribe(floatAction).AddTo(this);
    }

    public override IDisposable SubscribeC(Action<Collider> colliderAction)
    {
        throw new IllegalSensorSubscriptionException("Collider", "bool");
    }
    public override IDisposable SubscribeV(Action<Vector3> vec3Action)
    {
        throw new IllegalSensorSubscriptionException("Vector3", "bool");
    }
    public override Observable<Vector3> ExposeVec3Observable()
    {
        throw new IllegalSensorSubscriptionException("Vector3", "bool");
    }
}