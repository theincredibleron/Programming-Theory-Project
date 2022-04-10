using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Plane : Reparenting
{
    float m_FixedAngle = 90.0f;
    float m_CurrentAngle = 0f;
    Quaternion m_TargetRotation;
    float m_RotationSpeed = 5.0f;
    [SerializeField] bool m_IsRotating = false;
    public Vector3 RotationAxis;
    [SerializeField] Direction m_RotationDirection;
    
    virtual protected void Awake()
    {
        m_SphereRadius = 0.6f;
        RotationParent = gameObject;
    }

    // Update is called once per frame
    virtual protected void Update()
    {
        if (m_IsRotating && RotationAxis != null) {
            if (m_CurrentAngle < m_FixedAngle) {
                float step = m_RotationSpeed;// * Time.deltaTime;
                m_CurrentAngle += step;
                transform.Rotate(RotationAxis, ((int)m_RotationDirection) * step);
            } else {
                // to "snap-in" properly assign calculated target rotation
                transform.rotation = m_TargetRotation;
                // stop animation
                m_IsRotating = false;
                m_CurrentAngle = 0;
                OnRotationFinished();
            }
        }
    }

    void OnRotationFinished()
    { 
        
    }

    public void Rotate(Direction direction)
    {
        // exit early if already rotating...
        if (m_IsRotating) return;

        ReparentAdjacent("Edge");
        Transform tempTransform = transform;
        tempTransform.Rotate(RotationAxis, m_FixedAngle * ((int)direction));
        m_TargetRotation = tempTransform.rotation;
        m_RotationDirection = direction;
        m_IsRotating = true;
    }
}
