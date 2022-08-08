using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop : MonoBehaviour
{
    [SerializeField] int type;

    private int _bulletCount;

    private int _roundAmmo;

    private void Start()
    {
        if (type == 9 && GameClient.Instance.Team == 1)
        {
            //Activate bomb megamap visibility

            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
        }
    }

    public void Setup(int bulletCount, int roundAmmo)
    {
        _bulletCount = bulletCount;
        _roundAmmo = roundAmmo;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            if (GameHost.Instance != null && ((type == 9 && player.Team == 1) || type != 9))
            {
                bool playerGotDrop = player.GetDrop(type, _bulletCount, _roundAmmo);

                if (playerGotDrop)
                    Network.Instance.Send(new Destroy(transform.name));
            }
        }
    }
}
