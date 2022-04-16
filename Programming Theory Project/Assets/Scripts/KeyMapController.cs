using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[DefaultExecutionOrder(1000)]
public class KeyMapController : MonoBehaviour
{
    void Awake()
    {
        GetComponent<TextMeshProUGUI>().text = GameManager.Instance.KeyMapData.ToString();
    }
}
