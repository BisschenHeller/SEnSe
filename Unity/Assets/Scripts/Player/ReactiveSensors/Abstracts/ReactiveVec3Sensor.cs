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

    /// <summary>
    ///    Observes the magnitude of the Vector3 Sensor
    /// </summary>
    /// <returns></returns>
    public override Observable<float> ExposeFloatObservable()
    {
        return observable.Select(n => n.magnitude);
    }

    public override Observable<Vector3> ExposeVector3Observable() => observable;

    public override Observable<bool> ExposeBoolObservable()
    {
        throw new IllegalSensorExpositionException("bool", "Vector3");
    }
}