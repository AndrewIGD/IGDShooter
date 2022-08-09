using System.Collections;
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

    public Dictionary<string, GameObject> players;

    public Dictionary<string, Player> playerScripts;

    public Text playerCount;

    public InputField playerName;

    public Button playerTeam;

    public NetClient client;

    public string currentName = "NewPlayer";

    public string lastWhisper = "";

    public string ip;

    public float megamapSize = 300;

    public string playerId;

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
        /*if (FindObjectsOfType<GameClient>().Length >= 2 && GameHost.Instance == null)
        {
            Destroy(gameObject);

            return;
        }*/

        DontDestroyOnLoad(gameObject);

        MatchData.PlayerData = new Dictionary<string, PlayerData>();

        Instance = this;

        try
        {
            players = new Dictionary<string, GameObject>();
            playerScripts = new Dictionary<string, Player>();

            /*config = new NetPeerConfiguration("IGDShooter")
            {
                //EnableUPnP = true,
                AcceptIncomingConnections = true,
                PingInterval = 1f,
                ResendHandshakeInterval = 1f,
                MaximumHandshakeAttempts = 15,
                ConnectionTimeout = 1000f,
                ReceiveBufferSize = PlayerPrefs.GetInt("ClientReceiveBuffer", 3072),
                SendBufferSize = PlayerPrefs.GetInt("ClientSendBuffer", 1024)
            };

            client = new NetClient(config);
            client.Start();

            client.Connect(host: ip, port: 25565);

            Invoke("TestConnection", 3.5f);

            Invoke("SendConnectionMessage", 0.25f);

            StartCoroutine(MessageQueue(64));*/

            Network.Instance.OnMessagesReceived += ProcessMessages;
        }
        catch (Exception err)
        {
            GameObject.Find("ErrorText").GetComponent<Text>().text = "Error: Invalid IP!";
        }
    }

    private void ProcessMessages(object[] objects, string senderId)
    {
        for (int i = 0; i < objects.Length; i++)
        {
            switch (objects[i])
            {
                case PlayerCount playerCountPacket:
                    {
                        if (playerCount == null)
                            playerCount = GameObject.Find("PlayerCount").GetComponent<Text>();

                        playerCount.text = "Connected Players: " + playerCountPacket.Number;

                        break;
                    }
                case SystemMessage systemMessage:
                    {
                        Chat.Instance.AddMessage(systemMessage.Data);

                        break;
                    }
                case ServerNewPlayerName serverNewPlayerName:
                    {
                        if (serverNewPlayerName.ID == playerId)
                            currentName = serverNewPlayerName.Data;

                        if (players[serverNewPlayerName.ID] != null)
                            playerScripts[serverNewPlayerName.ID].SetName(serverNewPlayerName.Data);

                        Tab.Instance.ModifyName(serverNewPlayerName.ID, serverNewPlayerName.Data);

                        //Modificare nume in functie de senderId ^^^^^^^

                        break;
                    }
                case Play play:
                    {

                        if (play.Position.HasValue == false)
                        {
                            GameObject obj = new GameObject();
                            obj.transform.position = Vector3.zero;

                            AudioSource source = obj.AddComponent<AudioSource>();

                            source.clip = SoundArchive.Instance.sounds[play.SoundID];
                            source.Play();
                            Destroy(obj, obj.GetComponent<AudioSource>().clip.length + 1);

                            if (play.SoundID == 15)
                            {
                                ConsoleCanvas.Instance.Console.MatchRoundEnd();
                            }
                        }
                        else if (play.PlayerID == null)
                        {
                            GameObject obj = new GameObject();
                            obj.transform.position = play.Position.Value;

                            AudioSource source = obj.AddComponent<AudioSource>();
                            source.clip = SoundArchive.Instance.sounds[play.SoundID];

                            source.spatialBlend = 1f;
                            source.spread = 360f;
                            source.rolloffMode = AudioRolloffMode.Linear;
                            source.dopplerLevel = 0f;
                            source.maxDistance = 30f;
                            source.minDistance = 5f;

                            if (play.SoundID == 14 || play.SoundID == 17)
                            {
                                source.maxDistance = 60f;
                                source.minDistance = 10f;
                            }

                            if (play.SoundID == 25 || play.SoundID == 26)
                            {
                                source.maxDistance = 45f;
                                source.minDistance = 7.5f;
                            }

                            List<AudioSource> asList = new List<AudioSource>();

                            foreach (AudioSource a in FindObjectsOfType<AudioSource>())
                            {
                                if (Vector2.Distance(a.transform.position, Camera.main.transform.position) < 10f && a.clip == source.clip && a != source)
                                {
                                    asList.Add(a);
                                }
                            }

                            for (int k = asList.Count - 20; k >= 0; k--)
                            {
                                Destroy(asList[k].gameObject);
                            }

                            source.Play();
                            Destroy(obj, source.clip.length + 1);
                        }
                        else
                        {
                            GameObject obj = new GameObject();

                            obj.transform.parent = players[play.PlayerID].transform;

                            obj.transform.position = play.Position.Value;
                            AudioSource source = obj.AddComponent<AudioSource>();
                            if (play.SoundID == 14)
                                source.volume = 0.75f;

                            source.spatialBlend = 1f;
                            source.spread = 360f;
                            source.rolloffMode = AudioRolloffMode.Linear;
                            source.dopplerLevel = 0f;
                            source.maxDistance = 30f;
                            source.minDistance = 5f;
                            source.clip = SoundArchive.Instance.sounds[play.SoundID];

                            if (play.SoundID == 13 || play.SoundID == 12)
                            {
                                source.maxDistance = 60f;
                                source.minDistance = 20f;
                            }

                            List<AudioSource> asList = new List<AudioSource>();

                            foreach (AudioSource a in FindObjectsOfType<AudioSource>())
                            {
                                if (Vector2.Distance(a.transform.position, Camera.main.transform.position) < 10f && a.clip == source.clip && a != source)
                                {
                                    asList.Add(a);
                                }
                            }

                            for (int k = asList.Count - 20; k >= 0; k--)
                            {
                                Destroy(asList[k].gameObject);
                            }


                            source.Play();
                            Destroy(obj, source.clip.length + 1);
                        }

                        break;
                    }
                case Destroy destroy:
                    {
                        GameObject obj = GameObject.Find(destroy.ItemName);

                        if (destroy.ItemName.Contains("wall"))
                        {
                            GameObject vfx = Instantiate(FightSceneManager.Instance.WallVfx);
                            vfx.transform.position = obj.transform.position;
                            Destroy(vfx, 2f);

                            Walls.Remove(obj.GetComponent<Wall>());
                        }

                        Destroy(obj);

                        break;
                    }
                case Kick kick:
                    {
                        Tab.Instance.RemovePlayer(kick.PlayerID);

                        if (kick.PlayerID == playerId)
                        {
                            Escape.Instance.Kick(kick.Message);
                        }

                        break;
                    }
                case Ban ban:
                    {
                        Tab.Instance.RemovePlayer(ban.PlayerID);

                        if (ban.PlayerID == playerId)
                        {
                            Escape.Instance.Kick(ban.Message);
                        }

                        break;
                    }
                case KillFeedPacket killFeed:
                    {
                        FightSceneManager.Instance.KillFeed.AddKillFeed(killFeed.KillerName, killFeed.TargetName, killFeed.KillType, killFeed.KillerTeam, killFeed.TargetTeam);

                        break;
                    }
                case SetKills setKills:
                    {
                        Tab.Instance.ModifyKills(playerScripts[setKills.KillerId], setKills.Kills, playerScripts[setKills.KillerId].Team);

                        break;
                    }
                case FlashScreen flashScreen:
                    {
                        Vector2 flashPoint = flashScreen.Position;

                        Vector2 cameraPoint = Camera.main.WorldToViewportPoint(flashPoint);

                        if (cameraPoint.x > 0 && cameraPoint.x < 1 && cameraPoint.y > 0 && cameraPoint.y < 1)
                        {
                            FightSceneManager.Instance.CameraAnimator.Play("flash", 0, 0);

                            FightSceneManager.Instance.CameraAnimator.speed = ((float)((int)(Vector2.Distance(flashPoint, Camera.main.transform.position)) + 1)) / 6;
                        }

                        break;
                    }
                case SmokeVfx smokeVfx:
                    {
                        GameObject newVfx = Instantiate(FightSceneManager.Instance.SmokeVfx);
                        newVfx.transform.position = smokeVfx.Position;
                        newVfx.GetComponent<Smoke>().Activate();

                        break;
                    }
                case WallPacket wall:
                    {
                        GameObject newVfx = Instantiate(FightSceneManager.Instance.Wall);
                        newVfx.transform.position = wall.Position;
                        newVfx.transform.localEulerAngles = new Vector3(0, 0, wall.Angle);
                        newVfx.transform.name = "wall" + wall.ID;

                        Walls.Add(newVfx.GetComponent<Wall>());

                        break;
                    }
                case GrenadePacket grenade:
                    {
                        if (GameHost.Instance != null)
                            break;


                        GameObject newGren = null;

                        switch (grenade.GrenadeID)
                        {
                            case 0:
                                {
                                    newGren = Instantiate(FightSceneManager.Instance.HePrefab);
                                    break;
                                }
                            case 1:
                                {
                                    newGren = Instantiate(FightSceneManager.Instance.FlashPrefab);
                                    break;
                                }
                            case 2:
                                {
                                    newGren = Instantiate(FightSceneManager.Instance.SmokePrefab);
                                    break;
                                }
                            case 3:
                                {
                                    newGren = Instantiate(FightSceneManager.Instance.WallPrefab);
                                    break;
                                }
                        }

                        newGren.transform.position = grenade.Position;
                        newGren.GetComponent<Rigidbody2D>().velocity = grenade.Velocity;
                        newGren.transform.localEulerAngles = new Vector3(0, 0, grenade.Angle);

                        newGren.GetComponent<Grenade>().Invoke("Detonate", 1f);

                        break;
                    }
                case Bullet bullet:
                    {
                        if (GameHost.Instance != null)
                            break;

                        GameObject newBullet = Instantiate(FightSceneManager.Instance.BulletPrefab);
                        newBullet.transform.position = bullet.Position;
                        newBullet.GetComponent<Rigidbody2D>().velocity = bullet.Velocity;
                        newBullet.transform.localEulerAngles = new Vector3(0, 0, bullet.Angle);

                        newBullet.transform.name = bullet.ID.ToString(CultureInfo.InvariantCulture);

                        if (parameters.Length == 8)
                            Destroy(newBullet, 0.5f);

                        break;
                    }
                case Defused defused:
                    {
                        FightSceneManager.Instance.CanvasAnimator.Play("idle");
                        ConsoleCanvas.Instance.Console.MatchBombDefused();

                        break;
                    }
                case Warmup warmup:
                    {
                        FightSceneManager.Instance.Warmup.SetActive(true);

                        break;
                    }
                case BombExplosion bombExplosion:
                    {
                        GameObject vfx = Instantiate(FightSceneManager.Instance.HeExplosion);
                        vfx.transform.position = bombExplosion.Position;
                        vfx.transform.localScale *= 5;

                        Destroy(vfx, 3f);

                        break;
                    }
                case ServerDrop serverDrop:
                    {
                        GameObject drop = Instantiate(FightSceneManager.Instance.Drops[serverDrop.Type]);

                        drop.GetComponent<Drop>().Setup(serverDrop.BulletCount, serverDrop.RoundAmmo);
                        drop.transform.position = serverDrop.Position;
                        drop.GetComponent<Rigidbody2D>().velocity = serverDrop.Velocity;

                        drop.transform.name = "drop" + serverDrop.ID;

                        drop.transform.localScale *= 10;

                        break;
                    }
                case Disconnect disconnect:
                    {
                        Tab.Instance.RemovePlayer(disconnect.ID);

                        break;
                    }
                case SetDeaths setDeaths:
                    {
                        Tab.Instance.ModifyDeaths(playerScripts[setDeaths.Target], setDeaths.Deaths, playerScripts[setDeaths.Target].Team);

                        break;
                    }
                case Death death:
                    {
                        if (death.Dealt.HasValue && death.Id == playerId)
                        {
                            playerScripts[death.Id].DeathData(death.Dealt.Value, death.Received.Value, death.Attacker);
                            
                            if (playerScripts[death.Id].Team == 0)
                                ConsoleCanvas.Instance.Console.MatchCTDead(death.Attacker, playerScripts[death.Id].Name);
                            else ConsoleCanvas.Instance.Console.MatchTDead(death.Attacker, playerScripts[death.Id].Name);
                        }

                        playerScripts[death.Id].TriggerDeath();

                        Tab.Instance.ModifyAlive(playerScripts[death.Id], false, playerScripts[death.Id].Team);

                        break;
                    }
                case PlayerInfo playerInfo:
                    {
                        if (GameHost.Instance == null || (GameHost.Instance != null && RoundData.Instance.Spawned))
                        {
                            string id = playerInfo.PlayerID;

                            if (GameHost.Instance == null)
                            {
                                Vector2 playerPos = playerInfo.Position;

                                playerScripts[id].UpdateInfo(playerPos, playerInfo.Angle, playerInfo.Health, playerInfo.Armor, playerInfo.CanBuy, playerInfo.InBuyZone);
                            }

                            playerScripts[id].UpdateGunInfo(playerInfo.Armor, playerInfo.GunScriptType, playerInfo.GunBulletCount, playerInfo.GunRoundAmmo, playerInfo.Health);

                            if (playerScripts[id].Controlling)
                            {
                                FightSceneManager.Instance.CashText.GetComponent<Text>().text = "$" + playerInfo.Cash;

                                FightSceneManager.Instance.BulletText.text = playerInfo.GunRoundAmmo + "/" + playerInfo.GunBulletCount;
                                int gunType = playerInfo.GunType;

                                if (gunType != -1)
                                {
                                    FightSceneManager.Instance.WeaponUIObjects[0].SetActive(true);
                                    FightSceneManager.Instance.WeaponImages[0].sprite = FightSceneManager.Instance.Drops[gunType].GetComponent<SpriteRenderer>().sprite;
                                }
                                else FightSceneManager.Instance.WeaponUIObjects[0].SetActive(false);
                                int hasPistol = playerInfo.Pistol;
                                if (hasPistol == 1)
                                {
                                    FightSceneManager.Instance.WeaponUIObjects[1].SetActive(true);
                                }
                                else FightSceneManager.Instance.WeaponUIObjects[1].SetActive(false);

                                int hasHe = playerInfo.HE;
                                if (hasHe == 1)
                                {
                                    FightSceneManager.Instance.WeaponUIObjects[3].SetActive(true);
                                }
                                else FightSceneManager.Instance.WeaponUIObjects[3].SetActive(false);

                                int hasFlash = playerInfo.Flash;
                                if (hasFlash == 1)
                                {
                                    FightSceneManager.Instance.WeaponUIObjects[4].SetActive(true);
                                }
                                else FightSceneManager.Instance.WeaponUIObjects[4].SetActive(false);

                                int hasSmoke = playerInfo.Smoke;
                                if (hasSmoke == 1)
                                {
                                    FightSceneManager.Instance.WeaponUIObjects[5].SetActive(true);
                                }
                                else FightSceneManager.Instance.WeaponUIObjects[5].SetActive(false);

                                int hasBomb = playerInfo.HasBomb;
                                if (hasBomb == 1)
                                {
                                    FightSceneManager.Instance.WeaponUIObjects[6].SetActive(true);
                                }
                                else FightSceneManager.Instance.WeaponUIObjects[6].SetActive(false);

                                int hasWall = playerInfo.HasWall;
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
                                    gameTime = playerInfo.RoundTime;

                                    setTime = true;
                                }
                            }


                            if (GameHost.Instance == null)
                            {
                                playerScripts[id].UpdateOther(playerInfo.Cash, playerInfo.GunScriptType, playerInfo.MoveDir, playerInfo.Anim);
                            }
                        }

                        break;
                    }
            }
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
            else if (type == "BanMessage")
            {
                GameObject clonedText = Instantiate(Resources.Load<GameObject>("Prefabs\\banText"));
                clonedText.GetComponentInChildren<Text>().text = "You are banned on this server.";
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
            else if (type == "LoadScene")
            {
                string[] parameters = data.Split(' ');

                if (data.Split(' ')[1] != "OnlineWaitMenu")

                    SceneManager.LoadScene(data.Split(' ')[1]);
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
            else if (type == "FinishGame")
            {
                string[] parameters = data.Split(' ');

                int team = int.Parse(parameters[1], CultureInfo.InvariantCulture);

                if (team == -1)
                {
                    FightSceneManager.Instance.WinLose.InterpretText("Draw");
                }
                else if (team == this.Team)
                {
                    FightSceneManager.Instance.WinLose.InterpretText("Victory");
                }
                else
                {
                    FightSceneManager.Instance.WinLose.InterpretText("Defeat");
                }
            }
            else if (type == "CT" || type == "T")
            {
                FightSceneManager.Instance.WinLose.InterpretText(line);
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
                Network.Instance.Send(new ClientNewPlayerName(playerName.text));
            }
        }
        else
        {
            Network.Instance.Send(new ClientNewPlayerName(playerName.text));
        }
    }

    private void SendPlayerTeam()
    {
        Network.Instance.Send(new NewPlayerTeam(Team));
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
