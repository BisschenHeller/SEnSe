using R3;
using System;
using UnityEngine;


public abstract class ReactiveSensor : MonoBehaviour { 
    protected Color inactiveGizmoColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
    protected Color activeGizmoColor = new Color(0.4f, 0.4f, 0.8f, 0.9f);

    /// <summary>Returns the unique ID of the sensor, usually a hard-coded value.</summary>
    public abstract SensorID GetSensorID();

    public abstract string ToInspectorString();
    // Used for debugging

    /// <summary>
    /// Exposes the underlying R3.Observable of Type bool if available for subscription handling
    /// </summary>
    public virtual Observable<bool> ExposeBoolObservable()
    {
        throw new IllegalSensorExpositionException("bool", ToInspectorString());
    }
    // Needs to be overwritten by derived sensors to work.

    /// <summary>
    /// Exposes the underlying R3.Observable of Type Vector3 if available for subscription handling
    /// </summary>
    public virtual Observable<Vector3> ExposeVector3Observable()
    {
        throw new IllegalSensorExpositionException("Vector3", ToInspectorString());
    }
    // Needs to be overwritten by derived sensors to work.

    /// <summary>
    /// Exposes the underlying R3.Observable of Type float if available for subscription handling
    /// </summary>
    public virtual Observable<float> ExposeFloatObservable()
    {
        throw new IllegalSensorExpositionException("Float", ToInspectorString());
    }
    // Needs to be overwritten by derived sensors to work.
}

public enum SensorID
{
    UNASSIGNED = -1,
    Grounded = 0,
    InProximity = 1,
    HorizontalInput = 2,
    Movement = 3,
    LookAround = 4,
    Crouching = 5,
    Jump = 6,
    Sprint = 7,
    InsideDeepWater = 8,
    ParkourObject = 10,
    AnimationFinished = 12,
    TouchingClimbable = 13,
    PlayerVelocity = 14,
    AnimationCurve_Stroke = 15
}
