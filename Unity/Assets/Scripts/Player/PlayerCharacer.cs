using R3;
using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerCharacer : MonoBehaviour
{
    public PlayerStateMachine_OLD playerStateMachine;

    public Vector3 Velocity = Vector3.zero;

    public float accelleration = 1;
    public float maxSpeedXZ = 5;

    IDisposable clickSubscription;
    IDisposable movementSubscription;

    private CharacterController cc;

    // Start is called before the first frame update
    void Start()
    {
        clickSubscription = Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButtonDown(0))
            .TimeInterval()
            .Chunk(2, 1)
            .Where(clicks => clicks[1].Interval.TotalSeconds <= 2)
            .ThrottleFirst(TimeSpan.FromSeconds(2))
            .Subscribe(_ =>
            {
                Debug.Log("Two clicks within 2 seconds detected!");
            });

        movementSubscription = Observable.EveryUpdate()
            .Where(_ => Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            .Subscribe(_ =>
            {
                Move(Time.deltaTime * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
            });

        cc = GetComponent<CharacterController>();   
    }

    private void Move(Vector3 movementInput)
    {
     
    }

    void OnDestroy()
    {
        clickSubscription.Dispose();
        movementSubscription.Dispose();
    }
}

public class PlayerInventory
{
    public bool overencumbered;
}
