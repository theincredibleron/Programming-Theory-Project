using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientationHelper : MonoBehaviour
{
    Dictionary<Vector3, Vector3> m_AxisMapping = new Dictionary<Vector3, Vector3>(3);

    void Awake()
    {
        // set the detectors inital state up
        m_AxisMapping.Add(Vector3.right, Vector3.right);
        m_AxisMapping.Add(Vector3.up, Vector3.up);
        m_AxisMapping.Add(Vector3.forward, Vector3.forward);
    }

    public void CheckOrientation()
    {
        m_AxisMapping[Vector3.right] = GetCurrentTargetAxis(Vector3.right, gameObject.transform.right);
        m_AxisMapping[Vector3.up] = GetCurrentTargetAxis(Vector3.up, gameObject.transform.up);
        m_AxisMapping[Vector3.forward] = GetCurrentTargetAxis(Vector3.forward, gameObject.transform.forward);
        Debug.Log(this);
    }

    Vector3 GetCurrentTargetAxis(Vector3 axisToMap, Vector3 input)
    {
        Collider[] colliders = Physics.OverlapSphere(input, 0.2f);
        foreach (Collider col in colliders) {
            if (col.gameObject.CompareTag("Plane")) {
                return col.gameObject.GetComponent<Plane>().RotationAxis;
            }
        }
        // if no result found just return current mapping
        return m_AxisMapping[axisToMap];
    }

    public override string ToString()
    {
        return $"{gameObject.name} : right = {m_AxisMapping[Vector3.right]}; up = {m_AxisMapping[Vector3.up]}; forward = {m_AxisMapping[Vector3.forward]}";
    }
}
