using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundData : MonoBehaviour
{
    public static RoundData Instance;

    public bool Spawned = false;
    public bool RoundStart = false;
    public bool Ended = false;

    private void Awake()
    {
        Instance = this;
    }
}
