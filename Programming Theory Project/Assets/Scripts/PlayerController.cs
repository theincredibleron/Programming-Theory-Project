using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] CubeController m_CubeController;
    [SerializeField] float horizontalSpeed, verticalSpeed;    
    // Start is called before the first frame update
    void Start()
    {
        m_CubeController = GameObject.Find("Cube").GetComponent<CubeController>();
    }

    // Update is called once per frame
    // ToDo: change key z to y before publishing!!!
    void LateUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        if (horizontal != 0f || vertical != 0f)
            Rotate(horizontal, vertical);

        if (m_CubeController.RotationOngoing) return;
        
        if (Input.GetKeyDown(KeyCode.R)) {
            m_CubeController.RotatePlane(Vector3.left, Direction.CCW);
        } else if (Input.GetKeyDown(KeyCode.F)) {
            m_CubeController.RotatePlane(Vector3.left, Direction.CW);
        } else if (Input.GetKeyDown(KeyCode.T)) {
            m_CubeController.RotatePlane(Vector3.up, Direction.CW);
        } else if (Input.GetKeyDown(KeyCode.Z)) {
            m_CubeController.RotatePlane(Vector3.up, Direction.CCW);
        } else if (Input.GetKeyDown(KeyCode.U)) {
            m_CubeController.RotatePlane(Vector3.right, Direction.CW);
        } else if (Input.GetKeyDown(KeyCode.J)) {
            m_CubeController.RotatePlane(Vector3.right, Direction.CCW);
        } else if (Input.GetKeyDown(KeyCode.G)) {
            m_CubeController.RotatePlane(Vector3.down, Direction.CCW);
        } else if (Input.GetKeyDown(KeyCode.H)) {
            m_CubeController.RotatePlane(Vector3.down, Direction.CW);
        } else if (Input.GetKeyDown(KeyCode.V)) {
            m_CubeController.RotatePlane(Vector3.forward, Direction.CW);
        } else if (Input.GetKeyDown(KeyCode.B)) {
            m_CubeController.RotatePlane(Vector3.forward, Direction.CCW);
        } else if (Input.GetKeyDown(KeyCode.N)) {
            m_CubeController.RotatePlane(Vector3.back, Direction.CCW);
        } else if (Input.GetKeyDown(KeyCode.M)) {
            m_CubeController.RotatePlane(Vector3.back, Direction.CW);
        }
    }

    void Update()
    {
        if (Mathf.Abs(horizontalSpeed) > 0.1f)
            transform.RotateAround(m_CubeController.transform.position, 
                Vector3.up, horizontalSpeed * 360f * Time.deltaTime);    
        if (Mathf.Abs(verticalSpeed) > 0.1f)
            transform.RotateAround(m_CubeController.transform.position, 
                Vector3.right, verticalSpeed * 360f *  Time.deltaTime);    

    }

    public void Rotate(float horizontal, float vertical)
    {
        horizontalSpeed = horizontal;
        verticalSpeed = vertical;
    }
}
