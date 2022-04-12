using System.Collections;
using UnityEngine;

public class Plane : MonoBehaviour
{
    float m_FixedAngle = 90.0f;
    Quaternion m_TargetRotation;
    float m_RotationSpeed = 3.0f;
    [SerializeField] public bool IsRotating { get; protected set; }
    public Vector3 RotationAxis;
    [SerializeField] Direction m_RotationDirection;
    [SerializeField] Vector3 m_OverlapBox = new Vector3(3, 1, 3) / 3;

    public IEnumerator Rotate(Direction direction)
    {
        // exit early if already rotating...
        if (IsRotating) yield return null;
        // reparent all plane pieces to this transform
        ReparentAdjacent(gameObject.transform);
        IsRotating = true;
        Transform tempTransform = transform;
        tempTransform.Rotate(RotationAxis, m_FixedAngle * ((int)direction));
        m_TargetRotation = tempTransform.rotation;
        m_RotationDirection = direction;

        float step = Time.deltaTime * m_RotationSpeed * m_FixedAngle * ((int)direction);
        for (float i = 0.0f; Mathf.Abs(i) < m_FixedAngle; i += step) {
            transform.Rotate(RotationAxis, step);
            yield return null;
        }
        
        // to "snap-in" properly assign calculated target rotation
        transform.rotation = m_TargetRotation;
        // reparent children to cube
        ReparentAdjacent(transform.parent);
        // stop animation
        IsRotating = false;
    }

    void ReparentAdjacent(Transform parent)
    {
        Collider[] adjacentColliders = Physics.OverlapBox(transform.position, 
            m_OverlapBox, 
            transform.rotation * Quaternion.FromToRotation(Vector3.up, RotationAxis));

        foreach (Collider adjacent in adjacentColliders) { 
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
