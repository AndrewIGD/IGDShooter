using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombAssign : MonoBehaviour
{
    private void Update()
    {
        if (GameHost.Instance != null)
        {
            if (GameHost.Instance.Warmup == false && Config.BombDefusal)
            {
                List<Player> terrorists = new List<Player>();

                foreach (Player player in GameClient.Instance.AlivePlayers)
                {
                    if (player.Controllable && player.Team == 1)
                    {
                        terrorists.Add(player);
                    }
                }

                if (terrorists.Count != 0)
                {
                    terrorists[Random.Range(0, terrorists.Count)].AssignBomb();
                    enabled = false;
                }
            }
        }
    }
}
