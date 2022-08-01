using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Escape : MonoBehaviour
{
    [SerializeField] GameObject kickText;
    [SerializeField] GameObject banText;

    public static Escape Instance;

    public void Terminate()
    {
        if (GameHost.Instance != null)
        {
            GameHost.Instance.Disconnect();
            GameHost.Instance.Server.Shutdown("ServerShutDown");

            DisconnectClient();
        }
        else
        {
            DisconnectClient();
        }

        if (Chat.Instance != null)
            Destroy(Chat.Instance.gameObject);

        SceneManager.LoadScene("OnlineLobby");
    }

    public void Kick(string message)
    {
        Terminate();

        GameObject clonedText = Instantiate(kickText);
        clonedText.GetComponentInChildren<Text>().text = "You were kicked for '" + message + "'";

        DontDestroyOnLoad(clonedText);
    }

    public void Ban(string message)
    {
        Terminate();

        GameObject clonedText = Instantiate(banText);
        clonedText.GetComponentInChildren<Text>().text = "You were banned for '" + message + "'";

        DontDestroyOnLoad(clonedText);
    }

    // Update is called once per frame
    private void Update()
    {
        return;

        try
        {
            if (Input.GetKeyDown(KeyCode.Escape) && Chat.Instance.Focused == false && ConsoleCanvas.Instance.Content.activeInHierarchy == false)
            {
                try
                {
                    if (FightSceneManager.Instance.BuyMenu.activeInHierarchy == false)
                    {
                        Terminate();
                    }
                }
                catch
                {
                    Terminate();
                }
            }
        }
        catch
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                try
                {
                    if (FightSceneManager.Instance.BuyMenu.activeInHierarchy == false)
                    {
                        Terminate();
                    }
                }
                catch
                {
                    Terminate();
                }
            }
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void DisconnectClient()
    {
        GameClient.Instance.Disconnect();

        Destroy(GameClient.Instance.gameObject);
    }
}
