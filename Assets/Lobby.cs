using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lobby : MonoBehaviour
{
    void Start()
    {
        GameObject obj = new GameObject();

        if(Network.Instance.IsHost)
        {
            obj.AddComponent<GameHost>().Initialize();
            obj.name = "GameHost";
        }

        obj.AddComponent<GameClient>().Initialize();
        obj.name = "GameClient";
    }
}
