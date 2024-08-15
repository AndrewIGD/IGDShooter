using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DeleteYes : MonoBehaviour
{
    [SerializeField] GameObject parent;

    private GameObject _objToDelete;

    public void GetDeletionObject(GameObject obj) => _objToDelete = obj;

    public void Delete()
    {
        Directory.Delete(Application.dataPath + "\\Maps\\" + _objToDelete.GetComponentInChildren<Text>().text, true);

        Destroy(_objToDelete);

        parent.SetActive(false);
    }
}
