using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if(GameHost.Instance != null && player != null)
        {
            player.EnableBuyZone();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (GameHost.Instance != null && player != null)
        {
            player.DisableBuyZone();
        }
    }
}
