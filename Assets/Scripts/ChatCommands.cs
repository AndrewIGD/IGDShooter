using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatCommands : MonoBehaviour
{
    private InputField field;

    private void Start()
    {
        field = GetComponent<InputField>();
    }

    public void OnInput()
    {
        string[] parameters = field.text.Split(' ');
        if (parameters[0] == "/r")
        {
            parameters[0] = "/whisper " + GameClient.Instance.currentName;

            field.text = string.Join(" ", parameters);
        }
    }
}
