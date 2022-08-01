using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChatType : MonoBehaviour
{
    private bool _allChat = true;

    private Text _typeText;

    public void ChangeType()
    {
        _allChat = !_allChat;

        _typeText.text = _allChat ? "All" : "Team";

        Chat.Instance.CancelCanvasHide();
    }

    private void Start()
    {
        _typeText = GetComponentInChildren<Text>();
    }
}
