using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteNo : MonoBehaviour
{
    [SerializeField] GameObject parent;
    public void No()
    {
        parent.SetActive(false);
    }
}
