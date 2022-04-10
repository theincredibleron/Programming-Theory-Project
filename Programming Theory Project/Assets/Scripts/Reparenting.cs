using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reparenting : MonoBehaviour
{
    protected float m_SphereRadius;
    public GameObject RotationParent { get; protected set; }

    virtual public void ReparentAdjacent(string tag)
    {
        Collider[] adjacentColliders = Physics.OverlapSphere(transform.position, m_SphereRadius);
        foreach (Collider adjacent in adjacentColliders) {
            if (adjacent.gameObject.CompareTag(tag)) {
                adjacent.gameObject.transform.SetParent(RotationParent.transform);
            }
        }
    }
}
