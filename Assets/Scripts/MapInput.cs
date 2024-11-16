using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapInput : MonoBehaviour
{
    public void OnChange()
    {
        GameHost.Map = GetComponent<InputField>().text;
    }
}
