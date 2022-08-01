using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickButton : MonoBehaviour
{
    public void Ok()
    {
        Destroy(transform.parent.gameObject);
    }
}
