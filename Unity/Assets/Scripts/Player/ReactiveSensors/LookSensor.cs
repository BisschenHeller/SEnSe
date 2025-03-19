using R3;
using UnityEngine;
using UnityEngine.InputSystem;

public class LookSensor : ReactiveVec3Sensor
{
    [SerializeField]
    private Vector2 lookInput;

    public override string ToInspectorString() => "LookAroundSensor";

    protected override Observable<Vector3> ConstructObservable()
    {
        return Observable.EveryUpdate().Select(n => new Vector3(lookInput.x, 0, -lookInput.y));
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public override SensorID GetSensorID() => SensorID.LookAround;
}