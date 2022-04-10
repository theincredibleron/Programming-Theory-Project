using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    Dictionary<Vector3, GameObject> m_Planes = new Dictionary<Vector3, GameObject>(6);
    public bool RotationOngoing { get; private set; }

    void Awake()
    {
        for (int i = 0; i < transform.childCount; i++) {
            Transform childTransform = transform.GetChild(i);
            // continue, if we aren't looking at a plane...
            if (!childTransform.gameObject.CompareTag("Plane")) continue;
            // exit this if we already got all planes...
            if (m_Planes.Count == 6) return;
            
            Plane planeScript = childTransform.gameObject.GetComponent<Plane>();
            m_Planes.Add(
                planeScript.RotationAxis,
                childTransform.gameObject);
        }
    }

    public void RotatePlane(Vector3 planeAxis, Direction direction)
    {   
        Plane planeScript = m_Planes[planeAxis].GetComponent<Plane>();
        if (planeScript.IsRotating) return;
        
        StartCoroutine(planeScript.Rotate(direction));
    }
}
