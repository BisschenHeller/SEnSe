using DG.Tweening;
using R3;
using System;
using System.Linq;
using UnityEngine;
using static UnityEngine.Vector3;

public class TouchingSomethingCapsuleSensor : ReactiveSensor
{
    private Observable<bool> _observable = null;

#if UNITY_EDITOR
    // Debugging Only! Don't turn to the dark side!
    private bool lastValue = false;
#endif

    public SensorID sensorID = SensorID.UNASSIGNED;
    [Space]
    public float _touchingRadius = 0.1f;
    public Vector3 localCapsuleStart = Vector3.zero;
    public Vector3 localCapsuleEnd = Vector3.zero;


    public LayerMask _touchingLayer;

    public override SensorID GetSensorID() => sensorID;
    
    public override string ToInspectorString() => sensorID.ToString();

    public Observable<bool> observable
    {
        get { if (_observable == null) _observable = ConstructObservable(); return _observable; }
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.4f, 0.4f, 0.8f, 0.8f);

        for (float i = 0; i < 1.1; i+= 0.2f)
        {
            Gizmos.DrawSphere(transform.position + transform.rotation * Vector3.Lerp(localCapsuleStart, localCapsuleEnd, i), _touchingRadius);
        }

        
        Gizmos.DrawSphere(transform.position + transform.rotation * localCapsuleEnd, _touchingRadius);
    }

    protected virtual Observable<bool> ConstructObservable()
    {
        return Observable
            .EveryUpdate()
            .Select(n => { return CheckAtPosition(transform.position, _touchingRadius); });
    }

    protected bool CheckAtPosition(Vector3 pos, float radius)
    {
        Collider[] collisions = Physics.OverlapCapsule(transform.position + transform.rotation * localCapsuleStart, transform.position + transform.rotation * localCapsuleEnd, _touchingRadius, _touchingLayer, QueryTriggerInteraction.Collide);


        if (collisions.Length == 0)
        {
            return false;
        }

        CollidableElement elem = collisions[0].GetComponent<CollidableElement>();

        if (elem == null) return true;
        //Debug.Log(gameObject + ": And it has a CollidableElement");
        return elem.elligbleDirection(transform.forward);
    }

    public override Observable<bool> ExposeBoolObservable()
    {
        return observable;
    }
}
