public class Edge : Reparenting
{
    void Awake()
    {
        m_SphereRadius = 0.6f;        
    }

    private void OnTransformParentChanged() {
        RotationParent = transform.parent.gameObject;
        //if (RotationParent != null && RotationParent.GetComponent<Plane>().IsRotating) return;
        ReparentAdjacent("Corner");
    }
}
