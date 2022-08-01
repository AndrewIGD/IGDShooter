using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class SettingSave : MonoBehaviour
{
    [SerializeField] string name;
    [SerializeField] int defaultValue;

    private InputField _inputField;

    private void Start()
    {
        if (PlayerPrefs.GetInt(name, 0) == 0)
            PlayerPrefs.SetInt(name, defaultValue);

        _inputField = GetComponent<InputField>();

        _inputField.text = PlayerPrefs.GetInt(name, 0).ToString(CultureInfo.InvariantCulture);
    }

    public void ChangeValue()
    {
        if (_inputField == null)
            return;

        PlayerPrefs.SetInt(name, int.Parse(_inputField.text, CultureInfo.InvariantCulture));
    }
}
