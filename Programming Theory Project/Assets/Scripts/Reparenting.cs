using UnityEngine;

public class Reparenting : MonoBehaviour
{
    protected float m_SphereRadius;
    public GameObject RotationParent { get; protected set; }

    virtual public void ReparentAdjacent(string tag)
    {
        Collider[] adjacentColliders = Physics.OverlapSphere(transform.position, m_SphereRadius);
        Transform parent = RotationParent == null 
            ? GameObject.Find("Cube").transform
            : RotationParent.transform;
        foreach (Collider adjacent in adjacentColliders) {
            if (adjacent.gameObject.CompareTag(tag)) {
                adjacent.gameObject.transform.SetParent(parent);
            }
        }
    }
}
