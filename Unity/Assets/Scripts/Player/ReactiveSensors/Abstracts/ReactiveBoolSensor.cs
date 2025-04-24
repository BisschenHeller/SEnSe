using R3;
using System;
using Unity.VisualScripting;
using UnityEngine;

public abstract class ReactiveBoolSensor : ReactiveSensor
{
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

#if UNITY_EDITOR
    // This is for debugging only, don't fall for the dark side!
    public bool lastValue = false;
#endif
    protected abstract Observable<bool> ConstructObservable();

    protected abstract bool Check();

    public override Observable<bool> ExposeBoolObservable() 
        => observable;
}