using R3;
using System;
using UnityEngine;

/// <summary>
/// This sensor is not finished. I wanted to make a sensor that can listen to animation curves on the player
/// and inform the swimming animation when the player animation is actually displacing water.
/// </summary>
public class MidStrokeAnimationSensor : ReactiveSensor
{
    [SerializeField]
    private Animator anim;

    public SensorID sensorID = SensorID.AnimationCurve_Stroke;

    public string animationCurveName;
    private int animationCurveHash;

    [SerializeField]
    private float minValue = 0.2f;

    private void OnValidate()
    {
        animationCurveHash = Animator.StringToHash(animationCurveName);
    }

    private void Awake()
    {
        animationCurveHash = Animator.StringToHash(animationCurveName);
    }

    public override SensorID GetSensorID()
    {
        return sensorID;
    }

    public override string ToInspectorString()
    {
        return "MidStrokeAnimationSensor";
    }

    public override Observable<float> ExposeFloatObservable()
    {
        return Observable.EveryUpdate().Select(n => { return (1-minValue) * anim.GetFloat(animationCurveHash) + minValue; });
    }
}