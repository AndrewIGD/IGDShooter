using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleCanvas : MonoBehaviour
{
    #region Public Variables
    public static ConsoleCanvas Instance;
    public Console Console => _console;

    public GameObject Content => _content;

    #endregion

    #region Private Variables

    private GameObject _content;

    private Console _console;

    private InputField _inputField;

    private bool _pressed = false;

    #endregion

    private void Start()
    {
        if (FindObjectsOfType<ConsoleCanvas>().Length >= 2)
        {
            Destroy(gameObject);

            return;
        }

        DontDestroyOnLoad(gameObject);

        _content = transform.GetChild(0).gameObject;

        _console = _content.GetComponent<Console>();

        _inputField = _console.typeBox.GetComponent<InputField>();

        Instance = this;
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.BackQuote) && _console.focused == false)
        {
            if (_pressed == false)
            {
                _pressed = true;

                if (Chat.Instance != null)
                {
                    if (Chat.Instance.Focused == false)
                    {
                        ToggleConsole();
                    }
                }
                else
                {
                    ToggleConsole();
                }
            }
        }
        else _pressed = false;
    }

    private void ToggleConsole()
    {
        foreach (InputField input in FindObjectsOfType<InputField>())
            input.DeactivateInputField();

        _content.SetActive(!_content.activeInHierarchy);
        if (_content.activeInHierarchy == false)
        {
            _console.focused = false;
            _inputField.text = "";
        }
    }
}
