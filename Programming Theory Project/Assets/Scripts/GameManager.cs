using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Reflection;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Dictionary<KeyCode, PlaneDescriptor> KeyMap;
    public string m_ConfigFilePath;

    void Awake()
    {
        Instance = this;
        m_ConfigFilePath = Application.persistentDataPath + "/keymap.json";
        KeyMap = new Dictionary<KeyCode, PlaneDescriptor>(12);
        LoadKeymap();
    }

    void OnApplicationQuit() {
        SaveKeymap();    
    }

    KeyCode ParseKeyCodeString(string toParse) { return (KeyCode)System.Enum.Parse(typeof(KeyCode), toParse); }
    void LoadKeymap()
    {   
        if (!File.Exists(m_ConfigFilePath)) {
            InitDefaultKeymap();
            Debug.Assert(KeyMap.Count > 0, "InitDefaultKeymap failed");
            return;
        }
        string mapJson = File.ReadAllText(m_ConfigFilePath);
        KeyMapping keyMapping = JsonUtility.FromJson<KeyMapping>(mapJson);
        InitKeymap(keyMapping);
    }

    void InitDefaultKeymap()
    {
        InitKeymap(new KeyMapping());
    }

    void InitKeymap(KeyMapping mapping)
    {
        KeyMap.Add(ParseKeyCodeString(mapping.RightCW), mapping.GetPlaneDescriptor(mapping.RightCW));
        KeyMap.Add(ParseKeyCodeString(mapping.RightCCW), mapping.GetPlaneDescriptor(mapping.RightCCW));
        KeyMap.Add(ParseKeyCodeString(mapping.LeftCW), mapping.GetPlaneDescriptor(mapping.LeftCW));
        KeyMap.Add(ParseKeyCodeString(mapping.LeftCCW), mapping.GetPlaneDescriptor(mapping.LeftCCW));
        KeyMap.Add(ParseKeyCodeString(mapping.UpCW), mapping.GetPlaneDescriptor(mapping.UpCW));
        KeyMap.Add(ParseKeyCodeString(mapping.UpCCW), mapping.GetPlaneDescriptor(mapping.UpCCW));
        KeyMap.Add(ParseKeyCodeString(mapping.DownCW), mapping.GetPlaneDescriptor(mapping.DownCW));
        KeyMap.Add(ParseKeyCodeString(mapping.DownCCW), mapping.GetPlaneDescriptor(mapping.DownCCW));
        KeyMap.Add(ParseKeyCodeString(mapping.ForwardCW), mapping.GetPlaneDescriptor(mapping.ForwardCW));
        KeyMap.Add(ParseKeyCodeString(mapping.ForwardCCW), mapping.GetPlaneDescriptor(mapping.ForwardCCW));
        KeyMap.Add(ParseKeyCodeString(mapping.BackCW), mapping.GetPlaneDescriptor(mapping.BackCW));
        KeyMap.Add(ParseKeyCodeString(mapping.BackCCW), mapping.GetPlaneDescriptor(mapping.BackCCW));
    }

    void SaveKeymap()
    {
        KeyMapping mapping = new KeyMapping();
        foreach (KeyValuePair<KeyCode, PlaneDescriptor> entry in KeyMap) {
            mapping.SetKeyCode(entry.Value, entry.Key);
        }
        string mappingJson = JsonUtility.ToJson(mapping, true);
        File.WriteAllText(m_ConfigFilePath, mappingJson);
    }

    public class PlaneDescriptor {
        public Vector3 Orientation;
        public Direction RotationDirection;
    }

    [System.Serializable]
    public class KeyMapping {
        [SerializeField]
        public string RightCW = "U";
        [SerializeField]
        public string RightCCW = "J";
        [SerializeField]
        public string LeftCW = "F";
        [SerializeField]
        public string LeftCCW = "R";
        [SerializeField]
        public string UpCW = "T";
        [SerializeField]
        public string UpCCW = "Y";
        [SerializeField]
        public string DownCW = "H";
        [SerializeField]
        public string DownCCW = "G";
        [SerializeField]
        public string ForwardCW = "V";
        [SerializeField]
        public string ForwardCCW = "B";
        [SerializeField]
        public string BackCW = "M";
        [SerializeField]
        public string BackCCW = "N";

        public PlaneDescriptor GetPlaneDescriptor(string keyCode)
        {
            string propertyName = GetPropertyName(keyCode);
            Direction direction;
            Vector3 orientation;
            if (propertyName.EndsWith("CCW"))
                direction = Direction.CCW;
            else
                direction = Direction.CW;

            if (propertyName.StartsWith("Right"))
                orientation = Vector3.right;
            else if (propertyName.StartsWith("Left"))
                orientation = Vector3.left;
            else if (propertyName.StartsWith("Up"))
                orientation = Vector3.up;
            else if (propertyName.StartsWith("Down"))
                orientation = Vector3.down;
            else if (propertyName.StartsWith("Forward"))
                orientation = Vector3.forward;
            else if (propertyName.StartsWith("Back"))
                orientation = Vector3.back;
            else
                orientation = Vector3.zero;
            return new PlaneDescriptor
            {
                Orientation = orientation,
                RotationDirection = direction
            };            
        }

        public void SetKeyCode(PlaneDescriptor planeDescriptor, KeyCode keyCode)
        {   
            string prefix;
            if (planeDescriptor.Orientation.Equals(Vector3.right))
                prefix = "Right";
            else if (planeDescriptor.Orientation.Equals(Vector3.left))
                prefix = "Left";
            else if (planeDescriptor.Orientation.Equals(Vector3.up))
                prefix = "Up";
            else if (planeDescriptor.Orientation.Equals(Vector3.down))
                prefix = "Down";
            else if (planeDescriptor.Orientation.Equals(Vector3.forward))
                prefix = "Forward";
            else if (planeDescriptor.Orientation.Equals(Vector3.back))
                prefix = "Back";
            else
                prefix = "";
            string dirString = planeDescriptor.RotationDirection.ToString();
            MemberInfo[] infos = GetType().GetMember(prefix + dirString);
            foreach (MemberInfo info in infos) {
                if(info.MemberType != MemberTypes.Field) continue;
                FieldInfo fieldInfo = (FieldInfo)info;
                fieldInfo.SetValue(this, keyCode.ToString());
            }
        }

        string GetPropertyName(string keyCode) {
            MemberInfo[] infos = GetType().GetMembers();
            foreach (MemberInfo info in infos) {
                if(info.MemberType != MemberTypes.Field) continue;
                FieldInfo fieldInfo = (FieldInfo)info;
                if (keyCode == fieldInfo.GetValue(this).ToString()) {
                        return info.Name;
                }
            }

            return "";
        }
    }
}
