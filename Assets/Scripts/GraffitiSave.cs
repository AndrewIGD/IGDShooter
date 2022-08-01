using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class GraffitiSave : MonoBehaviour
{
    [SerializeField] int defaultGraf;

    private InputField _inputField;

    void Start()
    {
        _inputField = GetComponent<InputField>();

        _inputField.text = PlayerPrefs.GetInt("Graf" + defaultGraf, defaultGraf).ToString(CultureInfo.InvariantCulture);
    }

    public void SaveNum()
    {
        if (_inputField == null)
            return;

        PlayerPrefs.SetInt("Graf" + defaultGraf, int.Parse(_inputField.text, CultureInfo.InvariantCulture));
    }
}
