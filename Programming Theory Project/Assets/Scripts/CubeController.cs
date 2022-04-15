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
            planeScript.ActionFinished += OnActionFinished;
            m_Planes.Add(
                planeScript.RotationAxis,
                childTransform.gameObject);
        }
    }

    public void RotatePlane(Vector3 planeAxis, Direction direction)
    {   
        if (RotationOngoing) {
            Debug.Log("Skipping RotatPlane request..."); Debug.Break();
            return;
        }
        Plane planeScript = m_Planes[planeAxis].GetComponent<Plane>();
        StartCoroutine(planeScript.Rotate(direction));
        RotationOngoing = true;
    }

    void OnActionFinished(GameObject sender, string actionName)
    {
        if (actionName == "Rotation")
            RotationOngoing = sender.GetComponent<Plane>().IsRotating;
    }
}
