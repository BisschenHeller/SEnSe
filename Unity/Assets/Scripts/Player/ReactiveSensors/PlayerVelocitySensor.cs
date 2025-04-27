using R3;
using UnityEngine;

[RequireComponent(typeof(SensorEnabledMovementStateMachine))]
public class PlayerVelocitySensor : ReactiveVec3Sensor
{
    private SensorEnabledMovementStateMachine sensa;

    private void Awake()
    {
        sensa = GetComponent<SensorEnabledMovementStateMachine>();
    }

    public override SensorID GetSensorID()
    {
        return SensorID.PlayerVelocity;
    }

    public override string ToInspectorString()
    {
        return "PlayerVelocitySensor";
    }

    protected override Observable<Vector3> ConstructObservable()
    {
        return Observable.EveryUpdate().Select((n) => sensa.velocity).DistinctUntilChanged();
    }
}