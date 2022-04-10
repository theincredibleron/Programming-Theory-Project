using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge : Reparenting
{
    void Awake()
    {
        m_SphereRadius = 0.6f;        
    }

    private void OnTransformParentChanged() {
         RotationParent = transform.parent.gameObject;
         
         ReparentAdjacent("Corner");
    }
}
