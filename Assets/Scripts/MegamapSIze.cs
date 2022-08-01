using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegamapSIze : MonoBehaviour
{
    void Start()
    {
        GetComponent<Camera>().orthographicSize = GameClient.Instance.megamapSize;
    }
}
