using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class CollidableElement : MonoBehaviour
{
    public LayerMask colliderLayer;

    public abstract Vector3 GetWorldPointOfContact();

    public abstract bool elligbleDirection(Vector3 incomingDirection);
}