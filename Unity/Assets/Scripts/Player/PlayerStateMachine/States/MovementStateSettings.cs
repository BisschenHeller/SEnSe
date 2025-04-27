using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Movement State Settings", menuName = "Movement State Settings")]
public class MovementStateSettings : ScriptableObject
{
    [Header("--------------- General ---------------")]
    public MovementSettingsID movementSettingsID;
    public float regularSpeed;
    public float sprintSpeed;
    public float jumpPower;
    [Range(-3f, 3f)]
    public float gravityMultiplier;
    [Range(0.0f, 1.0f)]
    public float directionChangeRadius;
    [Range(0, 1)]
    [Tooltip("How fast should the player come to a standstill if no input is provided?")]
    public float drag = 0.1f;
    [Range(0, 1)]
    [Tooltip("How fast should the player accellerate?")]
    public float accelleration = 0.1f;
    [Space(20)]
    [Header("--------------- Animation ---------------")]
    public string animationBlendTreeName;
    [HideInInspector]
    public int animationBlendTreeIndex;
    [Space(20)]
    [Header("--------------- Camera ---------------")]
    
    [Range(-20, 90f)]
    [Tooltip("-90° = You can look straight down at top of your character's head.\n0° = You can't look from higher than hip level")]
    public float pitchClampTop = 80;
    [Range(-90f, 20)]
    [Tooltip("-90° = You can look straight at the soles of your player character\n0° = You can't look from lower than hip level")]
    public float pitchClampBottom = -40;
    [Space]
    public bool clampYaw = false;
    [Range(0, 180)]
    public float yawClampSymmetric;

    private void OnValidate()
    {
        pitchClampTop = Mathf.Clamp(pitchClampTop, pitchClampBottom, 90f);
        if (clampYaw && yawClampSymmetric == 180) clampYaw = false;
    }
}

public enum MovementSettingsID
{
    GroundMovement, Swimming, Jumping, Falling, Climbing, Transition, Ladder
}