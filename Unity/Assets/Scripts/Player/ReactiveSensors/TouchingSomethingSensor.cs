using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class TouchingSomethingSensor : ReactiveSensor
{
    private Observable<Tuple<bool, Vector3>> _observable = null;

    public Observable<Tuple<bool, Vector3>> observable
    {
        get { if (_observable == null) _observable = ConstructObservable(); return _observable; }
    }

    private Observable<Tuple<bool, Vector3>> ConstructObservable()
    {
        return Observable
            .EveryUpdate()
            .Select(n => { return Check(); });
    }

#if UNITY_EDITOR
    // Debugging Only! Don't turn to the dark side!
    private Tuple<bool, Vector3> lastValue = new Tuple<bool, Vector3>(false, Vector3.zero);
#endif

    public SensorID sensorID = SensorID.UNASSIGNED;
    [Space]
    public float _touchingRadius = 0.1f;

    public LayerMask _touchingLayer;

    public override SensorID GetSensorID() => sensorID;
    
    public override string ToInspectorString() => "[Touching] " + sensorID.ToString();

    protected Tuple<bool, Vector3> Check()
    {
        
        Vector3 spherePosition = new Vector3(transform.position.x,
                                             transform.position.y,
                                             transform.position.z);
        Collider[] collisions = Physics.OverlapSphere(spherePosition, _touchingRadius, _touchingLayer, QueryTriggerInteraction.Collide);

        if (collisions.Length == 0)
        {
            //Debug.Log(gameObject + ": I'm touching Nothing!");
            return new Tuple<bool, Vector3>(false, Vector3.zero);
        }
        
        //Debug.Log(gameObject + ": I'm touching " + string.Join(",", collisions.Select(n => n.gameObject.name).ToList()));

        CollidableElement elem = collisions[0].GetComponent<CollidableElement>();

        if (elem == null) return new Tuple<bool, Vector3>(true, Vector3.zero);
        //Debug.Log(gameObject + ": And it has a CollidableElement");
        return new Tuple<bool, Vector3>(elem.elligbleDirection(transform.forward), elem.GetWorldPointOfContact());
    }

    private void OnDrawGizmosSelected()
    {
        if (Application.IsPlaying(this))
        {
            Gizmos.color = lastValue.Item1 ? activeGizmoColor : inactiveGizmoColor;
        } else
        {
            Gizmos.color = inactiveGizmoColor;
        }
        
        Gizmos.DrawSphere(transform.position, _touchingRadius);
    }

    /// <summary>
    ///  Subscribes the vec3 action to the position of the collision if one occurs.
    /// </summary>
    /// <param name="vec3Action"></param>
    /// <returns></returns>
    public override IDisposable SubscribeV(Action<Vector3> vec3Action)
    {
        return observable.Where(n => n.Item1).Subscribe(n => vec3Action(n.Item2)).AddTo(this);
    }

    public override IDisposable SubscribeB(Action<bool> boolAction)
    {
        return observable.DistinctUntilChanged().Subscribe(n => boolAction(n.Item1)).AddTo(this);
    }

    public override Observable<Vector3> ExposeVec3Observable()
    {
        return observable.Where(n => n.Item1).Select(n => n.Item2);
    }

    public override Observable<bool> ExposeBoolObservable()
    {
        return observable.DistinctUntilChanged().Select(n => n.Item1);
    }

    public override IDisposable SubscribeF(Action<float> floatAction)
    {
        throw new IllegalSensorSubscriptionException("float", "Touch");
    }

    public override IDisposable SubscribeC(Action<Collider> colliderAction)
    {
        throw new IllegalSensorExpositionException("Collider", "Touch");
    }

    public override Observable<float> ExposeFloatObservable()
    {
        throw new IllegalSensorExpositionException("float", "Touch");
    }
}
