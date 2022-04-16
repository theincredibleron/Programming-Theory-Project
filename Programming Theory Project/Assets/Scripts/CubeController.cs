using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CubeController : MonoBehaviour
{
    Dictionary<Vector3, GameObject> m_Planes = new Dictionary<Vector3, GameObject>(6);
    public bool RotationOngoing { get; private set; }
    [SerializeField]
    public string SaveFile;
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

    public void ResetCube()
    {
        if (RotationOngoing) return;
        for (int i = 0; i < transform.childCount; i++) {
            Transform child = transform.GetChild(i);
            child.rotation = Quaternion.identity;
        }
    }

    public void PersistCube()
    {
        CubeFormat cubeFormat = new CubeFormat{
            Name = gameObject.name
        };
        List<CubeInfo> data = new List<CubeInfo>(27);
        data.Add(new CubeInfo {
            Name = gameObject.name,
            Rotation = transform.eulerAngles
        });
        for(int i = 0; i < transform.childCount; i++) {
            Transform child = transform.GetChild(i);
            if (child.gameObject.CompareTag("Edge") 
            || child.gameObject.CompareTag("Corner")
            || child.gameObject.CompareTag("Plane")) {
                data.Add(new CubeInfo {
                    Name = child.gameObject.name,
                    Tag = child.gameObject.tag,
                    Position = child.position,
                    Rotation = child.eulerAngles
                });
            }
        }
        cubeFormat.Data = data.ToArray();

        string json = JsonUtility.ToJson(cubeFormat, true);
        File.WriteAllText(SaveFile, json);
    }

    public void LoadCube()
    {
        if (!File.Exists(SaveFile)) return;
        string json = File.ReadAllText(SaveFile);
        CubeFormat cubeFormat = JsonUtility.FromJson<CubeFormat>(json);
        foreach (CubeInfo info in cubeFormat.Data) {
            Transform child = GameObject.Find(info.Name).transform;
            child.position = info.Position;
            child.eulerAngles = info.Rotation;
        }
    }

    void OnActionFinished(GameObject sender, string actionName)
    {
        if (actionName == "Rotation")
            RotationOngoing = sender.GetComponent<Plane>().IsRotating;
    }

    [System.Serializable]
    public class CubeFormat
    {
        [SerializeField]
        public string Name;
        [SerializeField]
        public string Script;
        [SerializeField]
        public CubeInfo[] Data;
    }

    [System.Serializable]
    public class CubeInfo
    {
        [SerializeField]
        public string Name;
        [SerializeField]
        public string Tag;
        [SerializeField]
        public Vector3 Position;
        [SerializeField]
        public Vector3 Rotation;
    }
}
