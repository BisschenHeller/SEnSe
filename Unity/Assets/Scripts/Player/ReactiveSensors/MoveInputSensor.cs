using R3;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class MoveInputSensor : ReactiveVec3Sensor
{
    [SerializeField]
    private Vector2 moveInput = new Vector2(0,0);

    protected override Observable<Vector3> ConstructObservable()
    {
        return Observable.EveryUpdate().Select(n => { return new Vector3(moveInput.x, 0, moveInput.y); });
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if (context.phase == InputActionPhase.Canceled) moveInput = Vector2.zero;
    }

    public override string ToInspectorString()
    {
        return "HorizontalInputSensor";
    }

    public override SensorID GetSensorID() => SensorID.HorizontalInput;
}