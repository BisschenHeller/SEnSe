using R3;
using System;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor;
using UnityEngine;

public class HeightSensor : ReactiveSensor
{
    public SensorID sensorID;

    [SerializeField]
    [Tooltip("Should the raycast only extend downwards as far as the capsule probe?")]
    private bool capRaycast = false;

    [Tooltip("To which layers should the Sensor react?")]
    public LayerMask _layerMask;

    [Tooltip("How should the sensor react to trigger colliders?")]
    public QueryTriggerInteraction _triggerInteraction;

    [Range(0.01f, 2.0f)]
    [SerializeField]
    public float probeRadius;

    [Range(-2.0f, 2.0f)]
    [SerializeField]
    public float lowestProbeY = -0.5f;

    [Range(-2.0f, 2.0f)]
    [SerializeField]
    public float highestProbeY = 0.5f;

    private bool validBounds = false;

#if UNITY_EDITOR
    [SerializeField]
    [Range(0, 1)]
    private float gizmoAlpha = 0.7f;
    [SerializeField]
    private int gizmoSphereCount = 5;

    public Vector3 lastParkourPoint = Vector3.zero;
        #endif

    private void OnValidate()
    {
        if (highestProbeY <= lowestProbeY)
        {
            validBounds = false;
            Debug.LogWarning("Bounds on probes set invalid. Make sure lowestProbeY is lower than highestProbeY.");
        } else
        {
            validBounds = true;
        }
    }
#if UNITY_EDITOR
    public void OnDrawGizmosSelected()
    {
        if (!validBounds) return;
        
        Gizmos.color = new Color(0.4f, 0.4f, 0.8f, gizmoAlpha);
        float sphereSpacing = (highestProbeY - lowestProbeY - 2 * probeRadius) / (gizmoSphereCount - 1);
        for (int i = 0; i < gizmoSphereCount; i++)
        {
            Gizmos.DrawSphere(transform.position + new Vector3(0, lowestProbeY + i * sphereSpacing + probeRadius, 0), probeRadius);
        }
        Gizmos.color = new Color(0.8f, 0.8f, 0.4f, gizmoAlpha);
        Gizmos.DrawSphere(transform.position + new Vector3(0, lowestProbeY, 0), 0.02f);
        Gizmos.DrawSphere(transform.position + new Vector3(0, highestProbeY, 0), 0.02f);

        Gizmos.color = Color.red;
        if (lastParkourPoint != Vector3.zero) { 
            Gizmos.DrawCube(lastParkourPoint, new Vector3(0.3f, 0.02f, 0.3f));
            Gizmos.DrawRay(lastParkourPoint, Vector3.up * (transform.position.y + highestProbeY - lastParkourPoint.y));
        }
    }
#endif
    public override Observable<bool> ExposeBoolObservable()
    {
        return Observable
            .EveryUpdate()
            .Select(n =>
            {
                return Physics.CheckCapsule(
                    transform.position + new Vector3(0, lowestProbeY + probeRadius, 0),
                    transform.position + new Vector3(0, highestProbeY - probeRadius, 0),
                    probeRadius, _layerMask, _triggerInteraction);
            });                        
    }

    public override Observable<float> ExposeFloatObservable()
    {
        return ExposeBoolObservable().Where(x => x).Select<bool, float>((n) => {
            if (Physics.Raycast(transform.position + new Vector3(0, highestProbeY, 0) + transform.forward * 0.1f, Vector3.down, out RaycastHit hit, capRaycast? highestProbeY - lowestProbeY : 1000, _layerMask, _triggerInteraction))
            {
                
#if UNITY_EDITOR
                lastParkourPoint = hit.point;
#endif
                return hit.point.y;
            }
            else
            {
                
#if UNITY_EDITOR
                //lastParkourPoint = Vector3.zero;
#endif
                return Mathf.Infinity;
            }
        });
    }

    public override SensorID GetSensorID()
    {
        return sensorID;
    }

    public override string ToInspectorString()
    {
        return "ParcourSensor";
    }
}