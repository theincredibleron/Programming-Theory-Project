using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] CubeController m_CubeController;
    [SerializeField] float horizontalSpeed, verticalSpeed;
    bool m_AnyKeyDown = false;
    bool m_IsOrientationLocked = false;
    Queue<CubeScript.CommandContext> m_ScriptCommands;
    Dictionary<Vector3, Vector3> m_ControlsOrientation;
    public InputField ScriptInput;

    void Awake()
    {
        m_CubeController = GameObject.Find("Cube").GetComponent<CubeController>();
        m_ControlsOrientation = new Dictionary<Vector3, Vector3>{
            {Vector3.right, Vector3.right},
            {Vector3.left, Vector3.left},
            {Vector3.up, Vector3.up},
            {Vector3.down, Vector3.down},
            {Vector3.forward, Vector3.forward},
            {Vector3.back, Vector3.back}
        };
    }

    #region script playing
    public void RunScript()
    {
        m_ScriptCommands = new Queue<CubeScript.CommandContext>();
        CubeScript cubeScript = new CubeScript();
        cubeScript.OnCommandComplete += OnCommandComplete;
        ScriptInput.interactable = false;
        string script = ScriptInput.textComponent.text;
        cubeScript.Parse(script);
    }

    // ABSTRACTION
    void ExecuteScriptCommand()
    {
        // process script command queue
        if(m_ScriptCommands != null && m_ScriptCommands.Count > 0) {
            m_IsOrientationLocked = true;
            CubeScript.CommandContext context = m_ScriptCommands.Dequeue();
            m_CubeController.RotatePlane(m_ControlsOrientation[context.Plane], context.RotationDirection);
        } else { // script ended
            m_IsOrientationLocked = false;
            ScriptInput.interactable = true;
        }
    }

    public void OnCommandComplete(CubeScript.CommandContext context)
    {
        m_ScriptCommands.Enqueue(context);        
    } 
    #endregion
    void LateUpdate()
    {
        // skip input processing if control is pressed
        if (Input.GetKey(KeyCode.LeftControl) || ScriptInput.isFocused) return;

        SetViewRotationInputs();
        
        if (!Input.anyKeyDown && m_AnyKeyDown) {
            RemapControlsOrientation();
            m_AnyKeyDown = false;
        } else m_AnyKeyDown = Input.anyKeyDown;

        if (m_CubeController.RotationOngoing) return;

        ExecuteScriptCommand();
        RotatePlane();
    }

    void Update()
    {
        RotateView();
    }

    // ABSTRACTION
    void RotatePlane()
    {
        foreach (KeyValuePair<KeyCode, GameManager.PlaneDescriptor> entry 
            in GameManager.Instance.KeyMap) {
            if (Input.GetKeyDown(entry.Key)) {
                m_CubeController.RotatePlane(
                    planeAxis: m_ControlsOrientation[entry.Value.Orientation],
                    direction: entry.Value.RotationDirection);
                break;
            }
        }
    }

    // ABSTRACTION
    void RotateView()
    {
        if (Mathf.Abs(horizontalSpeed) > 0.1f)
            transform.RotateAround(m_CubeController.transform.position, 
                Vector3.up, horizontalSpeed * 360f * Time.deltaTime);    
        if (Mathf.Abs(verticalSpeed) > 0.1f)
            transform.RotateAround(m_CubeController.transform.position, 
                Vector3.right, verticalSpeed * 360f *  Time.deltaTime);    
    }

    // ABSTRACTION
    public void SetViewRotationInputs()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        if (horizontal != 0f || vertical != 0f) {
            horizontalSpeed = horizontal;
            verticalSpeed = vertical;
        }
    }

    #region viewport to controls mapping
    // ABSTRACTION
    Vector3 DiscretizedDirection(Vector3 euler)
    {
        Vector3 resEuler = new Vector3() {
                x = Mathf.Clamp(((int)(euler.x + 45) / 90) * 90.0f, 0.0f, 359.0f),
                y = Mathf.Clamp(((int)(euler.y + 45) / 90) * 90.0f, 0.0f, 359.0f),
                z = Mathf.Clamp(((int)(euler.z + 45) / 90) * 90.0f, 0.0f, 359.0f)
            };
        return resEuler; 
    }

    // ABSTRACTION
    Vector3 CleanupVector(Vector3 input) {
        return new Vector3(
            Mathf.RoundToInt(input.x),
            Mathf.RoundToInt(input.y),
            Mathf.RoundToInt(input.z)
        );
    }

    // ABSTRACTION
    void RemapControlsOrientation()
    {
        // early return, if orientation must not be remapped, e.g. during script excecution
        if (m_IsOrientationLocked) return;
        Vector3 newEuler = DiscretizedDirection(transform.eulerAngles);
        Quaternion newRot = Quaternion.Euler(newEuler);

        Vector3 resRight = CleanupVector((newRot * Vector3.right).normalized);
        Vector3 resUp = CleanupVector((newRot * Vector3.up));
        Vector3 resForward = CleanupVector((newRot * Vector3.forward));

        // now remap
        m_ControlsOrientation[Vector3.right] = resRight;        
        m_ControlsOrientation[Vector3.left] = -resRight;        
        m_ControlsOrientation[Vector3.up] = resUp;        
        m_ControlsOrientation[Vector3.down] = -resUp;
        m_ControlsOrientation[Vector3.forward] = resForward;        
        m_ControlsOrientation[Vector3.back] = -resForward; 
    }
    #endregion

    #region visual debugging
#if UNITY_EDITOR
    public bool DebugControlOrientation = false;
    private void OnDrawGizmos() {
        if (!DebugControlOrientation || m_ControlsOrientation == null) return;
        Mesh mesh = new Mesh();
        mesh.name = "controlPlanes";
        mesh.vertices = new Vector3[4] {
            new Vector3(-2, 0, -2),
            new Vector3(-2, 0, 2),
            new Vector3(2, 0, 2),
            new Vector3(2, 0, -2)
        };
        mesh.triangles = new int[12]{
            0, 1, 3,
            3, 1, 2,
            0, 3, 1,
            1, 3, 2
        };
        
        mesh.RecalculateNormals();

        RemapControlsOrientation();
        Color color = Color.white;
        foreach (KeyValuePair<Vector3, Vector3> dir in m_ControlsOrientation){
            color.r = dir.Key.x;
            color.g = dir.Key.y;
            color.b = dir.Key.z;
            color.a = 0.5f;
            Vector3 scale = Vector3.one * (dir.Key == Vector3.right
                || dir.Key == Vector3.forward
                || dir.Key == Vector3.up ? 1 : 0.2f);
            Gizmos.color = color;
            Gizmos.DrawMesh(mesh, -dir.Value * 3, Quaternion.FromToRotation(Vector3.up, dir.Value), scale);
        }
    }
#endif
    #endregion
}
