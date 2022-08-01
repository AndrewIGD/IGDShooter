using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Globalization;

public class Tab : MonoBehaviour
{
    #region Public Variables

    public static Tab Instance;

    public GameObject content;
    public GameObject player;

    public GameObject redPlayerList;

    public List<PlayerScore> redPlayers;
    public List<PlayerScore> bluePlayers;

    public GameObject bluePlayerList;

    #endregion

    public void RemovePlayer(string name)
    {
        PlayerScore targetBluePlayer = null;

        foreach (PlayerScore player in bluePlayers)
        {
            if (player.name == name)
            {
                targetBluePlayer = player;
                break;
            }
        }

        bluePlayers.Remove(targetBluePlayer);

        if (targetBluePlayer != null)
        {
            Destroy(targetBluePlayer.playerObject);

            return;
        }

        PlayerScore targetRedPlayer = null;
        foreach (PlayerScore player in redPlayers)
        {
            if (player.name == name)
            {
                targetRedPlayer = player;
                break;
            }
        }

        if (targetRedPlayer != null)
            Destroy(targetRedPlayer.playerObject);

        redPlayers.Remove(targetRedPlayer);
    }

    public void ChangeTeam(Player player, int team)
    {
        if (team == 1)
        {
            for (int i = 0; i < bluePlayers.Count; i++)
            {
                if (bluePlayers[i].playerScript == player)
                {
                    AddPlayer(player, bluePlayers[i].name, bluePlayers[i].kills, bluePlayers[i].deaths, 1);

                    Destroy(bluePlayers[i].playerObject);

                    bluePlayers.Remove(bluePlayers[i]);

                    UpdateIndex();

                    break;
                }
            }
        }
        else if (team == 0)
        {
            for (int i = 0; i < redPlayers.Count; i++)
            {
                if (redPlayers[i].playerScript == player)
                {
                    AddPlayer(player, redPlayers[i].name, redPlayers[i].kills, redPlayers[i].deaths, 0);

                    Destroy(redPlayers[i].playerObject);

                    redPlayers.Remove(redPlayers[i]);

                    UpdateIndex();

                    break;
                }
            }
        }
    }

    public void AddPlayer(Player player, string name, int kills, int deaths, int team)
    {
        GameObject playerObject = Instantiate(this.player);
        playerObject.SetActive(true);

        if (team == 0)
        {
            playerObject.transform.parent = bluePlayerList.transform;

            bluePlayers.Add(new PlayerScore(playerObject, player, kills, deaths, name));
        }
        else
        {
            playerObject.transform.parent = redPlayerList.transform;

            redPlayers.Add(new PlayerScore(playerObject, player, kills, deaths, name));
        }

        playerObject.GetComponent<Text>().text = name;

        foreach (Transform child in playerObject.transform)
        {
            if (child.name == "Kills")
            {
                child.GetComponent<Text>().text = kills.ToString(CultureInfo.InvariantCulture);
            }
            if (child.name == "Deaths")
            {
                child.GetComponent<Text>().text = deaths.ToString(CultureInfo.InvariantCulture);
            }
        }
    }

    private void Start()
    {
        bluePlayers = new List<PlayerScore>();
        redPlayers = new List<PlayerScore>();

        Instance = this;
    }

    public void ModifyKills(Player player, int kills, int team)
    {
        if (team == 0)
        {
            for (int i = 0; i < bluePlayers.Count; i++)
            {
                if (bluePlayers[i].playerScript == player)
                {
                    foreach (Transform child in bluePlayers[i].playerObject.transform)
                    {
                        if (child.name == "Kills")
                        {
                            child.GetComponent<Text>().text = kills.ToString(CultureInfo.InvariantCulture);

                            break;
                        }
                    }
                    bluePlayers[i].kills = kills;
                    UpdateIndex();
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < redPlayers.Count; i++)
            {
                if (redPlayers[i].playerScript == player)
                {
                    foreach (Transform child in redPlayers[i].playerObject.transform)
                    {
                        if (child.name == "Kills")
                        {
                            child.GetComponent<Text>().text = kills.ToString(CultureInfo.InvariantCulture);
                        }
                    }
                    redPlayers[i].kills = kills;
                    UpdateIndex();
                    break;
                }
            }
        }
    }

    public void ModifyDeaths(Player player, int deaths, int team)
    {
        if (team == 0)
        {
            for (int i = 0; i < bluePlayers.Count; i++)
            {
                if (bluePlayers[i].playerScript == player)
                {
                    foreach (Transform child in bluePlayers[i].playerObject.transform)
                    {
                        if (child.name == "Deaths")
                        {
                            child.GetComponent<Text>().text = deaths.ToString(CultureInfo.InvariantCulture);
                        }
                    }

                    bluePlayers[i].deaths = deaths;
                    UpdateIndex();
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < redPlayers.Count; i++)
            {
                if (redPlayers[i].playerScript == player)
                {
                    foreach (Transform child2 in redPlayers[i].playerObject.transform)
                    {
                        if (child2.name == "Deaths")
                        {
                            child2.GetComponent<Text>().text = deaths.ToString(CultureInfo.InvariantCulture);
                        }
                    }
                    redPlayers[i].deaths = deaths;
                    UpdateIndex();
                    break;
                }
            }
        }
    }

    public void ModifyAlive(Player player, bool alive, int team)
    {
        if (team == 0)
        {
            for (int i = 0; i < bluePlayers.Count; i++)
            {
                if (bluePlayers[i].playerScript == player)
                {
                    foreach (Transform child in bluePlayers[i].playerObject.transform)
                    {
                        if (child.name == "Alive")
                        {
                            child.gameObject.SetActive(!alive);
                        }
                    }
                    UpdateIndex();
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < redPlayers.Count; i++)
            {
                if (redPlayers[i].playerScript == player)
                {
                    foreach (Transform child in redPlayers[i].playerObject.transform)
                    {
                        if (child.name == "Alive")
                        {
                            child.gameObject.SetActive(!alive);
                        }
                    }
                    UpdateIndex();
                    break;
                }
            }
        }
    }

    public void ModifyName(string old, string newName)
    {
        bool ok = false;
        for (int i = 0; i < bluePlayers.Count; i++)
        {
            if (bluePlayers[i].name == old)
            {
                bluePlayers[i].name = newName;
                ok = true;
                break;
            }
        }

        if (ok == false)
            for (int i = 0; i < redPlayers.Count; i++)
            {
                if (redPlayers[i].name == old)
                {
                    redPlayers[i].name = newName;
                    break;
                }
            }

        UpdateIndex();
    }

    public void UpdateIndex()
    {
        bluePlayers = bluePlayers.OrderByDescending(a => a.kills).ThenBy(a => a.deaths).ThenBy(a => a.name).ToList();

        for (int i = 0; i < bluePlayers.Count; i++)
        {
            Transform child = bluePlayerList.transform.GetChild(i);
            child.GetComponent<Text>().text = bluePlayers[i].name;
            foreach (Transform child2 in child)
            {
                if (child2.name == "Deaths")
                    child2.GetComponent<Text>().text = bluePlayers[i].deaths.ToString(CultureInfo.InvariantCulture);
                if (child2.name == "Kills")
                    child2.GetComponent<Text>().text = bluePlayers[i].kills.ToString(CultureInfo.InvariantCulture);
                if (child2.name == "Alive")
                {
                    if (bluePlayers[i].playerScript == null || bluePlayers[i].playerScript.Dead)
                        child2.gameObject.SetActive(true);
                    else child2.gameObject.SetActive(false);
                }

            }
        }

        redPlayers = redPlayers.OrderByDescending(a => a.kills).ThenBy(a => a.deaths).ThenBy(a => a.name).ToList();

        for (int i = 0; i < redPlayers.Count; i++)
        {
            Transform child = redPlayerList.transform.GetChild(i);
            child.GetComponent<Text>().text = redPlayers[i].name;
            foreach (Transform child2 in child)
            {
                if (child2.name == "Deaths")
                    child2.GetComponent<Text>().text = redPlayers[i].deaths.ToString(CultureInfo.InvariantCulture);
                if (child2.name == "Kills")
                    child2.GetComponent<Text>().text = redPlayers[i].kills.ToString(CultureInfo.InvariantCulture);
                if (child2.name == "Alive")
                {
                    if (redPlayers[i].playerScript == null || redPlayers[i].playerScript.Dead)
                        child2.gameObject.SetActive(true);
                    else child2.gameObject.SetActive(false);
                }

            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            content.SetActive(true);
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            content.SetActive(false);
        }
    }
}

public class PlayerScore
{
    public PlayerScore(GameObject playerObject, Player playerScript, int kills, int deaths, string name)
    {
        this.playerObject = playerObject;
        this.playerScript = playerScript;
        this.kills = kills;
        this.deaths = deaths;
        this.name = name;
        playerID = playerScript.ID;
    }

    public GameObject playerObject;
    public Player playerScript;
    public int kills;
    public int deaths;
    public string name;
    public long playerID;
};