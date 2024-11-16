using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Lidgren.Network;
using System.Linq;
using System;
using System.Net;
using TMPro;
using System.Globalization;
using UnityEngine.UI;

public class GameHost : MonoBehaviour
{
    #region Properties

    public bool Warmup => _warmup;

    #endregion

    #region Public Fields

    public static GameHost Instance;

    public static string Map;

    public string map;

    public List<string> bannedIps = new List<string>();

    public bool sentPlayers = false;

    public bool _bomb = false;

    public GameObject bombObject;

    public bool _bombDefused = false;

    public bool _testing = false;

    public bool _roundOver = false;

    #endregion

    #region Private Fields

    private bool canSendMsg = false;

    private bool ableToSend = false;

    private bool detected = false;


    public int drop;

    private int _sceneIndex = 0;

    private bool _inGame = false;

    private bool _deactivated = false;

    private bool _invokingWin = false;

    private bool _playersFreezed = true;

    private bool _warmup = true;

    private int _currentRoundIndex = 0;

    public int _lossBonusMoneyCount = 1;

    public bool blueWin = false;

    public bool redWin = false;

    public int _redRoundsWon = 0;

    public int _blueRoundsWon = 0;

    public float warmupTimeLeft = 90;

    private float _buyTimeLeft = 20;

    private float _freezeTimeLeft = 11;

    public float _bombTimeLeft = -1;

    private float _roundTimeLeft = 120;

    private string[] _playerAnimationList = new string[8] {
                "idle_gun",
                "run_gun",
                "idle_knife",
                "run_knife",
                "attack_knife",
                "bomb_plant",
                "dash_knife",
                "dash_gun" };

    public void RegisterKill(string killerPlayerNumber, Player targetPlayer, int killType)
    {
        MatchData.PlayerData[killerPlayerNumber].Kills++;

        GameClient.Instance.playerScripts[killerPlayerNumber].IncreaseConsecutiveKills();

        if (Config.ShowKillFeed)
            Network.Instance.Send(new KillFeedPacket(MatchData.PlayerData[killerPlayerNumber].Name, targetPlayer.Name, killType, MatchData.PlayerData[killerPlayerNumber].Team, targetPlayer.Team));

        Network.Instance.Send(new SetKills(killerPlayerNumber, MatchData.PlayerData[killerPlayerNumber].Kills));
    }

    private int playerCount = 1;

    #endregion

    #region Public Methods

    public void KickPlayer(string name, string message)
    {
        string playerID = GetClientByName(name);

        RemoveClient(playerID);

        Network.Instance.Send(new Kick(playerID, message));
    }

    public void BanPlayer(string name, string message)
    {
        string playerID = GetClientByName(name);

        RemoveClient(playerID);

        Network.Instance.Send(new Ban(playerID, message));

        bannedIps.Add(playerID);
    }

    public void RestartGame()
    {
        MatchData.ResetInventories();

        _lossBonusMoneyCount = 0;

        _blueRoundsWon = 0;
        _redRoundsWon = 0;
        _currentRoundIndex = 0;

        _warmup = false;

        StartRound();
    }

    public void InvokeWin()
    {
        if (_invokingWin == false)
        {
            _invokingWin = true;
            Invoke("Win", 5f);
        }
    }

    public void StartRound()
    {
        if (playerCount >= 0)
        {
            _inGame = true;

            if (_warmup == false)
                _currentRoundIndex++;

            if (_currentRoundIndex == Config.RoundsPerHalf + 1)
            {
                Swap();
            }
            else if (_currentRoundIndex >= Config.RoundsPerHalf * 2 + 1 || _redRoundsWon >= Config.RoundsToWin || _blueRoundsWon >= Config.RoundsToWin)
            {
                FinishGame();

                return;
            }
            else SavePlayerInventories();

            SendNewRoundPacket();
        }
    }

    public void Swap()
    {
        MatchData.Swap();

        int aux = _redRoundsWon;
        _redRoundsWon = _blueRoundsWon;
        _blueRoundsWon = aux;

        _lossBonusMoneyCount = 0;
    }

    private void ConnectGameClient()
    {
        GameObject clientObject = new GameObject();
        GameClient clientScript = clientObject.AddComponent<GameClient>();
        clientObject.name = "GameClient";
        DontDestroyOnLoad(clientObject);

        clientScript.ip = "127.0.0.1";
        clientScript.Initialize();
    }

    public void Initialize()
    {
        /*if (FindObjectsOfType<GameHost>().Length >= 2)
        {
            Destroy(gameObject);

            return;
        }*/


        //SceneManager.LoadScene("OnlineWaitMenu");

        /*config = new NetPeerConfiguration("IGDShooter")
        {
            Port = 25565,
            //EnableUPnP = true,
            AcceptIncomingConnections = true,
            PingInterval = 1f,
            ResendHandshakeInterval = 1f,
            MaximumHandshakeAttempts = 15,
            ConnectionTimeout = 1000f,
            ReceiveBufferSize = PlayerPrefs.GetInt("ServerReceiveBuffer", 1024),
            SendBufferSize = PlayerPrefs.GetInt("ServerSendBuffer", 3072)
        };

        config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
        config.EnableMessageType(NetIncomingMessageType.StatusChanged);*/

        //_server = new NetServer(config);
        //_server.Start();

        //StartCoroutine(MessageQueue(64));

        Instance = this;

        map = Map;

        Network.Instance.OnClientConnected += HandleClientConnection;

        Network.Instance.OnMessagesReceived += OnMessagesReceived;

        DontDestroyOnLoad(gameObject);

        StartCoroutine(MessageQueue());

        //ConnectGameClient();
    }

    private void OnMessagesReceived(object[] objects, string senderId)
    {
        for (int i = 0; i < objects.Length; i++)
        {
            switch (objects[i])
            {
                case ClientNewPlayerName clientNewPlayerName:
                    {
                        HandleNewPlayerName(clientNewPlayerName.Data, senderId);
                        break;
                    }
                case NewPlayerTeam clientNewPlayerTeam:
                    {
                        HandleNewPlayerTeam(clientNewPlayerTeam.TeamID, senderId);
                        break;
                    }
                case PlayerTarget playerTarget:
                    {
                        HandlePlayerTarget(playerTarget.Position, senderId);
                        break;
                    }
                case Shoot shoot:
                    {
                        HandleShoot(shoot.Angle, senderId);
                        break;
                    }
                case ThrowNade throwNade:
                    {
                        HandleThrowNade(throwNade.Position, senderId);
                        break;
                    }
                case Switch _switch:
                    {
                        HandleSwitch(_switch.InventoryID, senderId);
                        break;
                    }
                case Process process:
                    {
                        HandleProcess(process.BuyID, senderId);
                        break;
                    }
                case Reload reload:
                    {
                        HandleReload(senderId);
                        break;
                    }
                case Defuse defuse:
                    {
                        HandleDefuse(senderId);
                        break;
                    }
                case StopDef stopDef:
                    {
                        HandleStopDef(senderId);
                        break;
                    }
                case DropTarget dropTarget:
                    {
                        HandleDropTarget(senderId);
                        break;
                    }
                case Shift shift:
                    {
                        HandleShift(senderId);
                        break;
                    }
                case StopSh stopSh:
                    {
                        HandleStopSh(senderId);
                        break;
                    }
                case Throw thr:
                    {
                        HandleThrow(thr.Position, senderId);
                        break;
                    }
                case ScrollUp scrollUp:
                    {
                        HandleScrollUp(senderId);
                        break;
                    }
                case ScrollDown scrollDown:
                    {
                        HandleScrollDown(senderId);
                        break;
                    }
                case Graffiti graffiti:
                    {
                        HandleGraffiti(graffiti.GraffitiID, senderId);
                        break;
                    }
                case Msg msg:
                    {
                        HandleMsg(msg.All, msg.Team, msg.Data, senderId);
                        break;
                    }
                case Dash dash:
                    {
                        HandleDash(dash.Position, senderId);
                        break;
                    }
            }
        }
    }

    public void WarmupEnd()
    {
        RoundData.Instance.Ended = true;

        MatchData.ResetPlayerData();

        _currentRoundIndex = 1;
        _warmup = false;

        SendNewRoundPacket();
    }

    public void Disconnect()
    {
        if (Network.Instance.HasConnections == false)
            return;

        Network.Instance.Send(new ServerDisconnect());

        Network.Instance.ForceSendPackets();
    }

    #endregion

    #region Private Methods

    float delta;

    private float lastPlayerCountSend = 0;

    private IEnumerator MessageQueue()
    {
        delta = 1f / Network.Instance.UpdateRate;

        while (true)
        {
            yield return new WaitForSeconds(delta);

            try
            {
                //CheckForWarmupEnd();

                CheckForWinConditions();

                SpawnPlayers();

                int playersConnected = CheckPlayerConnectivity() + 1;

                SendPlayerCount(playersConnected);

                //Network.Instance.Send(new ExperimentalFeatures(Config.ExperimentalFeatures));
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
        }
    }

    private void SpawnPlayers()
    {
        if (_inGame && sentPlayers == false)
        {
            foreach (string i in MatchData.PlayerData.Keys)
            {
                if (MatchData.PlayerData[i].ClientAlive && MatchData.PlayerData[i].Team != 2)
                {
                    int hasPistol = 0;
                    if (MatchData.PlayerData[i].HasPistol)
                        hasPistol = 1;

                    int hasHe = 0;
                    if (MatchData.PlayerData[i].HasHe)
                        hasHe = 1;

                    int hasFlash = 0;
                    if (MatchData.PlayerData[i].HasFlash)
                        hasFlash = 1;

                    int hasSmoke = 0;
                    if (MatchData.PlayerData[i].HasSmoke)
                        hasSmoke = 1;

                    int hasWall = 0;
                    if (MatchData.PlayerData[i].HasWall)
                        hasWall = 1;

                    Network.Instance.Send(new SpawnPlayer(i, MatchData.PlayerData[i].Name, MatchData.PlayerData[i].Team, MatchData.PlayerData[i].GunID, hasPistol, hasHe, hasFlash, hasSmoke, MatchData.PlayerData[i].Armor, MatchData.PlayerData[i].Cash, hasWall, MatchData.PlayerData[i].Kills, MatchData.PlayerData[i].Deaths, _sceneIndex));
                }
            }
            sentPlayers = true;
        }
    }

    /*private void DisconnectClient(NetIncomingMessage message)
    {
        foreach (string i in MatchData.PlayerData.Keys)
        {
            if (clients[i].RemoteEndPoint == message.SenderConnection.RemoteEndPoint && MatchData.PlayerData[i].ClientAlive)
            {
                RemoveClient(i);

                Network.Instance.Send(new SystemMessage("<color=yellow>" + MatchData.PlayerData[i].Name + " disconnected.</color>"));
                Network.Instance.Send(new Disconnect(i));

                return;
            }
        }
    }*/

    private void CheckForWinConditions()
    {
        if (_bombDefused && canSendMsg)
            Network.Instance.Send(new Defused());

        try
        {
            if (RoundData.Instance == null)
                return;

            if (GameClient.Instance.loadingScene)
                return;

            if (RoundData.Instance.Spawned && RoundData.Instance.Ended == false)
            {
                bool bluePlayersExist = false;
                bool redPlayersExist = false;
                foreach (string i in MatchData.PlayerData.Keys)
                {
                    if (MatchData.PlayerData[i].ClientAlive)
                    {
                        if (MatchData.PlayerData[i].Team == 0)
                            bluePlayersExist = true;
                        else if (MatchData.PlayerData[i].Team == 1)
                            redPlayersExist = true;
                    }
                }

                if (bluePlayersExist && redPlayersExist)
                {
                    bool bluePlayersAlive = false, redPlayersAlive = false;

                    foreach (Player player in GameClient.Instance.AlivePlayers)
                    {
                        if (bluePlayersAlive && redPlayersAlive)
                            break;

                        if (player.Controllable)
                        {
                            if (player.Team == 0)
                                bluePlayersAlive = true;
                            else redPlayersAlive = true;
                        }
                    }

                    if ((bluePlayersAlive == false || redPlayersAlive == false) && _roundOver == false && _warmup == false)
                    {
                        Debug.Log(bluePlayersAlive + " " + redPlayersAlive + " " + _roundOver + " " + _warmup);
                        if (bluePlayersAlive == false)
                        {
                            Debug.Log("Bugged Win");

                            RedWin();

                            _roundOver = true;
                            StartRound();
                        }
                        else if (_bomb == false)
                        {
                            BlueWin();

                            _roundOver = true;
                            StartRound();
                        }
                    }
                }


                //Timers
                _buyTimeLeft -= delta;
                if (_bombDefused == false)
                {
                    if (_freezeTimeLeft > 0)
                        _freezeTimeLeft -= delta;
                    else if (warmupTimeLeft > 0 && _warmup)
                        warmupTimeLeft -= delta;
                    else if (_bomb)
                        _bombTimeLeft -= delta;
                    else _roundTimeLeft -= delta;
                }

                if (_bombTimeLeft < 0 && _bomb)
                {
                    //Bomb Explosion
                    _bombTimeLeft = 0;
                    if (_roundOver == false)
                    {
                        Network.Instance.Send(new BombExplosion(bombObject.transform.position));

                        //Kill Players
                        foreach (Player player in GameClient.Instance.AlivePlayers)
                        {
                            float dist = Vector2.Distance(bombObject.transform.position, player.transform.position);

                            if (dist < 45f && player.Dead == false)
                            {
                                if (player.FullHealth - 1400 / ((int)dist + 1) <= 0)
                                {
                                    if (Config.ShowKillFeed)
                                        Network.Instance.Send(new KillFeedPacket(player.Name, player.Name, 8, player.Team, player.Team));
                                }

                                player.DecreaseHp(null, 1400 / ((int)dist + 1), 8);
                            }
                        }

                        //Destroy Walls
                        foreach (Wall wall in GameClient.Instance.Walls)
                        {
                            float dist = Vector2.Distance(bombObject.transform.position, wall.SpriteRenderer.bounds.ClosestPoint(bombObject.transform.position));

                            if (dist < 45f)
                            {
                                wall.DecreaseHp(14000 / ((int)dist + 1));
                            }
                        }

                        Network.Instance.Send(new Play(17, bombObject.transform.position));
                        Network.Instance.Send(new Play(15));

                        RedWin();

                        if (bombObject != null)
                            Network.Instance.Send(new Destroy(bombObject.transform.name));

                        InvokeWin();
                    }
                    _roundOver = true;
                }
                else if (_roundTimeLeft < 0)
                {
                    _roundTimeLeft = 0;
                    if (_roundOver == false)
                    {
                        Network.Instance.Send(new Play(15));
                        _roundOver = true;

                        BlueWin();

                        InvokeWin();
                    }
                }


                if (_buyTimeLeft < 0)
                {
                    DisableBuy();
                }
            }

            if (_freezeTimeLeft <= 0)
            {
                StopPlayerFreeze();
            }

            if (_warmup && warmupTimeLeft < 0)
            {
                WarmupEnd();
            }

            int roundTime = 0;

            if (_freezeTimeLeft > 0)
                roundTime = (int)_freezeTimeLeft;
            else if (warmupTimeLeft > 0 && _warmup)
                roundTime = (int)warmupTimeLeft;
            else if (_bomb)
                roundTime = (int)_bombTimeLeft;
            else if (_roundTimeLeft > 0)
                roundTime = (int)_roundTimeLeft;

            int blueCount = 0;
            int redCount = 0;

            foreach (Player player in GameClient.Instance.AlivePlayers)
            {
                if (player.Controllable)
                {
                    if (player.Team == 0 && player.Dead == false)
                        blueCount++;
                    else if (player.Dead == false)
                        redCount++;


                    if (sentPlayers)
                    {
                        Vector2 movementDirection = player.MoveDir;

                        string playerAnim = "";

                        foreach (string animation in _playerAnimationList)
                        {
                            if (player.IsAnimation(animation))
                            {
                                playerAnim = animation;
                                break;
                            }
                        }

                        try
                        {
                            int hasPistol = 0;
                            if (player.HasPistol)
                                hasPistol = 1;

                            int hasHe = 0;
                            if (player.HasHe)
                                hasHe = 1;

                            int hasFlash = 0;
                            if (player.HasFlash)
                                hasFlash = 1;

                            int hasSmoke = 0;
                            if (player.HasSmoke)
                                hasSmoke = 1;

                            int hasWall = 0;
                            if (player.HasWall)
                                hasWall = 1;

                            int hasBomb = 0;
                            if (player.HasBomb)
                                hasBomb = 1;

                            int canBuy = 0;
                            if (player.CanBuy)
                                canBuy = 1;

                            int inBuyZone = 0;
                            if (player.InBuyZone)
                                inBuyZone = 1;

                            Network.Instance.Send(new PlayerInfo(player.ID, player.transform.position, player.transform.localEulerAngles.z, (int)player.Health, movementDirection, playerAnim, player.GunScript.Type, (int)player.Armor, (int)player.Cash, player.GunType, hasPistol, hasHe, hasFlash, hasSmoke, player.GunScript.RoundAmmo, player.GunScript.BulletCount, roundTime, canBuy, inBuyZone, hasBomb, hasWall, player.DashTimer));
                        }
                        catch (Exception ex)
                        {
                            Debug.Log(ex.ToString());
                        }
                    }
                }
            }

            Network.Instance.Send(new Count(redCount, blueCount));
            Network.Instance.Send(new Rounds(_redRoundsWon, _blueRoundsWon));
            Network.Instance.Send(new TimePacket(roundTime));
        }
        catch (Exception err)
        {
            Debug.Log(err);
        }
    }

    private void BlueWin()
    {
        RoundData.Instance.Ended = true;

        _blueRoundsWon++;

        if (blueWin == false)
            _lossBonusMoneyCount = 0;

        foreach (string i in MatchData.PlayerData.Keys)
        {
            if (MatchData.PlayerData[i].Team == 0)
            {
                MatchData.PlayerData[i].Cash += Config.WinCash;
            }
            else
            {
                MatchData.PlayerData[i].Cash += Config.LoseCash + Config.LoseStreakCash * _lossBonusMoneyCount;
            }
            if (MatchData.PlayerData[i].Cash > Config.MaxCash)
                MatchData.PlayerData[i].Cash = Config.MaxCash;
        }

        if (blueWin)
        {
            _lossBonusMoneyCount++;
            if (_lossBonusMoneyCount > 3)
                _lossBonusMoneyCount = 3;
        }
        else
        {
            blueWin = true;
            redWin = false;
            _lossBonusMoneyCount = 1;
        }
    }

    private void RedWin()
    {
        RoundData.Instance.Ended = true;

        _redRoundsWon++;

        if (redWin == false)
            _lossBonusMoneyCount = 0;

        foreach (string i in MatchData.PlayerData.Keys)
        {
            if (MatchData.PlayerData[i].Team == 1)
            {
                MatchData.PlayerData[i].Cash += Config.WinCash;
            }
            else
            {
                MatchData.PlayerData[i].Cash += Config.LoseCash + _lossBonusMoneyCount * Config.LoseStreakCash;
            }

            if (MatchData.PlayerData[i].Cash > Config.MaxCash)
                MatchData.PlayerData[i].Cash = Config.MaxCash;
        }

        if (redWin)
        {
            _lossBonusMoneyCount++;
            if (_lossBonusMoneyCount > 3)
                _lossBonusMoneyCount = 3;
        }
        else
        {
            redWin = true;
            blueWin = false;
            _lossBonusMoneyCount = 1;
        }
    }

    private int CheckPlayerConnectivity()
    {
        return Network.Instance.ConnectionCount;

        /*int playersConnected = 1;

        foreach (string i in MatchData.PlayerData.Keys)
        {
            if (MatchData.PlayerData[i].ClientAlive)
            {
                playersConnected++;
                MatchData.PlayerData[i].TimeOut += delta;
                if (MatchData.PlayerData[i].TimeOut >= 10)
                    DisconnectClient(clients[i].RemoteEndPoint);
            }
        }

        return playersConnected;*/
    }

    private void SendPlayerCount(int playersConnected)
    {
        if (_inGame == false)
        {
            if (Time.realtimeSinceStartup - lastPlayerCountSend > 1)
            {
                Network.Instance.Send(new PlayerCount(playersConnected++));
                lastPlayerCountSend = Time.realtimeSinceStartup;
            }
        }
        else if (_warmup)
            Network.Instance.Send(new Warmup());
    }

    private void HandleMsg(bool all, int team, string msg, string senderId)
    {
        string[] parameters = msg.Split(' ');
        if (parameters[0] == "/whisper")
        {
            Network.Instance.Send(new Whisper(parameters[1], MatchData.PlayerData[senderId].Name, string.Join(" ", parameters.Skip(2))));
        }
        else if (parameters[0] == "/all" || all)
        {
            Network.Instance.Send(new All(MatchData.PlayerData[senderId].Name, MatchData.PlayerData[senderId].Team, string.Join(" ", parameters.Skip(1))));
        }
        else Network.Instance.Send(new ServerMessage(MatchData.PlayerData[senderId].Name, MatchData.PlayerData[senderId].Team, msg));
    }

    private void HandleGraffiti(int id, string senderId)
    {
        Player player = GameClient.Instance.playerScripts[senderId];

        if (player.IsAbleToGraffiti)
        {
            Network.Instance.Send(new ServerGraffiti(player.transform.position, id));
            Network.Instance.Send(new Play(27, player.transform.position, player.ID));
            player.Grafitied();
        }
    }

    private void HandleScrollDown(string senderId)
    {
        Player player = GameClient.Instance.playerScripts[senderId];

        player.ScrollDown();
    }

    private void HandleScrollUp(string senderId)
    {
        Player player = GameClient.Instance.playerScripts[senderId];

        player.ScrollUp();
    }

    private void HandleThrow(Vector2 position, string senderId)
    {
        Player player = GameClient.Instance.playerScripts[senderId];

        player.Throw(position);
    }

    private void HandleStopSh(string senderId)
    {
        Player player = GameClient.Instance.playerScripts[senderId];

        player.StopSlowWalk();
    }

    private void HandleShift(string senderId)
    {
        Player player = GameClient.Instance.playerScripts[senderId];

        player.SlowWalk();
    }

    private void HandleDropTarget(string senderId)
    {
        Player player = GameClient.Instance.playerScripts[senderId];

        player.NullTargetPosition();
    }

    private void HandleStopDef(string senderId)
    {
        Player player = GameClient.Instance.playerScripts[senderId];

        player.StopDefuse();
    }

    private void HandleDefuse(string senderId)
    {
        Player player = GameClient.Instance.playerScripts[senderId];

        player.Defuse();
    }

    private void HandleReload(string senderId)
    {
        Player player = GameClient.Instance.playerScripts[senderId];

        player.GunScript.ReloadGun();
    }

    private void HandleProcess(int buyId, string senderId)
    {
        Player player = GameClient.Instance.playerScripts[senderId];

        player.ProcessPurchase(buyId);
    }

    private void HandleSwitch(int inventoryId, string senderId)
    {
        Player player = GameClient.Instance.playerScripts[senderId];

        player.SwitchWeapon(inventoryId);
    }

    private void HandleThrowNade(Vector2 position, string senderId)
    {
        Player player = GameClient.Instance.playerScripts[senderId];

        player.ThrowNade(position);
    }

    private void HandleShoot(float angle, string senderId)
    {
        Player player = GameClient.Instance.playerScripts[senderId];

        player.transform.localEulerAngles = new Vector3(0, 0, angle);

        player.Shoot();
    }

    private void HandlePlayerTarget(Vector2 position, string senderId)
    {
        Player player = GameClient.Instance.playerScripts[senderId];

        player.UpdateTargetCoordinates(position);
    }

    private void HandleNewPlayerTeam(int teamID, string senderID)
    {
        if (teamID == 0)
            Network.Instance.Send("<color=yellow>" + MatchData.PlayerData[senderID].Name + " joined CT side.</color>");
        else if (teamID == 1)
            Network.Instance.Send("<color=yellow>" + MatchData.PlayerData[senderID].Name + " joined T side.</color>");
        else Network.Instance.Send("<color=yellow>" + MatchData.PlayerData[senderID].Name + " is now a spectator.</color>");

        if (GameClient.Instance.players.ContainsKey(senderID))
        {
            if (GameClient.Instance.playerScripts[senderID].Dead == false)
            {
                GameClient.Instance.playerScripts[senderID].DecreaseHp(null, 9999, 0);
            }
        }
        try
        {
            MatchData.PlayerData[senderID].Team = teamID;

            if (_warmup && MatchData.PlayerData[senderID].Team != 2)
            {
                int hasPistol = 0;

                int hasHe = 0;

                int hasFlash = 0;

                int hasSmoke = 0;

                int hasWall = 0;

                if (_warmup)
                    MatchData.PlayerData[senderID].Cash = Config.MaxCash;
                else
                    MatchData.PlayerData[senderID].Cash = 800;

                Network.Instance.Send(new SpawnPlayer(senderID, MatchData.PlayerData[senderID].Name, MatchData.PlayerData[senderID].Team, MatchData.PlayerData[senderID].GunID, hasPistol, hasHe, hasFlash, hasSmoke, MatchData.PlayerData[senderID].Armor, MatchData.PlayerData[senderID].Cash, hasWall, MatchData.PlayerData[senderID].Kills, MatchData.PlayerData[senderID].Deaths, _sceneIndex));
            }
            else if (_warmup == false && MatchData.PlayerData[senderID].Team != 2)
            {
                Network.Instance.Send(new TabPlayer(senderID, MatchData.PlayerData[senderID].Name, MatchData.PlayerData[senderID].Team));
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }

    private void HandleNewPlayerName(string data, string id)
    {
        string currentName = MatchData.PlayerData[id].Name;
        MatchData.PlayerData[id].Name = data.Split(' ')[0];
        CheckName(id);

        if (Chat.Instance != null)
        {
            Network.Instance.Send(new ServerNewPlayerName(id, MatchData.PlayerData[id].Name));

            if (ConsoleCanvas.Instance.Console.sendCommandFeedback)
                Network.Instance.Send(new SystemMessage("<color=yellow>" + currentName + " changed his name to " + data.Split(' ')[2] + ".</color>"));
        }
    }

    private void HandleClientConnection(string id)
    {
        if (bannedIps.Contains(id))
        {
            Network.Instance.Send(new SystemMessage("<color=yellow>A banned player attempted to connect.</color>"));

            Network.Instance.SendTo(id, new BanMessage());
        }
        else
        {
            Network.Instance.Send(new SystemMessage("<color=yellow>New player connected.</color>"));

            Network.Instance.SendTo(id, new ReceiveID(id, map));

            Network.Instance.SendTo(id, new LoadScene(SceneManager.GetActiveScene().name, _sceneIndex));

            PlayerData data = MatchData.GetPlayerData(id);
            data.Name = "NewPlayer";
            data.Team = 2;

            CheckNameLobby(id, true);

            playerCount++;
        }
    }

    private void HandleDash(Vector2 position, string senderId)
    {
        Player player = GameClient.Instance.playerScripts[senderId];

        player.Dash(position);
    }

    private void CheckForWarmupEnd()
    {
        try
        {
            if (Input.GetKeyDown(KeyCode.P) && _warmup && Chat.Instance.Focused == false && ConsoleCanvas.Instance.Content.activeInHierarchy == false)
            {
                if (GameClient.Instance.players[GameClient.Instance.playerId] != null)
                {
                    WarmupEnd();
                }
            }
        }
        catch
        {

        }
    }

    private void DisableBuy()
    {
        _deactivated = true;
        foreach (Player player in GameClient.Instance.AlivePlayers)
        {
            player.DisableBuying();
        }
    }

    private void Win()
    {
        foreach (Player player in GameClient.Instance.AlivePlayers)
        {
            if (player.ID != "")
                player.RoundEnd();
        }

        StartRound();
    }

    private void StopPlayerFreeze()
    {
        _playersFreezed = false;
        foreach (Player player in GameClient.Instance.AlivePlayers)
        {
            player.Unfreeze();
        }
    }

    private void FinishGame()
    {
        int winningTeam = -1;

        if (Config.RoundsToWin == _blueRoundsWon)
            winningTeam = 0;
        else if (Config.RoundsToWin == _redRoundsWon)
            winningTeam = 1;

        Network.Instance.Send(new EndGame(winningTeam));

        List<PlayerScore> bluePlayers = Tab.Instance.bluePlayers;
        List<PlayerScore> redPlayers = Tab.Instance.redPlayers;

        CalculateScores(bluePlayers, "CT");

        CalculateScores(redPlayers, "T");
    }

    private void CalculateScores(List<PlayerScore> players, string teamName)
    {
        List<int> awardIDs = new List<int>
        {
            0,1,2,3,4,5,6,7,8,9
        };

        List<Award> awards = new List<Award>();

        List<string> awardedPlayers = new List<string>();

        for (int i = 0; i < players.Count; i++)
        {
            string playerID = players[i].playerID;

            PlayerData data = MatchData.PlayerData[playerID];

            int kills = data.Kills;
            int damageDealt = data.DamageDealt;
            int bombDefusals = data.BombDefusals;
            int bombPlants = data.BombPlants;
            int silentSteps = data.SilentSteps;
            int consecutiveKills = data.ConsecutiveKills;
            int thrownWeapons = data.ThrownWeapons;
            int heDamage = data.HeDamage;
            int roundsSurvived = data.RoundsSurvived;
            int nadesThrown = data.NadesThrown;

            awards.Add(new Award(0, playerID, kills * 200, "Top Fragger", kills.ToString(CultureInfo.InvariantCulture) + " players killed"));
            awards.Add(new Award(1, playerID, (int)(damageDealt * 0.66f), "Blood Thirsty", damageDealt.ToString(CultureInfo.InvariantCulture) + " damage dealt"));
            awards.Add(new Award(2, playerID, bombDefusals * 250, "Party Crasher", bombDefusals.ToString(CultureInfo.InvariantCulture) + " bombs defused"));
            awards.Add(new Award(3, playerID, bombPlants * 250, "Kamikaze", bombPlants.ToString(CultureInfo.InvariantCulture) + " bombs plants"));
            awards.Add(new Award(4, playerID, silentSteps * 5, "Silent Ninja", silentSteps.ToString(CultureInfo.InvariantCulture) + " silent steps"));
            awards.Add(new Award(5, playerID, consecutiveKills * 1000, "Genocidal Mastermind", consecutiveKills.ToString(CultureInfo.InvariantCulture) + " consecutive kills"));
            awards.Add(new Award(6, playerID, thrownWeapons * 100, "Killer Assistance", thrownWeapons.ToString(CultureInfo.InvariantCulture) + " weapon donations"));
            awards.Add(new Award(7, playerID, heDamage * 5, "Precise Bomber", heDamage.ToString(CultureInfo.InvariantCulture) + " grenade damage"));
            awards.Add(new Award(8, playerID, roundsSurvived * 200, "Immortal God", roundsSurvived.ToString(CultureInfo.InvariantCulture) + " rounds survived in a row"));
            awards.Add(new Award(9, playerID, nadesThrown * 35, "Vision Thief", nadesThrown.ToString(CultureInfo.InvariantCulture) + " flashes and smokes thrown"));
        }

        awards.Sort((a, b) => b.value.CompareTo(a.value));

        int awardsGiven = 0;
        for (int i = 0; i < awards.Count && awardsGiven < 5; i++)
        {
            if (awardIDs.Contains(awards[i].id) && awardedPlayers.Contains(awards[i].playerID) == false)
            {
                awardedPlayers.Add(awards[i].playerID);

                awardIDs.Remove(awards[i].id);

                if (teamName == "CT")
                    Network.Instance.Send(new CT(awardsGiven, MatchData.PlayerData[awards[i].playerID].Name, MatchData.PlayerData[awards[i].playerID].Kills, MatchData.PlayerData[awards[i].playerID].Deaths, awards[i].title, awards[i].description));
                else Network.Instance.Send(new T(awardsGiven, MatchData.PlayerData[awards[i].playerID].Name, MatchData.PlayerData[awards[i].playerID].Kills, MatchData.PlayerData[awards[i].playerID].Deaths, awards[i].title, awards[i].description));

                awardsGiven++;
            }
        }
    }

    private void CheckName(string id)
    {
        string temporaryName = GetNewName(id);

        MatchData.PlayerData[id].Name = temporaryName;
    }

    private string GetNewName(string id)
    {
        bool playerNotFound = true;
        string temporaryName = MatchData.PlayerData[id].Name;
        int index = 1;

        while (playerNotFound)
        {
            playerNotFound = false;
            foreach (string i in MatchData.PlayerData.Keys)
            {
                if (i != id && MatchData.PlayerData[i].Name == temporaryName)
                {
                    playerNotFound = true;
                    temporaryName = MatchData.PlayerData[id].Name + "(" + (index++).ToString(CultureInfo.InvariantCulture) + ")";
                    break;
                }
            }
        }

        return temporaryName;
    }

    private void CheckNameLobby(string id, bool connect)
    {
        string temporaryName = GetNewName(id);

        //if (connect == false)
        //    message += "NewName " + id.ToString(CultureInfo.InvariantCulture)  + " " + temporaryName + "\n";

        Network.Instance.Send(new NewName(id, temporaryName));

        MatchData.PlayerData[id].Name = temporaryName;
    }

    private void OnDestroy()
    {
        Disconnect();
    }

    private void OnApplicationQuit()
    {
        Disconnect();
    }

    public string GetClientByName(string name)
    {
        foreach (string i in MatchData.PlayerData.Keys)
        {
            if (MatchData.PlayerData[i].Name == name && MatchData.PlayerData[i].ClientAlive)
            {
                return i;
            }
        }

        return "";
    }

    private void RemoveClient(string playerID)
    {
        MatchData.PlayerData[playerID].ClientAlive = false;

        MatchData.PlayerData[playerID].Name = "";

        if (GameClient.Instance.players.ContainsKey(playerID))
        {
            if (GameClient.Instance.playerScripts[playerID].Dead == false)
            {
                GameClient.Instance.playerScripts[playerID].DecreaseHp(null, 9999, 0);
            }
        }
    }

    private void SavePlayerInventories()
    {
        if (_warmup == false)
        {
            foreach (Player player in GameClient.Instance.AlivePlayers)
            {
                MatchData.PlayerData[player.ID].GunID = player.GunType;
                MatchData.PlayerData[player.ID].HasHe = player.HasHe;
                MatchData.PlayerData[player.ID].HasFlash = player.HasFlash;
                MatchData.PlayerData[player.ID].HasSmoke = player.HasSmoke;
                MatchData.PlayerData[player.ID].HasWall = player.HasWall;
                MatchData.PlayerData[player.ID].HasPistol = player.HasPistol;
                MatchData.PlayerData[player.ID].Armor = (int)player.Armor;
                Destroy(player.gameObject);
            }
        }
    }

    public void SendNewRoundPacket()
    {
        _freezeTimeLeft = Config.FreezeTime;

        _bombTimeLeft = -1;
        _roundTimeLeft = Config.RoundTime;
        _buyTimeLeft = Config.BuyTime;

        _playersFreezed = true;
        _invokingWin = false;
        _bombDefused = false;
        _bomb = false;
        _deactivated = false;
        _roundOver = false;

        if (_warmup)
        {
            _freezeTimeLeft = 1;
            _buyTimeLeft = warmupTimeLeft;
        }

        Network.Instance.Send(new StartGame(++_sceneIndex));

        GameClient.Instance.AlivePlayers = new List<Player>();
        GameClient.Instance.Walls = new List<Wall>();

        sentPlayers = false;
    }

    public void DefuseBomb()
    {
        _bombDefused = true;

        if (_roundOver == false)
        {
            _roundOver = true;

            BlueWin();

            InvokeWin();
        }

        Network.Instance.Send(new Destroy(bombObject.transform.name));
    }

    #endregion
}

public class Award
{
    public Award(int id, string playerID, int value, string title, string description)
    {
        this.id = id;
        this.playerID = playerID;
        this.value = value;
        this.title = title;
        this.description = description;
    }

    public int id;
    public string playerID;
    public int value;
    public string title;
    public string description;
}