using System.Collections;
using UnityEngine;

public class Plane : NotifyingFinishBehavior
{
    float m_FixedAngle = 90.0f;
    Quaternion m_TargetRotation;
    float m_RotationSpeed = 3.0f;
    [SerializeField] public bool IsRotating { get; protected set; }
    public Vector3 RotationAxis;
    [SerializeField] Direction m_RotationDirection;
    [SerializeField] Vector3 m_OverlapBox = new Vector3(3, 1, 3) / 3;
    Collider[] m_AdjacentColliders;

    public IEnumerator Rotate(Direction direction)
    {
        // exit early if already rotating...
        if (IsRotating) yield break;
        IsRotating = true;
        // reparent all plane pieces to this transform
        ReparentAdjacent(gameObject.transform);
        // create target rotation for later snap-in
        int directionInt = ((int)direction);
        m_TargetRotation = transform.rotation * Quaternion.AngleAxis(m_FixedAngle * directionInt, RotationAxis);
        m_RotationDirection = direction;

        float step = m_FixedAngle * Time.deltaTime * m_RotationSpeed;
        for (float i = 0.0f; i < m_FixedAngle; i += step) {
            transform.Rotate(RotationAxis, step * directionInt);
            yield return null;
        }
        
        // to "snap-in" properly assign previously calculated target rotation
        transform.rotation = m_TargetRotation;
        // reparent children to cube
        ReparentAdjacent(null);
        // stop animation
        IsRotating = false;
        InvokeEvent(this.gameObject, "Rotation");
    }

    void ReparentAdjacent(Transform parent)
    {
        // reparent to plane's parent if argument is null
        if (parent == null) {
            foreach (Collider adjacent in m_AdjacentColliders)
                adjacent.gameObject.transform.parent = transform.parent;
            return;
        }

        m_AdjacentColliders = Physics.OverlapBox(transform.position, 
            m_OverlapBox, 
            transform.rotation * Quaternion.FromToRotation(Vector3.up, RotationAxis));

        foreach (Collider adjacent in m_AdjacentColliders) { 
            if (adjacent.gameObject == gameObject) continue;
            if (!tag.Equals("Center")) {
                adjacent.gameObject.transform.SetParent(transform);
            }
        }
    }

    public bool overlapBoxDebug = false;
    private void OnDrawGizmos() {
        if (!overlapBoxDebug) return;
        // draw local coords gizmo
        Gizmos.color = Color.green;
        Gizmos.DrawRay(gameObject.transform.position, gameObject.transform.up);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(gameObject.transform.position, gameObject.transform.right);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(gameObject.transform.position, gameObject.transform.forward);
        // draw rotation axis
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(gameObject.transform.position, gameObject.transform.TransformDirection(RotationAxis));
        // draw overlap box
        Gizmos.color = Color.green;

        Gizmos.matrix = Matrix4x4.Translate(transform.position);
        Gizmos.matrix *= Matrix4x4.Rotate(transform.rotation);
        Gizmos.matrix *= Matrix4x4.Rotate(Quaternion.FromToRotation(Vector3.up, RotationAxis));
        Gizmos.DrawWireCube(Vector3.zero, m_OverlapBox * 2);
    }
}
