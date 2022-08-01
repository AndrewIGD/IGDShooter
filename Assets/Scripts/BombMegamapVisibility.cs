using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombMegamapVisibility : MonoBehaviour
{
    [SerializeField] private GameObject megamapBomb;

    private int _visiblePlayers;

    private int VisiblePlayers
    {
        get
        {
            return _visiblePlayers;
        }
        set
        {
            _visiblePlayers = value;

            megamapBomb.SetActive(_visiblePlayers != 0);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        MegamapDetect md = collision.gameObject.GetComponent<MegamapDetect>();

        if (md != null)
        {
            if(md.GetTarget().Team == 0)
            {
                VisiblePlayers++;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        MegamapDetect md = collision.gameObject.GetComponent<MegamapDetect>();

        if (md != null)
        {
            if (md.GetTarget().Team == 0)
            {
                VisiblePlayers--;
            }
        }
    }
}
