using Cinemachine;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class SensorEnabledMovementStateMachine : MonoBehaviour
{

    [Header("____________________ Camera ____________________")]
    [Space]
    [Range(0.0001f, 1.0f)]
    public float cameraRotationSpeed = 0.1f;
    public Transform cameraOrientation;
    public CinemachineVirtualCamera cinemachine;
    public float _cinemachineTargetPitch;
    public float _cinemachineTargetYaw;

    [Header("____________________ MovementSettings ____________________")]
    
    
    [SerializeField]
    public float gravity = -9.81f;
    [Space]
    public float terminalVelocity = 20.0f;
    
    public MovementStateSettings _groundMovementSettings;
    
    public MovementStateSettings _swimmingSettings;
    
    public MovementStateSettings _climbingSettings;
    
    public MovementStateSettings _ladderSettings;

    public MovementStateSettings _midAirSettings;

    public MovementStateSettings _transitionSettings;

    [HideInInspector]
    public Vector3 intendedDirection;

    private Dictionary<MovementSettingsID, MovementStateSettings> _movementSettingsDict = new Dictionary<MovementSettingsID, MovementStateSettings>();
    
    public Vector3 velocity { get { return _characterController.velocity; } }
    public float currentSpeed = 0;
    public float rotationVelocity = 0;
    
    public float verticalVelocity = 0.0f;

    [SerializeField]
    public bool grounded = false;
    
    public bool isCurrentDeviceMouse { get { return playerInput.currentControlScheme == "Keyboard&Mouse"; } }

    [Header("____________________ GizmoSettings ____________________")]
    [Space]
    public bool drawSkinWidth = true;

    public MovementSettingsID drawGizmosFor;
    [Range(4, 360)]
    public int gizmoCircleResolution = 180;
    [Range(0.5f, 3f)]
    public float gizmoCircleRadius = 2;

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
    public void SetAnimationSpeed(float value) { _animator.SetFloat(_motionSpeedHash, value); }
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
    
    public MovementBaseState _currentState;

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

    private MovementStateSettings GetMovementStateSettings(MovementSettingsID id)
    {
        switch (id)
        {
            case MovementSettingsID.GroundMovement:
                return _groundMovementSettings;
            case MovementSettingsID.Swimming:
                return _swimmingSettings;
            case MovementSettingsID.Midair:
                return _midAirSettings;
            case MovementSettingsID.Climbing:
                return _climbingSettings;
            case MovementSettingsID.Ladder:
                return _ladderSettings;
            default:
                return _transitionSettings;
        }
    }

    public void PlaceParkourSensor(float metersInFront)
    {

    }

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
        _currentState = new GroundMovementState(this);

        _currentState.EnterState();
    }

    public void SetMovementSettingsAndBlendTree(MovementSettingsID id)
    {
        if (!_movementSettingsDict.TryGetValue(id, out MovementStateSettings mS)) throw new Exception("No Movement Settings for ID " + id + "found");

        _animator.CrossFadeInFixedTime(mS.animationBlendTreeIndex, blendTreeBlendTime);
    }

    public IDisposable TrySubscribe(SensorID sensorID, Action<bool> boolAction)
    {
        _sensorsByKey.TryGetValue(sensorID, out ReactiveSensor sensorFound);
        if (sensorFound == null) throw new UnassignedReferenceException("Sensor " + sensorID.ToSafeString() + " could not be found in Children of PlayerStateMachine.");
        //Debug.Log("Subscribing to " + sensorFound.ToInspectorString());
        return sensorFound.ExposeBoolObservable().Subscribe(boolAction);
    }

    public IDisposable TrySubscribe(SensorID sensorID, Action<Vector3> vec3Action)
    {
        _sensorsByKey.TryGetValue(sensorID, out ReactiveSensor sensorFound);
        if (sensorFound == null) throw new UnassignedReferenceException("Sensor " + sensorID.ToSafeString() + " could not be found in Children of PlayerStateMachine.");
        //Debug.Log("Subscribing to " + sensorFound.ToInspectorString());
        return sensorFound.ExposeVector3Observable().Subscribe(vec3Action);
    }

    public IDisposable TrySubscribe(SensorID sensorID, Action<float> floatAction)
    {
        _sensorsByKey.TryGetValue(sensorID, out ReactiveSensor sensorFound);
        if (sensorFound == null) throw new UnassignedReferenceException("Sensor " + sensorID.ToSafeString() + " could not be found in Children of PlayerStateMachine.");
        //Debug.Log("Subscribing to " + sensorFound.ToInspectorString());
        return sensorFound.ExposeFloatObservable().Subscribe(floatAction);
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

        MovementStateSettings settings = GetMovementStateSettings(drawGizmosFor);

        Gizmos.color = Color.green;
        for (int i = 0; i < gizmoCircleResolution; i++)
        {
            if (settings.clampYaw)
            {
                float startDegrees = Mathf.Abs(i * (360.0f / gizmoCircleResolution) - 180);
                float endDegrees = Mathf.Abs((i+1) * (360.0f / gizmoCircleResolution) - 180);
                if (startDegrees > settings.yawClampSymmetric || endDegrees > settings.yawClampSymmetric) continue;

                Gizmos.DrawLine(
                Vector3.up + transform.position + Quaternion.Euler(0, i * 360.0f / gizmoCircleResolution, 0) * transform.forward * gizmoCircleRadius,
                Vector3.up + transform.position + Quaternion.Euler(0, (i + 1) * (360.0f / gizmoCircleResolution), 0) * transform.forward * gizmoCircleRadius);

            } else { 
            Gizmos.DrawLine(
                Vector3.up + transform.position + Quaternion.Euler(0, i * 360.0f / gizmoCircleResolution, 0) * Vector3.right * gizmoCircleRadius,
                Vector3.up + transform.position + Quaternion.Euler(0, (i+1) * (360.0f / gizmoCircleResolution), 0) * Vector3.right * gizmoCircleRadius);
            }
        }
        if (settings.clampYaw) {
            Gizmos.DrawLine(
                    new Vector3(0, 1.2f, 0) + transform.position + Quaternion.Euler(0, 180 + settings.yawClampSymmetric, 0) * transform.forward * gizmoCircleRadius,
                    new Vector3(0, 0.8f, 0) + transform.position + Quaternion.Euler(0, 180 + settings.yawClampSymmetric, 0) * transform.forward * gizmoCircleRadius);
            Gizmos.DrawLine(
                    new Vector3(0, 1.2f, 0) + transform.position + Quaternion.Euler(0, 180 - settings.yawClampSymmetric, 0) * transform.forward * gizmoCircleRadius,
                    new Vector3(0, 0.8f, 0) + transform.position + Quaternion.Euler(0, 180 - settings.yawClampSymmetric, 0) * transform.forward * gizmoCircleRadius);
        }

        Gizmos.color = Color.red;
        for (int i = 0; i < gizmoCircleResolution/2; i++)
        {
            float startDegrees = (i * (360.0f / gizmoCircleResolution));
            float endDegrees = ((i + 1) * (360.0f / gizmoCircleResolution));
            //if (startDegrees > settings.yawClampSymmetric || endDegrees > settings.yawClampSymmetric) continue;
            
            Gizmos.DrawLine(
            Vector3.up + transform.position + Vector3.RotateTowards(gizmoCircleRadius * -transform.forward, gizmoCircleRadius * Vector3.up, (i * 360.0f / gizmoCircleResolution) * 0.0174533f, 0),
            Vector3.up + transform.position + Vector3.RotateTowards(gizmoCircleRadius * -transform.forward, gizmoCircleRadius * Vector3.up, ((i+1) * 360.0f / gizmoCircleResolution) * 0.0174533f, 0));   
        }
        for (int i = 0; i < gizmoCircleResolution / 2; i++)
        {
            float startDegrees = (i * (360.0f / gizmoCircleResolution));
            float endDegrees = ((i + 1) * (360.0f / gizmoCircleResolution));
            //if (startDegrees > settings.yawClampSymmetric || endDegrees > settings.yawClampSymmetric) continue;

            Gizmos.DrawLine(
            Vector3.up + transform.position + Vector3.RotateTowards(gizmoCircleRadius * -transform.forward, gizmoCircleRadius * Vector3.down, (i * 360.0f / gizmoCircleResolution) * 0.0174533f, 0),
            Vector3.up + transform.position + Vector3.RotateTowards(gizmoCircleRadius * -transform.forward, gizmoCircleRadius * Vector3.down, ((i + 1) * 360.0f / gizmoCircleResolution) * 0.0174533f, 0));
        }

        Gizmos.DrawLine(
                transform.rotation * new Vector3(0.2f, 1.0f, 0) + transform.position + Quaternion.Euler(0, 180 + settings.yawClampSymmetric, 0) * transform.forward * gizmoCircleRadius,
                transform.rotation * new Vector3(-0.2f, 1.0f, 0) + transform.position + Quaternion.Euler(0, 180 + settings.yawClampSymmetric, 0) * transform.forward * gizmoCircleRadius);
        Gizmos.DrawLine(
                    transform.rotation * new Vector3(0.2f, 1.0f, 0) + transform.position + Quaternion.Euler(180 - settings.yawClampSymmetric, 0, 0) * transform.forward * gizmoCircleRadius,
                    transform.rotation * new Vector3(-0.2f, 1.0f, 0) + transform.position + Quaternion.Euler(180 - settings.yawClampSymmetric, 0, 0) * transform.forward * gizmoCircleRadius);
        
    }

    public string ToDebugString()
    {
        return string.Format("{0}\n  Velocity: {1}\n  Speed: {2}m/s\n", _currentState.GetStateName(), velocity, Math.Round(velocity.magnitude, 2));
    }

    public Observable<bool> CombineBoolSensors(SensorID boolSensorID1, SensorID boolSensorID2, Func<bool, bool, bool> combinator)
    {
        if (_sensorsByKey.TryGetValue(boolSensorID1, out ReactiveSensor boolSensor1) &&
            _sensorsByKey.TryGetValue(boolSensorID2, out ReactiveSensor boolSensor2))
        {
            return (boolSensor1.ExposeBoolObservable()
                .CombineLatest<bool, bool, bool>(boolSensor2
                .ExposeBoolObservable(), (bool1, bool2) => { return combinator(bool1, bool2); })
                );
        }
        else
        {
            throw new System.Exception("Either sensor " + boolSensorID1.ToString() + " or Sensor " + boolSensorID2.ToString() + " could not be found.");
        }
    }

    public Observable<T> CombineBoolAndFloatSensorTo<T>(SensorID boolSensorID, SensorID floatSensorID, Func<bool, float, T> combinator)
    {
        if (_sensorsByKey.TryGetValue(boolSensorID, out ReactiveSensor boolSensor) &&
            _sensorsByKey.TryGetValue(floatSensorID, out ReactiveSensor floatSensor))
        {
            return (boolSensor.ExposeBoolObservable()
                .CombineLatest(floatSensor
                .ExposeFloatObservable(), (boolValue, floatValue) => { return combinator(boolValue, floatValue); })
                );
        }
        else
        {
            throw new System.Exception("Either sensor " + boolSensorID.ToString() + " or Sensor " + floatSensorID.ToString() + " could not be found.");
        }
    }

    private void OnDestroy()
    {
        _currentState.ExitState();
    }
}
