using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeleteMap : MonoBehaviour
{
    [SerializeField] GameObject deleteMapObj;
    [SerializeField] GameObject parent;
    [SerializeField] Text mapName;

    public void Delete()
    {
        deleteMapObj.GetComponentInChildren<Text>().text = "Are you sure you want to delete " + mapName.text + "?";
        deleteMapObj.GetComponentInChildren<DeleteYes>().GetDeletionObject(parent);
        deleteMapObj.SetActive(true);
    }
}
