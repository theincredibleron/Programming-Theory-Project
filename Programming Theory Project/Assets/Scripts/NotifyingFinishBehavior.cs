using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotifyingFinishBehavior : MonoBehaviour
{
    public delegate void OnActionFinished(GameObject sender, string actionName);
    public event OnActionFinished ActionFinished;

    public void InvokeEvent(GameObject sender, string actionName)
    {
        ActionFinished?.Invoke(sender, actionName);
    }
}
