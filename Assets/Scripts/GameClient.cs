﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Lidgren.Network;
using System;
using System.Linq;
using TMPro;
using System.IO;
using System.Globalization;

public class GameClient : MonoBehaviour
{
    #region Public Variables
    public static bool IsRed => Instance.Team == 1;
    public static bool IsBlue => Instance.Team == 0;

    public static GameClient Instance;

    public List<Player> AlivePlayers = new List<Player>();
    public List<Wall> Walls = new List<Wall>();

    public string map;

    public GameObject tile;

    public GameObject mapTransform;

    public GameObject teamSelect;

    public GameObject megamapDetect;

    public GameObject megamap;

    public GameObject bomb;

    public GameObject heGren;

    public GameObject flashGren;

    public GameObject smokeGren;

    public GameObject smokeVfx;

    public GameObject bullet;

    public GameObject ctObj;

    public GameObject tObj;

    public GameObject waitingForServer;

    public GameObject startGame;

    public Dictionary<long, GameObject> players;

    public Dictionary<long, Player> playerScripts;

    public Text playerCount;

    public InputField playerName;

    public Button playerTeam;

    public NetClient client;

    public string currentName = "NewPlayer";

    public string lastWhisper = "";

    public string ip;

    public float megamapSize = 300;

    public long playerId;

    public bool selectedTeam = false;

    public bool upnp = false;

    public string roundStartExec = "";

    public bool canDo = false;

    public float timeBetweenResponses = 0;

    public int Team = -1;

    #endregion

    #region Private Variables

    private bool setTime = false;

    private int gameTime;

    private GameObject warmupObj;

    private bool gameStart = false;

    private NetPeerConfiguration config;

    private bool buttonPress2 = false;

    private string spawnPlayers = "";

    private bool detected = false;
    private string targetMessage = "";
    private string shootMessage = "";
    private string nadeMessage = "";
    private string switchWeaponMessage = "";
    private string buyMessage = "";
    private string reloadMessage = "";
    private string defuseMessage = "";
    private string stopDefuseMessage = "";
    private string dropTargetMessage = "";
    private string shiftMessage = "";
    private string noShiftMessage = "";
    private string throwMessage = "";
    private string scrollUpMessage = "";
    private string scrollDownMessage = "";
    private string graffitiMessage = "";

    private string message = "";

    private int ctSpawnIndex = 0;

    private int tSpawnIndex = 0;

    private GameObject timeObj;

    #endregion

    #region Public Methods

    public void Initialize()
    {
        if (FindObjectsOfType<GameClient>().Length >= 2 && GameHost.Instance == null)
        {
            Destroy(gameObject);

            return;
        }

        MatchData.PlayerData = new Dictionary<long, PlayerData>();

        Instance = this;

        try
        {
            players = new Dictionary<long, GameObject>();
            playerScripts = new Dictionary<long, Player>();

            config = new NetPeerConfiguration("IGDShooter")
            {
                EnableUPnP = true,
                AcceptIncomingConnections = true,
                PingInterval = 1f,
                ResendHandshakeInterval = 1f,
                MaximumHandshakeAttempts = 15,
                ConnectionTimeout = 1000f,
                ReceiveBufferSize = PlayerPrefs.GetInt("ClientReceiveBuffer", 3072),
                SendBufferSize = PlayerPrefs.GetInt("ClientSendBuffer", 1024)
            };

            config.EnableMessageType(NetIncomingMessageType.StatusChanged);

            client = new NetClient(config);
            client.Start();

            client.Connect(host: ip, port: 25565);

            Invoke("TestConnection", 3.5f);

            Invoke("SendConnectionMessage", 0.25f);

            StartCoroutine(MessageQueue(64));
        }
        catch (Exception err)
        {
            GameObject.Find("ErrorText").GetComponent<Text>().text = "Error: Invalid IP!";
        }
    }

    public void ClientSetTeam(int team)
    {
        if (team != this.Team)
        {
            this.Team = team;

            SendPlayerTeam();

            ActivateHUD();

            //Destroy Radar
            foreach (MegamapDetect md in FindObjectsOfType<MegamapDetect>())
            {
                if (md.transform.name.Contains("Clone"))
                    Destroy(md.gameObject);
            }

            //Show teammates radar and health
            foreach (Player player in GameClient.Instance.AlivePlayers)
            {
                if ((player.Team == team && player.ID != playerId) || team == 2)
                {
                    player.NameText.transform.localPosition = new Vector3(0, 0.148f, 0);
                    player.HealthBar.SetActive(true);

                    MegamapDetect md = Instantiate(FightSceneManager.Instance.BoxMD).GetComponent<MegamapDetect>();
                    md.enabled = true;
                    md.SetTarget(player);
                }
                else
                {
                    player.NameText.transform.localPosition = new Vector3(0, 0.1115999f, 0);
                    player.HealthBar.SetActive(false);
                }
            }
        }
    }

    private static void ActivateHUD()
    {
        //Activate HUD
        foreach (GameObject obj in FightSceneManager.Instance.ActivateAfterRoundStart)
        {
            obj.SetActive(true);
        }
    }

    public void Disconnect()
    {
        if (client.ServerConnection == null)
            return;

        var message3 = client.CreateMessage();
        message3.Write("Disconnect");

        client.SendMessage(message3,
        NetDeliveryMethod.ReliableOrdered);
    }

    public void Send(string message)
    {
        string type = message.Split(" ")[0];

        switch (type)
        {
            case "PlayerTarget":
                targetMessage = message;
                break;
            case "Shoot":
                shootMessage = message;
                break;
            case "ThrowNade":
                nadeMessage = message;
                break;
            case "Switch":
                switchWeaponMessage = message;
                break;
            case "Process":
                buyMessage = message;
                break;
            case "Reload":
                reloadMessage = message;
                break;
            case "Defuse":
                defuseMessage = message;
                break;
            case "StopDef":
                stopDefuseMessage = message;
                break;
            case "DropTarget":
                dropTargetMessage = message;
                break;
            case "Shift":
                shiftMessage = message;
                break;
            case "StopSh":
                noShiftMessage = message;
                break;
            case "Throw":
                throwMessage = message;
                break;
            case "ScrollUp":
                scrollUpMessage = message;
                break;
            case "ScrollDown":
                scrollDownMessage = message;
                break;
            case "Graffiti":
                graffitiMessage = message;
                break;

            default:
                this.message += message + "\n";
                break;
        }
    }


    #endregion

    #region Private Methods

    private void Update()
    {
        ActivateMegamap();
        ActivateTeamSelection();
        SpawnPlayers();
        RunOnRoundInitialization();
        LobbyUpdate();

        if (client != null)
        {
            try
            {
                if (GameHost.Instance != null)
                {
                    GameObject.Find("Start").SetActive(true);
                }
                else
                {
                    GameObject.Find("Start").SetActive(false);
                }
            }
            catch
            {

            }

            UpdateTimeOut();

            HandleMessages();

        }
    }

    private void LobbyUpdate()
    {
        buttonPress2 = false;
        try
        {
            GameObject.Find("Ping").GetComponent<Text>().text = ((int)(client.ServerConnection.AverageRoundtripTime * 1000)).ToString(CultureInfo.InvariantCulture) + "ms";

            playerName = GameObject.Find("PlayerName").GetComponent<InputField>();

            playerName.onValueChanged.AddListener(delegate { SendPlayerName(); });
        }
        catch
        {

        }
    }

    private void RunOnRoundInitialization()
    {
        RoundData spawnedPlayers = RoundData.Instance;

        if (roundStartExec != "" && spawnedPlayers.RoundStart == false)
        {
            foreach (string line in roundStartExec.Split('\n'))
            {
                InterpretLine(line);
            }

            roundStartExec = "";
            spawnedPlayers.RoundStart = true;
        }
    }

    private void UpdateTimeOut()
    {
        timeBetweenResponses += Time.deltaTime;
        if (timeBetweenResponses >= 10f)
        {
            SceneManager.LoadScene("OnlineLobby");
            message += "Disconnect" + "\n";

            if (GameHost.Instance != null)
                Destroy(GameHost.Instance.gameObject);

            Destroy(gameObject);
        }
    }

    private void HandleMessages()
    {
        int size = 0;

        List<NetIncomingMessage> messages = new List<NetIncomingMessage>();
        client.ReadMessages(messages);
        foreach (NetIncomingMessage message in messages)
        {
            string text = message.ReadString();
            //    Debug.Log("GameClient: " + text);

            size += System.Text.ASCIIEncoding.UTF32.GetByteCount(text);

            string[] lines = text.Split('\n');

            foreach (string line in lines)
            {

                InterpretLine(line);
            }
        }
    }

    private void UpdateBuyMenuText()
    {
        string buyMenuText = "";

        if (players.ContainsKey(playerId) && FightSceneManager.Instance.BuyMenuTeamCash != null)
        {
            foreach (Player player in GameClient.Instance.AlivePlayers)
            {
                if (player.gameObject != players[playerId] && player.Controllable && player.Team == Team)
                {
                    buyMenuText += player.Name + ": $" + player.Cash.ToString(CultureInfo.InvariantCulture) + "\n";
                }
            }
            FightSceneManager.Instance.BuyMenuTeamCash.text = buyMenuText;
        }
    }

    private void SpawnPlayers()
    {
        if (spawnPlayers != "")
        {
            if (RoundData.Instance.Spawned == false)
            {
                string[] lines = spawnPlayers.Split('\n');

                foreach (string line in lines)
                {
                    string data = line;

                    if (data.Split(' ').Length >= 2)
                    {
                        if (long.Parse(data.Split(' ')[1]) == playerId)
                        {
                            if (int.Parse(data.Split(' ')[3], CultureInfo.InvariantCulture) == 0)
                                Team = 0;
                            else if (int.Parse(data.Split(' ')[3], CultureInfo.InvariantCulture) == 1) Team = 1;
                            break;
                        }
                    }
                }

                foreach (string line in lines)
                {
                    try
                    {
                        string data = line;

                        bool ok = true;
                        if (data.Split(' ').Length >= 2)
                        {
                            foreach (Player p in GameClient.Instance.AlivePlayers)
                            {
                                if (p.Controllable && p.ID == long.Parse(data.Split(' ')[1], CultureInfo.InvariantCulture) && p.Dead == false)
                                {
                                    ok = false;
                                }
                            }

                            if (ok)
                            {
                                GameObject player = null;

                                Player playerScript = null;
                                if (long.Parse(data.Split(' ')[3], CultureInfo.InvariantCulture) == 0)
                                {
                                    player = Instantiate(FightSceneManager.Instance.CTPrefab);

                                    playerScript = player.GetComponent<Player>();

                                    playerScript.Initialize();
                                    if ((Team == 0 && long.Parse(data.Split(' ')[1], CultureInfo.InvariantCulture) != playerId) || Team == 2)
                                    {
                                        playerScript.NameText.transform.localPosition = new Vector3(0, 0.148f, 0);
                                        playerScript.HealthBar.SetActive(true);
                                    }
                                    if (Team == 0 || Team == 2)
                                    {
                                        GameObject md = Instantiate(FightSceneManager.Instance.BoxMD);
                                        md.GetComponent<MegamapDetect>().enabled = true;
                                        md.GetComponent<MegamapDetect>().SetTarget(playerScript);
                                    }
                                }
                                else
                                {
                                    player = Instantiate(FightSceneManager.Instance.TPrefab);
                                    playerScript = player.GetComponent<Player>();
                                    playerScript.Initialize();
                                    if ((Team == 1 && long.Parse(data.Split(' ')[1], CultureInfo.InvariantCulture) != playerId) || Team == 2)
                                    {
                                        playerScript.NameText.transform.localPosition = new Vector3(0, 0.148f, 0);
                                        playerScript.HealthBar.SetActive(true);
                                    }
                                    if (Team == 1 || Team == 2)
                                    {
                                        GameObject md = Instantiate(FightSceneManager.Instance.BoxMD);
                                        md.GetComponent<MegamapDetect>().enabled = true;
                                        md.GetComponent<MegamapDetect>().SetTarget(playerScript);
                                    }
                                }

                                if (long.Parse(data.Split(' ')[1], CultureInfo.InvariantCulture) == playerId)
                                {
                                    playerScript.Control();
                                    ActivateHUD();
                                    FightSceneManager.Instance.PlayerFollow.GetTarget(playerScript);
                                }

                                players[long.Parse(data.Split(' ')[1], CultureInfo.InvariantCulture)] = player;
                                playerScripts[long.Parse(data.Split(' ')[1], CultureInfo.InvariantCulture)] = playerScript;

                                string[] parameters = data.Split(' ');

                                playerScript.Setup(long.Parse(data.Split(' ')[1], CultureInfo.InvariantCulture),
                                    parameters[2], int.Parse(parameters[4], CultureInfo.InvariantCulture),
                                    int.Parse(parameters[5], CultureInfo.InvariantCulture) == 1,
                                    int.Parse(parameters[6], CultureInfo.InvariantCulture) == 1,
                                    int.Parse(parameters[7], CultureInfo.InvariantCulture) == 1,
                                    int.Parse(parameters[8], CultureInfo.InvariantCulture) == 1,
                                    int.Parse(parameters[11], CultureInfo.InvariantCulture) == 1,
                                    int.Parse(parameters[9], CultureInfo.InvariantCulture),
                                    int.Parse(parameters[10], CultureInfo.InvariantCulture), int.Parse(parameters[3]));

                                Vector2 spawnPos = new Vector2(0, 0);

                                if (playerScript.Team == 0)
                                {
                                    spawnPos = GameObject.Find("ctSpawn" + (ctSpawnIndex++).ToString(CultureInfo.InvariantCulture)).transform.position;
                                    if (GameObject.Find("ctSpawn" + ctSpawnIndex.ToString(CultureInfo.InvariantCulture)) == null)
                                        ctSpawnIndex = 0;

                                }
                                else
                                {
                                    spawnPos = GameObject.Find("tSpawn" + (tSpawnIndex++).ToString(CultureInfo.InvariantCulture)).transform.position;
                                    if (GameObject.Find("tSpawn" + tSpawnIndex.ToString(CultureInfo.InvariantCulture)) == null)
                                        tSpawnIndex = 0;
                                }

                                player.transform.position = spawnPos;

                                AlivePlayers.Add(playerScript);

                                Tab.Instance.AddPlayer(playerScript, parameters[2], int.Parse(parameters[12], CultureInfo.InvariantCulture), int.Parse(parameters[13], CultureInfo.InvariantCulture), playerScript.Team);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Log(ex.ToString() + "\n line 549 \n");
                    }
                }
                spawnPlayers = "";
                RoundData.Instance.Spawned = true;
                Tab.Instance.UpdateIndex();

                int blueRounds = int.Parse(FightSceneManager.Instance.BlueRounds.text, CultureInfo.InvariantCulture);
                int redRounds = int.Parse(FightSceneManager.Instance.RedRounds.text, CultureInfo.InvariantCulture);

                if (blueRounds + redRounds == 14)
                {
                    warmupObj = FightSceneManager.Instance.Warmup;

                    warmupObj.GetComponent<Text>().text = "Last Round Of The Half";
                    warmupObj.SetActive(true);
                    Invoke("StopWarmup", 5f);
                }
                else if (blueRounds + redRounds == 29)
                {
                    warmupObj = FightSceneManager.Instance.Warmup;

                    warmupObj.GetComponent<Text>().text = "Last Round";
                    warmupObj.SetActive(true);
                    Invoke("StopWarmup", 5f);
                }
                else if (blueRounds == 15 || redRounds == 15)
                {
                    warmupObj = FightSceneManager.Instance.Warmup;

                    warmupObj.GetComponent<Text>().text = "Match Point";
                    warmupObj.SetActive(true);
                    Invoke("StopWarmup", 5f);
                }
                if (gameStart)
                {
                    if (GameHost.Instance != null)
                        ConsoleCanvas.Instance.Console.MatchStartHost();
                    else ConsoleCanvas.Instance.Console.MatchStartClient();
                    gameStart = false;
                }
                ConsoleCanvas.Instance.Console.MatchRoundStart();

            }
        }
    }

    private void ActivateTeamSelection()
    {
        if (Input.GetKeyDown(KeyCode.N) && teamSelect != null && Chat.Instance.Focused == false && ConsoleCanvas.Instance.Content.activeInHierarchy == false)
        {
            teamSelect.SetActive(!teamSelect.activeInHierarchy);
        }
    }

    private void ActivateMegamap()
    {
        if (Input.GetKeyDown(KeyCode.M) && megamap != null && Chat.Instance.Focused == false && ConsoleCanvas.Instance.Content.activeInHierarchy == false)
        {
            megamap.SetActive(!megamap.activeInHierarchy);
        }
    }

    private bool InterpretLine(string line)
    {
        try
        {
            string data = line;

            string type = data.Split(' ')[0];
            if (type == "ReceiveID")
            {
                playerId = long.Parse(data.Split(' ')[1], CultureInfo.InvariantCulture);

                map = data.Split(' ')[2];

                if (Directory.Exists(Application.dataPath + "\\Maps\\" + map))
                {
                    SceneManager.LoadScene("OnlineWaitMenu");

                    if (File.Exists(Application.dataPath + "\\Maps\\" + map + "\\megamap.igd"))
                    {
                        string data2 = File.ReadAllText(Application.dataPath + "\\Maps\\" + map + "\\megamap.igd");
                        try
                        {
                            megamapSize = float.Parse(data2, CultureInfo.InvariantCulture);
                        }
                        catch
                        {

                        }
                    }

                    string mapData = File.ReadAllText(Application.dataPath + "\\Maps\\" + map + "\\data.igd");

                    int ctSpawnIndex = 0;
                    int tSpawnIndex = 0;

                    foreach (string mapLine in mapData.Split('\n'))
                    {
                        string[] parameters = mapLine.Split(' ');

                        switch (parameters[0])
                        {
                            case "Tile":
                                {
                                    GameObject obj = Instantiate(tile);

                                    parameters[1] = Application.dataPath + "\\" + parameters[1];

                                    obj.transform.position = new Vector3(float.Parse(parameters[2], CultureInfo.InvariantCulture), float.Parse(parameters[3], CultureInfo.InvariantCulture), float.Parse(parameters[4], CultureInfo.InvariantCulture));
                                    obj.transform.eulerAngles = new Vector3(float.Parse(parameters[11], CultureInfo.InvariantCulture), float.Parse(parameters[12], CultureInfo.InvariantCulture), float.Parse(parameters[13], CultureInfo.InvariantCulture));
                                    obj.transform.localScale = new Vector2(float.Parse(parameters[5], CultureInfo.InvariantCulture), float.Parse(parameters[6], CultureInfo.InvariantCulture));

                                    Texture2D tex = new Texture2D(1, 1);
                                    WWW www = new WWW(parameters[1]);
                                    www.LoadImageIntoTexture(tex);

                                    obj.GetComponent<SpriteRenderer>().sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

                                    obj.GetComponent<SpriteMask>().sprite = obj.GetComponent<SpriteRenderer>().sprite;

                                    if (bool.Parse(parameters[14]))
                                        obj.GetComponent<SpriteRenderer>().sprite.texture.filterMode = FilterMode.Point;

                                    if (bool.Parse(parameters[10]) == false)
                                    {
                                        obj.AddComponent<BoxCollider2D>();
                                        obj.AddComponent<BulletDestroyer>();
                                    }
                                    if (bool.Parse(parameters[8]) || bool.Parse(parameters[9]))
                                    {
                                        obj.AddComponent<BoxCollider2D>();
                                        obj.GetComponent<BoxCollider2D>().isTrigger = true;
                                        obj.AddComponent<BuyZone>();
                                    }
                                    if (bool.Parse(parameters[7]))
                                    {
                                        obj.AddComponent<BoxCollider2D>();
                                        obj.GetComponent<BoxCollider2D>().isTrigger = true;

                                        obj.tag = "BombSite";
                                    }

                                    obj.transform.parent = mapTransform.transform;

                                    if (obj.transform.position.z <= 10000)
                                        obj.GetComponent<SpriteRenderer>().sortingLayerName = "AbovePlayer";

                                    break;
                                }
                            case "CtSpawn":
                                {

                                    GameObject obj = new GameObject();

                                    obj.transform.name = "ctSpawn" + (ctSpawnIndex++).ToString(CultureInfo.InvariantCulture);

                                    obj.transform.parent = mapTransform.transform;

                                    obj.transform.position = new Vector3(float.Parse(parameters[1], CultureInfo.InvariantCulture), float.Parse(parameters[2], CultureInfo.InvariantCulture), 10001); ;



                                    break;
                                }
                            case "TSpawn":
                                {

                                    GameObject obj = new GameObject();

                                    obj.transform.name = "tSpawn" + (tSpawnIndex++).ToString(CultureInfo.InvariantCulture);

                                    obj.transform.parent = mapTransform.transform;

                                    obj.transform.position = new Vector3(float.Parse(parameters[1], CultureInfo.InvariantCulture), float.Parse(parameters[2], CultureInfo.InvariantCulture), 10001);

                                    break;
                                }
                        }
                    }
                }
                else
                {
                    GameObject.Find("ErrorText").GetComponent<Text>().text = "Error: Map " + map + " not found!";
                    if (GameHost.Instance)
                        Destroy(GameHost.Instance.gameObject);
                    Destroy(gameObject);
                }
            }
            else if (type.Contains("Conn"))
            {
                var message3 = client.CreateMessage();
                message3.Write("Connected");

                client.SendMessage(message3,
                NetDeliveryMethod.ReliableOrdered);
            }
            else if (type == "Disconnect")
            {
                Tab.Instance.RemovePlayer(data.Split(' ')[1]);
            }
            else if (type == "BanMessage")
            {
                GameObject clonedText = Instantiate(Resources.Load<GameObject>("Prefabs\\banText"));
                clonedText.GetComponentInChildren<Text>().text = "You are banned on this server.";
            }
            else if (type == "Kick" || type == "Ban")
            {
                Tab.Instance.RemovePlayer(data.Split(' ')[1]);

                if (data.Split(' ')[1] == currentName)
                {
                    if (type == "Kick")
                        Escape.Instance.Kick(string.Join(" ", data.Split(' ').Skip(2)));
                    else Escape.Instance.Ban(string.Join(" ", data.Split(' ').Skip(2)));
                }
            }
            else if (type == "TabPlayer")
            {
                Tab.Instance.AddPlayer(null, data.Split(' ')[1], 0, 0, int.Parse(data.Split(' ')[2], CultureInfo.InvariantCulture));
            }
            else if (type == "RoundStart")
            {
                Debug.Log("receivedMsg");
                roundStartExec += string.Join(" ", data.Split(' ').Skip(1)) + "\n";
            }
            else if (type == "ServerAlive")
            {
                timeBetweenResponses = 0;
            }
            else if (type == "PlayerCount")
            {
                try
                {
                    playerCount = GameObject.Find("PlayerCount").GetComponent<Text>();
                    playerCount.text = "Player Count: " + data.Split(' ')[1];
                }
                catch
                {

                }
            }
            else if (type == "LoadScene")
            {
                string[] parameters = data.Split(' ');

                if (data.Split(' ')[1] != "OnlineWaitMenu")

                    SceneManager.LoadScene(data.Split(' ')[1]);
            }
            else if (type == "Warmup")
            {
                FightSceneManager.Instance.Warmup.SetActive(true);
            }
            else if (type == "Disconnect" || (type.Contains("Server") && type.Contains("ServerAlive") == false))
            {
                SceneManager.LoadScene("OnlineLobby");

                if (GameHost.Instance != null)
                    Destroy(GameHost.Instance.gameObject);

                Destroy(gameObject);
            }
            else if (type == "StartGame")
            {
                string[] parameters = data.Split(' ');

                SceneManager.LoadScene(Config.FightSceneName);

                AlivePlayers = new List<Player>();
                Walls = new List<Wall>();

                gameStart = true;

                mapTransform.SetActive(true);

                ctSpawnIndex = 0;
                tSpawnIndex = 0;
            }
            else if (type == "SpawnPlayer")
            {
                try
                {
                    RoundData.Instance.Spawned = false;

                    spawnPlayers += line + "\n";
                }
                catch (Exception err)
                {
                    Debug.Log(err.ToString());
                }
            }
            else if (type == "Expl")
            {
                string[] parameters = data.Split(' ');

                GameObject vfx = Instantiate(FightSceneManager.Instance.HeExplosion);
                vfx.transform.position = new Vector2(float.Parse(parameters[1], CultureInfo.InvariantCulture), float.Parse(parameters[2], CultureInfo.InvariantCulture));
                vfx.transform.localScale *= 5;

                Destroy(vfx, 3f);
            }
            else if (type == "SetDeaths")
            {
                string[] parameters = data.Split(' ');

                long id = long.Parse(parameters[1], CultureInfo.InvariantCulture);


                Tab.Instance.ModifyDeaths(playerScripts[id], int.Parse(parameters[2], CultureInfo.InvariantCulture), playerScripts[id].Team);
            }
            else if (type == "SetKills")
            {
                string[] parameters = data.Split(' ');

                long id = long.Parse(parameters[1], CultureInfo.InvariantCulture);

                Tab.Instance.ModifyKills(playerScripts[id], int.Parse(parameters[2], CultureInfo.InvariantCulture), playerScripts[id].Team);
            }
            else if (type == "KillFeed")
            {
                string[] parameters = data.Split(' ');

                FindObjectOfType<KillFeed>().AddKillFeed(parameters[1], parameters[2], int.Parse(parameters[3], CultureInfo.InvariantCulture), parameters[4], parameters[5]);
            }
            else if (type == "PlayerInfo")
            {
                if (GameHost.Instance == null || (GameHost.Instance != null && RoundData.Instance.Spawned))
                {
                    string[] parameters = data.Split(' ');

                    long id = long.Parse(parameters[1], CultureInfo.InvariantCulture);

                    if (GameHost.Instance == null)
                    {
                        Vector2 playerPos = new Vector2(float.Parse(parameters[2], CultureInfo.InvariantCulture), float.Parse(parameters[3], CultureInfo.InvariantCulture));

                        playerScripts[id].UpdateInfo(playerPos, float.Parse(parameters[4], CultureInfo.InvariantCulture), float.Parse(parameters[5], CultureInfo.InvariantCulture), parameters[10], int.Parse(parameters[20], CultureInfo.InvariantCulture), int.Parse(parameters[21], CultureInfo.InvariantCulture));
                    }

                    playerScripts[id].UpdateGunInfo(float.Parse(parameters[10], CultureInfo.InvariantCulture), parameters[9], int.Parse(parameters[18], CultureInfo.InvariantCulture), int.Parse(parameters[17], CultureInfo.InvariantCulture), parameters[5], parameters[10]);

                    if (playerScripts[id].Controlling)
                    {
                        FightSceneManager.Instance.CashText.GetComponent<Text>().text = "$" + parameters[11];

                        FightSceneManager.Instance.BulletText.text = parameters[17] + "/" + parameters[18];
                        int gunType = int.Parse(parameters[12], CultureInfo.InvariantCulture);

                        if (gunType != -1)
                        {
                            FightSceneManager.Instance.WeaponUIObjects[0].SetActive(true);
                            FightSceneManager.Instance.WeaponImages[0].sprite = FightSceneManager.Instance.Drops[gunType].GetComponent<SpriteRenderer>().sprite;
                        }
                        else FightSceneManager.Instance.WeaponUIObjects[0].SetActive(false);
                        int hasPistol = int.Parse(parameters[13], CultureInfo.InvariantCulture);
                        if (hasPistol == 1)
                        {
                            FightSceneManager.Instance.WeaponUIObjects[1].SetActive(true);
                        }
                        else FightSceneManager.Instance.WeaponUIObjects[1].SetActive(false);

                        int hasHe = int.Parse(parameters[14], CultureInfo.InvariantCulture);
                        if (hasHe == 1)
                        {
                            FightSceneManager.Instance.WeaponUIObjects[3].SetActive(true);
                        }
                        else FightSceneManager.Instance.WeaponUIObjects[3].SetActive(false);

                        int hasFlash = int.Parse(parameters[15], CultureInfo.InvariantCulture);
                        if (hasFlash == 1)
                        {
                            FightSceneManager.Instance.WeaponUIObjects[4].SetActive(true);
                        }
                        else FightSceneManager.Instance.WeaponUIObjects[4].SetActive(false);

                        int hasSmoke = int.Parse(parameters[16], CultureInfo.InvariantCulture);
                        if (hasSmoke == 1)
                        {
                            FightSceneManager.Instance.WeaponUIObjects[5].SetActive(true);
                        }
                        else FightSceneManager.Instance.WeaponUIObjects[5].SetActive(false);

                        int hasBomb = int.Parse(parameters[22], CultureInfo.InvariantCulture);
                        if (hasBomb == 1)
                        {
                            FightSceneManager.Instance.WeaponUIObjects[6].SetActive(true);
                        }
                        else FightSceneManager.Instance.WeaponUIObjects[6].SetActive(false);

                        int hasWall = int.Parse(parameters[23], CultureInfo.InvariantCulture);
                        if (hasWall == 1)
                        {
                            FightSceneManager.Instance.WeaponUIObjects[7].SetActive(true);
                        }
                        else FightSceneManager.Instance.WeaponUIObjects[7].SetActive(false);
                    }
                    else
                    {
                        if (setTime == false)
                        {
                            gameTime = int.Parse(parameters[19], CultureInfo.InvariantCulture);

                            setTime = true;
                        }
                    }


                    if (GameHost.Instance == null)
                    {
                        playerScripts[id].UpdateOther(int.Parse(parameters[11], CultureInfo.InvariantCulture), int.Parse(parameters[9], CultureInfo.InvariantCulture), new Vector2(float.Parse(parameters[6], CultureInfo.InvariantCulture), float.Parse(parameters[7], CultureInfo.InvariantCulture)), parameters[8]);
                    }
                }
            }
            else if (type == "Graffiti")
            {
                string[] parameters = data.Split(' ');

                GameObject grafiti = Instantiate(FightSceneManager.Instance.Graffities[int.Parse(parameters[3], CultureInfo.InvariantCulture) - 1]);
                grafiti.transform.position = new Vector2(float.Parse(parameters[1], CultureInfo.InvariantCulture), float.Parse(parameters[2], CultureInfo.InvariantCulture));
                Destroy(grafiti, 20f);
            }
            else if (type == "Time")
            {
                string[] parameters = data.Split(' ');

                int time = int.Parse(parameters[1], CultureInfo.InvariantCulture);
                try
                {
                    if (time % 60 < 10)
                        timeObj.GetComponent<Text>().text = (time / 60).ToString(CultureInfo.InvariantCulture) + ":0" + (time % 60).ToString(CultureInfo.InvariantCulture);
                    else timeObj.GetComponent<Text>().text = (time / 60).ToString(CultureInfo.InvariantCulture) + ":" + (time % 60).ToString(CultureInfo.InvariantCulture);
                }
                catch
                {
                    timeObj = GameObject.Find("RoundTime");
                }
            }
            else if (type == "Bullet" && GameHost.Instance == null)
            {
                string[] parameters = data.Split(' ');

                GameObject newBullet = Instantiate(FightSceneManager.Instance.BulletPrefab);
                newBullet.transform.position = new Vector2(float.Parse(parameters[1], CultureInfo.InvariantCulture), float.Parse(parameters[2], CultureInfo.InvariantCulture));
                newBullet.GetComponent<Rigidbody2D>().velocity = new Vector2(float.Parse(parameters[3], CultureInfo.InvariantCulture), float.Parse(parameters[4], CultureInfo.InvariantCulture));
                newBullet.transform.localEulerAngles = new Vector3(0, 0, float.Parse(parameters[5], CultureInfo.InvariantCulture));

                newBullet.transform.name = parameters[6];

                if (parameters.Length == 8)
                    Destroy(newBullet, 0.5f);
            }
            else if (type == "HE" && GameHost.Instance == null)
            {
                string[] parameters = data.Split(' ');

                GameObject newGren = Instantiate(FightSceneManager.Instance.HePrefab);
                newGren.transform.position = new Vector2(float.Parse(parameters[1], CultureInfo.InvariantCulture), float.Parse(parameters[2], CultureInfo.InvariantCulture));
                newGren.GetComponent<Rigidbody2D>().velocity = new Vector2(float.Parse(parameters[3], CultureInfo.InvariantCulture), float.Parse(parameters[4], CultureInfo.InvariantCulture));
                newGren.transform.localEulerAngles = new Vector3(0, 0, float.Parse(parameters[5], CultureInfo.InvariantCulture));

                newGren.GetComponent<Grenade>().Invoke("Detonate", 1f);
            }
            else if (type == "Flash" && GameHost.Instance == null)
            {
                string[] parameters = data.Split(' ');

                GameObject newGren = Instantiate(FightSceneManager.Instance.FlashPrefab);
                newGren.transform.position = new Vector2(float.Parse(parameters[1], CultureInfo.InvariantCulture), float.Parse(parameters[2], CultureInfo.InvariantCulture));
                newGren.GetComponent<Rigidbody2D>().velocity = new Vector2(float.Parse(parameters[3], CultureInfo.InvariantCulture), float.Parse(parameters[4], CultureInfo.InvariantCulture));
                newGren.transform.localEulerAngles = new Vector3(0, 0, float.Parse(parameters[5], CultureInfo.InvariantCulture));

                newGren.GetComponent<Grenade>().Invoke("Detonate", 1f);
            }
            else if (type == "Smoke" && GameHost.Instance == null)
            {
                string[] parameters = data.Split(' ');

                GameObject newGren = Instantiate(FightSceneManager.Instance.SmokePrefab);
                newGren.transform.position = new Vector2(float.Parse(parameters[1], CultureInfo.InvariantCulture), float.Parse(parameters[2], CultureInfo.InvariantCulture));
                newGren.GetComponent<Rigidbody2D>().velocity = new Vector2(float.Parse(parameters[3], CultureInfo.InvariantCulture), float.Parse(parameters[4], CultureInfo.InvariantCulture));
                newGren.transform.localEulerAngles = new Vector3(0, 0, float.Parse(parameters[5], CultureInfo.InvariantCulture));


                newGren.GetComponent<Grenade>().Invoke("Detonate", 1f);
            }
            else if (type == "WallGren" && GameHost.Instance == null)
            {
                string[] parameters = data.Split(' ');

                GameObject newGren = Instantiate(FightSceneManager.Instance.WallPrefab);
                newGren.transform.position = new Vector2(float.Parse(parameters[1], CultureInfo.InvariantCulture), float.Parse(parameters[2], CultureInfo.InvariantCulture));
                newGren.GetComponent<Rigidbody2D>().velocity = new Vector2(float.Parse(parameters[3], CultureInfo.InvariantCulture), float.Parse(parameters[4], CultureInfo.InvariantCulture));
                newGren.transform.localEulerAngles = new Vector3(0, 0, float.Parse(parameters[5], CultureInfo.InvariantCulture));


                newGren.GetComponent<Grenade>().Invoke("Detonate", 1f);
            }
            else if (type == "FlashScreen" && GameHost.Instance == null)
            {
                string[] parameters = data.Split(' ');

                Vector2 flashPoint = new Vector2(float.Parse(parameters[1], CultureInfo.InvariantCulture), float.Parse(parameters[2], CultureInfo.InvariantCulture));

                Vector2 cameraPoint = Camera.main.WorldToViewportPoint(flashPoint);

                if (cameraPoint.x > 0 && cameraPoint.x < 1 && cameraPoint.y > 0 && cameraPoint.y < 1)
                {
                    if (FightSceneManager.Instance.CameraAnimator.GetCurrentAnimatorStateInfo(0).IsName("flash"))
                        FightSceneManager.Instance.CameraAnimator.Play("flash2");
                    else FightSceneManager.Instance.CameraAnimator.Play("flash");
                    FightSceneManager.Instance.CameraAnimator.speed = ((float)((int)(Vector2.Distance(flashPoint, Camera.main.transform.position)) + 1)) / 6;
                }
            }
            else if (type == "SmokeVfx" && GameHost.Instance == null)
            {
                string[] parameters = data.Split(' ');

                GameObject newVfx = Instantiate(FightSceneManager.Instance.SmokeVfx);
                newVfx.transform.position = new Vector2(float.Parse(parameters[1], CultureInfo.InvariantCulture), float.Parse(parameters[2], CultureInfo.InvariantCulture));
                newVfx.GetComponent<Smoke>().Activate();
            }
            else if (type == "Wall")
            {
                string[] parameters = data.Split(' ');

                GameObject newVfx = Instantiate(FightSceneManager.Instance.Wall);
                newVfx.transform.position = new Vector2(float.Parse(parameters[1], CultureInfo.InvariantCulture), float.Parse(parameters[2], CultureInfo.InvariantCulture));
                newVfx.transform.localEulerAngles = new Vector3(0, 0, float.Parse(parameters[3], CultureInfo.InvariantCulture));
                newVfx.transform.name = parameters[4];

                Walls.Add(newVfx.GetComponent<Wall>());
            }
            else if (type == "Death")
            {
                string[] parameters = data.Split(' ');

                long id = long.Parse(parameters[1], CultureInfo.InvariantCulture);

                if (parameters.Length > 2 && id == playerId)
                {

                    playerScripts[id].DeathData(parameters[2], parameters[3], parameters[4]);
                    if (playerScripts[id].Team == 0)
                        ConsoleCanvas.Instance.Console.MatchCTDead(parameters[4], playerScripts[id].Name);
                    else ConsoleCanvas.Instance.Console.MatchTDead(parameters[4], playerScripts[id].Name);

                }

                playerScripts[id].TriggerDeath();

                Tab.Instance.ModifyAlive(playerScripts[id], false, playerScripts[id].Team);
            }
            else if (type == "Drop")
            {
                string[] parameters = data.Split(' ');

                GameObject drop = Instantiate(FightSceneManager.Instance.Drops[int.Parse(parameters[1], CultureInfo.InvariantCulture)]);

                drop.GetComponent<Drop>().Setup(int.Parse(parameters[7], CultureInfo.InvariantCulture), int.Parse(parameters[8], CultureInfo.InvariantCulture));
                drop.transform.position = new Vector2(float.Parse(parameters[2], CultureInfo.InvariantCulture), float.Parse(parameters[3], CultureInfo.InvariantCulture));
                drop.GetComponent<Rigidbody2D>().velocity = new Vector2(float.Parse(parameters[4], CultureInfo.InvariantCulture), float.Parse(parameters[5], CultureInfo.InvariantCulture));

                drop.transform.name = "drop" + parameters[6];

                drop.transform.localScale *= 10;
            }
            else if (type == "Destroy")
            {
                string[] parameters = data.Split(' ');

                GameObject obj = GameObject.Find(parameters[1]);

                if (parameters[1].Contains("wall"))
                {
                    GameObject vfx = Instantiate(FightSceneManager.Instance.WallVfx);
                    vfx.transform.position = GameObject.Find(parameters[1]).transform.position;
                    Destroy(vfx, 2f);

                    Walls.Remove(obj.GetComponent<Wall>());
                }

                Destroy(obj);
            }
            else if (type == "Plant")
            {
                string[] parameters = data.Split(' ');

                if (FindObjectsOfType<Bomb>().Length == 1)
                {
                    GameObject bombClone = Instantiate(FightSceneManager.Instance.Bomb);
                    bombClone.transform.position = new Vector2(float.Parse(parameters[1], CultureInfo.InvariantCulture), float.Parse(parameters[2], CultureInfo.InvariantCulture));

                    ConsoleCanvas.Instance.Console.MatchBombPlanted();

                    if (GameHost.Instance != null)
                    {
                        GameHost.Instance.bombObject = bombClone;
                    }
                }
            }
            else if (type == "Defusing")
            {
                string[] parameters = data.Split(' ');

                long id = long.Parse(parameters[1], CultureInfo.InvariantCulture);

                if (playerScripts[id].Controlling)
                {
                    FightSceneManager.Instance.CanvasAnimator.Play("defusie");
                }
            }
            else if (type == "Visibility")
            {
                string[] parameters = data.Split(' ');

                long id = long.Parse(parameters[1], CultureInfo.InvariantCulture);

                if (players[id] != null)
                {
                    playerScripts[id].SetVisibility(bool.Parse(parameters[2]));
                }
            }
            else if (type == "StopDef")
            {
                string[] parameters = data.Split(' ');

                long id = long.Parse(parameters[1], CultureInfo.InvariantCulture);

                if (playerScripts[id].Controlling)
                {
                    FightSceneManager.Instance.CanvasAnimator.Play("idle");
                }
            }
            else if (type == "Defused")
            {
                FightSceneManager.Instance.CanvasAnimator.Play("idle");
                ConsoleCanvas.Instance.Console.MatchBombDefused();
            }
            else if (type == "Rounds")
            {
                try
                {
                    string[] parameters = data.Split(' ');

                    FightSceneManager.Instance.RedRounds.text = parameters[1];
                    FightSceneManager.Instance.BlueRounds.text = parameters[2];

                }
                catch
                {

                }
            }
            else if (type == "Count")
            {
                try
                {
                    string[] parameters = data.Split(' ');


                    FightSceneManager.Instance.RedCount.text = parameters[1];
                    FightSceneManager.Instance.BlueCount.text = parameters[2];
                }
                catch
                {

                }
            }
            else if (type == "Play")
            {
                string[] parameters = data.Split(' ');

                if (parameters.Length == 2)
                {
                    GameObject obj = new GameObject();
                    obj.transform.position = Vector3.zero;
                    obj.AddComponent<AudioSource>();
                    obj.GetComponent<AudioSource>().clip = SoundArchive.Instance.sounds[int.Parse(parameters[1], CultureInfo.InvariantCulture)];
                    obj.GetComponent<AudioSource>().Play();
                    Destroy(obj, obj.GetComponent<AudioSource>().clip.length + 1);
                    if (parameters[1] == "15")
                    {
                        ConsoleCanvas.Instance.Console.MatchRoundEnd();
                    }
                }
                else if (parameters.Length == 4)
                {
                    GameObject obj = new GameObject();
                    obj.transform.position = new Vector3(float.Parse(parameters[2], CultureInfo.InvariantCulture), float.Parse(parameters[3], CultureInfo.InvariantCulture), 0);

                    obj.AddComponent<AudioSource>();
                    obj.GetComponent<AudioSource>().clip = SoundArchive.Instance.sounds[int.Parse(parameters[1], CultureInfo.InvariantCulture)];

                    obj.GetComponent<AudioSource>().spatialBlend = 1f;
                    obj.GetComponent<AudioSource>().spread = 360f;
                    obj.GetComponent<AudioSource>().rolloffMode = AudioRolloffMode.Linear;
                    obj.GetComponent<AudioSource>().dopplerLevel = 0f;
                    obj.GetComponent<AudioSource>().maxDistance = 30f;
                    obj.GetComponent<AudioSource>().minDistance = 5f;

                    if (parameters[1] == "14" || parameters[1] == "17")
                    {
                        obj.GetComponent<AudioSource>().maxDistance = 60f;
                        obj.GetComponent<AudioSource>().minDistance = 10f;
                    }
                    if (parameters[1] == "25" || parameters[1] == "26")
                    {
                        obj.GetComponent<AudioSource>().maxDistance = 45f;
                        obj.GetComponent<AudioSource>().minDistance = 7.5f;
                    }


                    List<AudioSource> asList = new List<AudioSource>();

                    foreach (AudioSource a in FindObjectsOfType<AudioSource>())
                    {
                        if (Vector2.Distance(a.transform.position, Camera.main.transform.position) < 10f && a.clip == obj.GetComponent<AudioSource>().clip && a != obj.GetComponent<AudioSource>())
                        {
                            asList.Add(a);
                        }
                    }

                    for (int i = asList.Count - 20; i >= 0; i--)
                    {
                        Destroy(asList[i].gameObject);
                    }

                    obj.GetComponent<AudioSource>().Play();
                    Destroy(obj, obj.GetComponent<AudioSource>().clip.length + 1);
                }
                else if (parameters.Length == 5)
                {
                    GameObject obj = new GameObject();

                    long id = long.Parse(parameters[4], CultureInfo.InvariantCulture);
                    obj.transform.parent = players[id].transform;

                    obj.transform.position = new Vector3(float.Parse(parameters[2], CultureInfo.InvariantCulture), float.Parse(parameters[3], CultureInfo.InvariantCulture), 0);
                    obj.AddComponent<AudioSource>();
                    if (parameters[1] == "14")
                        obj.GetComponent<AudioSource>().volume = 0.75f;

                    obj.GetComponent<AudioSource>().clip = SoundArchive.Instance.sounds[int.Parse(parameters[1], CultureInfo.InvariantCulture)];

                    if (parameters[1] == "13" || parameters[1] == "12")
                    {
                        obj.GetComponent<AudioSource>().maxDistance = 60f;
                        obj.GetComponent<AudioSource>().minDistance = 20f;
                    }

                    List<AudioSource> asList = new List<AudioSource>();

                    foreach (AudioSource a in FindObjectsOfType<AudioSource>())
                    {
                        if (Vector2.Distance(a.transform.position, Camera.main.transform.position) < 10f && a.clip == obj.GetComponent<AudioSource>().clip && a != obj.GetComponent<AudioSource>())
                        {
                            asList.Add(a);
                        }
                    }

                    for (int i = asList.Count - 20; i >= 0; i--)
                    {
                        Destroy(asList[i].gameObject);
                    }


                    obj.GetComponent<AudioSource>().Play();
                    Destroy(obj, obj.GetComponent<AudioSource>().clip.length + 1);
                }
            }
            else if (type == "Msg")
            {
                string[] parameters = data.Split(' ');

                if (int.Parse(parameters[2], CultureInfo.InvariantCulture) == Team)
                {
                    string msg = string.Join(" ", parameters.Skip(3));



                    Chat.Instance.AddMessage(msg);
                }
            }
            else if (type == "All")
            {
                string[] parameters = data.Split(' ');

                string msg = "(All) " + string.Join(" ", parameters.Skip(3));

                Chat.Instance.AddMessage(msg);
            }
            else if (type == "System")
            {
                string[] parameters = data.Split(' ');

                string msg = string.Join(" ", parameters.Skip(3));

                Chat.Instance.AddMessage(msg);
            }
            else if (type == "FinishGame")
            {
                string[] parameters = data.Split(' ');

                int team = int.Parse(parameters[1], CultureInfo.InvariantCulture);

                if (team == -1)
                {
                    FindObjectOfType<WinLoseAnim>().InterpretText("Draw");
                }
                else if (team == this.Team)
                {
                    FindObjectOfType<WinLoseAnim>().InterpretText("Victory");
                }
                else
                {
                    FindObjectOfType<WinLoseAnim>().InterpretText("Defeat");
                }
            }
            else if (type == "CT" || type == "T")
            {
                FindObjectOfType<WinLoseAnim>().InterpretText(line);
            }
            else if (type == "Whisper")
            {
                string[] parameters = data.Split(' ');

                Debug.Log(data);

                if (parameters[5] == currentName)
                {
                    string msg = "<color=grey>(From " + new string(parameters[3].Split('<', '>')[2].ToArray()) + ")</color> " + string.Join(" ", parameters.Skip(6));

                    lastWhisper = new string(parameters[3].Take(1).ToArray());

                    Chat.Instance.AddMessage(msg);
                }

                if (parameters[3].Split('<', '>')[2] == currentName)
                {
                    string msg = "<color=grey>(To " + parameters[5] + ")</color> " + string.Join(" ", parameters.Skip(6));

                    lastWhisper = new string(parameters[3].Take(1).ToArray());

                    Chat.Instance.AddMessage(msg);
                }
            }
            else if (type == "SrvWhisper")
            {
                string[] parameters = data.Split(' ');

                if (parameters[1] == currentName || (parameters[1] == "$allT" && Team == 1) || (parameters[1] == "$allCT" && Team == 0) || parameters[1] == "$allPlayers")
                {
                    string msg = "<color=grey>(From $server)</color> " + string.Join(" ", parameters.Skip(2));

                    Chat.Instance.AddMessage(msg);
                }
            }
            else if (type == "ChangeTeam")
            {
                string[] parameters = data.Split(' ');

                if (parameters[1].Trim().Equals(currentName))
                {
                    Debug.Log(parameters[1] + " " + parameters[2]);

                    players[playerId].GetComponent<Player>().ChangeTeam(parameters[2]);

                    if (parameters[2].Trim().Equals("ct"))
                        Team = 0;
                    if (parameters[2].Trim().Equals("t"))
                        Team = 1;


                }
                else if (parameters[1].Trim().Equals("$allT"))
                {
                    foreach (GameObject player in players.Values)
                    {
                        if (player != null)
                            if (player.GetComponent<Player>().Team == 1)
                            {
                                player.GetComponent<Player>().ChangeTeam(parameters[2]);
                                if (player.GetComponent<Player>().ID == playerId)
                                {
                                    if (parameters[2].Trim().Equals("ct"))
                                        Team = 0;
                                    if (parameters[2].Trim().Equals("t"))
                                        Team = 1;
                                }
                            }
                    }
                }
                else if (parameters[1].Trim().Equals("$allCT"))
                {
                    foreach (GameObject player in players.Values)
                    {
                        if (player != null)
                            if (player.GetComponent<Player>().Team == 0)
                            {
                                player.GetComponent<Player>().ChangeTeam(parameters[2]);
                                if (player.GetComponent<Player>().ID == playerId)
                                {
                                    if (parameters[2].Trim().Equals("ct"))
                                        Team = 0;
                                    if (parameters[2].Trim().Equals("t"))
                                        Team = 1;
                                }
                            }
                    }
                }
                else if (parameters[1].Trim().Equals("$allPlayers"))
                {
                    foreach (GameObject player in players.Values)
                    {
                        if (player != null)
                        {
                            player.GetComponent<Player>().ChangeTeam(parameters[2]);
                            if (player.GetComponent<Player>().ID == playerId)
                            {
                                if (parameters[2].Trim().Equals("ct"))
                                    Team = 0;
                                if (parameters[2].Trim().Equals("t"))
                                    Team = 1;
                            }
                        }
                    }
                }
            }
            else if (type == "Teleport")
            {
                string[] parameters = data.Split(' ');

                if (parameters[1].Trim().Equals(currentName))
                {
                    if (parameters[2].Trim().Equals("ctBase"))
                    {
                        players[playerId].transform.position = GameObject.Find("ctSpawn0").transform.position;
                    }
                    else if (parameters[2].Trim().Equals("tBase"))
                    {
                        players[playerId].transform.position = GameObject.Find("tSpawn0").transform.position;
                    }
                    else if (parameters[2].Trim().Equals("bombSite"))
                    {
                        players[playerId].transform.position = GameObject.FindGameObjectWithTag("BombSite").transform.position;
                    }
                }
                else if (parameters[1].Trim().Equals("$allT"))
                {
                    if (parameters[2].Trim().Equals("ctBase"))
                    {
                        foreach (GameObject player in players.Values)
                        {
                            if (player != null)
                                if (player.GetComponent<Player>().Team == 1)
                                {
                                    player.transform.position = GameObject.Find("ctSpawn0").transform.position;
                                }
                        }
                    }
                    else if (parameters[2].Trim().Equals("tBase"))
                    {
                        foreach (GameObject player in players.Values)
                        {
                            if (player != null)
                                if (player.GetComponent<Player>().Team == 1)
                                {
                                    player.transform.position = GameObject.Find("tSpawn0").transform.position;
                                }
                        }
                    }
                    else if (parameters[2].Trim().Equals("bombSite"))
                    {
                        foreach (GameObject player in players.Values)
                        {
                            if (player != null)
                                if (player.GetComponent<Player>().Team == 1)
                                {
                                    player.transform.position = GameObject.FindGameObjectWithTag("BombSite").transform.position;
                                }
                        }
                    }
                }
                else if (parameters[1].Trim().Equals("$allCT"))
                {
                    if (parameters[2].Trim().Equals("ctBase"))
                    {
                        foreach (GameObject player in players.Values)
                        {
                            if (player != null)
                                if (player.GetComponent<Player>().Team == 0)
                                {
                                    player.transform.position = GameObject.Find("ctSpawn0").transform.position;
                                }
                        }
                    }
                    else if (parameters[2].Trim().Equals("tBase"))
                    {
                        foreach (GameObject player in players.Values)
                        {
                            if (player != null)
                                if (player.GetComponent<Player>().Team == 0)
                                {
                                    player.transform.position = GameObject.Find("tSpawn0").transform.position;
                                }
                        }
                    }
                    else if (parameters[2].Trim().Equals("bombSite"))
                    {
                        foreach (GameObject player in players.Values)
                        {
                            if (player != null)
                                if (player.GetComponent<Player>().Team == 0)
                                {
                                    player.transform.position = GameObject.FindGameObjectWithTag("BombSite").transform.position;
                                }
                        }
                    }
                }
                else if (parameters[1].Trim().Equals("$allPlayers"))
                {
                    Debug.Log("smecheri2");
                    if (parameters[2].Trim().Equals("ctBase"))
                    {
                        foreach (GameObject player in players.Values)
                        {
                            if (player != null)
                                player.transform.position = GameObject.Find("ctSpawn0").transform.position;

                        }
                    }
                    else if (parameters[2].Trim().Equals("tBase"))
                    {
                        foreach (GameObject player in players.Values)
                        {
                            if (player != null)
                                player.transform.position = GameObject.Find("tSpawn0").transform.position;

                        }
                    }
                    else if (parameters[2].Trim().Equals("bombSite"))
                    {
                        Debug.Log("smecheri3");
                        foreach (GameObject player in players.Values)
                        {
                            if (player != null)
                                player.transform.position = GameObject.FindGameObjectWithTag("BombSite").transform.position;

                        }
                    }
                }
            }
            else if (type == "NewName")
            {
                string[] parameters = data.Split(' ');

                long id = long.Parse(parameters[1], CultureInfo.InvariantCulture);

                if (id == playerId)
                    currentName = parameters[3];

                try
                {
                    if (players[id + 1] != null)
                        playerScripts[id + 1].SetName(parameters[3]);
                }
                catch
                {

                }

                Tab.Instance.ModifyName(parameters[2], parameters[3]);

            }
            else if (type == "NewLobbyName")
            {
                string[] parameters = data.Split(' ');

                long id = long.Parse(parameters[1], CultureInfo.InvariantCulture);

                if (id == playerId)
                    currentName = parameters[3];

                try
                {
                    if (players[id + 1] != null)
                        playerScripts[id + 1].SetName(parameters[3]);
                }
                catch
                {

                }

                try
                {
                    Tab.Instance.ModifyName(parameters[2], parameters[3]);
                }
                catch
                {

                }

            }
        }
        catch (Exception err)
        {
            Debug.Log(err.ToString());
        }

        return setTime;
    }

    private void StopWarmup()
    {
        warmupObj.SetActive(false);
    }

    public void ClientSetName(string name)
    {
        currentName = name;

        SendPlayerName();
    }

    private void SendPlayerName()
    {
        if (playerName != null)
        {
            if (playerName.text != currentName)
            {
                currentName = playerName.text;

                message += "NewPlayerName " + playerId.ToString(CultureInfo.InvariantCulture) + ' ' + playerName.text + "\n";
            }
        }
        else
        {
            message += "NewPlayerName " + playerId.ToString(CultureInfo.InvariantCulture) + ' ' + currentName + "\n";
        }
    }

    private void SendPlayerTeam()
    {
        message += "NewPlayerTeam " + playerId.ToString(CultureInfo.InvariantCulture) + ' ' + Team.ToString(CultureInfo.InvariantCulture) + "\n";
    }

    private void OnDestroy()
    {
        Disconnect();
    }

    private void OnApplicationQuit()
    {
        Disconnect();
    }

    private IEnumerator MessageQueue(int ticks)
    {
        while (true)
        {
            yield return new WaitForSeconds(1f / ticks);

            SendMessage();
        }
    }

    private void SendMessage()
    {
        try
        {
            this.message += "ClientAlive " + playerId.ToString(CultureInfo.InvariantCulture) + "\n";

            if (timeObj == null)
                timeObj = GameObject.Find("RoundTime");

            UpdateBuyMenuText();

            this.message += nadeMessage + "\n";
            this.message += shootMessage + "\n";
            this.message += targetMessage + "\n";
            this.message += switchWeaponMessage;
            this.message += buyMessage;
            this.message += reloadMessage;
            this.message += defuseMessage;
            this.message += stopDefuseMessage;
            this.message += dropTargetMessage;
            this.message += shiftMessage;
            this.message += noShiftMessage;
            this.message += throwMessage;
            this.message += scrollUpMessage;
            this.message += scrollDownMessage;
            this.message += graffitiMessage;
            var message = client.CreateMessage();
            message.Write(this.message);
            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
            this.message = "";
            nadeMessage = "";
            targetMessage = "";
            shootMessage = "";
            buyMessage = "";
            switchWeaponMessage = "";
            reloadMessage = "";
            defuseMessage = "";
            stopDefuseMessage = "";
            dropTargetMessage = "";
            noShiftMessage = "";
            shiftMessage = "";
            throwMessage = "";
            scrollDownMessage = "";
            scrollUpMessage = "";
            graffitiMessage = "";
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());

            message = "";
        }
    }

    private void SendConnectionMessage()
    {
        message += "NewPlayer" + "\n";
    }

    #endregion
}