using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Globalization;

public class Chat : MonoBehaviour
{
    #region Public Variables

    public static Chat Instance;

    public bool Focused => _focused;

    public GameObject canvas;

    public GameObject typeBox;

    public GameObject typeParent;

    public GameObject chat;

    public Image chatImage;

    public GameObject scrollbar;

    #endregion

    #region Private Variables

    private bool hovered = false;

    private bool _focused = false;

    private InputField _typeBoxInputField;

    private Text _chatText;

    private bool clickedOutside = false;

    private bool pressedChatKey = false;

    private bool pressedSendKey = false;

    private bool invokingChatHide = false;

    #endregion

    #region Public Methods

    public void ShowText()
    {
        canvas.SetActive(true);
    }

    public void HideText()
    {
        CancelInvoke("HideText");

        canvas.SetActive(false);
        typeParent.SetActive(true);
        chatImage.enabled = true;
        scrollbar.SetActive(true);
    }

    public void AddMessage(string message)
    {
        _chatText.text += message + "\n";

        ShowText();
    }

    public void Clear()
    {
        _chatText.text = "";
    }

    public void MouseEnter()
    {
        hovered = true;
    }

    public void MouseExit()
    {
        hovered = false;
    }

    public void CancelCanvasHide()
    {
        CancelInvoke("CanvasOff");
    }

    #endregion

    #region Private Methods

    private void FixedUpdate()
    {
        _focused = _typeBoxInputField.isFocused;

        if (_focused == false)
            _typeBoxInputField.interactable = false;

        DisableChat();
        ToggleChat();
        CheckReturnKey();

        if (_focused == false && canvas.activeInHierarchy)
        {
            if (invokingChatHide == false)
            {
                invokingChatHide = true;

                CanvasOff();
                ShowText();
                Invoke("HideText", 5f);
            }
        }
        else
        {
            invokingChatHide = false;
            CancelInvoke("HideText");
        }
    }

    private void DisableChat()
    {
        if (hovered == false && Input.GetMouseButton(0))
        {
            if (clickedOutside == false)
            {
                clickedOutside = true;
                _focused = false;

                CanvasOff();
                ShowText();

                _typeBoxInputField.interactable = false;
                _typeBoxInputField.DeactivateInputField();
            }
        }
    }

    private void ToggleChat()
    {
        if (Input.GetKey(KeyCode.T) && ConsoleCanvas.Instance.Console.focused == false)
        {
            if (pressedChatKey == false)
            {
                pressedChatKey = true;

                canvas.SetActive(true);
                typeParent.SetActive(true);
                chatImage.enabled = true;
                scrollbar.SetActive(true);

                _typeBoxInputField.interactable = true;
                _typeBoxInputField.ActivateInputField();
                _focused = true;
            }
        }
        else pressedChatKey = false;
    }

    private void CheckReturnKey()
    {
        if (Input.GetKey(KeyCode.Return) && ConsoleCanvas.Instance.Console.focused == false)
        {
            if (pressedSendKey == false)
            {
                pressedSendKey = true;

                if (_typeBoxInputField.text != "" && typeParent.activeInHierarchy)
                {
                    int allChat = 1;

                    //Unde-i codul pentru team chat? Nu stiu

                    SendMessage(_typeBoxInputField.text, allChat);

                    _typeBoxInputField.text = "";
                }

                CanvasOff();
                ShowText();
                _focused = false;
            }
        }
        else pressedSendKey = false;
    }

    public void SendMessage(string message, int allType)
    {
        int all = 1;

        if (allType == 1)
            message = "/all " + message;

        Network.Instance.Send(new Msg(all == 1, GameClient.Instance.Team, message));

        //GameClient.Instance.Send("Msg " + all.ToString(CultureInfo.InvariantCulture) + " " + GameClient.Instance.Team.ToString(CultureInfo.InvariantCulture) + " " +
        //    (GameClient.Instance.Team == 0 ? "<color=aqua>" + GameClient.Instance.currentName + "</color>" : "") + (GameClient.Instance.Team == 1 ? "<color=red>" + GameClient.Instance.currentName + "</color>" : "") +
        //    ": " + message + "\n");
    }

    private void Start()
    {
        if (FindObjectsOfType<Chat>().Length >= 2)
        {
            Destroy(gameObject);

            return;
        }

        transform.parent = null;

        DontDestroyOnLoad(gameObject);

        Instance = this;

        CanvasOff();
        ShowText();

        _typeBoxInputField = typeBox.GetComponent<InputField>();

        _typeBoxInputField.interactable = false;

        _chatText = chat.GetComponent<Text>();
    }

    private void CanvasOff()
    {
        CancelInvoke("CanvasOff");

        typeParent.SetActive(false);
        scrollbar.SetActive(false);
        chatImage.enabled = false;
        canvas.SetActive(false);
    }

    private void OnLevelWasLoaded(int level)
    {
        if (level != Config.FightSceneIndex)
        {
            Destroy(gameObject);
        }
    }

    #endregion
}
