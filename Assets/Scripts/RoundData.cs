﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundData : MonoBehaviour
{
    public static RoundData Instance;

    public GameObject SpawnedBomb = null;

    public bool Spawned = false;

    public bool RoundStart = false;
    public bool Ended = false;

    public int RoundIndex = -1;

    private void Awake()
    {
        Instance = this;

        RoundIndex = GameClient.Instance.RoundIndex;
    }
}
