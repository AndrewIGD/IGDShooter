using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePos : MonoBehaviour
{
    public static Vector3 Position;

    // Update is called once per frame
    void Update()
    {
        Position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
