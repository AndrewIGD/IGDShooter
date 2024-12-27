using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lidgren.Network;
using System.Net;
using System;
using UnityEngine.SceneManagement;
using System.IO;

public class MainMenuActions : MonoBehaviour
{
    [SerializeField] InputField ipAdress;

    [SerializeField] InputField map;
    [SerializeField] GameObject error;

    [SerializeField] GameObject mapName;

    [SerializeField] bool changeMouseScroll = false;
    [SerializeField] bool changeShowInv = false;

    private int _mouseScroll = 0;

    private int _showInv = 0;

    public void Exit()
    {
        if (mapName.activeInHierarchy)
            mapName.SetActive(false);
        else SceneManager.LoadScene("OnlineLobby");
    }

    public void MapEditor()
    {
        SceneManager.LoadScene("MapEditorMenu");
    }

    public void HostGame()
    {
        try
        {
            error.SetActive(false);
            if (Directory.Exists(Application.dataPath + "\\Maps\\" + map.text))
            {
                if (GameClient.Instance != null)
                {
                    Destroy(GameClient.Instance.gameObject);
                }

                GameObject hostObject = new GameObject();
                GameHost hostScript = hostObject.AddComponent<GameHost>();

                DontDestroyOnLoad(hostObject);
                try
                {
                    hostScript.Initialize();
                }
                catch (Exception err)
                {
                    Destroy(hostObject);

                    SceneManager.LoadScene("OnlineLobby");

                    ipAdress.text = err.ToString();

                    File.WriteAllText(Application.dataPath+"\\lastError.igd", err.ToString());

                    return;
                }

                /*var localHost = Dns.GetHostEntry(Dns.GetHostName());
                string ownIp = "";
                foreach (var ip in localHost.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        ownIp = ip.ToString();
                    }
                }

                Connect(ownIp);*/
            }
            else
            {
                error.SetActive(true);
            }
        }
        catch
        {

        }
    }

    public void JoinGame()
    {
        Connect(ipAdress.text);
    }

    private void Connect(string ip)
    {
        GameObject.Find("ErrorText").GetComponent<Text>().text = "";

        GameObject clientObject = new GameObject();
        GameClient clientScript = clientObject.AddComponent<GameClient>();
        clientObject.name = "GameClient";
        DontDestroyOnLoad(clientObject);

        GameObject clientMap = new GameObject();
        clientMap.transform.position = clientObject.transform.position;
        clientMap.transform.parent = clientObject.transform;

        clientScript.mapTransform = clientMap;
        clientMap.SetActive(false);

        clientScript.tile = Resources.Load("Prefabs\\tile") as GameObject;

        clientScript.ip = ip;
        clientScript.Initialize();
    }

    public void Settings()
    {
        SceneManager.LoadScene("Settings");
    }

    public void ChangeMouseScroll()
    {
        if (_mouseScroll == 0)
        {
            _mouseScroll = 1;

        }
        else _mouseScroll = 0;

        PlayerPrefs.SetInt("MouseScroll", _mouseScroll);
        if (_mouseScroll == 0)
            GetComponentInChildren<Text>().text = "Switch with Scroll: OFF";
        else if (_mouseScroll == 1)
            GetComponentInChildren<Text>().text = "Switch with Scroll: ON";
    }

    public void ChangeShowInv()
    {
        if (_showInv == 0)
        {
            _showInv = 1;

        }
        else _showInv = 0;

        PlayerPrefs.SetInt("ShowInv", _showInv);
        if (_showInv == 0)
            GetComponentInChildren<Text>().text = "Permanently Show Inventory: OFF";
        else if (_showInv == 1)
            GetComponentInChildren<Text>().text = "Permanently Show Inventory: ON";
    }

    public void GoLobby()
    {
        SceneManager.LoadScene("StartScene");
    }

    public void Quit() => Application.Quit();

    public void LAN()
    {
        MatchData.NetworkingLibrary = NetworkingLibrary.LiteNetLib;

        SceneManager.LoadScene("HostRoom");
    }

    public void Online()
    {
        
    }

    public void Graf()
    {
        SceneManager.LoadScene("Graffiti");
    }

    public void Buffer()
    {
        SceneManager.LoadScene("Buffer");
    }

    private void Update()
    {
        if(Chat.Instance != null)
        {
            Destroy(Chat.Instance.gameObject);
        }
    }

    private void Start()
    {
        if (changeMouseScroll)
        {
            _mouseScroll = PlayerPrefs.GetInt("MouseScroll", 0);
            if (_mouseScroll == 0)
                GetComponentInChildren<Text>().text = "Switch with Scroll: OFF";
            else if (_mouseScroll == 1)
                GetComponentInChildren<Text>().text = "Switch with Scroll: ON";
        }

        if (changeShowInv)
        {
            _showInv = PlayerPrefs.GetInt("ShowInv", 0);
            if (_showInv == 0)
                GetComponentInChildren<Text>().text = "Permanently Show Inventory: OFF";
            else if (_showInv == 1)
                GetComponentInChildren<Text>().text = "Permanently Show Inventory: ON";
        }
    }
}
