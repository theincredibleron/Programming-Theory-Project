using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] CubeController m_CubeController;
    [SerializeField] float horizontalSpeed, verticalSpeed;
    bool m_AnyKeyDown = false;
    OrientationHelper m_OrientationHelper;


    // Start is called before the first frame update
    void Awake()
    {
        m_CubeController = GameObject.Find("Cube").GetComponent<CubeController>();
        m_OrientationHelper = GameObject.Find("OrientationHelper").GetComponent<OrientationHelper>();
    }

    // Update is called once per frame
    // ToDo: change key z to y before publishing!!!
    void LateUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        if (horizontal != 0f || vertical != 0f) {
            Rotate(horizontal, vertical);
        }
        
        if (!Input.anyKeyDown && m_AnyKeyDown) {
            m_OrientationHelper.CheckOrientation();
            m_AnyKeyDown = false;
        } else m_AnyKeyDown = Input.anyKeyDown;

        if (m_CubeController.RotationOngoing) return;
        
        foreach (KeyValuePair<KeyCode, GameManager.PlaneDescriptor> entry 
            in GameManager.Instance.KeyMap) {
            if (Input.GetKeyDown(entry.Key)) {
                m_CubeController.RotatePlane(entry.Value.Orientation, entry.Value.RotationDirection);
                break;
            }
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
