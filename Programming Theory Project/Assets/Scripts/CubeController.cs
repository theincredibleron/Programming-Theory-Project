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
            // add coroutine finish event handler
            planeScript.RotationFinished += OnPlaneRotationFinished;
            m_Planes.Add(
                planeScript.RotationAxis,
                childTransform.gameObject);
        }
    }

    public void RotatePlane(Vector3 planeAxis, Direction direction)
    {   
        if (RotationOngoing) return;
        Plane planeScript = m_Planes[planeAxis].GetComponent<Plane>();
        StartCoroutine(planeScript.Rotate(direction));
        RotationOngoing = true;
    }

    void OnPlaneRotationFinished(Plane sender)
    {
        RotationOngoing = sender.IsRotating;
    }
}
