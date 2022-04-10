using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    Dictionary<Vector3, GameObject> m_Planes = new Dictionary<Vector3, GameObject>(6);
    void Awake()
    {
        for (int i = 0; i < transform.childCount; i++) {
            Transform childTransform = transform.GetChild(i);
            // continue, if we aren't looking at a plane...
            if (!childTransform.gameObject.CompareTag("Plane")) continue;
            // exit this if we already got all planes...
            if (m_Planes.Count == 6) return;
            
            m_Planes.Add(
                childTransform.gameObject.GetComponent<Plane>().RotationAxis,
                childTransform.gameObject);
        }
    }

    public void RotatePlane(Vector3 planeAxis, Direction direction)
    {
        m_Planes[planeAxis].GetComponent<Plane>().Rotate(direction);
    }
}
