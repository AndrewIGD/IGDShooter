using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStarter : MonoBehaviour
{
    private void Start()
    {
        if (Network.Instance.IsHost == false)
            gameObject.SetActive(false);
    }

    public void StartGame()
    {
        GameHost.Instance.StartRound();
    }
}
