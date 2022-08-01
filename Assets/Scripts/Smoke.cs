using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smoke : MonoBehaviour
{
    [SerializeField] bool active = false;

    public void Activate() => active = true;

    public void DestroySmoke()
    {
        Destroy(gameObject);
    }
    private void Start()
    {
        if(active == false)
        {
            GetComponent<Animator>().speed = 0;
        }
        else GetComponent<Animator>().speed = 1;
    }
}
