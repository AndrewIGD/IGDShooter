using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    public static Vector2 Position;
   
    private void Update()
    {
        Position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
