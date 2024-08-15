using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ChangeMegamapSize : MonoBehaviour
{
    [SerializeField] private GameObject viewArea;

    private float _size = 0.04f;
    private InputField _inputField;

    public void SetSize(float size)
    {
        _size = size;

        viewArea.transform.localScale = new Vector2(25 * _size, 25 * _size * 0.5625f);
    }

    public void OnInputChanged()
    {
        SetSize(float.Parse(_inputField.text, CultureInfo.InvariantCulture));

        File.WriteAllText(Application.dataPath + "\\Maps\\" + MapCreatorCamera.Instance.SaveName + "\\megamap.igd", _size.ToString(CultureInfo.InvariantCulture));
    }

    private void Start()
    {
        _inputField = GetComponent<InputField>();
    }
}
