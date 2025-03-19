using UnityEngine;


public class CollidableHeightObject : CollidableElement
{
    public float height;

    public Vector2 gizmoScale = new Vector2(1, 1);

    public Vector3 localReceptionDirection = Vector3.zero;
    public float allowAngles = 45;

    private Vector3 usableReception { get { return new Vector3(localReceptionDirection.x, 0, localReceptionDirection.z).normalized; } }

    private Vector3 globalDirection { get { return transform.rotation * usableReception; } }

    public override bool elligbleDirection(Vector3 incomingDirection)
    {
        if (localReceptionDirection == Vector3.zero) return true;

        Vector3 flattenedDir = new Vector3(incomingDirection.x, 0, incomingDirection.z);

        float angle = Mathf.Abs(Vector3.Angle(-flattenedDir, globalDirection));

        Debug.Log("Incoming dir: " + flattenedDir + "   Receiving: " + globalDirection + "   Angle: " + angle + "   Passing: " + (angle < allowAngles));

        return angle < allowAngles;
    }

    public override Vector3 GetWorldPointOfContact()
    {
        Vector3 ret = transform.position + new Vector3(0, height, 0);

        return ret;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(GetWorldPointOfContact(), new Vector3(gizmoScale.x, 0.01f, gizmoScale.y));

        if (localReceptionDirection != Vector3.zero)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position + globalDirection + Vector3.up * 0.3f, transform.position + 0.5f * globalDirection);
            Gizmos.DrawLine(transform.position + globalDirection - Vector3.up * 0.3f, transform.position + 0.5f * globalDirection);
            Gizmos.DrawLine(transform.position + globalDirection * 2, transform.position + 0.5f * globalDirection);

            Vector3 rightOne = Quaternion.Euler(0, allowAngles, 0) * globalDirection;
            Vector3 leftOne = Quaternion.Euler(0, -allowAngles, 0) * globalDirection;
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, leftOne);
            Gizmos.DrawRay(transform.position, rightOne);
        }
    }

    
}