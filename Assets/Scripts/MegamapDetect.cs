using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegamapDetect : MonoBehaviour
{
    private Player _target;

    public Player GetTarget() => _target;

    public void SetTarget(Player player) => _target = player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            player.Visible();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            player.Invisible();
        }
    }

    private void Update()
    {
        try
        {
            transform.position = _target.transform.position;
        }
        catch
        {
            Destroy(gameObject);
        }
    }
}
