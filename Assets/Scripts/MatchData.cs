using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchData : MonoBehaviour
{
    public static NetworkingLibrary NetworkingLibrary;

    public static Dictionary<string, PlayerData> PlayerData = new Dictionary<string, PlayerData>(); 
    
    public static PlayerData GetPlayerData(string id)
    {
        if (PlayerData.ContainsKey(id) == false)
            PlayerData.Add(id, new PlayerData());

        return PlayerData[id];
    }

    public static void ResetPlayerData()
    {
        foreach(string i in PlayerData.Keys)
        {
            PlayerData[i].Cash = 800;
            PlayerData[i].GunID = -1;
            PlayerData[i].Armor = 0;
            PlayerData[i].HasPistol = false;
            PlayerData[i].HasHe = false;
            PlayerData[i].HasFlash = false;
            PlayerData[i].HasSmoke = false;
            PlayerData[i].HasWall = false;


            PlayerData[i].Kills = 0;
            PlayerData[i].DamageDealt = 0;
            PlayerData[i].BombDefusals = 0;
            PlayerData[i].BombPlants = 0;
            PlayerData[i].SilentSteps = 0;
            PlayerData[i].ConsecutiveKills = 0;
            PlayerData[i].ThrownWeapons = 0;
            PlayerData[i].HeDamage = 0;
            PlayerData[i].MaximumRoundsSurvived = 0;
            PlayerData[i].RoundsSurvived = 0;
            PlayerData[i].NadesThrown = 0;
            PlayerData[i].Deaths = 0;
        }
    }

    public static void Swap()
    {
        foreach (string i in PlayerData.Keys)
        {
            PlayerData[i].Cash = 800;
            PlayerData[i].GunID = -1;
            PlayerData[i].Armor = 0;
            PlayerData[i].HasPistol = false;
            PlayerData[i].HasHe = false;
            PlayerData[i].HasFlash = false;
            PlayerData[i].HasSmoke = false;
            PlayerData[i].HasWall = false;

            if (PlayerData[i].Team == 1)
                PlayerData[i].Team = 0;
            else PlayerData[i].Team = 1;
        }
    }

    public static void ResetInventories()
    {
        foreach (string i in PlayerData.Keys)
        {
            PlayerData[i].Cash = 800;
            PlayerData[i].GunID = -1;
            PlayerData[i].Armor = 0;
            PlayerData[i].HasPistol = false;
            PlayerData[i].HasHe = false;
            PlayerData[i].HasFlash = false;
            PlayerData[i].HasSmoke = false;
            PlayerData[i].HasWall = false;
        }
    }
}

public class PlayerData
{
    public bool ClientAlive = true;

    public float TimeOut;


    public string Name;

    public int Team;

    public int Cash;

    public int GunID = -1;

    public int Armor;

    public bool HasPistol;

    public bool HasHe;

    public bool HasFlash;

    public bool HasSmoke;

    public bool HasWall;



    public int Kills;

    public int DamageDealt;

    public int BombDefusals;

    public int BombPlants;

    public int SilentSteps;

    public int ConsecutiveKills;

    public int ThrownWeapons;

    public int HeDamage;

    public int MaximumRoundsSurvived;

    public int RoundsSurvived;

    public int NadesThrown;

    public int Deaths;
}
