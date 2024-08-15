using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CreateMap : MonoBehaviour
{
    [SerializeField] GameObject newField;

    [SerializeField] InputField nameField;

    [SerializeField] GameObject plusButton;

    [SerializeField] GameObject mapList;

    [SerializeField] GameObject parent;

    [SerializeField] GameObject errorText;

    private void OnEnable()
    {
        nameField.ActivateInputField();
    }

    public void Create()
    {
        if (parent.activeInHierarchy)
        {
            errorText.SetActive(false);

            string mapPath = Application.dataPath + "\\Maps";

            Directory.CreateDirectory(mapPath);

            if (Directory.Exists(mapPath + nameField.text))
            {
                errorText.SetActive(true);
                return;
            }

            try
            {
                Directory.CreateDirectory(mapPath + nameField.text);
            }
            catch
            {
                errorText.SetActive(true);
                return;
            }

            GameObject field = Instantiate(newField);
            field.transform.parent = mapList.transform;
            field.GetComponentInChildren<Text>().text = nameField.text;

            nameField.text = "";

            plusButton.transform.SetAsLastSibling();

            parent.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            Create();
    }
}
