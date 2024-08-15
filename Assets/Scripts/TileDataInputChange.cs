using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class TileDataInputChange : MonoBehaviour
{
    [SerializeField] TileData td;

    private InputField _inputField;
    private Toggle _toggle;

    public void ChangeX()
    {
        td.ChangeX(float.Parse(_inputField.text, CultureInfo.InvariantCulture));
    }
    public void ChangeY()
    {
        td.ChangeY(float.Parse(_inputField.text, CultureInfo.InvariantCulture));
    }
    public void ChangeZ()
    {
        td.ChangeZ(float.Parse(_inputField.text, CultureInfo.InvariantCulture));
    }
    public void ChangeRotX()
    {
        td.ChangeRotX(float.Parse(_inputField.text, CultureInfo.InvariantCulture));
    }
    public void ChangeRotY()
    {
        td.ChangeRotY(float.Parse(_inputField.text, CultureInfo.InvariantCulture));
    }

    public void ChangeRotZ()
    {
        td.ChangeRotZ(float.Parse(_inputField.text, CultureInfo.InvariantCulture));
    }
    public void ChangeScaleX()
    {
        td.ChangeScaleX(float.Parse(_inputField.text, CultureInfo.InvariantCulture));
    }
    public void ChangeScaleY()
    {
        td.ChangeScaleY(float.Parse(_inputField.text, CultureInfo.InvariantCulture));
    }

    public void ChangeBombSite()
    {
        td.ChangeBombSite(_toggle.isOn);
    }

    public void ChangeTSpawn()
    {
        td.ChangeTSpawn(_toggle.isOn);
    }
    public void ChangeCtSpawn()
    {
        td.ChangeCtSpawn(_toggle.isOn);
    }
    public void ChangeBoxCollider()
    {
        td.ChangeBoxCollider(_toggle.isOn);
    }
    public void ChangePixelArt()
    {
        td.ChangePixelArt(_toggle.isOn);
    }

    private void Start()
    {
        _inputField = GetComponent<InputField>();
        _toggle = GetComponent<Toggle>();
    }
}
