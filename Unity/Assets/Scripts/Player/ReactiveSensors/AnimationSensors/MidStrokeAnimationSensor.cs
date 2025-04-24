using R3;
using System;
using UnityEngine;

public class MidStrokeAnimationSensor : ReactiveSensor
{
    private Observable<bool> midStrokeObservable;

    public override SensorID GetSensorID()
    {
        return SensorID.AnimationCurve1;
    }

    public override string ToInspectorString()
    {
        return "MidStrokeAnimationSensor";
    }
}