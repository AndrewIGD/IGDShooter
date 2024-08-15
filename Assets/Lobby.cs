using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lobby : MonoBehaviour
{
    void Start()
    {
        if (Network.Instance.IsHost == false)
            return;


        GameObject obj = new GameObject();

        obj.AddComponent<GameHost>().Initialize();
        obj.name = "GameHost";

        obj.AddComponent<GameClient>().Initialize();
        obj.name = "GameClient";

        StartCoroutine(Network.Instance.SelfConnect());
    }
}
