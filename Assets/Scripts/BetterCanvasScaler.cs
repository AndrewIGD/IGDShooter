using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BetterCanvasScaler : MonoBehaviour
{
    void Awake()
    {
        float aspect = Screen.width / (float)Screen.height;

        if (aspect <= 16f/9f)
        {
            GetComponent<CanvasScaler>().matchWidthOrHeight = 0;
        }
        else GetComponent<CanvasScaler>().matchWidthOrHeight = 1;
    }
}
