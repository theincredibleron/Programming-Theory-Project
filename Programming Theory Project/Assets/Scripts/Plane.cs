using System.Collections;
using UnityEngine;

public partial class Plane : Reparenting
{
    float m_FixedAngle = 90.0f;
    float m_CurrentAngle = 0f;
    Quaternion m_TargetRotation;
    float m_RotationSpeed = 3.0f;
    [SerializeField] public bool IsRotating { get; protected set; }
    public Vector3 RotationAxis;
    [SerializeField] Direction m_RotationDirection;

    virtual protected void Awake()
    {
        m_SphereRadius = 0.6f;
    }

    public IEnumerator Rotate(Direction direction)
    {
        // exit early if already rotating...
        if (IsRotating) yield return null;
        
        RotationParent = gameObject;
        ReparentAdjacent("Edge");
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
        RotationParent = null;
        ReparentAdjacent("Edge");
        // stop animation
        IsRotating = false;
    }
}
