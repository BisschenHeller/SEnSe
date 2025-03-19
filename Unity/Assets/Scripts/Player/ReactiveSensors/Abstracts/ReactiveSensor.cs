using R3;
using System;
using UnityEngine;


public abstract class ReactiveSensor : MonoBehaviour { 
    protected Color inactiveGizmoColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
    protected Color activeGizmoColor = new Color(0.4f, 0.4f, 0.8f, 0.9f);

    public abstract SensorID GetSensorID();

    public abstract string ToInspectorString();

    /// <summary>
    /// Subscribes a function of type Action<bool> to the sensor. Will throw an exception if this is not allowed.
    /// </summary>
    /// <param name="boolAction">A Function with return type void that receives a bool as input</param>
    public abstract IDisposable SubscribeB(Action<bool> boolAction);


    /// <summary>
    /// Subscribes a function of type Action<Vector3> to the sensor. Will throw an exception if this is not allowed.
    /// </summary>
    /// <param name="vec3">A Function with return type void that receives a Vector3 as input</param>
    public abstract IDisposable SubscribeV(Action<Vector3> vec3Action);


    /// <summary>
    /// Subscribes a function of type Action<float> to the sensor. Will throw an exception if this is not allowed.
    /// </summary>
    /// <param name="floatAction">A Function with return type void that receives a float as input</param>
    public abstract IDisposable SubscribeF(Action<float> floatAction);


    /// <summary>
    /// Subscribes a function of type Action<Collider> to the sensor. Will throw an exception if this is not allowed.
    /// </summary>
    /// <param name="colliderAction">A Function with return type void that receives a bool as input</param>
    public abstract IDisposable SubscribeC(Action<Collider> colliderAction);

    public abstract Observable<bool> ExposeBoolObservable();

    public abstract Observable<Vector3> ExposeVec3Observable();

    public abstract Observable<float> ExposeFloatObservable();
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
    TouchingWater = 8,
    GroundedAndNotTouchingWater = 9,
    ParkourObject = 10,
    ParkourAndSprinting = 11,
    AnimationFinished = 12,
    Climbable = 13
}
