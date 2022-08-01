using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MapLoadScript : MonoBehaviour
{
    [SerializeField] GameObject newField;

    [SerializeField] GameObject plusButton;

    [SerializeField] GameObject mapList;

    void Awake()
    {
        string mapPath = Application.dataPath + "\\Maps";

        if (Directory.Exists(mapPath))
        {
            foreach (string directory in Directory.EnumerateDirectories(mapPath))
            {
                GameObject field = Instantiate(newField);
                field.transform.parent = mapList.transform;
                field.GetComponentInChildren<Text>().text = directory.Split(new string[] { @"Maps\" }, System.StringSplitOptions.None).Last();
            }

            plusButton.transform.SetAsLastSibling();
        }
    }
}
