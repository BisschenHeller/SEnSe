using Cinemachine;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{

    [Header("____________________ Camera ____________________")]
    [Space]
    [Range(0.0001f, 1.0f)]
    public float cameraRotationSpeed = 0.1f;
    public Transform cameraOrientation;
    public CinemachineVirtualCamera cinemachine;
    public float cameraClampBottom;
    public float cameraClampTop;
    public float _cinemachineTargetPitch;
    public float _cinemachineTargetYaw;

    [Header("____________________ MovementSettings ____________________")]
    [Space]
    public float terminalVelocity = 20.0f;
    [SerializeField]
    private MovementSettings _groundMovementSettings;
    [SerializeField]
    private MovementSettings _swimmingSettings;
    [HideInInspector]
    public Vector3 intendedDirection;

    private Dictionary<MovementSettingsID, MovementSettings> _movementSettingsDict = new Dictionary<MovementSettingsID, MovementSettings>();
    public MovementSettings currentMovementSettings;
    public Observable<MovementSettings> _movementSettingsObservable;

    public Vector3 velocity { get { return _characterController.velocity; } }
    public float currentSpeed = 0;
    public float rotationVelocity = 0;
    
    public float verticalVelocity = 0.0f;
    /*public float verticalVelocity
    {
        get { return _verticalVelocity; }
        set { _verticalVelocity = Mathf.Clamp(_verticalVelocity + value, -terminalVelocity, terminalVelocity); }
    }*/

    [SerializeField]
    public bool grounded = false;
    
    public bool isCurrentDeviceMouse { get { return playerInput.currentControlScheme == "Keyboard&Mouse"; } }

    [Header("____________________ GizmoSettings ____________________")]
    [Space]
    public bool drawSkinWidth = true;

    [Header("____________________ Animation ____________________")]
    [Space]
    // Expose the animator to the states
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    [Tooltip("Time to blend between animations BlendTrees in seconds")]
    private float blendTreeBlendTime;
    
    [Range(0.01f, 10.0f)]
    [Tooltip("Swiftness of Animation speed changes")]
    public float animationChangeSpeed = 0.5f;
    
    [Range(0.0f, 1.0f)]
    public float currentMotionSpeed;
    [Range(0.0f, 1.0f)]
    public float desiredMotionSpeed;
    public void PlayAnimation(int animationHash) { _animator.Play(animationHash); }
    public void CrossFadeAnimation(int animationHash, float fadeTime = 0.2f) { _animator.CrossFadeInFixedTime(animationHash, fadeTime); }
    public void SetAnimatedMotionSpeed(float value) { _animator.SetFloat(_motionSpeedHash, value); }
    private int _motionSpeedHash = -1;

    [Header("____________________ References ____________________")]
    [Space]

    public PlayerInput playerInput;
    // Expose the character controller to the states
    [SerializeField]
    private CharacterController _characterController;
    public void SetKinematic(bool collide) { /* Debug.Log("Collisions " + (collide? "disabled." : "enabled.")); */ _characterController.enabled = !collide; }
    public Vector3 center { get { return _characterController.center + transform.position;} }
    public float height { get { return _characterController.height; } }
    public void Move(Vector3 vec3) { _characterController.Move(vec3); }
    public Vector3 hipsPosition { get { return hips.position; } }
    [SerializeField]
    private Transform hips;
    

    public PlayerStateFactory _factory;
    public PlayerState _currentState;

    public Dictionary<SensorID, ReactiveSensor> _sensorsByKey = new Dictionary<SensorID, ReactiveSensor>();

    [Header("____________________ Reactive Sensors ____________________")]
    [Space]
    public List<string> sensorsFound;
    [HideInInspector]
    private SensorFactory sensorFactory;
    public void AddSensor(SensorID id)
    {
        _sensorsByKey.Add(id, sensorFactory.CreateSensor(id));
    }

    // Expose position and rotation to the states as read-only
    public Vector3 position { get { return transform.position; } }

    public Dictionary<AnimationID, int> animationHashes = new Dictionary<AnimationID, int>();

    private void Awake()
    {
        animationHashes.Add(AnimationID.Parcour_Low, Animator.StringToHash("ParcourVault"));
        animationHashes.Add(AnimationID.Parcour_High, Animator.StringToHash("ParcourVaultHigh"));
        animationHashes.Add(AnimationID.Parcour_WallClimb, Animator.StringToHash("ParcourRunUpWall"));
        animationHashes.Add(AnimationID.Parcour_Slide, Animator.StringToHash("ParcourSlide"));
        animationHashes.Add(AnimationID.Climbing_TopOut, Animator.StringToHash("HangToTop"));

        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        sensorFactory = GetComponent<SensorFactory>();

        _groundMovementSettings.animationBlendTreeIndex = Animator.StringToHash(_groundMovementSettings.animationBlendTreeName);
        _swimmingSettings.animationBlendTreeIndex = Animator.StringToHash(_swimmingSettings.animationBlendTreeName);

        _movementSettingsDict.Add(_groundMovementSettings.movementSettingsID, _groundMovementSettings);
        _movementSettingsDict.Add(_swimmingSettings.movementSettingsID, _swimmingSettings);

        _movementSettingsObservable = Observable.EveryUpdate().Select(n => currentMovementSettings).DistinctUntilChanged();

        _motionSpeedHash = Animator.StringToHash("MotionSpeed");

        List<ReactiveSensor> sensors = transform.GetComponentsInChildren<ReactiveSensor>().ToList();
        sensors.ForEach(n => {
            _sensorsByKey.Add(n.GetSensorID(), n);
            sensorsFound.Add(n.ToInspectorString());
        });
        Debug.Log("Sensors Found: " + sensors.Count);
    }

    private void Update()
    {
        _currentState.UpdateState();
    }

    private void Start()
    {
        _factory = new PlayerStateFactory(this);
        _currentState = _factory.GroundMovement();
        _currentState.EnterState();
    }

    public void SetMovementSettingsAndBlendTree(MovementSettingsID id)
    {

        if (!_movementSettingsDict.TryGetValue(id, out currentMovementSettings)) throw new Exception("No Movement Settings for ID " + id + "found");



        _animator.CrossFadeInFixedTime(currentMovementSettings.animationBlendTreeIndex, blendTreeBlendTime);
        
        //_animator.Play(currentMovementSettings.animationBlendTreeIndex);
    }

    public IDisposable TrySubscribe(SensorID sensorID, Action<bool> action)
    {
        _sensorsByKey.TryGetValue(sensorID, out ReactiveSensor sensorFound);
        if (sensorFound == null) throw new UnassignedReferenceException("bool Sensor " + sensorID.ToSafeString() + " could not be found in Children of PlayerStateMachine.");
        //Debug.Log("Subscribing to " + sensorFound.ToInspectorString());
        return sensorFound.SubscribeB(action);
    }

    public IDisposable TrySubscribe(SensorID sensorID, Action<Vector3> action)
    {

        _sensorsByKey.TryGetValue(sensorID, out ReactiveSensor sensorFound);
        if (sensorFound == null) throw new UnassignedReferenceException("Vector3 Sensor " + sensorID.ToSafeString() + " could not be found in Children of PlayerStateMachine.");
        //Debug.Log("Subscribing to " + sensorFound.ToInspectorString());
        return sensorFound.SubscribeV(action);
    }

    public IDisposable TrySubscribe(SensorID sensorID, Action<float> action)
    {
        _sensorsByKey.TryGetValue(sensorID, out ReactiveSensor sensorFound);
        if (sensorFound == null) throw new UnassignedReferenceException("Sensor " + sensorID.ToSafeString() + " could not be found in Children of PlayerStateMachine.");
        //Debug.Log("Subscribing to " + sensorFound.ToInspectorString());
        return sensorFound.SubscribeF(action);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.5f, 0.5f, 0.8f, 0.8f);
        
        Gizmos.DrawLine(transform.position + _characterController.center + transform.right * 0.2f, transform.position + _characterController.center + intendedDirection);
        Gizmos.DrawLine(transform.position + _characterController.center - transform.right * 0.2f, transform.position + _characterController.center + intendedDirection);
        Gizmos.DrawWireSphere(center + intendedDirection, 0.5f);

        if (drawSkinWidth) { 
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + _characterController.center + new Vector3(0, _characterController.height / 2 - _characterController.radius, 0), _characterController.radius + _characterController.skinWidth);
            Gizmos.DrawWireSphere(transform.position + _characterController.center - new Vector3(0, _characterController.height / 2 - _characterController.radius, 0), _characterController.radius + _characterController.skinWidth);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawRay(center, transform.forward);
        Gizmos.DrawSphere(center + 0.1f * transform.forward, 0.01f);
        Gizmos.DrawSphere(center + 0.2f * transform.forward, 0.01f);
        Gizmos.DrawSphere(center + 0.3f * transform.forward, 0.01f);
        Gizmos.DrawSphere(center + 0.4f * transform.forward, 0.01f);
        Gizmos.DrawSphere(center + 0.5f * transform.forward, 0.01f);
        Gizmos.DrawSphere(center + 0.6f * transform.forward, 0.01f);
    }

    public string ToDebugString()
    {
        return string.Format("{0}\n  Velocity: {1}\n  Speed: {2}m/s\n", _currentState.GetStateName(), velocity, Math.Round(velocity.magnitude, 2));
    }
}
[Serializable]
public class MovementSettings
{
    public MovementSettingsID movementSettingsID;
    public float regularSpeed;
    public float sprintSpeed;
    public float gravity;
    [Range(0.0f, 1.0f)]
    public float directionChangeRadius;
    public string animationBlendTreeName;
    [HideInInspector]
    public int animationBlendTreeIndex;
}

public enum MovementSettingsID
{
    GroundMovement, Swimming, Midair
}
