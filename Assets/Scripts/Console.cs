using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Console : MonoBehaviour
{
    public static Console Instance;

    public string matchStartHost;
    public string matchStartClient;
    public string matchRoundStart;
    public string matchRoundEnd;
    public string matchBombPlanted;
    public string matchBombDefused;
    public string matchCtDead;
    public string matchTDead;

    public bool sendCommandFeedback = true;

    public bool hovered = false;
    public bool focused = false;
    public GameObject typeBox;
    public GameObject chatBox;

    bool pressed1 = false;
    bool pressed2 = false;

    InputField typeBoxInputField;

    private void Awake()
    {
        Instance = this;

        typeBoxInputField = typeBox.GetComponent<InputField>();
    }

    public void MatchStartHost()
    {
        InterpretText(matchStartHost, false);
    }
    public void MatchStartClient()
    {
        InterpretText(matchStartClient, false);
    }
    public void MatchRoundStart()
    {
        InterpretText(matchRoundStart, true);
    }
    public void MatchRoundEnd()
    {
        InterpretText(matchRoundEnd, false);
    }
    public void MatchBombPlanted()
    {
        InterpretText(matchBombPlanted, false);
    }
    public void MatchBombDefused()
    {
        InterpretText(matchBombDefused, false);
    }
    public void MatchCTDead(string killerName, string targetName)
    {
        InterpretText(matchCtDead, false, killerName, targetName);
    }
    public void MatchTDead(string killerName, string targetName)
    {
        InterpretText(matchTDead, false, killerName, targetName);
    }

    private void InterpretText(string text, bool roundStart = false, string killerName = "", string targetName = "")
    {
        foreach (string line in text.Split('\n'))
        {
            string lineText = line;

            if (GameHost.Instance != null)
            {
                GameHost gameHost = GameHost.Instance;
                GameClient gameClient = GameClient.Instance;
                if (lineText.Contains("$randomAlive"))
                {
                    List<string> names = new List<string>();

                    for (int i = 0; i < MatchData.PlayerData.Count; i++)
                    {
                        if (MatchData.PlayerData[MatchData.PlayerData.Keys.ElementAt(i)].ClientAlive && gameClient.players[MatchData.PlayerData.Keys.ElementAt(i)] != null)
                        {
                            names.Add(MatchData.PlayerData[MatchData.PlayerData.Keys.ElementAt(i)].Name);
                        }
                    }

                    if (names.Count == 0)
                        lineText = lineText.Replace("$randomAlive", "$none");
                    else lineText = lineText.Replace("$randomAlive", names[UnityEngine.Random.Range(1, 999999) % names.Count]);
                }
                if (lineText.Contains("$randomDead"))
                {

                    List<string> names = new List<string>();

                    foreach (string i in MatchData.PlayerData.Keys)
                    {
                        if (MatchData.PlayerData[i].ClientAlive && gameClient.players[i] == null)
                        {
                            names.Add(MatchData.PlayerData[i].Name);
                        }
                    }

                    if (names.Count == 0)
                        lineText = lineText.Replace("$randomDead", "$none");
                    else lineText = lineText.Replace("$randomDead", names[UnityEngine.Random.Range(1, 999999) % names.Count]);
                }
                if (lineText.Contains("$random"))
                {
                    List<string> names = new List<string>();

                    foreach (string i in MatchData.PlayerData.Keys)
                    {
                        if (MatchData.PlayerData[i].ClientAlive)
                        {
                            names.Add(MatchData.PlayerData[i].Name);
                        }
                    }

                    if (names.Count == 0)
                        lineText = lineText.Replace("$random", "$none");
                    else lineText = lineText.Replace("$random", names[UnityEngine.Random.Range(1, 999999) % names.Count]);
                }
            }

            string[] parameters = lineText.Split(' ');

            try
            {
                if (parameters[0] == "name")
                {
                    if (GameClient.Instance != null)
                    {
                        if (parameters[1].Length > 16)
                            parameters[1].Remove(16);

                        parameters[1] = parameters[1].Replace("$", "");
                        parameters[1] = parameters[1].Replace("<", "");
                        parameters[1] = parameters[1].Replace(">", "");

                        GameClient.Instance.ClientSetName(parameters[1]);
                    }
                }
                else if (parameters[0] == "help")
                {
                    chatBox.GetComponent<Text>().text += @"
*CLIENT COMMANDS*

name <desired_name> - Changes local player name.
writeChat <all/team> <text> - Writes text.
clearChat - Clears Chat.
clearConsole - Clears Console.
writeConsole <text> - Writes text in console.
leaveMatch - Leaves Match.
quit - Closes game.
exec <fileName> <parameter1> <parameter2> <parameter3> ... - Load a config file from the /Config folder

*SERVER COMMANDS*

reset <time> - Restarts the game in <time> seconds.
setCash <playerName> <value> - Sets the cash of target player. 
setSpeed <playerName> <speed> - Sets the speed of target player.
setHealth <playerName> <health> - Sets the health of target player.
addCash <playerName> <value> - Adds cash of target player. 
addSpeed <playerName> <speed> - Adds speed to target player.
addHealth <playerName> <health> - Adds health to target player.
setVisibility <playerName> <true/false> - Sets the visibility of target player.
setRecoil <playerName> <true/false> - Sets the recoil of target player.
kill <playerName> - Kills player.
roundTime <time> - Changes the round time in seconds. Does not apply in the current round.
buyTime <time> - Changes the time allowed to buy. Does not apply in the current round.
freezeTime <time> - Changes the time you are frozen in place. Does not apply in the current round.
bombTime <time> - Changes the time it takes a bomb to explode. Does not apply in the current round.
warmupTime <time> - Changes the warmup time. This applies immediatly to the current round.
roundsPerHalf <value> - Changes how many rounds a half has.
roundsToWin <value> - Changes how many rounds it takes to win.
swapTeams <time> - Swaps the teams and restarts the round in <time> seconds.
warmupEnd <time> - End warmup in <time> seconds.
maxCash <value> - Changes the maximum amount of cash a player can hold.
loseCash <value> - Changes how much cash you gain from losing.
winCash <value> - Changes how much cash you gain from losing.
loseStreakCash <value> - Changes how much cash you gain from losing consecutively.
sendCommandFeedback <true/false> - Changes if a command should send feedback.
friendlyFire <true/false> - Changes whether or not teammates can shoot each other.
showKillFeed <true/false> - Changes whether or not kill feed is shown.
serverWhisper <playerName> <text> - Whispers to a player without anybody receiving feedback, not even the host.

";
                }
                else if (parameters[0] == "kick")
                {
                    if (parameters[1].Contains("$target"))
                        parameters[1] = targetName;
                    if (parameters[1].Contains("$killer"))
                        parameters[1] = killerName;

                    if (GameHost.Instance != null)
                    {
                        if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
                        {
                            if (parameters[1] == "$allPlayers")
                            {
                                if (sendCommandFeedback)
                                    Network.Instance.Send(new SystemMessage("<color=yellow>Kicked everybody for " + string.Join(" ", parameters.Skip(2)) + ".</color>"));
                            }
                            else if (parameters[1] == "$allT")
                            {
                                if (sendCommandFeedback)
                                    Network.Instance.Send(new SystemMessage("<color=yellow>Kicked T Team for " + string.Join(" ", parameters.Skip(2)) + ".</color>"));
                            }
                            else if (parameters[1] == "$allCT")
                            {
                                if (sendCommandFeedback)
                                    Network.Instance.Send(new SystemMessage("<color=yellow>Kicked CT Team for " + string.Join(" ", parameters.Skip(2)) + ".</color>"));
                            }
                            else if (parameters[1] == "$thisPlayer")
                            {
                                if (sendCommandFeedback)
                                    Network.Instance.Send(new SystemMessage("<color=yellow>Kicked " + GameClient.Instance.players[GameClient.Instance.playerId] + " for " + string.Join(" ", parameters.Skip(2)) + ".</color>"));

                                GameObject player = GameClient.Instance.players[GameClient.Instance.playerId];

                                if (player != null)
                                {
                                    GameHost.Instance.KickPlayer(player.GetComponent<Player>().Name, parameters[2]);
                                }
                            }
                            else
                            {

                                if (sendCommandFeedback)
                                    Network.Instance.Send(new SystemMessage("<color=yellow>Kicked " + parameters[1] + " for " + string.Join(" ", parameters.Skip(2)) + ".</color>"));

                            }

                            if (parameters[1] != "$thisPlayer")
                            {
                                GameHost.Instance.KickPlayer(parameters[1], parameters[2]);
                            }

                        }
                    }
                }
                else if (parameters[0] == "ban")
                {
                    if (parameters[1].Contains("$target"))
                        parameters[1] = targetName;
                    if (parameters[1].Contains("$killer"))
                        parameters[1] = killerName;

                    if (GameHost.Instance != null)
                    {
                        if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
                        {
                            if (parameters[1] == "$allPlayers")
                            {
                                if (sendCommandFeedback)
                                    Network.Instance.Send(new SystemMessage("<color=yellow>Banned everybody for " + string.Join(" ", parameters.Skip(2)) + ".</color>"));

                            }
                            else if (parameters[1] == "$allT")
                            {
                                if (sendCommandFeedback)
                                    Network.Instance.Send(new SystemMessage("<color=yellow>Banned T Team for " + string.Join(" ", parameters.Skip(2)) + ".</color>"));
                            }
                            else if (parameters[1] == "$allCT")
                            {
                                if (sendCommandFeedback)
                                    Network.Instance.Send(new SystemMessage("<color=yellow>Banned CT Team for " + string.Join(" ", parameters.Skip(2)) + ".</color>"));
                            }
                            else if (parameters[1] == "$thisPlayer")
                            {
                                if (sendCommandFeedback)
                                    Network.Instance.Send(new SystemMessage("<color=yellow>Banned " + GameClient.Instance.players[GameClient.Instance.playerId] + " for " + string.Join(" ", parameters.Skip(2)) + ".</color>"));

                                GameObject player = GameClient.Instance.players[GameClient.Instance.playerId];

                                if (player != null)
                                {
                                    GameHost.Instance.BanPlayer(player.GetComponent<Player>().Name, parameters[2]);
                                }
                            }
                            else
                            {

                                if (sendCommandFeedback)
                                    Network.Instance.Send(new SystemMessage("<color=yellow>Banned " + parameters[1] + " for " + string.Join(" ", parameters.Skip(2)) + ".</color>"));

                            }

                            if (parameters[1] != "$thisPlayer")
                            {
                                GameHost.Instance.BanPlayer(parameters[1], parameters[2]);
                            }

                        }
                    }
                }
                else if (parameters[0] == "changeTeam")
                {
                    if (parameters[1].Contains("$target"))
                        parameters[1] = targetName;
                    if (parameters[1].Contains("$killer"))
                        parameters[1] = killerName;
                    if (GameHost.Instance != null)
                    {
                        if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
                        {
                            if (parameters[2].Trim().Equals("ct") || parameters[2].Trim().Equals("t"))
                            {
                                if (parameters[1] == "$allPlayers")
                                {
                                    if (sendCommandFeedback)
                                        Network.Instance.Send(new SystemMessage("<color=yellow>Moved everybody to " + parameters[2] + ".</color>"));

                                }
                                else if (parameters[1] == "$allT")
                                {
                                    if (sendCommandFeedback)
                                        Network.Instance.Send(new SystemMessage("<color=yellow>Moved T Team to " + parameters[2] + ".</color>"));
                                }
                                else if (parameters[1] == "$allCT")
                                {
                                    if (sendCommandFeedback)
                                        Network.Instance.Send(new SystemMessage("<color=yellow>Moved CT Team to " + parameters[2] + ".</color>"));
                                }
                                else if (parameters[1] == "$thisPlayer")
                                {
                                    if (sendCommandFeedback)
                                        Network.Instance.Send(new SystemMessage("<color=yellow>Moved " + parameters[1] + " to " + GameClient.Instance.players[GameClient.Instance.playerId] + ".</color>"));

                                    GameObject player = GameClient.Instance.players[GameClient.Instance.playerId];

                                    if (player != null)
                                    {
                                        Network.Instance.Send(new ChangeTeam(player.GetComponent<Player>().Name, parameters[2]));
                                    }
                                }
                                else
                                {

                                    if (sendCommandFeedback)
                                        Network.Instance.Send(new SystemMessage("<color=yellow>Moved " + parameters[1] + " to " + parameters[2] + ".</color>"));

                                }

                                if (parameters[1] != "$thisPlayer")
                                {
                                    Network.Instance.Send(new ChangeTeam(parameters[1], parameters[2]));
                                }
                            }
                        }
                    }
                }
                else if (parameters[0] == "teleport")
                {
                    if (parameters[1].Contains("$target"))
                        parameters[1] = targetName;
                    if (parameters[1].Contains("$killer"))
                        parameters[1] = killerName;
                    if (GameHost.Instance != null)
                    {
                        if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
                        {
                            if (parameters[2].Trim().Equals("ctBase") || parameters[2].Trim().Equals("tbase") || parameters[1].Trim().Equals("bombSite"))
                            {
                                if (parameters[1] == "$allPlayers")
                                {
                                    if (sendCommandFeedback)
                                        Network.Instance.Send(new SystemMessage("<color=yellow>Teleported everybody to " + parameters[2] + ".</color>"));

                                }
                                else if (parameters[1] == "$allT")
                                {
                                    if (sendCommandFeedback)
                                        Network.Instance.Send(new SystemMessage("<color=yellow>Teleported T Team to " + parameters[2] + ".</color>"));
                                }
                                else if (parameters[1] == "$allCT")
                                {
                                    if (sendCommandFeedback)
                                        Network.Instance.Send(new SystemMessage("<color=yellow>Teleported CT Team to " + parameters[2] + ".</color>"));
                                }
                                else if (parameters[1] == "$thisPlayer")
                                {
                                    if (sendCommandFeedback)
                                        Network.Instance.Send(new SystemMessage("<color=yellow>Teleported " + parameters[1] + " to " + GameClient.Instance.players[GameClient.Instance.playerId] + ".</color>"));

                                    GameObject player = GameClient.Instance.players[GameClient.Instance.playerId];

                                    if (player != null)
                                    {
                                        Network.Instance.Send(new Teleport(player.GetComponent<Player>().name, parameters[2]));
                                    }
                                }
                                else
                                {
                                    if (sendCommandFeedback)
                                        Network.Instance.Send(new SystemMessage("<color=yellow>Teleported " + parameters[1] + " to " + parameters[2] + ".</color>"));
                                }

                                if (parameters[1] != "$thisPlayer")
                                {
                                    Network.Instance.Send(new Teleport(parameters[1], parameters[2]));
                                }
                            }
                        }
                    }
                }
                else if (parameters[0] == "serverWhisper")
                {
                    if (parameters[1].Contains("$target"))
                        parameters[1] = targetName;
                    if (parameters[1].Contains("$killer"))
                        parameters[1] = killerName;
                    if (GameHost.Instance != null)
                    {
                        if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
                        {
                            if (parameters[1] == "$thisPlayer")
                            {
                                Network.Instance.Send(new ServerWhisper(GameClient.Instance.currentName, string.Join(" ", parameters.Skip(2))));
                            }
                            else
                            {
                                Network.Instance.Send(new ServerWhisper(parameters[1], string.Join(" ", parameters.Skip(2))));
                            }
                        }
                    }
                }
                else if (parameters[0] == "friendlyFire")
                {
                    if (GameHost.Instance != null)
                    {
                        if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
                        {
                            if (sendCommandFeedback)
                                Network.Instance.Send(new SystemMessage("<color=yellow>Friendly Fire set to " + parameters[1] + ".</color>"));

                            if (parameters[1].Trim().Equals("true"))
                                Config.FriendlyFire = true;
                            else if (parameters[1].Trim().Equals("false"))
                                Config.FriendlyFire = false;
                        }
                    }
                }
                else if (parameters[0] == "showKillFeed")
                {
                    if (GameHost.Instance != null)
                    {
                        if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
                        {

                            if (sendCommandFeedback)
                                Network.Instance.Send(new SystemMessage("System 1 0 <color=yellow>Kill Feed Visibility set to " + parameters[1] + ".</color>"));

                            if (parameters[1].Trim().Equals("true"))
                                Config.ShowKillFeed = true;
                            else if (parameters[1].Trim().Equals("false"))
                                Config.ShowKillFeed = false;
                        }
                    }
                }
                else if (parameters[0] == "winByBombPlants")
                {
                    if (GameHost.Instance != null)
                    {
                        if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
                        {

                            if (sendCommandFeedback)
                                Network.Instance.Send(new SystemMessage("<color=yellow>Bomb Defusal gamemode set to " + parameters[1] + ".</color>"));

                            if (parameters[1].Trim().Equals("true"))
                                Config.BombDefusal = true;
                            else if (parameters[1].Trim().Equals("false"))
                                Config.BombDefusal = false;
                        }
                    }
                }
                else if (parameters[0] == "experimentalFeatures")
                {
                    if (GameHost.Instance != null)
                    {
                        if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
                        {
                            if (sendCommandFeedback)
                                Network.Instance.Send(new SystemMessage("<color=yellow>Experimental Features set to " + parameters[1] + ".</color>"));

                            if (parameters[1].Trim().Equals("true"))
                                Config.ExperimentalFeatures = true;
                            else if (parameters[1].Trim().Equals("false"))
                                Config.ExperimentalFeatures = false;
                        }
                    }
                }
                else if (parameters[0] == "sendCommandFeedback")
                {
                    bool value = false;
                    if (parameters[1].Trim().Equals("false"))
                        value = true;
                    if (parameters[1].Trim().Equals("true"))
                        value = false;
                    if (value)
                    {
                        sendCommandFeedback = false;
                    }
                }
                else if (parameters[0] == "reset")
                {
                    if (GameHost.Instance != null)
                    {
                        if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
                        {
                            try
                            {
                                int.Parse(parameters[1], CultureInfo.InvariantCulture);
                            }
                            catch
                            {
                                return;
                            }
                            if (sendCommandFeedback)
                                Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Game restarting in " + parameters[1] + " seconds.</color>")));
                                

                            Invoke("Restart", int.Parse(parameters[1], CultureInfo.InvariantCulture));
                        }
                    }
                }
                else if (parameters[0] == "addCash")
                {
                    if (parameters[1].Contains("$target"))
                        parameters[1] = targetName;
                    if (parameters[1].Contains("$killer"))
                        parameters[1] = killerName;
                    if (GameHost.Instance != null)
                    {
                        if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
                        {
                            try
                            {
                                int.Parse(parameters[2], CultureInfo.InvariantCulture);
                            }
                            catch
                            {
                                return;
                            }
                            if (parameters[1] == "$allPlayers")
                            {
                                if (sendCommandFeedback)
                                    Network.Instance.Send(new SystemMessage("<color=yellow>Added " + parameters[2] + " cash to everybody.</color>"));

                                GameHost host = GameHost.Instance;
                                foreach (string i in MatchData.PlayerData.Keys)
                                {
                                    if (MatchData.PlayerData[i].ClientAlive)
                                    {
                                        MatchData.PlayerData[i].Cash += int.Parse(parameters[2], CultureInfo.InvariantCulture);
                                    }
                                }

                                foreach (GameObject player in GameClient.Instance.players.Values)
                                {
                                    if (player != null)
                                        MatchData.PlayerData[player.GetComponent<Player>().ID].Cash += int.Parse(parameters[2], CultureInfo.InvariantCulture);
                                }
                            }
                            else if (parameters[1] == "$allT")
                            {
                                if (sendCommandFeedback)
                                    Network.Instance.Send(new SystemMessage("<color=yellow>Added " + parameters[2] + " cash to tero.</color>"));

                                GameHost host = GameHost.Instance;
                                foreach (string i in MatchData.PlayerData.Keys)
                                {
                                    if (MatchData.PlayerData[i].ClientAlive && MatchData.PlayerData[i].Team == 1)
                                    {
                                        MatchData.PlayerData[i].Cash += int.Parse(parameters[2], CultureInfo.InvariantCulture);
                                    }
                                }

                                foreach (GameObject player in GameClient.Instance.players.Values)
                                {
                                    if (player != null)
                                    {
                                        if (player.GetComponent<Player>().Team == 1)
                                            MatchData.PlayerData[player.GetComponent<Player>().ID].Cash += int.Parse(parameters[2], CultureInfo.InvariantCulture);
                                    }
                                }
                            }
                            else if (parameters[1] == "$allCT")
                            {
                                if (sendCommandFeedback)
                                    Network.Instance.Send(new SystemMessage("<color=yellow>Added " + parameters[2] + " cash to CTs.</color>"));

                                GameHost host = GameHost.Instance;
                                foreach (string i in MatchData.PlayerData.Keys)
                                {
                                    if (MatchData.PlayerData[i].ClientAlive && MatchData.PlayerData[i].Team == 0)
                                    {
                                        MatchData.PlayerData[i].Cash += int.Parse(parameters[2], CultureInfo.InvariantCulture);
                                    }
                                }

                                foreach (GameObject player in GameClient.Instance.players.Values)
                                {
                                    if (player != null)
                                    {
                                        if (player.GetComponent<Player>().Team == 0)
                                            MatchData.PlayerData[player.GetComponent<Player>().ID].Cash += int.Parse(parameters[2], CultureInfo.InvariantCulture);
                                    }
                                }
                            }
                            else if (parameters[1] == "$thisPlayer")
                            {
                                if (sendCommandFeedback)
                                    Network.Instance.Send(new SystemMessage("<color=yellow>Added " + GameClient.Instance.players[GameClient.Instance.playerId].GetComponent<Player>().Name + " cash to everybody.</color>"));

                                GameObject player = GameClient.Instance.players[GameClient.Instance.playerId];

                                MatchData.PlayerData[GameClient.Instance.playerId].Cash += int.Parse(parameters[2], CultureInfo.InvariantCulture);

                                if (player != null)
                                {
                                    MatchData.PlayerData[player.GetComponent<Player>().ID].Cash += int.Parse(parameters[2], CultureInfo.InvariantCulture);
                                }
                            }
                            else
                            {
                                if (sendCommandFeedback)
                                    Network.Instance.Send(new SystemMessage("<color=yellow>Set " + parameters[1] + "'s cash to " + parameters[2] + ".</color>"));

                                Debug.Log("test1");
                                GameHost host = GameHost.Instance;
                                foreach (string i in MatchData.PlayerData.Keys)
                                {
                                    if (MatchData.PlayerData[i].ClientAlive && MatchData.PlayerData[i].Name == parameters[1])
                                    {
                                        MatchData.PlayerData[i].Cash += int.Parse(parameters[2], CultureInfo.InvariantCulture);
                                        break;
                                    }
                                }
                                foreach (GameObject player in GameClient.Instance.players.Values)
                                {

                                    if (player != null)
                                    {
                                        if (player.GetComponent<Player>().Name.Equals(parameters[1]))
                                        {
                                            MatchData.PlayerData[player.GetComponent<Player>().ID].Cash += int.Parse(parameters[2], CultureInfo.InvariantCulture);

                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (parameters[0] == "setCash")
                {
                    Debug.Log("test-1");
                    if (parameters[1].Contains("$target"))
                        parameters[1] = targetName;
                    if (parameters[1].Contains("$killer"))
                        parameters[1] = killerName;
                    if (GameHost.Instance != null)
                    {
                        if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
                        {
                            try
                            {
                                int.Parse(parameters[2], CultureInfo.InvariantCulture);
                            }
                            catch
                            {
                                return;
                            }
                            if (parameters[1] == "$allPlayers")
                            {
                                if (sendCommandFeedback)
                                    Network.Instance.Send(new SystemMessage("<color=yellow>Set everybody's cash to " + parameters[2] + ".</color>"));

                                GameHost host = GameHost.Instance;
                                foreach (string i in MatchData.PlayerData.Keys)
                                {
                                    if (MatchData.PlayerData[i].ClientAlive)
                                    {
                                        MatchData.PlayerData[i].Cash = int.Parse(parameters[2], CultureInfo.InvariantCulture);
                                    }
                                }

                                foreach (GameObject player in GameClient.Instance.players.Values)
                                {
                                    if (player != null)
                                        MatchData.PlayerData[player.GetComponent<Player>().ID].Cash = int.Parse(parameters[2], CultureInfo.InvariantCulture);
                                }
                            }
                            else if (parameters[1] == "$allT")
                            {
                                if (sendCommandFeedback)
                                    Network.Instance.Send(new SystemMessage("<color=yellow>Set terorist team's cash to " + parameters[2] + ".</color>"));

                                GameHost host = GameHost.Instance;
                                foreach (string i in MatchData.PlayerData.Keys)
                                {
                                    if (MatchData.PlayerData[i].ClientAlive && MatchData.PlayerData[i].Team == 1)
                                    {
                                        MatchData.PlayerData[i].Cash = int.Parse(parameters[2], CultureInfo.InvariantCulture);
                                    }
                                }


                                foreach (GameObject player in GameClient.Instance.players.Values)
                                {
                                    if (player != null)
                                    {
                                        if (player.GetComponent<Player>().Team == 1)
                                            MatchData.PlayerData[player.GetComponent<Player>().ID].Cash = int.Parse(parameters[2], CultureInfo.InvariantCulture);
                                    }
                                }
                            }
                            else if (parameters[1] == "$allCT")
                            {
                                if (sendCommandFeedback)
                                    Network.Instance.Send(new SystemMessage("<color=yellow>Set CT team's cash to " + parameters[2] + ".</color>"));

                                GameHost host = GameHost.Instance;
                                foreach (string i in MatchData.PlayerData.Keys)
                                {
                                    if (MatchData.PlayerData[i].ClientAlive && MatchData.PlayerData[i].Team == 0)
                                    {
                                        MatchData.PlayerData[i].Cash = int.Parse(parameters[2], CultureInfo.InvariantCulture);
                                    }
                                }

                                foreach (GameObject player in GameClient.Instance.players.Values)
                                {
                                    if (player != null)
                                    {
                                        if (player.GetComponent<Player>().Team == 0)
                                            MatchData.PlayerData[player.GetComponent<Player>().ID].Cash = int.Parse(parameters[2], CultureInfo.InvariantCulture);
                                    }
                                }
                            }
                            else if (parameters[1] == "$thisPlayer")
                            {
                                if (sendCommandFeedback)
                                    Network.Instance.Send(new SystemMessage("<color=yellow>Set " + GameClient.Instance.players[GameClient.Instance.playerId].GetComponent<Player>().Name + "'s cash to " + parameters[2] + ".</color>"));

                                GameObject player = GameClient.Instance.players[GameClient.Instance.playerId];

                                MatchData.PlayerData[GameClient.Instance.playerId].Cash = int.Parse(parameters[2], CultureInfo.InvariantCulture);

                                if (player != null)
                                {
                                    MatchData.PlayerData[player.GetComponent<Player>().ID].Cash = int.Parse(parameters[2], CultureInfo.InvariantCulture);
                                }
                            }
                            else
                            {

                                Debug.Log("test0");

                                if (sendCommandFeedback)
                                    Network.Instance.Send(new SystemMessage("<color=yellow>Set " + parameters[1] + "'s cash to " + parameters[2] + ".</color>"));

                                Debug.Log("test1");
                                GameHost host = GameHost.Instance;
                                foreach (string i in MatchData.PlayerData.Keys)
                                {
                                    Debug.Log("test2");
                                    if (MatchData.PlayerData[i].ClientAlive && MatchData.PlayerData[i].Name == parameters[1])
                                    {
                                        MatchData.PlayerData[i].Cash = int.Parse(parameters[2], CultureInfo.InvariantCulture);
                                        break;
                                    }
                                }
                                foreach (GameObject player in GameClient.Instance.players.Values)
                                {

                                    if (player != null)
                                    {
                                        if (player.GetComponent<Player>().Name.Equals(parameters[1]))
                                        {
                                            MatchData.PlayerData[player.GetComponent<Player>().ID].Cash = int.Parse(parameters[2], CultureInfo.InvariantCulture);

                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (parameters[0] == "addSpeed")
                {
                    if (parameters[1].Contains("$target"))
                        parameters[1] = targetName;
                    if (parameters[1].Contains("$killer"))
                        parameters[1] = killerName;
                    if (GameHost.Instance != null)
                    {
                        if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
                        {
                            try
                            {
                                int.Parse(parameters[2], CultureInfo.InvariantCulture);
                            }
                            catch
                            {
                                return;
                            }
                            if (parameters[1] == "$allPlayers")
                            {
                                if (sendCommandFeedback)
                                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")));

                                foreach (GameObject player in GameClient.Instance.players.Values)
                                {
                                    if (player != null)
                                        player.GetComponent<Player>().SetSpeed(float.Parse(parameters[2], CultureInfo.InvariantCulture));
                                }
                            }
                            else if (parameters[1] == "$allT")
                            {
                                if (sendCommandFeedback)
                                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to T Team.</color>")));

                                foreach (GameObject player in GameClient.Instance.players.Values)
                                {
                                    if (player != null)
                                    {
                                        if (player.GetComponent<Player>().Team == 1)
                                            player.GetComponent<Player>().SetSpeed(float.Parse(parameters[2], CultureInfo.InvariantCulture));
                                    }
                                }
                            }
                            else if (parameters[1] == "$allCT")
                            {
                                if (sendCommandFeedback)
                                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to CT Team.</color>")));

                                foreach (GameObject player in GameClient.Instance.players.Values)
                                {
                                    if (player != null)
                                    {
                                        if (player.GetComponent<Player>().Team == 0)
                                            player.GetComponent<Player>().SetSpeed(float.Parse(parameters[2], CultureInfo.InvariantCulture));
                                    }
                                }
                            }
                            else if (parameters[1] == "$thisPlayer")
                            {
                                if (sendCommandFeedback)
                                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to " + GameClient.Instance.players[GameClient.Instance.playerId] + ".</color>")));

                                GameObject player = GameClient.Instance.players[GameClient.Instance.playerId];

                                if (player != null)
                                {
                                    player.GetComponent<Player>().SetSpeed(float.Parse(parameters[2], CultureInfo.InvariantCulture));
                                }
                            }
                            else
                            {

                                if (sendCommandFeedback)
                                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to " + parameters[1] + ".</color>")));


                                foreach (GameObject player in GameClient.Instance.players.Values)
                                {
                                    if (player != null)
                                    {
                                        if (player.GetComponent<Player>().Name.Equals(parameters[1]))
                                        {
                                            player.GetComponent<Player>().SetSpeed(float.Parse(parameters[2], CultureInfo.InvariantCulture));

                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (parameters[0] == "addHealth")
                {
                    if (parameters[1].Contains("$target"))
                        parameters[1] = targetName;
                    if (parameters[1].Contains("$killer"))
                        parameters[1] = killerName;
                    if (GameHost.Instance != null)
                    {
                        if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
                        {
                            try
                            {
                                int.Parse(parameters[2], CultureInfo.InvariantCulture);
                            }
                            catch
                            {
                                return;
                            }
                            if (parameters[1] == "$allPlayers")
                            {
                                if (sendCommandFeedback)
                                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " health to everybody.</color>")));

                                foreach (GameObject player in GameClient.Instance.players.Values)
                                {
                                    if (player != null)
                                    {
                                        player.GetComponent<Player>().AddHealth(float.Parse(parameters[2], CultureInfo.InvariantCulture), roundStart);
                                    }
                                }
                            }
                            else if (parameters[1] == "$allT")
                            {
                                if (sendCommandFeedback)
                                Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " health to tero.</color>")));

                                foreach (GameObject player in GameClient.Instance.players.Values)
                                {
                                    if (player != null)
                                    {
                                        if (player.GetComponent<Player>().Team == 1)
                                        {
                                            player.GetComponent<Player>().AddHealth(float.Parse(parameters[2], CultureInfo.InvariantCulture), roundStart);
                                        }
                                    }
                                }
                            }
                            else if (parameters[1] == "$allCT")
                            {
                                if (sendCommandFeedback)
                                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " health to CTs.</color>")));

                                foreach (GameObject player in GameClient.Instance.players.Values)
                                {
                                    if (player != null)
                                    {
                                        if (player.GetComponent<Player>().Team == 0)
                                        {
                                            player.GetComponent<Player>().AddHealth(float.Parse(parameters[2], CultureInfo.InvariantCulture), roundStart);
                                        }
                                    }
                                }
                            }
                            else if (parameters[1] == "$thisPlayer")
                            {
                                if (sendCommandFeedback)
                                Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + GameClient.Instance.players[GameClient.Instance.playerId].GetComponent<Player>().Name + " health to everybody.</color>")));

                                GameObject player = GameClient.Instance.players[GameClient.Instance.playerId];

                                if (player != null)
                                {
                                    player.GetComponent<Player>().AddHealth(float.Parse(parameters[2], CultureInfo.InvariantCulture), roundStart);
                                }
                            }
                            else
                            {
                                if (sendCommandFeedback)
                                Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Set " + parameters[1] + "'s health to " + parameters[2] + ".</color>")));


                                foreach (GameObject player in GameClient.Instance.players.Values)
                                {
                                    if (player != null)
                                    {
                                        if (player.GetComponent<Player>().Name.Equals(parameters[1]))
                                        {
                                            player.GetComponent<Player>().AddHealth(float.Parse(parameters[2], CultureInfo.InvariantCulture), roundStart);

                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (parameters[0] == "setSpeed")
                {
                    if (parameters[1].Contains("$target"))
                        parameters[1] = targetName;
                    if (parameters[1].Contains("$killer"))
                        parameters[1] = killerName;
                    if (GameHost.Instance != null)
                    {
                        if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
                        {
                            try
                            {
                                int.Parse(parameters[2], CultureInfo.InvariantCulture);
                            }
                            catch
                            {
                                return;
                            }
                            if (parameters[1] == "$allPlayers")
                            {
                                if (sendCommandFeedback)
                                Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Set everybody's speed to " + parameters[2] + ".</color>")));

                                foreach (GameObject player in GameClient.Instance.players.Values)
                                {
                                    if (player != null)
                                        player.GetComponent<Player>().SetSpeed(float.Parse(parameters[2], CultureInfo.InvariantCulture));
                                }
                            }
                            else if (parameters[1] == "$allT")
                            {
                                if (sendCommandFeedback)
                                Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Set terorist team's speed to " + parameters[2] + ".</color>")));

                                foreach (GameObject player in GameClient.Instance.players.Values)
                                {
                                    if (player != null)
                                    {
                                        if (player.GetComponent<Player>().Team == 1)
                                            player.GetComponent<Player>().SetSpeed(float.Parse(parameters[2], CultureInfo.InvariantCulture));
                                    }
                                }
                            }
                            else if (parameters[1] == "$allCT")
                            {
                                if (sendCommandFeedback)
                                Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Set CT team's speed to " + parameters[2] + ".</color>")));

                                foreach (GameObject player in GameClient.Instance.players.Values)
                                {
                                    if (player != null)
                                    {
                                        if (player.GetComponent<Player>().Team == 0)
                                            player.GetComponent<Player>().SetSpeed(float.Parse(parameters[2], CultureInfo.InvariantCulture));
                                    }
                                }
                            }
                            else if (parameters[1] == "$thisPlayer")
                            {
                                if (sendCommandFeedback)
                                Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Set " + GameClient.Instance.players[GameClient.Instance.playerId].GetComponent<Player>().Name + "'s speed to " + parameters[2] + ".</color>")));

                                GameObject player = GameClient.Instance.players[GameClient.Instance.playerId];

                                if (player != null)
                                {
                                    player.GetComponent<Player>().SetSpeed(float.Parse(parameters[2], CultureInfo.InvariantCulture));
                                }
                            }
                            else
                            {

                                if (sendCommandFeedback)
                                Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Set " + parameters[1] + "'s speed to " + parameters[2] + ".</color>")));


                                foreach (GameObject player in GameClient.Instance.players.Values)
                                {
                                    if (player != null)
                                    {
                                        if (player.GetComponent<Player>().Name.Equals(parameters[1]))
                                        {
                                            player.GetComponent<Player>().SetSpeed(float.Parse(parameters[2], CultureInfo.InvariantCulture));

                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (parameters[0] == "setHealth")
                {
                    if (parameters[1].Contains("$target"))
                        parameters[1] = targetName;
                    if (parameters[1].Contains("$killer"))
                        parameters[1] = killerName;
                    if (GameHost.Instance != null)
                    {
                        if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
                        {
                            try
                            {
                                int.Parse(parameters[2], CultureInfo.InvariantCulture);
                            }
                            catch
                            {
                                return;
                            }
                            if (parameters[1] == "$allPlayers")
                            {
                                if (sendCommandFeedback)
                                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Set everybody's health to " + parameters[2] + ".</color>")));

                                foreach (GameObject player in GameClient.Instance.players.Values)
                                {
                                    if (player != null)
                                    {
                                        player.GetComponent<Player>().SetHealth(float.Parse(parameters[2], CultureInfo.InvariantCulture), roundStart);
                                    }
                                }
                            }
                            else if (parameters[1] == "$allT")
                            {
                                if (sendCommandFeedback)
                                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Set terorist team's health to " + parameters[2] + ".</color>")));

                                foreach (GameObject player in GameClient.Instance.players.Values)
                                {
                                    if (player != null)
                                    {
                                        if (player.GetComponent<Player>().Team == 1)
                                        {
                                            player.GetComponent<Player>().SetHealth(float.Parse(parameters[2], CultureInfo.InvariantCulture), roundStart);
                                        }
                                    }
                                }
                            }
                            else if (parameters[1] == "$allCT")
                            {
                                if (sendCommandFeedback)
                                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Set CT team's health to " + parameters[2] + ".</color>")));

                                foreach (GameObject player in GameClient.Instance.players.Values)
                                {
                                    if (player != null)
                                    {
                                        if (player.GetComponent<Player>().Team == 0)
                                        {
                                            player.GetComponent<Player>().SetHealth(float.Parse(parameters[2], CultureInfo.InvariantCulture), roundStart);
                                        }
                                    }
                                }
                            }
                            else if (parameters[1] == "$thisPlayer")
                            {
                                if (sendCommandFeedback)
                                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Set " + GameClient.Instance.players[GameClient.Instance.playerId].GetComponent<Player>().Name + "'s health to " + parameters[2] + ".</color>")));

                                GameObject player = GameClient.Instance.players[GameClient.Instance.playerId];

                                if (player != null)
                                {
                                    player.GetComponent<Player>().SetHealth(float.Parse(parameters[2], CultureInfo.InvariantCulture), roundStart);
                                }
                            }
                            else
                            {
                                if (sendCommandFeedback)
                                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Set " + parameters[1] + "'s health to " + parameters[2] + ".</color>")));


                                foreach (GameObject player in GameClient.Instance.players.Values)
                                {
                                    if (player != null)
                                    {
                                        if (player.GetComponent<Player>().Name.Equals(parameters[1]))
                                        {
                                            player.GetComponent<Player>().SetHealth(float.Parse(parameters[2], CultureInfo.InvariantCulture), roundStart);

                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (parameters[0] == "setVisibility")
                {
                    if (parameters[1].Contains("$target"))
                        parameters[1] = targetName;
                    if (parameters[1].Contains("$killer"))
                        parameters[1] = killerName;
                    if (GameHost.Instance != null)
                    {
                        if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
                        {
                            try
                            {
                                bool.Parse(parameters[2]);
                            }
                            catch
                            {
                                return;
                            }
                            if (parameters[1] == "$allPlayers")
                            {
                                if (sendCommandFeedback)
                                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Set everybody's visibility to " + parameters[2] + ".</color>")));

                                foreach (GameObject player in GameClient.Instance.players.Values)
                                {
                                    if (player != null)
                                    {
                                        Network.Instance.Send(new RoundStart(roundStart, new Visibility(player.GetComponent<Player>().ID, parameters[2])));
                                        player.GetComponent<Player>().SetVisibility(bool.Parse(parameters[2]));
                                    }
                                }
                            }
                            else if (parameters[1] == "$allT")
                            {
                                if (sendCommandFeedback)
                                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Set terorist team's visibility to " + parameters[2] + ".</color>")));

                                foreach (GameObject player in GameClient.Instance.players.Values)
                                {
                                    if (player != null)
                                    {
                                        if (player.GetComponent<Player>().Team == 1)
                                        {
                                            Network.Instance.Send(new RoundStart(roundStart, new Visibility(player.GetComponent<Player>().ID, parameters[2])));
                                            
                                            player.GetComponent<Player>().SetVisibility(bool.Parse(parameters[2]));
                                        }
                                    }
                                }
                            }
                            else if (parameters[1] == "$allCT")
                            {
                                if (sendCommandFeedback)
                                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Set CT team's visibility to " + parameters[2] + ".</color>")));

                                foreach (GameObject player in GameClient.Instance.players.Values)
                                {
                                    if (player != null)
                                    {
                                        if (player.GetComponent<Player>().Team == 0)
                                        {
                                            Network.Instance.Send(new RoundStart(roundStart, new Visibility(player.GetComponent<Player>().ID, parameters[2])));
                                            player.GetComponent<Player>().SetVisibility(bool.Parse(parameters[2]));
                                        }
                                    }
                                }
                            }
                            else if (parameters[1] == "$thisPlayer")
                            {
                                if (sendCommandFeedback)
                                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Set " + GameClient.Instance.players[GameClient.Instance.playerId].GetComponent<Player>().Name + "'s visibility to " + parameters[2] + ".</color>"))); 

                                GameObject player = GameClient.Instance.players[GameClient.Instance.playerId];

                                if (player != null)
                                {
                                    Network.Instance.Send(new RoundStart(roundStart, new Visibility(player.GetComponent<Player>().ID, parameters[2])));
                                    player.GetComponent<Player>().SetVisibility(bool.Parse(parameters[2]));
                                }
                            }
                            else
                            {
                                if (sendCommandFeedback)
                                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Set " + parameters[1] + "'s visibility to " + parameters[2] + ".</color>")));


                                foreach (GameObject player in GameClient.Instance.players.Values)
                                {
                                    if (player != null)
                                    {
                                        if (player.GetComponent<Player>().Name.Equals(parameters[1]))
                                        {
                                            Network.Instance.Send(new RoundStart(roundStart, new Visibility(player.GetComponent<Player>().ID, parameters[2])));

                                            player.GetComponent<Player>().SetVisibility(bool.Parse(parameters[2]));

                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (parameters[0] == "setRecoil")
                {
                    if (parameters[1].Contains("$target"))
                        parameters[1] = targetName;
                    if (parameters[1].Contains("$killer"))
                        parameters[1] = killerName;
                    if (GameHost.Instance != null)
                    {
                        if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
                        {
                            try
                            {
                                bool.Parse(parameters[2]);
                            }
                            catch
                            {
                                return;
                            }
                            if (parameters[1] == "$allPlayers")
                            {
                                if (sendCommandFeedback)
                                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Set everybody's recoil to " + parameters[2] + ".</color>")));

                                foreach (GameObject player in GameClient.Instance.players.Values)
                                {
                                    if (player != null)
                                    {
                                        player.GetComponent<Player>().GunRecoil = bool.Parse(parameters[2]);
                                    }
                                }
                            }
                            else if (parameters[1] == "$allT")
                            {
                                if (sendCommandFeedback)
                                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Set terorist team's recoil to " + parameters[2] + ".</color>")));

                                foreach (GameObject player in GameClient.Instance.players.Values)
                                {
                                    if (player != null)
                                    {
                                        if (player.GetComponent<Player>().Team == 1)
                                            player.GetComponent<Player>().GunRecoil = bool.Parse(parameters[2]);
                                    }
                                }
                            }
                            else if (parameters[1] == "$allCT")
                            {
                                if (sendCommandFeedback)
                                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Set CT team's recoil to " + parameters[2] + ".</color>")));

                                foreach (GameObject player in GameClient.Instance.players.Values)
                                {
                                    if (player != null)
                                    {
                                        if (player.GetComponent<Player>().Team == 0)
                                            player.GetComponent<Player>().GunRecoil = bool.Parse(parameters[2]);
                                    }
                                }
                            }
                            else if (parameters[1] == "$thisPlayer")
                            {
                                if (sendCommandFeedback)
                                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Set " + GameClient.Instance.players[GameClient.Instance.playerId].GetComponent<Player>().Name + "'s recoil to " + parameters[2] + ".</color>")));

                                GameObject player = GameClient.Instance.players[GameClient.Instance.playerId];

                                if (player != null)
                                {
                                    player.GetComponent<Player>().GunRecoil = bool.Parse(parameters[2]);
                                }
                            }
                            else
                            {
                                if (sendCommandFeedback)
                                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Set " + parameters[1] + "'s recoil to " + parameters[2] + ".</color>")));


                                foreach (GameObject player in GameClient.Instance.players.Values)
                                {
                                    if (player != null)
                                    {
                                        if (player.GetComponent<Player>().Name.Equals(parameters[1]))
                                        {
                                            player.GetComponent<Player>().GunRecoil = bool.Parse(parameters[2]);

                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (parameters[0] == "kill")
                {
                    Debug.Log(parameters[1]);
                    if (parameters[1].Contains("$target"))
                        parameters[1] = targetName;
                    if (parameters[1].Contains("$killer"))
                        parameters[1] = killerName;
                    Debug.Log(parameters[1]);
                    if (GameHost.Instance != null)
                    {
                        if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
                        {
                            if (parameters[1] == "$allPlayers")
                            {
                                if (sendCommandFeedback)
                                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Killed all players.</color>")));

                                foreach (GameObject player in GameClient.Instance.players.Values)
                                {
                                    if (player != null)
                                    {
                                        MatchData.PlayerData[player.GetComponent<Player>().ID].Deaths++;

                                        Network.Instance.Send(new RoundStart(roundStart, new SetDeaths(player.GetComponent<Player>().ID, MatchData.PlayerData[player.GetComponent<Player>().ID].Deaths)));

                                        Network.Instance.Send(new RoundStart(roundStart, new Death(player.GetComponent<Player>().ID)));
                                    }
                                }
                            }
                            else if (parameters[1] == "$allT")
                            {
                                if (sendCommandFeedback)
                                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Killed all terrorists.</color>")));

                                foreach (GameObject player in GameClient.Instance.players.Values)
                                {
                                    if (player != null)
                                    {
                                        if (player.GetComponent<Player>().Team == 1)
                                        {
                                            MatchData.PlayerData[player.GetComponent<Player>().ID].Deaths++;

                                            Network.Instance.Send(new RoundStart(roundStart, new SetDeaths(player.GetComponent<Player>().ID, MatchData.PlayerData[player.GetComponent<Player>().ID].Deaths)));

                                            Network.Instance.Send(new RoundStart(roundStart, new Death(player.GetComponent<Player>().ID)));
                                        }
                                    }
                                }
                            }
                            else if (parameters[1] == "$allCT")
                            {
                                if (sendCommandFeedback)
                                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Killed all CTs.</color>")));

                                foreach (GameObject player in GameClient.Instance.players.Values)
                                {
                                    if (player != null)
                                    {
                                        if (player.GetComponent<Player>().Team == 0)
                                        {
                                            MatchData.PlayerData[player.GetComponent<Player>().ID].Deaths++;

                                            Network.Instance.Send(new RoundStart(roundStart, new SetDeaths(player.GetComponent<Player>().ID, MatchData.PlayerData[player.GetComponent<Player>().ID].Deaths)));

                                            Network.Instance.Send(new RoundStart(roundStart, new Death(player.GetComponent<Player>().ID)));
                                        }
                                    }
                                }
                            }
                            else if (parameters[1] == "$thisPlayer")
                            {
                                if (sendCommandFeedback)
                                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Killed " + GameClient.Instance.players[GameClient.Instance.playerId].GetComponent<Player>().Name + ".</color>")));

                                GameObject player = GameClient.Instance.players[GameClient.Instance.playerId];

                                if (player != null)
                                {
                                    MatchData.PlayerData[player.GetComponent<Player>().ID].Deaths++;

                                    Network.Instance.Send(new RoundStart(roundStart, new SetDeaths(player.GetComponent<Player>().ID, MatchData.PlayerData[player.GetComponent<Player>().ID].Deaths)));

                                    Network.Instance.Send(new RoundStart(roundStart, new Death(player.GetComponent<Player>().ID)));

                                }
                            }
                            else
                            {
                                if (sendCommandFeedback)
                                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Killed " + parameters[1] + ".</color>")));


                                foreach (GameObject player in GameClient.Instance.players.Values)
                                {
                                    if (player != null)
                                    {
                                        if (player.GetComponent<Player>().Name.Equals(parameters[1]))
                                        {
                                            MatchData.PlayerData[player.GetComponent<Player>().ID].Deaths++;

                                            Network.Instance.Send(new RoundStart(roundStart, new SetDeaths(player.GetComponent<Player>().ID, MatchData.PlayerData[player.GetComponent<Player>().ID].Deaths)));

                                            Network.Instance.Send(new RoundStart(roundStart, new Death(player.GetComponent<Player>().ID)));

                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (parameters[0] == "buyTime")
                {
                    if (GameHost.Instance != null)
                    {
                        if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
                        {
                            try
                            {
                                int.Parse(parameters[1], CultureInfo.InvariantCulture);
                            }
                            catch
                            {
                                return;
                            }
                            if (sendCommandFeedback)
                                Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Set buy time to " + parameters[1] + ".</color>")));

                            Config.BuyTime = int.Parse(parameters[1], CultureInfo.InvariantCulture);
                        }
                    }
                }
                else if (parameters[0] == "freezeTime")
                {
                    if (GameHost.Instance != null)
                    {
                        if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
                        {
                            try
                            {
                                int.Parse(parameters[1], CultureInfo.InvariantCulture);
                            }
                            catch
                            {
                                return;
                            }
                            if (sendCommandFeedback)
                                Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Set freeze time to " + parameters[1] + ".</color>")));

                            Config.FreezeTime = int.Parse(parameters[1], CultureInfo.InvariantCulture);
                        }
                    }
                }
                else if (parameters[0] == "bombTime")
                {
                    if (GameHost.Instance != null)
                    {
                        if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
                        {
                            try
                            {
                                int.Parse(parameters[1], CultureInfo.InvariantCulture);
                            }
                            catch
                            {
                                return;
                            }
                            if (sendCommandFeedback)
                                Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Set bomb time to " + parameters[1] + ".</color>")));

                            Config.BombTime = int.Parse(parameters[1], CultureInfo.InvariantCulture);
                        }
                    }
                }
                else if (parameters[0] == "roundTime")
                {
                    if (GameHost.Instance != null)
                    {
                        if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
                        {
                            try
                            {
                                int.Parse(parameters[1], CultureInfo.InvariantCulture);
                            }
                            catch
                            {
                                return;
                            }
                            if (sendCommandFeedback)
                                Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Set round time to " + parameters[1] + ".</color>")));

                            Config.RoundTime = int.Parse(parameters[1], CultureInfo.InvariantCulture);
                        }
                    }
                }
                else if (parameters[0] == "warmupTime")
                {
                    if (GameHost.Instance != null)
                    {
                        if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
                        {
                            try
                            {
                                int.Parse(parameters[1], CultureInfo.InvariantCulture);
                            }
                            catch
                            {
                                return;
                            }
                            if (sendCommandFeedback)
                                Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Changed warmup time to " + parameters[1] + ".</color>")));

                            GameHost.Instance.warmupTimeLeft = int.Parse(parameters[1], CultureInfo.InvariantCulture);
                        }
                    }
                }
                else if (parameters[0] == "roundsPerHalf")
                {
                    if (GameHost.Instance != null)
                    {
                        if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
                        {
                            try
                            {
                                int.Parse(parameters[1], CultureInfo.InvariantCulture);
                            }
                            catch
                            {
                                return;
                            }
                            if (sendCommandFeedback)
                                Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Set rounds per half to " + parameters[1] + ".</color>")));

                            Config.RoundsPerHalf = int.Parse(parameters[1], CultureInfo.InvariantCulture);
                        }
                    }
                }
                else if (parameters[0] == "roundsToWin")
                {
                    if (GameHost.Instance != null)
                    {
                        if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
                        {
                            try
                            {
                                int.Parse(parameters[1], CultureInfo.InvariantCulture);
                            }
                            catch
                            {
                                return;
                            }
                            if (sendCommandFeedback)
                                Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Set rounds to win to " + parameters[1] + ".</color>")));

                            Config.RoundsToWin = int.Parse(parameters[1], CultureInfo.InvariantCulture);
                        }
                    }
                }
                else if (parameters[0] == "swapTeams")
                {
                    if (GameHost.Instance != null)
                    {
                        if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
                        {
                            try
                            {
                                int.Parse(parameters[1], CultureInfo.InvariantCulture);
                            }
                            catch
                            {
                                return;
                            }
                            if (sendCommandFeedback)
                                Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Swapping teams in " + parameters[1] + " seconds.</color>")));

                            Invoke("Swap", int.Parse(parameters[1], CultureInfo.InvariantCulture));
                        }
                    }
                }
                else if (parameters[0] == "warmupEnd")
                {
                    if (GameHost.Instance != null)
                    {
                        if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
                        {
                            try
                            {
                                int.Parse(parameters[1], CultureInfo.InvariantCulture);
                            }
                            catch
                            {
                                return;
                            }
                            if (sendCommandFeedback)
                                Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Ending warmup in " + parameters[1] + " seconds.</color>")));

                            Invoke("WarmupEnd", int.Parse(parameters[1], CultureInfo.InvariantCulture));
                        }
                    }
                }
                else if (parameters[0] == "maxCash")
                {
                    if (GameHost.Instance != null)
                    {
                        if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
                        {
                            try
                            {
                                int.Parse(parameters[1], CultureInfo.InvariantCulture);
                            }
                            catch
                            {
                                return;
                            }
                            if (sendCommandFeedback)
                                Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Set max cash to " + parameters[1] + ".</color>")));

                            Config.MaxCash = int.Parse(parameters[1], CultureInfo.InvariantCulture);
                        }
                    }
                }
                else if (parameters[0] == "loseCash")
                {
                    if (GameHost.Instance != null)
                    {
                        if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
                        {
                            try
                            {
                                int.Parse(parameters[1], CultureInfo.InvariantCulture);
                            }
                            catch
                            {
                                return;
                            }
                            if (sendCommandFeedback)
                                Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Set lose cash to " + parameters[1] + ".</color>")));

                            Config.LoseCash = int.Parse(parameters[1], CultureInfo.InvariantCulture);
                        }
                    }
                }
                else if (parameters[0] == "loseStreakCash")
                {
                    if (GameHost.Instance != null)
                    {
                        if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
                        {
                            try
                            {
                                int.Parse(parameters[1], CultureInfo.InvariantCulture);
                            }
                            catch
                            {
                                return;
                            }
                            if (sendCommandFeedback)
                                Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Set lose streak cash to " + parameters[1] + ".</color>")));

                            Config.LoseStreakCash = int.Parse(parameters[1], CultureInfo.InvariantCulture);
                        }
                    }
                }
                else if (parameters[0] == "winCash")
                {
                    if (GameHost.Instance != null)
                    {
                        if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
                        {
                            try
                            {
                                int.Parse(parameters[1], CultureInfo.InvariantCulture);
                            }
                            catch
                            {
                                return;
                            }
                            if (sendCommandFeedback)
                                Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Set win cash to " + parameters[1] + ".</color>")));

                            Config.WinCash = int.Parse(parameters[1], CultureInfo.InvariantCulture);
                        }
                    }
                }
                else if (parameters[0] == "writeChat")
                {
                    int all = -1;
                    if (parameters[1] == "all")
                        all = 1;
                    else if (parameters[1] == "team")
                        all = 0;
                    if (all > -1)
                        Chat.Instance.SendMessage(string.Join(" ", parameters.Skip(2)), all);
                }
                else if (parameters[0] == "clearChat")
                {
                    Chat.Instance.Clear();
                }
                else if (parameters[0] == "clearConsole")
                {
                    chatBox.GetComponent<Text>().text = "";
                }
                else if (parameters[0] == "writeConsole")
                {
                    chatBox.GetComponent<Text>().text += string.Join(" ", parameters.Skip(1)) + "\n";
                }
                else if (parameters[0] == "leaveMatch")
                {
                    Escape.Instance.Terminate();
                }
                else if (parameters[0] == "quit")
                {
                    Application.Quit();
                }
            }
            catch
            {

            }
        }
    }

    public void MouseEnter()
    {
        hovered = true;
    }

    public void MouseExit()
    {
        hovered = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        /*if (Input.GetKeyDown(KeyCode.T) && ConsoleCanvas.Instance.content.GetComponent<Console>().focused == false)
        {
            if (focused == false)
            {
                typeBoxInputField.interactable = true;
                typeBoxInputField.ActivateInputField();
                //focused = true;
            }
        }*/
        if (Input.GetKey(KeyCode.Return))
        {
            if (pressed2 == false)
            {
                pressed2 = true;
                bool ok = false;
                if (Chat.Instance != null)
                {
                    if (Chat.Instance.Focused == false)
                        ok = true;
                }
                else ok = true;
                if (ok)
                {

                    if (typeBoxInputField.text != "" && focused)
                    {
                        string text = typeBoxInputField.text;

                        InterpretText(text);

                        typeBoxInputField.text = "";

                    }
                }
            }
        }
        else pressed2 = false;

        focused = typeBoxInputField.isFocused;
        if (focused == false)
            typeBoxInputField.interactable = false;
        if (hovered == false && Input.GetMouseButton(0))
        {
            if (pressed1 == false)
            {
                pressed1 = true;
                typeBoxInputField.interactable = false;
                typeBoxInputField.DeactivateInputField();
            }
        }
        else if (hovered && Input.GetMouseButton(0))
        {
            if (pressed1 == false)
            {
                pressed1 = true;
                typeBoxInputField.interactable = true;
                typeBoxInputField.ActivateInputField();
            }
        }
        else pressed1 = false;
    }

    void Restart()
    {
        GameHost.Instance.RestartGame();
    }

    void Swap()
    {
        GameHost.Instance.Swap();
        GameHost.Instance.SendNewRoundPacket();
    }

    void WarmupEnd()
    {
        GameHost.Instance.WarmupEnd();
    }
}

#region FixedUpdate Code
/*if (GameHost.Instance != null)
{
    Debug.Log("test1");
    GameHost gameHost = GameHost.Instance;
    GameClient gameClient = GameClient.Instance;
    if (text.Contains("$randomAlive"))
    {
        List<string> names = new List<string>();

        foreach (long i in MatchData.PlayerData.Keys)
        {
            if (MatchData.PlayerData[i].ClientAlive && gameClient.players[i] != null)
            {
                names.Add(MatchData.PlayerData[i].Name);
            }
        }

        if (names.Count == 0)
            text = text.Replace("$randomAlive", "$none");
        else text = text.Replace("$randomAlive", names[UnityEngine.Random.Range(1, 999999) % names.Count]);
    }
    if (text.Contains("$randomDead"))
    {

        List<string> names = new List<string>();

        foreach (long i in MatchData.PlayerData.Keys)
        {
            if (MatchData.PlayerData[i].ClientAlive && gameClient.players[i] == null)
            {
                names.Add(MatchData.PlayerData[i].Name);
            }
        }

        if (names.Count == 0)
            text = text.Replace("$randomDead", "$none");
        else text = text.Replace("$randomDead", names[UnityEngine.Random.Range(1, 999999) % names.Count]);
    }
    if (text.Contains("$random"))
    {
        List<string> names = new List<string>();

        foreach (long i in MatchData.PlayerData.Keys)
        {
            if (MatchData.PlayerData[i].ClientAlive)
            {
                names.Add(MatchData.PlayerData[i].Name);
            }
        }

        if (names.Count == 0)
            text = text.Replace("$random", "$none");
        else text = text.Replace("$random", names[UnityEngine.Random.Range(1, 999999) % names.Count]);
    }
}


chatBox.GetComponent<Text>().text += text + "\n";

string[] parameters = text.Split(' ');

try
{

    if (parameters[0] == "name")
    {
        if (GameClient.Instance != null)
        {
            if (parameters[1].Length > 16)
                parameters[1].Remove(16);

            parameters[1] = parameters[1].Replace("$", "");
            parameters[1] = parameters[1].Replace("<", "");
            parameters[1] = parameters[1].Replace(">", "");

            GameClient.Instance.ClientSetName(parameters[1]);
        }
    }
    else if (parameters[0] == "kick")
    {
        if (GameHost.Instance != null)
        {
            if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
            {
                if (parameters[1] == "$allPlayers")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Kicked everybody for " + string.Join(" ", parameters.Skip(2)) + ".</color>\n";

                }
                else if (parameters[1] == "$allT")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Kicked T Team for " + string.Join(" ", parameters.Skip(2)) + ".</color>\n";
                }
                else if (parameters[1] == "$allCT")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Kicked CT Team for " + string.Join(" ", parameters.Skip(2)) + ".</color>\n";
                }
                else if (parameters[1] == "$thisPlayer")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Kicked " + GameClient.Instance.players[GameClient.Instance.playerId] + " for " + string.Join(" ", parameters.Skip(2)) + ".</color>\n";

                    GameObject player = GameClient.Instance.players[GameClient.Instance.playerId];

                    if (player != null)
                    {
                        GameHost.Instance.KickPlayer(player.GetComponent<Player>().Name, parameters[2]);
                    }
                }
                else
                {

                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Kicked " + parameters[1] + " for " + string.Join(" ", parameters.Skip(2)) + ".</color>\n";

                }

                if (parameters[1] != "$thisPlayer")
                {
                    GameHost.Instance.KickPlayer(parameters[1], parameters[2]);
                }

            }
        }
    }
    else if (parameters[0] == "ban")
    {
        if (GameHost.Instance != null)
        {
            if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
            {
                if (parameters[1] == "$allPlayers")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Banned everybody for " + string.Join(" ", parameters.Skip(2)) + ".</color>\n";

                }
                else if (parameters[1] == "$allT")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Banned T Team for " + string.Join(" ", parameters.Skip(2)) + ".</color>\n";
                }
                else if (parameters[1] == "$allCT")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Banned CT Team for " + string.Join(" ", parameters.Skip(2)) + ".</color>\n";
                }
                else if (parameters[1] == "$thisPlayer")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Banned " + GameClient.Instance.players[GameClient.Instance.playerId] + " for " + string.Join(" ", parameters.Skip(2)) + ".</color>\n";

                    GameObject player = GameClient.Instance.players[GameClient.Instance.playerId];

                    if (player != null)
                    {
                        GameHost.Instance.BanPlayer(player.GetComponent<Player>().Name, parameters[2]);
                    }
                }
                else
                {

                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Banned " + parameters[1] + " for " + string.Join(" ", parameters.Skip(2)) + ".</color>\n";

                }

                if (parameters[1] != "$thisPlayer")
                {
                    GameHost.Instance.BanPlayer(parameters[1], parameters[2]);
                }

            }
        }
    }
    else if (parameters[0] == "serverWhisper")
    {
        if (GameHost.Instance != null)
        {
            if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
            {
                if (parameters[1] == "$thisPlayer")
                {
                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "ServerWhisper " + GameClient.Instance.currentName + " " + string.Join(" ", parameters.Skip(2));
                }
                else
                {
                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "ServerWhisper " + string.Join(" ", parameters.Skip(1));
                }
            }
        }
    }
    else if (parameters[0] == "friendlyFire")
    {
        if (GameHost.Instance != null)
        {
            if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
            {
                if (sendCommandFeedback)
                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Friendly Fire set to " + parameters[1] + ".</color>\n";

                if (parameters[1] == "true")
                    Config.FriendlyFire = true;
                else if (parameters[1] == "false")
                    Config.FriendlyFire = false;
            }
        }
    }
    else if (parameters[0] == "showKillFeed")
    {
        if (GameHost.Instance != null)
        {
            if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
            {
                if (sendCommandFeedback)
                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Kill Feed Visibility set to " + parameters[1] + ".</color>\n";

                if (parameters[1] == "true")
                    Config.ShowKillFeed = true;
                else if (parameters[1] == "false")
                    Config.ShowKillFeed = false;
            }
        }
    }
    else if (parameters[0] == "winByBombPlants")
    {
        if (GameHost.Instance != null)
        {
            if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
            {

                if (sendCommandFeedback)
                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Bomb Defusal gamemode set to " + parameters[1] + ".</color>\n";

                if (parameters[1] == "true")
                    Config.BombDefusal = true;
                else if (parameters[1] == "false")
                    Config.BombDefusal = false;
            }
        }
    }
    else if (parameters[0] == "reset")
    {
        if (GameHost.Instance != null)
        {
            if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
            {
                try
                {
                    int.Parse(parameters[1], CultureInfo.InvariantCulture);
                }
                catch
                {
                    return;
                }
                if (sendCommandFeedback)
                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Game restarting in " + parameters[1] + " seconds.</color>\n";

                Invoke("Restart", int.Parse(parameters[1], CultureInfo.InvariantCulture));
            }
        }
    }
    else if (parameters[0] == "changeTeam")
    {
        if (GameHost.Instance != null)
        {
            if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
            {
                if (parameters[2] == "ct" || parameters[2] == "t")
                {
                    if (parameters[1] == "$allPlayers")
                    {
                        if (sendCommandFeedback)
                            Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Moved everybody to " + parameters[2] + ".</color>\n";

                    }
                    else if (parameters[1] == "$allT")
                    {
                        if (sendCommandFeedback)
                            Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Moved T Team to " + parameters[2] + ".</color>\n";
                    }
                    else if (parameters[1] == "$allCT")
                    {
                        if (sendCommandFeedback)
                            Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Moved CT Team to " + parameters[2] + ".</color>\n";
                    }
                    else if (parameters[1] == "$thisPlayer")
                    {
                        if (sendCommandFeedback)
                            Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Moved " + parameters[1] + " to " + GameClient.Instance.players[GameClient.Instance.playerId] + ".</color>\n";

                        GameObject player = GameClient.Instance.players[GameClient.Instance.playerId];

                        if (player != null)
                        {
                            Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "ChangeTeam " + player.GetComponent<Player>().Name + " " + parameters[2] + "\n";
                        }
                    }
                    else
                    {

                        if (sendCommandFeedback)
                            Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Moved " + parameters[1] + " to " + parameters[2] + ".</color>\n";

                    }

                    if (parameters[1] != "$thisPlayer")
                    {
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "ChangeTeam " + parameters[1] + " " + parameters[2] + "\n";
                    }
                }
            }
        }
    }
    else if (parameters[0] == "teleport")
    {
        if (GameHost.Instance != null)
        {
            if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
            {
                if (parameters[2] == "ctBase" || parameters[2] == "tBase" || parameters[2].Contains("bombSite"))
                {
                    if (parameters[1] == "$allPlayers")
                    {
                        if (sendCommandFeedback)
                            Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Teleported everybody to " + parameters[2] + ".</color>\n";

                    }
                    else if (parameters[1] == "$allT")
                    {
                        if (sendCommandFeedback)
                            Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Teleported T Team to " + parameters[2] + ".</color>\n";
                    }
                    else if (parameters[1] == "$allCT")
                    {
                        if (sendCommandFeedback)
                            Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Teleported CT Team to " + parameters[2] + ".</color>\n";
                    }
                    else if (parameters[1] == "$thisPlayer")
                    {
                        if (sendCommandFeedback)
                            Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Teleported " + parameters[1] + " to " + GameClient.Instance.players[GameClient.Instance.playerId] + ".</color>\n";

                        GameObject player = GameClient.Instance.players[GameClient.Instance.playerId];

                        if (player != null)
                        {
                            Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "Teleport " + player.GetComponent<Player>().Name + " " + parameters[2] + "\n";
                        }
                    }
                    else
                    {

                        if (sendCommandFeedback)
                            Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Teleported " + parameters[1] + " to " + parameters[2] + ".</color>\n";

                    }

                    if (parameters[1] != "$thisPlayer")
                    {
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "Teleport " + parameters[1] + " " + parameters[2] + "\n";
                    }
                }
            }
        }
    }
    else if (parameters[0] == "addSpeed")
    {
        if (GameHost.Instance != null)
        {
            if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
            {
                try
                {
                    int.Parse(parameters[2], CultureInfo.InvariantCulture);
                }
                catch
                {
                    return;
                }
                if (parameters[1] == "$allPlayers")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Added " + parameters[2] + " speed to everybody.</color>\n";

                    foreach (GameObject player in GameClient.Instance.players.Values)
                    {
                        if (player != null)
                            player.GetComponent<Player>().SetSpeed(float.Parse(parameters[2], CultureInfo.InvariantCulture));
                    }
                }
                else if (parameters[1] == "$allT")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Added " + parameters[2] + " speed to T Team.</color>\n";

                    foreach (GameObject player in GameClient.Instance.players.Values)
                    {
                        if (player != null)
                        {
                            if (player.GetComponent<Player>().team == 1)
                                player.GetComponent<Player>().SetSpeed(float.Parse(parameters[2], CultureInfo.InvariantCulture));
                        }
                    }
                }
                else if (parameters[1] == "$allCT")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Added " + parameters[2] + " speed to CT Team.</color>\n";

                    foreach (GameObject player in GameClient.Instance.players.Values)
                    {
                        if (player != null)
                        {
                            if (player.GetComponent<Player>().team == 0)
                                player.GetComponent<Player>().SetSpeed(float.Parse(parameters[2], CultureInfo.InvariantCulture));
                        }
                    }
                }
                else if (parameters[1] == "$thisPlayer")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Added " + parameters[2] + " speed to " + GameClient.Instance.players[GameClient.Instance.playerId] + ".</color>\n";

                    GameObject player = GameClient.Instance.players[GameClient.Instance.playerId];

                    if (player != null)
                    {
                        player.GetComponent<Player>().SetSpeed(float.Parse(parameters[2], CultureInfo.InvariantCulture));
                    }
                }
                else
                {

                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Added " + parameters[2] + " speed to " + parameters[1] + ".</color>\n";


                    foreach (GameObject player in GameClient.Instance.players.Values)
                    {
                        if (player != null)
                        {
                            if (player.GetComponent<Player>().Name.Equals(parameters[1]))
                            {
                                player.GetComponent<Player>().SetSpeed(float.Parse(parameters[2], CultureInfo.InvariantCulture));

                                break;
                            }
                        }
                    }
                }
            }
        }
    }
    else if (parameters[0] == "addHealth")
    {
        if (GameHost.Instance != null)
        {
            if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
            {
                try
                {
                    int.Parse(parameters[2], CultureInfo.InvariantCulture);
                }
                catch
                {
                    return;
                }
                if (parameters[1] == "$allPlayers")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Added " + parameters[2] + " health to everybody.</color>\n";

                    foreach (GameObject player in GameClient.Instance.players.Values)
                    {
                        if (player != null)
                        {
                            player.GetComponent<Player>().health += float.Parse(parameters[2], CultureInfo.InvariantCulture);

                            if (player.GetComponent<Player>().health > player.GetComponent<Player>().maxHealth)
                                player.GetComponent<Player>().maxHealth = player.GetComponent<Player>().health;
                            else if (player.GetComponent<Player>().health <= 0)
                            {
                                MatchData.PlayerData[player.GetComponent<Player>().ID].Deaths++;

                                Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "SetDeaths " + player.GetComponent<Player>().ID.ToString(CultureInfo.InvariantCulture) + " " + MatchData.PlayerData[player.GetComponent<Player>().ID].Deaths.ToString(CultureInfo.InvariantCulture) + "\n";

                                Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "Death " + player.GetComponent<Player>().ID.ToString(CultureInfo.InvariantCulture) + "\n";
                            }

                            if (GameClient.Instance.players[GameClient.Instance.playerId] == player)
                            {
                                player.GetComponent<Player>().healthText.GetComponent<Text>().text = player.GetComponent<Player>().health.ToString(CultureInfo.InvariantCulture);
                            }
                        }
                    }
                }
                else if (parameters[1] == "$allT")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Added " + parameters[2] + " health to tero.</color>\n";

                    foreach (GameObject player in GameClient.Instance.players.Values)
                    {
                        if (player != null)
                        {
                            if (player.GetComponent<Player>().team == 1)
                            {
                                player.GetComponent<Player>().health += float.Parse(parameters[2], CultureInfo.InvariantCulture);

                                if (player.GetComponent<Player>().health > player.GetComponent<Player>().maxHealth)
                                    player.GetComponent<Player>().maxHealth = player.GetComponent<Player>().health;
                                else if (player.GetComponent<Player>().health <= 0)
                                {
                                    MatchData.PlayerData[player.GetComponent<Player>().ID].Deaths++;

                                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "SetDeaths " + player.GetComponent<Player>().ID.ToString(CultureInfo.InvariantCulture) + " " + MatchData.PlayerData[player.GetComponent<Player>().ID].Deaths.ToString(CultureInfo.InvariantCulture) + "\n";

                                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "Death " + player.GetComponent<Player>().ID.ToString(CultureInfo.InvariantCulture) + "\n";
                                }

                                if (GameClient.Instance.players[GameClient.Instance.playerId] == player)
                                {
                                    player.GetComponent<Player>().healthText.GetComponent<Text>().text = player.GetComponent<Player>().health.ToString(CultureInfo.InvariantCulture);
                                }
                            }
                        }
                    }
                }
                else if (parameters[1] == "$allCT")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Added " + parameters[2] + " health to CTs.</color>\n";

                    foreach (GameObject player in GameClient.Instance.players.Values)
                    {
                        if (player != null)
                        {
                            if (player.GetComponent<Player>().team == 0)
                            {
                                player.GetComponent<Player>().health += float.Parse(parameters[2], CultureInfo.InvariantCulture);

                                if (player.GetComponent<Player>().health > player.GetComponent<Player>().maxHealth)
                                    player.GetComponent<Player>().maxHealth = player.GetComponent<Player>().health;
                                else if (player.GetComponent<Player>().health <= 0)
                                {
                                    MatchData.PlayerData[player.GetComponent<Player>().ID].Deaths++;

                                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "SetDeaths " + player.GetComponent<Player>().ID.ToString(CultureInfo.InvariantCulture) + " " + MatchData.PlayerData[player.GetComponent<Player>().ID].Deaths.ToString(CultureInfo.InvariantCulture) + "\n";

                                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "Death " + player.GetComponent<Player>().ID.ToString(CultureInfo.InvariantCulture) + "\n";
                                }

                                if (GameClient.Instance.players[GameClient.Instance.playerId] == player)
                                {
                                    player.GetComponent<Player>().healthText.GetComponent<Text>().text = player.GetComponent<Player>().health.ToString(CultureInfo.InvariantCulture);
                                }
                            }
                        }
                    }
                }
                else if (parameters[1] == "$thisPlayer")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Added " + GameClient.Instance.players[GameClient.Instance.playerId].GetComponent<Player>().Name + " health to everybody.</color>\n";

                    GameObject player = GameClient.Instance.players[GameClient.Instance.playerId];

                    if (player != null)
                    {
                        player.GetComponent<Player>().health += float.Parse(parameters[2], CultureInfo.InvariantCulture);

                        if (player.GetComponent<Player>().health > player.GetComponent<Player>().maxHealth)
                            player.GetComponent<Player>().maxHealth = player.GetComponent<Player>().health;
                        else if (player.GetComponent<Player>().health <= 0)
                        {
                            MatchData.PlayerData[player.GetComponent<Player>().ID].Deaths++;

                            Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "SetDeaths " + player.GetComponent<Player>().ID.ToString(CultureInfo.InvariantCulture) + " " + MatchData.PlayerData[player.GetComponent<Player>().ID].Deaths.ToString(CultureInfo.InvariantCulture) + "\n";

                            Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "Death " + player.GetComponent<Player>().ID.ToString(CultureInfo.InvariantCulture) + "\n";
                        }

                        if (GameClient.Instance.players[GameClient.Instance.playerId] == player)
                        {
                            player.GetComponent<Player>().healthText.GetComponent<Text>().text = player.GetComponent<Player>().health.ToString(CultureInfo.InvariantCulture);
                        }
                    }
                }
                else
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Set " + parameters[1] + "'s health to " + parameters[2] + ".</color>\n";


                    foreach (GameObject player in GameClient.Instance.players.Values)
                    {
                        if (player != null)
                        {
                            if (player.GetComponent<Player>().Name.Equals(parameters[1]))
                            {
                                player.GetComponent<Player>().health += float.Parse(parameters[2], CultureInfo.InvariantCulture);

                                if (player.GetComponent<Player>().health > player.GetComponent<Player>().maxHealth)
                                    player.GetComponent<Player>().maxHealth = player.GetComponent<Player>().health;
                                else if (player.GetComponent<Player>().health <= 0)
                                {
                                    MatchData.PlayerData[player.GetComponent<Player>().ID].Deaths++;

                                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "SetDeaths " + player.GetComponent<Player>().ID.ToString(CultureInfo.InvariantCulture) + " " + MatchData.PlayerData[player.GetComponent<Player>().ID].Deaths.ToString(CultureInfo.InvariantCulture) + "\n";

                                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "Death " + player.GetComponent<Player>().ID.ToString(CultureInfo.InvariantCulture) + "\n";
                                }

                                if (GameClient.Instance.players[GameClient.Instance.playerId] == player)
                                {
                                    player.GetComponent<Player>().healthText.GetComponent<Text>().text = player.GetComponent<Player>().health.ToString(CultureInfo.InvariantCulture);
                                }

                                break;
                            }
                        }
                    }
                }
            }
        }
    }
    else if (parameters[0] == "addCash")
    {
        if (GameHost.Instance != null)
        {
            if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
            {
                try
                {
                    int.Parse(parameters[2], CultureInfo.InvariantCulture);
                }
                catch
                {
                    return;
                }
                if (parameters[1] == "$allPlayers")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Added " + parameters[2] + " cash to everybody.</color>\n";

                    GameHost host = GameHost.Instance;
                    for (int i = 1; i <= MatchData.PlayerData.Count; i++)
                    {
                        if (MatchData.PlayerData[i].ClientAlive)
                        {
                            MatchData.PlayerData[i + 1].Cash += int.Parse(parameters[2], CultureInfo.InvariantCulture);
                        }
                    }

                    foreach (GameObject player in GameClient.Instance.players.Values)
                    {
                        if (player != null)
                            player.GetComponent<Player>().cash += int.Parse(parameters[2], CultureInfo.InvariantCulture);
                    }
                }
                else if (parameters[1] == "$allT")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Added " + parameters[2] + " cash to tero.</color>\n";

                    GameHost host = GameHost.Instance;
                    for (int i = 1; i <= MatchData.PlayerData.Count; i++)
                    {
                        if (MatchData.PlayerData[i].ClientAlive && MatchData.PlayerData[i].Team == 1)
                        {
                            MatchData.PlayerData[i + 1].Cash += int.Parse(parameters[2], CultureInfo.InvariantCulture);
                        }
                    }

                    foreach (GameObject player in GameClient.Instance.players.Values)
                    {
                        if (player != null)
                        {
                            if (player.GetComponent<Player>().team == 1)
                                player.GetComponent<Player>().cash += int.Parse(parameters[2], CultureInfo.InvariantCulture);
                        }
                    }
                }
                else if (parameters[1] == "$allCT")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Added " + parameters[2] + " cash to CTs.</color>\n";

                    GameHost host = GameHost.Instance;
                    for (int i = 1; i <= MatchData.PlayerData.Count; i++)
                    {
                        if (MatchData.PlayerData[i].ClientAlive && MatchData.PlayerData[i].Team == 0)
                        {
                            MatchData.PlayerData[i + 1].Cash += int.Parse(parameters[2], CultureInfo.InvariantCulture);
                        }
                    }

                    foreach (GameObject player in GameClient.Instance.players.Values)
                    {
                        if (player != null)
                        {
                            if (player.GetComponent<Player>().team == 0)
                                player.GetComponent<Player>().cash += int.Parse(parameters[2], CultureInfo.InvariantCulture);
                        }
                    }
                }
                else if (parameters[1] == "$thisPlayer")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Added " + GameClient.Instance.players[GameClient.Instance.playerId].GetComponent<Player>().Name + " cash to everybody.</color>\n";

                    GameObject player = GameClient.Instance.players[GameClient.Instance.playerId];

                    MatchData.PlayerData[GameClient.Instance.playerId].Cash += int.Parse(parameters[2], CultureInfo.InvariantCulture);

                    if (player != null)
                    {
                        player.GetComponent<Player>().cash += int.Parse(parameters[2], CultureInfo.InvariantCulture);
                    }
                }
                else
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Set " + parameters[1] + "'s cash to " + parameters[2] + ".</color>\n";


                    foreach (GameObject player in GameClient.Instance.players.Values)
                    {
                        GameHost host = GameHost.Instance;
                        for (int i = 1; i <= MatchData.PlayerData.Count; i++)
                        {
                            if (MatchData.PlayerData[i].ClientAlive && MatchData.PlayerData[i].Name == parameters[1])
                            {
                                MatchData.PlayerData[i + 1].Cash += int.Parse(parameters[2], CultureInfo.InvariantCulture);
                                break;
                            }
                        }

                        if (player != null)
                        {
                            if (player.GetComponent<Player>().Name.Equals(parameters[1]))
                            {
                                player.GetComponent<Player>().cash += int.Parse(parameters[2], CultureInfo.InvariantCulture);

                                break;
                            }
                        }
                    }
                }
            }
        }
    }
    else if (parameters[0] == "setCash")
    {
        if (GameHost.Instance != null)
        {
            if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
            {
                try
                {
                    int.Parse(parameters[2], CultureInfo.InvariantCulture);
                }
                catch
                {
                    return;
                }
                if (parameters[1] == "$allPlayers")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Set everybody's cash to " + parameters[2] + ".</color>\n";

                    GameHost host = GameHost.Instance;
                    for (int i = 1; i <= MatchData.PlayerData.Count; i++)
                    {
                        if (MatchData.PlayerData[i].ClientAlive)
                        {
                            MatchData.PlayerData[i + 1].Cash = int.Parse(parameters[2], CultureInfo.InvariantCulture);
                        }
                    }

                    foreach (GameObject player in GameClient.Instance.players.Values)
                    {
                        if (player != null)
                            player.GetComponent<Player>().cash = int.Parse(parameters[2], CultureInfo.InvariantCulture);
                    }
                }
                else if (parameters[1] == "$allT")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Set terorist team's cash to " + parameters[2] + ".</color>\n";

                    GameHost host = GameHost.Instance;
                    for (int i = 1; i <= MatchData.PlayerData.Count; i++)
                    {
                        if (MatchData.PlayerData[i].ClientAlive && MatchData.PlayerData[i].Team == 1)
                        {
                            MatchData.PlayerData[i + 1].Cash = int.Parse(parameters[2], CultureInfo.InvariantCulture);
                        }
                    }


                    foreach (GameObject player in GameClient.Instance.players.Values)
                    {
                        if (player != null)
                        {
                            if (player.GetComponent<Player>().team == 1)
                                player.GetComponent<Player>().cash = int.Parse(parameters[2], CultureInfo.InvariantCulture);
                        }
                    }
                }
                else if (parameters[1] == "$allCT")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Set CT team's cash to " + parameters[2] + ".</color>\n";

                    GameHost host = GameHost.Instance;
                    for (int i = 1; i <= MatchData.PlayerData.Count; i++)
                    {
                        if (MatchData.PlayerData[i].ClientAlive && MatchData.PlayerData[i].Team == 0)
                        {
                            MatchData.PlayerData[i + 1].Cash = int.Parse(parameters[2], CultureInfo.InvariantCulture);
                        }
                    }

                    foreach (GameObject player in GameClient.Instance.players.Values)
                    {
                        if (player != null)
                        {
                            if (player.GetComponent<Player>().team == 0)
                                player.GetComponent<Player>().cash = int.Parse(parameters[2], CultureInfo.InvariantCulture);
                        }
                    }
                }
                else if (parameters[1] == "$thisPlayer")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Set " + GameClient.Instance.players[GameClient.Instance.playerId].GetComponent<Player>().Name + "'s cash to " + parameters[2] + ".</color>\n";

                    GameObject player = GameClient.Instance.players[GameClient.Instance.playerId];

                    MatchData.PlayerData[GameClient.Instance.playerId].Cash = int.Parse(parameters[2], CultureInfo.InvariantCulture);

                    if (player != null)
                    {
                        player.GetComponent<Player>().cash = int.Parse(parameters[2], CultureInfo.InvariantCulture);
                    }
                }
                else
                {

                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Set " + parameters[1] + "'s cash to " + parameters[2] + ".</color>\n";


                    foreach (GameObject player in GameClient.Instance.players.Values)
                    {
                        GameHost host = GameHost.Instance;
                        for (int i = 1; i <= MatchData.PlayerData.Count; i++)
                        {
                            if (MatchData.PlayerData[i].ClientAlive && MatchData.PlayerData[i].Name == parameters[1])
                            {
                                MatchData.PlayerData[i + 1].Cash = int.Parse(parameters[2], CultureInfo.InvariantCulture);
                                break;
                            }
                        }

                        if (player != null)
                        {
                            if (player.GetComponent<Player>().Name.Equals(parameters[1]))
                            {
                                player.GetComponent<Player>().cash = int.Parse(parameters[2], CultureInfo.InvariantCulture);

                                break;
                            }
                        }
                    }
                }
            }
        }
    }
    else if (parameters[0] == "setSpeed")
    {
        if (GameHost.Instance != null)
        {
            if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
            {
                try
                {
                    int.Parse(parameters[2], CultureInfo.InvariantCulture);
                }
                catch
                {
                    return;
                }
                if (parameters[1] == "$allPlayers")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Set everybody's speed to " + parameters[2] + ".</color>\n";

                    foreach (GameObject player in GameClient.Instance.players.Values)
                    {
                        if (player != null)
                            player.GetComponent<Player>().SetSpeed(float.Parse(parameters[2], CultureInfo.InvariantCulture));
                    }
                }
                else if (parameters[1] == "$allT")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Set terorist team's speed to " + parameters[2] + ".</color>\n";

                    foreach (GameObject player in GameClient.Instance.players.Values)
                    {
                        if (player != null)
                        {
                            if (player.GetComponent<Player>().team == 1)
                                player.GetComponent<Player>().SetSpeed(float.Parse(parameters[2], CultureInfo.InvariantCulture));
                        }
                    }
                }
                else if (parameters[1] == "$allCT")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Set CT team's speed to " + parameters[2] + ".</color>\n";

                    foreach (GameObject player in GameClient.Instance.players.Values)
                    {
                        if (player != null)
                        {
                            if (player.GetComponent<Player>().team == 0)
                                player.GetComponent<Player>().SetSpeed(float.Parse(parameters[2], CultureInfo.InvariantCulture));
                        }
                    }
                }
                else if (parameters[1] == "$thisPlayer")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Set " + GameClient.Instance.players[GameClient.Instance.playerId].GetComponent<Player>().Name + "'s speed to " + parameters[2] + ".</color>\n";

                    GameObject player = GameClient.Instance.players[GameClient.Instance.playerId];

                    if (player != null)
                    {
                        player.GetComponent<Player>().SetSpeed(float.Parse(parameters[2], CultureInfo.InvariantCulture));
                    }
                }
                else
                {

                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Set " + parameters[1] + "'s speed to " + parameters[2] + ".</color>\n";


                    foreach (GameObject player in GameClient.Instance.players.Values)
                    {
                        if (player != null)
                        {
                            if (player.GetComponent<Player>().Name.Equals(parameters[1]))
                            {
                                player.GetComponent<Player>().SetSpeed(float.Parse(parameters[2], CultureInfo.InvariantCulture));

                                break;
                            }
                        }
                    }
                }
            }
        }
    }
    else if (parameters[0] == "setHealth")
    {
        if (GameHost.Instance != null)
        {
            if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
            {
                try
                {
                    int.Parse(parameters[2], CultureInfo.InvariantCulture);
                }
                catch
                {
                    return;
                }
                if (parameters[1] == "$allPlayers")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Set everybody's health to " + parameters[2] + ".</color>\n";

                    foreach (GameObject player in GameClient.Instance.players.Values)
                    {
                        if (player != null)
                        {
                            player.GetComponent<Player>().health = float.Parse(parameters[2], CultureInfo.InvariantCulture);

                            if (player.GetComponent<Player>().health > player.GetComponent<Player>().maxHealth)
                                player.GetComponent<Player>().maxHealth = player.GetComponent<Player>().health;

                            if (GameClient.Instance.players[GameClient.Instance.playerId] == player)
                            {
                                player.GetComponent<Player>().healthText.GetComponent<Text>().text = parameters[2];
                            }
                        }
                    }
                }
                else if (parameters[1] == "$allT")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Set terorist team's health to " + parameters[2] + ".</color>\n";

                    foreach (GameObject player in GameClient.Instance.players.Values)
                    {
                        if (player != null)
                        {
                            if (player.GetComponent<Player>().team == 1)
                            {
                                player.GetComponent<Player>().health = float.Parse(parameters[2], CultureInfo.InvariantCulture);

                                if (player.GetComponent<Player>().health > player.GetComponent<Player>().maxHealth)
                                    player.GetComponent<Player>().maxHealth = player.GetComponent<Player>().health;

                                if (GameClient.Instance.players[GameClient.Instance.playerId] == player)
                                {
                                    player.GetComponent<Player>().healthText.GetComponent<Text>().text = parameters[2];
                                }
                            }
                        }
                    }
                }
                else if (parameters[1] == "$allCT")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Set CT team's health to " + parameters[2] + ".</color>\n";

                    foreach (GameObject player in GameClient.Instance.players.Values)
                    {
                        if (player != null)
                        {
                            if (player.GetComponent<Player>().team == 0)
                            {
                                player.GetComponent<Player>().health = float.Parse(parameters[2], CultureInfo.InvariantCulture);

                                if (player.GetComponent<Player>().health > player.GetComponent<Player>().maxHealth)
                                    player.GetComponent<Player>().maxHealth = player.GetComponent<Player>().health;

                                if (GameClient.Instance.players[GameClient.Instance.playerId] == player)
                                {
                                    player.GetComponent<Player>().healthText.GetComponent<Text>().text = parameters[2];
                                }
                            }
                        }
                    }
                }
                else if (parameters[1] == "$thisPlayer")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Set " + GameClient.Instance.players[GameClient.Instance.playerId].GetComponent<Player>().Name + "'s health to " + parameters[2] + ".</color>\n";

                    GameObject player = GameClient.Instance.players[GameClient.Instance.playerId];

                    if (player != null)
                    {
                        player.GetComponent<Player>().health = float.Parse(parameters[2], CultureInfo.InvariantCulture);

                        if (player.GetComponent<Player>().health > player.GetComponent<Player>().maxHealth)
                            player.GetComponent<Player>().maxHealth = player.GetComponent<Player>().health;

                        if (GameClient.Instance.players[GameClient.Instance.playerId] == player)
                        {
                            player.GetComponent<Player>().healthText.GetComponent<Text>().text = parameters[2];
                        }
                    }
                }
                else
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Set " + parameters[1] + "'s health to " + parameters[2] + ".</color>\n";


                    foreach (GameObject player in GameClient.Instance.players.Values)
                    {
                        if (player != null)
                        {
                            if (player.GetComponent<Player>().Name.Equals(parameters[1]))
                            {
                                player.GetComponent<Player>().health = float.Parse(parameters[2], CultureInfo.InvariantCulture);

                                if (player.GetComponent<Player>().health > player.GetComponent<Player>().maxHealth)
                                    player.GetComponent<Player>().maxHealth = player.GetComponent<Player>().health;

                                if (GameClient.Instance.players[GameClient.Instance.playerId] == player)
                                {
                                    player.GetComponent<Player>().healthText.GetComponent<Text>().text = parameters[2];
                                }

                                break;
                            }
                        }
                    }
                }
            }
        }
    }
    else if (parameters[0] == "setVisibility")
    {
        if (GameHost.Instance != null)
        {
            if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
            {
                try
                {
                    bool.Parse(parameters[2]);
                }
                catch
                {
                    return;
                }
                if (parameters[1] == "$allPlayers")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Set everybody's visibility to " + parameters[2] + ".</color>\n";

                    foreach (GameObject player in GameClient.Instance.players.Values)
                    {
                        if (player != null)
                        {
                            Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "Visibility " + player.GetComponent<Player>().ID.ToString(CultureInfo.InvariantCulture) + " " + parameters[2] + "\n";
                            foreach (Transform child in player.GetComponent<Player>().limbsObj.transform)
                            {
                                if (child.name.Contains("Gun"))
                                {
                                    foreach (Transform child2 in child)
                                    {
                                        if (player.GetComponent<Player>().team == GameClient.Instance.team)
                                            child2.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 128);
                                        else child2.GetComponent<SpriteRenderer>().enabled = bool.Parse(parameters[2]);
                                    }
                                }
                                else
                                {
                                    if (player.GetComponent<Player>().team == GameClient.Instance.team)
                                        child.gameObject.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 128);
                                    else child.gameObject.SetActive(bool.Parse(parameters[2]));
                                }
                            }
                        }
                    }
                }
                else if (parameters[1] == "$allT")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Set terorist team's visibility to " + parameters[2] + ".</color>\n";

                    foreach (GameObject player in GameClient.Instance.players.Values)
                    {
                        if (player != null)
                        {
                            if (player.GetComponent<Player>().team == 1)
                            {
                                Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "Visibility " + player.GetComponent<Player>().ID.ToString(CultureInfo.InvariantCulture) + " " + parameters[2] + "\n";
                                foreach (Transform child in player.GetComponent<Player>().limbsObj.transform)
                                {
                                    if (child.name.Contains("Gun"))
                                    {
                                        foreach (Transform child2 in child)
                                        {
                                            if (player.GetComponent<Player>().team == GameClient.Instance.team)
                                                child2.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 128);
                                            else child2.GetComponent<SpriteRenderer>().enabled = bool.Parse(parameters[2]);
                                        }
                                    }
                                    else
                                    {
                                        if (player.GetComponent<Player>().team == GameClient.Instance.team)
                                            child.gameObject.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 128);
                                        else child.gameObject.SetActive(bool.Parse(parameters[2]));
                                    }
                                }
                            }
                        }
                    }
                }
                else if (parameters[1] == "$allCT")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Set CT team's visibility to " + parameters[2] + ".</color>\n";

                    foreach (GameObject player in GameClient.Instance.players.Values)
                    {
                        if (player != null)
                        {
                            if (player.GetComponent<Player>().team == 0)
                            {
                                Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "Visibility " + player.GetComponent<Player>().ID.ToString(CultureInfo.InvariantCulture) + " " + parameters[2] + "\n";
                                foreach (Transform child in player.GetComponent<Player>().limbsObj.transform)
                                {
                                    if (child.name.Contains("Gun"))
                                    {
                                        foreach (Transform child2 in child)
                                        {
                                            if (player.GetComponent<Player>().team == GameClient.Instance.team)
                                                child2.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 128);
                                            else child2.GetComponent<SpriteRenderer>().enabled = bool.Parse(parameters[2]);
                                        }
                                    }
                                    else
                                    {
                                        if (player.GetComponent<Player>().team == GameClient.Instance.team)
                                            child.gameObject.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 128);
                                        else child.gameObject.SetActive(bool.Parse(parameters[2]));
                                    }
                                }
                            }
                        }
                    }
                }
                else if (parameters[1] == "$thisPlayer")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Set " + GameClient.Instance.players[GameClient.Instance.playerId].GetComponent<Player>().Name + "'s visibility to " + parameters[2] + ".</color>\n";

                    GameObject player = GameClient.Instance.players[GameClient.Instance.playerId];

                    if (player != null)
                    {
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "Visibility " + player.GetComponent<Player>().ID.ToString(CultureInfo.InvariantCulture) + " " + parameters[2] + "\n";
                        foreach (Transform child in player.GetComponent<Player>().limbsObj.transform)
                        {
                            if (child.name.Contains("Gun"))
                            {
                                foreach (Transform child2 in child)
                                {
                                    if (player.GetComponent<Player>().team == GameClient.Instance.team)
                                        child2.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 128);
                                    else child2.GetComponent<SpriteRenderer>().enabled = bool.Parse(parameters[2]);
                                }
                            }
                            else
                            {
                                if (player.GetComponent<Player>().team == GameClient.Instance.team)
                                    child.gameObject.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 128);
                                else child.gameObject.SetActive(bool.Parse(parameters[2]));
                            }
                        }
                    }
                }
                else
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Set " + parameters[1] + "'s visibility to " + parameters[2] + ".</color>\n";


                    foreach (GameObject player in GameClient.Instance.players.Values)
                    {
                        if (player != null)
                        {
                            if (player.GetComponent<Player>().Name.Equals(parameters[1]))
                            {
                                Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "Visibility " + player.GetComponent<Player>().ID.ToString(CultureInfo.InvariantCulture) + " " + parameters[2] + "\n";
                                foreach (Transform child in player.GetComponent<Player>().limbsObj.transform)
                                {
                                    if (child.name.Contains("Gun"))
                                    {
                                        foreach (Transform child2 in child)
                                        {
                                            if (player.GetComponent<Player>().team == GameClient.Instance.team)
                                                child2.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 128);
                                            else child2.GetComponent<SpriteRenderer>().enabled = bool.Parse(parameters[2]);
                                        }
                                    }
                                    else
                                    {
                                        if (player.GetComponent<Player>().team == GameClient.Instance.team)
                                            child.gameObject.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 128);
                                        else child.gameObject.SetActive(bool.Parse(parameters[2]));
                                    }
                                }

                                break;
                            }
                        }
                    }
                }
            }
        }
    }
    else if (parameters[0] == "setRecoil")
    {
        if (GameHost.Instance != null)
        {
            if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
            {
                try
                {
                    bool.Parse(parameters[2]);
                }
                catch
                {
                    return;
                }
                if (parameters[1] == "$allPlayers")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Set everybody's recoil to " + parameters[2] + ".</color>\n";

                    foreach (GameObject player in GameClient.Instance.players.Values)
                    {
                        if (player != null)
                        {
                            player.GetComponent<Player>().recoil = bool.Parse(parameters[2]);
                        }
                    }
                }
                else if (parameters[1] == "$allT")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Set terorist team's recoil to " + parameters[2] + ".</color>\n";

                    foreach (GameObject player in GameClient.Instance.players.Values)
                    {
                        if (player != null)
                        {
                            if (player.GetComponent<Player>().team == 1)
                                player.GetComponent<Player>().recoil = bool.Parse(parameters[2]);
                        }
                    }
                }
                else if (parameters[1] == "$allCT")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Set CT team's recoil to " + parameters[2] + ".</color>\n";

                    foreach (GameObject player in GameClient.Instance.players.Values)
                    {
                        if (player != null)
                        {
                            if (player.GetComponent<Player>().team == 0)
                                player.GetComponent<Player>().recoil = bool.Parse(parameters[2]);
                        }
                    }
                }
                else if (parameters[1] == "$thisPlayer")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Set " + GameClient.Instance.players[GameClient.Instance.playerId].GetComponent<Player>().Name + "'s recoil to " + parameters[2] + ".</color>\n";

                    GameObject player = GameClient.Instance.players[GameClient.Instance.playerId];

                    if (player != null)
                    {
                        player.GetComponent<Player>().recoil = bool.Parse(parameters[2]);
                    }
                }
                else
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Set " + parameters[1] + "'s recoil to " + parameters[2] + ".</color>\n";


                    foreach (GameObject player in GameClient.Instance.players.Values)
                    {
                        if (player != null)
                        {
                            if (player.GetComponent<Player>().Name.Equals(parameters[1]))
                            {
                                player.GetComponent<Player>().recoil = bool.Parse(parameters[2]);

                                break;
                            }
                        }
                    }
                }
            }
        }
    }
    else if (parameters[0] == "kill")
    {
        if (GameHost.Instance != null)
        {
            if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
            {
                if (parameters[1] == "$allPlayers")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Killed all players.</color>\n";

                    foreach (GameObject player in GameClient.Instance.players.Values)
                    {
                        if (player != null)
                        {
                            MatchData.PlayerData[player.GetComponent<Player>().ID].Deaths++;

                            Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "SetDeaths " + player.GetComponent<Player>().ID.ToString(CultureInfo.InvariantCulture) + " " + MatchData.PlayerData[player.GetComponent<Player>().ID].Deaths.ToString(CultureInfo.InvariantCulture) + "\n";

                            Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "Death " + player.GetComponent<Player>().ID.ToString(CultureInfo.InvariantCulture) + "\n";
                        }
                    }
                }
                else if (parameters[1] == "$allT")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Killed all terrorists.</color>\n";

                    foreach (GameObject player in GameClient.Instance.players.Values)
                    {
                        if (player != null)
                        {
                            if (player.GetComponent<Player>().team == 1)
                            {
                                MatchData.PlayerData[player.GetComponent<Player>().ID].Deaths++;

                                Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "SetDeaths " + player.GetComponent<Player>().ID.ToString(CultureInfo.InvariantCulture) + " " + MatchData.PlayerData[player.GetComponent<Player>().ID].Deaths.ToString(CultureInfo.InvariantCulture) + "\n";

                                Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "Death " + player.GetComponent<Player>().ID.ToString(CultureInfo.InvariantCulture) + "\n";
                            }
                        }
                    }
                }
                else if (parameters[1] == "$allCT")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Killed all CTs.</color>\n";

                    foreach (GameObject player in GameClient.Instance.players.Values)
                    {
                        if (player != null)
                        {
                            if (player.GetComponent<Player>().team == 0)
                            {
                                MatchData.PlayerData[player.GetComponent<Player>().ID].Deaths++;

                                Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "SetDeaths " + player.GetComponent<Player>().ID.ToString(CultureInfo.InvariantCulture) + " " + MatchData.PlayerData[player.GetComponent<Player>().ID].Deaths.ToString(CultureInfo.InvariantCulture) + "\n";

                                Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "Death " + player.GetComponent<Player>().ID.ToString(CultureInfo.InvariantCulture) + "\n";
                            }
                        }
                    }
                }
                else if (parameters[1] == "$thisPlayer")
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Killed " + GameClient.Instance.players[GameClient.Instance.playerId].GetComponent<Player>().Name + ".</color>\n";

                    GameObject player = GameClient.Instance.players[GameClient.Instance.playerId];

                    if (player != null)
                    {
                        MatchData.PlayerData[player.GetComponent<Player>().ID].Deaths++;

                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "SetDeaths " + player.GetComponent<Player>().ID.ToString(CultureInfo.InvariantCulture) + " " + MatchData.PlayerData[player.GetComponent<Player>().ID].Deaths.ToString(CultureInfo.InvariantCulture) + "\n";

                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "Death " + player.GetComponent<Player>().ID.ToString(CultureInfo.InvariantCulture) + "\n";

                    }
                }
                else
                {
                    if (sendCommandFeedback)
                        Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Killed " + parameters[1] + ".</color>\n";


                    foreach (GameObject player in GameClient.Instance.players.Values)
                    {
                        if (player != null)
                        {
                            if (player.GetComponent<Player>().Name.Equals(parameters[1]))
                            {
                                MatchData.PlayerData[player.GetComponent<Player>().ID].Deaths++;

                                Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "SetDeaths " + player.GetComponent<Player>().ID.ToString(CultureInfo.InvariantCulture) + " " + MatchData.PlayerData[player.GetComponent<Player>().ID].Deaths.ToString(CultureInfo.InvariantCulture) + "\n";

                                Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "Death " + player.GetComponent<Player>().ID.ToString(CultureInfo.InvariantCulture) + "\n";

                                break;
                            }
                        }
                    }
                }
            }
        }
    }
    else if (parameters[0] == "buyTime")
    {
        if (GameHost.Instance != null)
        {
            if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
            {
                try
                {
                    int.Parse(parameters[1], CultureInfo.InvariantCulture);
                }
                catch
                {
                    return;
                }
                if (sendCommandFeedback)
                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Set buy time to " + parameters[1] + ".</color>\n";

                Config.BuyTime = int.Parse(parameters[1], CultureInfo.InvariantCulture);
            }
        }
    }
    else if (parameters[0] == "freezeTime")
    {
        if (GameHost.Instance != null)
        {
            if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
            {
                try
                {
                    int.Parse(parameters[1], CultureInfo.InvariantCulture);
                }
                catch
                {
                    return;
                }
                if (sendCommandFeedback)
                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Set freeze time to " + parameters[1] + ".</color>\n";

                Config.FreezeTime = int.Parse(parameters[1], CultureInfo.InvariantCulture);
            }
        }
    }
    else if (parameters[0] == "bombTime")
    {
        if (GameHost.Instance != null)
        {
            if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
            {
                try
                {
                    int.Parse(parameters[1], CultureInfo.InvariantCulture);
                }
                catch
                {
                    return;
                }
                if (sendCommandFeedback)
                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Set bomb time to " + parameters[1] + ".</color>\n";

                Config.BombTime = int.Parse(parameters[1], CultureInfo.InvariantCulture);
            }
        }
    }
    else if (parameters[0] == "roundTime")
    {
        if (GameHost.Instance != null)
        {
            if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
            {
                try
                {
                    int.Parse(parameters[1], CultureInfo.InvariantCulture);
                }
                catch
                {
                    return;
                }
                if (sendCommandFeedback)
                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Set round time to " + parameters[1] + ".</color>\n";

                Config.RoundTime = int.Parse(parameters[1], CultureInfo.InvariantCulture);
            }
        }
    }
    else if (parameters[0] == "warmupTime")
    {
        if (GameHost.Instance != null)
        {
            if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
            {
                try
                {
                    int.Parse(parameters[1], CultureInfo.InvariantCulture);
                }
                catch
                {
                    return;
                }
                if (sendCommandFeedback)
                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Changed warmup time to " + parameters[1] + ".</color>\n";

                GameHost.Instance.warmupTimeLeft = int.Parse(parameters[1], CultureInfo.InvariantCulture);
            }
        }
    }
    else if (parameters[0] == "roundsPerHalf")
    {
        if (GameHost.Instance != null)
        {
            if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
            {
                try
                {
                    int.Parse(parameters[1], CultureInfo.InvariantCulture);
                }
                catch
                {
                    return;
                }
                if (sendCommandFeedback)
                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Set rounds per half to " + parameters[1] + ".</color>\n";

                Config.RoundsPerHalf = int.Parse(parameters[1], CultureInfo.InvariantCulture);
            }
        }
    }
    else if (parameters[0] == "roundsToWin")
    {
        if (GameHost.Instance != null)
        {
            if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
            {
                try
                {
                    int.Parse(parameters[1], CultureInfo.InvariantCulture);
                }
                catch
                {
                    return;
                }
                if (sendCommandFeedback)
                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Set rounds to win to " + parameters[1] + ".</color>\n";

                Config.RoundsToWin = int.Parse(parameters[1], CultureInfo.InvariantCulture);
            }
        }
    }
    else if (parameters[0] == "swapTeams")
    {
        if (GameHost.Instance != null)
        {
            if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
            {
                try
                {
                    int.Parse(parameters[1], CultureInfo.InvariantCulture);
                }
                catch
                {
                    return;
                }
                if (sendCommandFeedback)
                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Swapping teams in " + parameters[1] + " seconds.</color>\n";

                Invoke("Swap", int.Parse(parameters[1], CultureInfo.InvariantCulture));
            }
        }
    }
    else if (parameters[0] == "warmupEnd")
    {
        if (GameHost.Instance != null)
        {
            if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
            {
                try
                {
                    int.Parse(parameters[1], CultureInfo.InvariantCulture);
                }
                catch
                {
                    return;
                }
                if (sendCommandFeedback)
                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Ending warmup in " + parameters[1] + " seconds.</color>\n";

                Invoke("WarmupEnd", int.Parse(parameters[1], CultureInfo.InvariantCulture));
            }
        }
    }
    else if (parameters[0] == "maxCash")
    {
        if (GameHost.Instance != null)
        {
            if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
            {
                try
                {
                    int.Parse(parameters[1], CultureInfo.InvariantCulture);
                }
                catch
                {
                    return;
                }
                if (sendCommandFeedback)
                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Set max cash to " + parameters[1] + ".</color>\n";

                Config.MaxCash = int.Parse(parameters[1], CultureInfo.InvariantCulture);
            }
        }
    }
    else if (parameters[0] == "loseCash")
    {
        if (GameHost.Instance != null)
        {
            if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
            {
                try
                {
                    int.Parse(parameters[1], CultureInfo.InvariantCulture);
                }
                catch
                {
                    return;
                }
                if (sendCommandFeedback)
                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Set lose cash to " + parameters[1] + ".</color>\n";

                Config.LoseCash = int.Parse(parameters[1], CultureInfo.InvariantCulture);
            }
        }
    }
    else if (parameters[0] == "loseStreakCash")
    {
        if (GameHost.Instance != null)
        {
            if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
            {
                try
                {
                    int.Parse(parameters[1], CultureInfo.InvariantCulture);
                }
                catch
                {
                    return;
                }
                if (sendCommandFeedback)
                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Set lose streak cash to " + parameters[1] + ".</color>\n";

                Config.LoseStreakCash = int.Parse(parameters[1], CultureInfo.InvariantCulture);
            }
        }
    }
    else if (parameters[0] == "winCash")
    {
        if (GameHost.Instance != null)
        {
            if (SceneManager.GetActiveScene().name.Contains(Config.FightSceneName))
            {
                try
                {
                    int.Parse(parameters[1], CultureInfo.InvariantCulture);
                }
                catch
                {
                    return;
                }
                if (sendCommandFeedback)
                    Network.Instance.Send(new RoundStart(roundStart, new SystemMessage("<color=yellow>Added " + parameters[2] + " speed to everybody.</color>")); += "System 1 0 <color=yellow>Set win cash to " + parameters[1] + ".</color>\n";

                Config.WinCash = int.Parse(parameters[1], CultureInfo.InvariantCulture);
            }
        }
    }
    else if (parameters[0] == "help")
    {
        chatBox.GetComponent<Text>().text += @"
*CLIENT COMMANDS*

name <desired_name> - Changes local player name.
writeChat <all/team> <text> - Writes text.
clearChat - Clears Chat.
clearConsole - Clears Console.
writeConsole <text> - Writes text in console.
leaveMatch - Leaves Match.
quit - Closes game.
exec <fileName> <parameter1> <parameter2> <parameter3> ... - Load a config file from the /Config folder

*SERVER COMMANDS*

reset <time> - Restarts the game in <time> seconds.
setCash <playerName> <value> - Sets the cash of target player. 
setSpeed <playerName> <speed> - Sets the speed of target player.
setHealth <playerName> <health> - Sets the health of target player.
addCash <playerName> <value> - Adds cash of target player. 
addSpeed <playerName> <speed> - Adds speed to target player.
addHealth <playerName> <health> - Adds health to target player.
setVisibility <playerName> <true/false> - Sets the visibility of target player.
setRecoil <playerName> <true/false> - Sets the recoil of target player.
kill <playerName> - Kills player.
roundTime <time> - Changes the round time in seconds. Does not apply in the current round.
buyTime <time> - Changes the time allowed to buy. Does not apply in the current round.
freezeTime <time> - Changes the time you are frozen in place. Does not apply in the current round.
bombTime <time> - Changes the time it takes a bomb to explode. Does not apply in the current round.
warmupTime <time> - Changes the warmup time. This applies immediatly to the current round.
roundsPerHalf <value> - Changes how many rounds a half has.
roundsToWin <value> - Changes how many rounds it takes to win.
swapTeams <time> - Swaps the teams and restarts the round in <time> seconds.
warmupEnd <time> - End warmup in <time> seconds.
maxCash <value> - Changes the maximum amount of cash a player can hold.
loseCash <value> - Changes how much cash you gain from losing.
winCash <value> - Changes how much cash you gain from losing.
loseStreakCash <value> - Changes how much cash you gain from losing consecutively.
sendCommandFeedback <true/false> - Changes if a command should send feedback.
friendlyFire <true/false> - Changes whether or not teammates can shoot each other.
showKillFeed <true/false> - Changes whether or not kill feed is shown.
serverWhisper <playerName> <text> - Whispers to a player without anybody receiving feedback, not even the host.

";
    }
    else if (parameters[0] == "sendCommandFeedback")
    {
        bool value = false;
        if (parameters[1] == "false")
            value = true;
        if (parameters[1] == "true")
            value = false;
        if (value)
        {
            sendCommandFeedback = false;
        }
    }
    else if (parameters[0] == "writeChat")
    {
        int all = -1;
        if (parameters[1] == "all")
            all = 1;
        else if (parameters[1] == "team")
            all = 0;
        if (all > -1)
            Chat.Instance.SendMessage(string.Join(" ", parameters.Skip(2)), all);
    }
    else if (parameters[0] == "clearChat")
    {
        Chat.Instance.Clear();
    }
    else if (parameters[0] == "clearConsole")
    {
        chatBox.GetComponent<Text>().text = "";
    }
    else if (parameters[0] == "writeConsole")
    {
        chatBox.GetComponent<Text>().text += string.Join(" ", parameters.Skip(1)) + "\n";
    }
    else if (parameters[0] == "leaveMatch")
    {
        Escape.Instance.Terminate();
    }
    else if (parameters[0] == "quit")
    {
        Application.Quit();
    }
    else if (parameters[0] == "exec")
    {
        string fileName = parameters[1];
        string[] fileParameters = parameters.Skip(2).ToArray();

        if (File.Exists(Application.dataPath + "\\Config\\" + fileName))
        {
            string data = File.ReadAllText(Application.dataPath + "\\Config\\" + fileName);

            for (int i = 1; i <= fileParameters.Length; i++)
            {
                data = data.Replace("<parameter" + i.ToString(CultureInfo.InvariantCulture) + ">", fileParameters[i - 1]);
            }

            if (data.Contains("MATCH_STARTUP_HOST"))
            {

                List<string> scriptEnd = new List<string>
                                        {
                                            "MATCH_STARTUP_CLIENT", "MATCH_ROUND_END", "MATCH_ROUND_START", "MATCH_BOMB_PLANTED", "MATCH_BOMB_DEFUSED", "MATCH_CT_DEAD", "MATCH_T_DEAD", "ON_EXECUTION"
                                        };

                string executableScript = "";

                bool canAdd = false;

                foreach (string line in data.Split('\n'))
                {

                    if (scriptEnd.FirstOrDefault(stringToCheck => line.Contains(stringToCheck)) == null && canAdd)
                    {

                        executableScript += line + "\n";
                    }
                    else if (scriptEnd.FirstOrDefault(stringToCheck => line.Contains(stringToCheck)) != null && canAdd) break;
                    if (line.Contains("MATCH_STARTUP_HOST"))
                        canAdd = true;
                }

                matchStartHost = executableScript;
            }
            if (data.Contains("MATCH_STARTUP_CLIENT"))
            {

                List<string> scriptEnd = new List<string>
                                        {
                                            "MATCH_STARTUP_HOST", "MATCH_ROUND_END", "MATCH_ROUND_START", "MATCH_BOMB_PLANTED", "MATCH_BOMB_DEFUSED", "MATCH_CT_DEAD", "MATCH_T_DEAD", "ON_EXECUTION"
                                        };

                string executableScript = "";

                bool canAdd = false;

                foreach (string line in data.Split('\n'))
                {
                    if (scriptEnd.FirstOrDefault(stringToCheck => line.Contains(stringToCheck)) == null && canAdd)
                    {

                        executableScript += line + "\n";
                    }
                    else if (scriptEnd.FirstOrDefault(stringToCheck => line.Contains(stringToCheck)) != null && canAdd) break;
                    if (line.Contains("MATCH_STARTUP_CLIENT"))
                        canAdd = true;
                }

                matchStartClient = executableScript;
            }
            if (data.Contains("MATCH_ROUND_END"))
            {

                List<string> scriptEnd = new List<string>
                                        {
                                            "MATCH_STARTUP_HOST", "MATCH_STARTUP_CLIENT", "MATCH_ROUND_START", "MATCH_BOMB_PLANTED", "MATCH_BOMB_DEFUSED", "MATCH_CT_DEAD", "MATCH_T_DEAD", "ON_EXECUTION"
                                        };

                string executableScript = "";

                bool canAdd = false;

                foreach (string line in data.Split('\n'))
                {
                    if (scriptEnd.FirstOrDefault(stringToCheck => line.Contains(stringToCheck)) == null && canAdd)
                    {

                        executableScript += line + "\n";
                    }
                    else if (scriptEnd.FirstOrDefault(stringToCheck => line.Contains(stringToCheck)) != null && canAdd) break;
                    if (line.Contains("MATCH_ROUND_END"))
                        canAdd = true;
                }

                matchRoundEnd = executableScript;
            }
            if (data.Contains("MATCH_ROUND_START"))
            {

                List<string> scriptEnd = new List<string>
                                        {
                                            "MATCH_STARTUP_HOST", "MATCH_ROUND_END", "MATCH_STARTUP_CLIENT", "MATCH_BOMB_PLANTED", "MATCH_BOMB_DEFUSED", "MATCH_CT_DEAD", "MATCH_T_DEAD", "ON_EXECUTION"
                                        };

                string executableScript = "";

                bool canAdd = false;

                foreach (string line in data.Split('\n'))
                {
                    if (scriptEnd.FirstOrDefault(stringToCheck => line.Contains(stringToCheck)) == null && canAdd)
                    {

                        executableScript += line + "\n";
                    }
                    else if (scriptEnd.FirstOrDefault(stringToCheck => line.Contains(stringToCheck)) != null && canAdd) break;
                    if (line.Contains("MATCH_ROUND_START"))
                        canAdd = true;
                }

                matchRoundStart = executableScript;
            }
            if (data.Contains("MATCH_BOMB_PLANTED"))
            {

                List<string> scriptEnd = new List<string>
                                        {
                                            "MATCH_STARTUP_HOST", "MATCH_ROUND_END", "MATCH_ROUND_START", "MATCH_STARTUP_CLIENT", "MATCH_BOMB_DEFUSED", "MATCH_CT_DEAD", "MATCH_T_DEAD", "ON_EXECUTION"
                                        };

                string executableScript = "";

                bool canAdd = false;

                foreach (string line in data.Split('\n'))
                {
                    if (scriptEnd.FirstOrDefault(stringToCheck => line.Contains(stringToCheck)) == null && canAdd)
                    {

                        executableScript += line + "\n";
                    }
                    else if (scriptEnd.FirstOrDefault(stringToCheck => line.Contains(stringToCheck)) != null && canAdd) break;
                    if (line.Contains("MATCH_BOMB_PLANTED"))
                        canAdd = true;
                }

                matchBombPlanted = executableScript;
            }
            if (data.Contains("MATCH_BOMB_DEFUSED"))
            {

                List<string> scriptEnd = new List<string>
                                        {
                                            "MATCH_STARTUP_HOST", "MATCH_ROUND_END", "MATCH_ROUND_START", "MATCH_BOMB_PLANTED", "MATCH_STARTUP_CLIENT", "MATCH_CT_DEAD", "MATCH_T_DEAD", "ON_EXECUTION"
                                        };

                string executableScript = "";

                bool canAdd = false;

                foreach (string line in data.Split('\n'))
                {
                    if (scriptEnd.FirstOrDefault(stringToCheck => line.Contains(stringToCheck)) == null && canAdd)
                    {

                        executableScript += line + "\n";
                    }
                    else if (scriptEnd.FirstOrDefault(stringToCheck => line.Contains(stringToCheck)) != null && canAdd) break;
                    if (line.Contains("MATCH_BOMB_DEFUSED"))
                        canAdd = true;
                }

                matchBombDefused = executableScript;
            }
            if (data.Contains("MATCH_CT_DEAD"))
            {

                List<string> scriptEnd = new List<string>
                                        {
                                            "MATCH_STARTUP_HOST", "MATCH_ROUND_END", "MATCH_ROUND_START", "MATCH_BOMB_PLANTED", "MATCH_STARTUP_CLIENT", "MATCH_BOMB_DEFUSED", "MATCH_T_DEAD", "ON_EXECUTION"
                                        };

                string executableScript = "";

                bool canAdd = false;

                foreach (string line in data.Split('\n'))
                {
                    if (scriptEnd.FirstOrDefault(stringToCheck => line.Contains(stringToCheck)) == null && canAdd)
                    {

                        executableScript += line + "\n";
                    }
                    else if (scriptEnd.FirstOrDefault(stringToCheck => line.Contains(stringToCheck)) != null && canAdd) break;
                    if (line.Contains("MATCH_CT_DEAD"))
                        canAdd = true;
                }

                matchCtDead = executableScript;
            }
            if (data.Contains("MATCH_T_DEAD"))
            {

                List<string> scriptEnd = new List<string>
                                        {
                                            "MATCH_STARTUP_HOST", "MATCH_ROUND_END", "MATCH_ROUND_START", "MATCH_BOMB_PLANTED", "MATCH_STARTUP_CLIENT", "MATCH_CT_DEAD", "MATCH_BOMB_DEFUSED", "ON_EXECUTION"
                                        };

                string executableScript = "";

                bool canAdd = false;

                foreach (string line in data.Split('\n'))
                {
                    if (scriptEnd.FirstOrDefault(stringToCheck => line.Contains(stringToCheck)) == null && canAdd)
                    {

                        executableScript += line + "\n";
                    }
                    else if (scriptEnd.FirstOrDefault(stringToCheck => line.Contains(stringToCheck)) != null && canAdd) break;
                    if (line.Contains("MATCH_T_DEAD"))
                        canAdd = true;
                }

                matchTDead = executableScript;
            }
            if (data.Contains("ON_EXECUTION"))
            {

                List<string> scriptEnd = new List<string>
                                        {
                                            "MATCH_STARTUP_HOST", "MATCH_ROUND_END", "MATCH_ROUND_START", "MATCH_BOMB_PLANTED", "MATCH_STARTUP_CLIENT", "MATCH_CT_DEAD", "MATCH_BOMB_DEFUSED"
                                        };

                string executableScript = "";

                bool canAdd = false;

                foreach (string line in data.Split('\n'))
                {
                    if (scriptEnd.FirstOrDefault(stringToCheck => line.Contains(stringToCheck)) == null && canAdd)
                    {

                        executableScript += line + "\n";
                    }
                    else if (scriptEnd.FirstOrDefault(stringToCheck => line.Contains(stringToCheck)) != null && canAdd) break;
                    if (line.Contains("ON_EXECUTION"))
                        canAdd = true;
                }

                InterpretText(executableScript, false);
            }
            chatBox.GetComponent<Text>().text += "Script read succesfully.\n";
        }
        else chatBox.GetComponent<Text>().text += "Error: Script at specified path does not exist.";
    }
}
catch
{

}*/
#endregion