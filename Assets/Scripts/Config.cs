using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config : MonoBehaviour
{
    public static string FightSceneName = "FightScene";
    public static int FightSceneIndex = 3;

    public static bool ExperimentalFeatures = false;

    public static bool BombDefusal = true;
    public static bool FriendlyFire = false;
    public static bool ShowKillFeed = true;

    public static string CurrentMap;

    public static int BuyTime = 20;
    public static int BombTime = 41;
    public static int FreezeTime = 11;
    public static int RoundTime = 120;
    public static int RoundsPerHalf = 15;
    public static int RoundsToWin = 16;
    public static int MaxCash = 16000;
    public static int WinCash = 5000;
    public static int LoseCash = 2000;
    public static int LoseStreakCash = 1000;

    public static int DashSpeed = 30;
}
