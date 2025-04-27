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
        get { if (_boolObservable == null) _boolObservable = Observable.EveryUpdate().Select(n => { if (animFinished) { Debug.Log("Anim Finished!");  animFinished = false; return true; } return false; }).DistinctUntilChanged(); return _boolObservable; } 
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

    public override Observable<bool> ExposeBoolObservable()
    {
        return boolObservable;
    }
}