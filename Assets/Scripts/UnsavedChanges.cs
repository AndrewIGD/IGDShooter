using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnsavedChanges : MonoBehaviour
{
    public static UnsavedChanges Instance;

    private Text _text;

    public void Unsave()
    {
        _text.enabled = true;
    }

    public void Saved()
    {
        _text.enabled = false;
    }

    private void Awake()
    {
        Instance = this;

        _text = GetComponent<Text>();
    }
}
