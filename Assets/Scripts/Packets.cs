using UnityEngine;

public enum BinaryType
{
    Integer,
    Float,
    String,

    PlayerCount,
    SystemMessage,
    ClientNewPlayerName,
    ServerNewPlayerName,
    NewPlayerTeam,
    PlayerTarget,
    Shoot,
    ThrowNade,
    Switch,
    Process,
    Reload,
    Defuse,
    StopDef,
    DropTarget,
    Shift,
    StopSh,
    Throw,
    ScrollUp,
    ScrollDown,
    Graffiti,
    Msg,
    Play,
    Destroy,
    Kick,
    Ban,
    KillFeed,
    SetKills,
    FlashScreen,
    SmokeVfx,
    Wall,
    Grenade,
    Bullet,
    Defused,
    BombExplosion,
    Warmup,
    ServerDrop,
    SpawnPlayer,
    Disconnect,
    SetDeaths,
    Death,
    PlayerInfo,
    Count,
    Rounds,
    Time,
    ServerGraffiti,
    ServerDefuse,
    ServerStopDefuse,
    TabPlayer,
    ServerPlant,
    NewName,
    Whisper,
    All,
    ServerMessage,
    EndGame,
    CT,
    T,
    StartGame,
    ChangeTeam,
    Teleport,
    ServerWhisper,
    RoundStart,
    Visibility,
    ServerDisconnect,
    ReceiveID,
    BanMessage,
    LoadScene,
    Dash,
    ExperimentalFeatures
}

public class PlayerCount
{
    public PlayerCount(int count) => Number = count;

    public int Number;
}

public class SystemMessage
{
    public SystemMessage(string data) => Data = data;

    public string Data;
}

public class ClientNewPlayerName
{
    public ClientNewPlayerName(string data) => Data = data;

    public string Data;
}

public class ServerNewPlayerName
{
    public ServerNewPlayerName(string id, string data)
    {
        ID = id;
        Data = data;
    }

    public string ID;
    public string Data;
}

public class NewPlayerTeam
{
    public NewPlayerTeam(int teamID) => TeamID = teamID;

    public int TeamID;
}

public class PlayerTarget
{
    public PlayerTarget(Vector2 position) => Position = position;

    public Vector2 Position;
}

public class Shoot
{
    public Shoot(float angle) => Angle = angle;

    public float Angle;
}

public class ThrowNade
{
    public ThrowNade(Vector2 position) => Position = position;

    public Vector2 Position;
}

public class Switch
{
    public Switch(int inventoryID) => InventoryID = inventoryID;

    public int InventoryID;
}

public class Process
{
    public Process(int buyID) => BuyID = buyID;

    public int BuyID;
}

public class Reload
{ }

public class Defuse
{ }

public class StopDef
{ }

public class DropTarget
{ }

public class Shift
{ }

public class StopSh
{ }

public class ServerDisconnect
{ }

public class BanMessage
{ }

public class ReceiveID
{
    public ReceiveID(string id, string map)
    {
        ID = id;
        Map = map;
    }

    public string ID;
    public string Map;
}

public class Throw
{
    public Throw(Vector2 position) => Position = position;

    public Vector2 Position;
}

public class LoadScene
{
    public LoadScene(string name, int sceneIndex)
    {
        Name = name;
        SceneIndex = sceneIndex;
    }

    public int SceneIndex;
    public string Name;
}

public class ScrollUp
{ }

public class ScrollDown
{ }

public class ExperimentalFeatures
{
    public ExperimentalFeatures(bool on) => On = on;

    public bool On;
}

public class Graffiti
{
    public Graffiti(int graffitiID) => GraffitiID = graffitiID;

    public int GraffitiID;
}

public class Msg
{
    public Msg(bool all, int team, string data)
    {
        All = all;
        Team = team;
        Data = data;
    }

    public bool All;
    public int Team;
    public string Data;
}

public class Play
{
    public Play(int soundId, Vector2? position = null, string playerID = null)
    {
        SoundID = soundId;
        Position = position;
        PlayerID = playerID;
    }

    public int SoundID;
    public Vector2? Position;
    public string PlayerID;
}

public class Destroy
{
    public Destroy(string itemName)
    {
        ItemName = itemName;
    }

    public string ItemName;
}

public class Kick
{
    public Kick(string playerID, string message)
    {
        PlayerID = playerID;
        Message = message;
    }

    public string PlayerID;
    public string Message;
}

public class Ban
{
    public Ban(string playerID, string message)
    {
        PlayerID = playerID;
        Message = message;
    }

    public string PlayerID;
    public string Message;
}

public class KillFeedPacket
{
    public KillFeedPacket(string killerName, string targetName, int killType, int killerTeam, int targetTeam)
    {
        KillerName = killerName;
        TargetName = targetName;
        KillType = killType;
        KillerTeam = killerTeam;
        TargetTeam = targetTeam;
    }

    public string KillerName;
    public string TargetName;
    public int KillType;
    public int KillerTeam;
    public int TargetTeam;
}

public class SetKills
{
    public SetKills(string killerId, int kills)
    {
        KillerId = killerId;
        Kills = kills;
    }

    public string KillerId;
    public int Kills;
}

public class SetDeaths
{
    public SetDeaths(string target, int deaths)
    {
        Target = target;
        Deaths = deaths;
    }

    public string Target;
    public int Deaths;
}

public class FlashScreen
{
    public FlashScreen(Vector2 position) => Position = position;

    public Vector2 Position;
}

public class SmokeVfx
{
    public SmokeVfx(Vector2 position) => Position = position;

    public Vector2 Position;
}

public class WallPacket
{
    public WallPacket(Vector2 position, float angle, int id)
    {
        Position = position;
        Angle = angle;
        ID = id;
    }

    public Vector2 Position;
    public float Angle;
    public int ID;
}

public class GrenadePacket
{
    public GrenadePacket(int grenadeId, Vector2 position, Vector2 velocity, float angle) {
        GrenadeID = grenadeId;
        Position = position;
        Velocity = velocity;
        Angle = angle;
    }

    public int GrenadeID;
    public Vector2 Position;
    public Vector2 Velocity;
    public float Angle;
}

public class Bullet
{
    public Bullet(Vector2 position, Vector2 velocity, float angle, int id, bool destroy)
    {
        Position = position;
        Velocity = velocity;
        Angle = angle;
        ID = id;
        Destroy = destroy;
    }

    public int ID;
    public Vector2 Position;
    public Vector2 Velocity;
    public float Angle;
    public bool Destroy;
}

public class Defused { }
public class Warmup { }
public class StartGame
{
    public StartGame(int sceneIndex) => SceneIndex = sceneIndex;

    public int SceneIndex;
}

public class BombExplosion
{
    public BombExplosion(Vector2 position) => Position = position;

    public Vector2 Position;
}

public class ServerDrop
{
    public ServerDrop(int type, Vector2 position, Vector2 velocity, int dropId, int bulletCount, int roundAmmo)
    {
        Position = position;
        Velocity = velocity;
        Type = type;
        ID = dropId;
        BulletCount = bulletCount;
        RoundAmmo = roundAmmo;
    }

    public int Type;
    public int ID;
    public int BulletCount;
    public int RoundAmmo;
    public Vector2 Position;
    public Vector2 Velocity;
}

public class SpawnPlayer
{
    public SpawnPlayer(string playerID, string name, int team, int gunID, int pistol, int he, int flash, int smoke, int armor, int cash, int wall, int kills, int deaths, int sceneIndex)
    {
        PlayerID = playerID;
        Name = name;
        Team = team;
        GunID = gunID;
        Pistol = pistol;
        HE = he;
        Flash = flash;
        Smoke = smoke;
        Armor = armor;
        Cash = cash;
        Wall = wall;
        Kills = kills;
        Deaths = deaths;
        SceneIndex = sceneIndex;
    }

    public string PlayerID;
    public string Name;
    public int Team;
    public int GunID;
    public int Pistol;
    public int HE;
    public int Flash;
    public int Smoke;
    public int Armor;
    public int Cash;
    public int Wall;
    public int Kills;
    public int Deaths;
    public int SceneIndex;
}

public class Disconnect
{
    public Disconnect(string id) => ID = id;

    public string ID;
}

public class Death
{
    public Death(string id, int? dealt = null, int? received = null, string attacker = null)
    {
        Id = id;
        Dealt = dealt;
        Received = received;
        Attacker = attacker;
    }

    public string Id;
    public int? Dealt;
    public int? Received;
    public string Attacker;
}

public class PlayerInfo
{
    public PlayerInfo(string playerID, Vector2 position, float angle, int health, Vector2 moveDir, string anim, int gunScriptType, int armor, int cash, int gunType, int pistol, int he, int flash, int smoke, int gunRoundAmmo, int gunBulletCount, int roundTime, int canBuy, int inBuyZone, int hasBomb, int hasWall, float dashTimer)
    {
        PlayerID = playerID;
        Position = position;
        Angle = angle;
        Health = health;
        MoveDir = moveDir;
        Anim = anim;
        GunScriptType = gunScriptType;
        GunType = gunType;
        Pistol = pistol;
        HE = he;
        Flash = flash;
        Smoke = smoke;
        Armor = armor;
        Cash = cash;
        GunRoundAmmo = gunRoundAmmo;
        GunBulletCount = gunBulletCount;
        RoundTime = roundTime;
        CanBuy = canBuy;
        InBuyZone = inBuyZone;
        HasBomb = hasBomb;
        HasWall = hasWall;
        DashTimer = dashTimer;
    }

    public string PlayerID;
    public Vector2 Position;
    public float Angle;
    public int Health;
    public Vector2 MoveDir;
    public string Anim;
    public int GunScriptType;
    public int GunRoundAmmo;
    public int GunBulletCount;
    public int RoundTime;
    public int CanBuy;
    public int InBuyZone;
    public int HasBomb;
    public int GunType;
    public int Pistol;
    public int HE;
    public int Flash;
    public int Smoke;
    public int Armor;
    public int Cash;
    public int HasWall;
    public float DashTimer;
}

public class Count
{
    public Count(int red, int blue)
    {
        Red = red;
        Blue = blue;
    }

    public int Red;
    public int Blue;
}

public class Rounds
{
    public Rounds(int red, int blue)
    {
        Red = red;
        Blue = blue;
    }

    public int Red;
    public int Blue;
}

public class TimePacket
{
    public TimePacket(int value)
    {
        Value = value;
    }

    public int Value;
}

public class ServerGraffiti
{
    public ServerGraffiti(Vector2 position, int id)
    {
        Position = position;
        Id = id;
    }

    public Vector2 Position;
    public int Id;
}

public class ServerDefuse
{
    public ServerDefuse(string playerId)
    {
        PlayerId = playerId;
    }

    public string PlayerId;
}

public class ServerStopDefuse
{
    public ServerStopDefuse(string playerId)
    {
        PlayerId = playerId;
    }

    public string PlayerId;
}

public class TabPlayer
{
    public TabPlayer(string playerId, string name, int team)
    {
        PlayerId = playerId;
        Name = name;
        Team = team;
    }

    public string PlayerId;
    public string Name;
    public int Team;
}

public class ServerPlant
{
    public ServerPlant(Vector2 position) => Position = position;

    public Vector2 Position;
}

public class NewName
{
    public NewName(string playerId, string name)
    {
        PlayerId = playerId;
        Name = name;
    }

    public string PlayerId;
    public string Name;
}

public class Whisper
{
    public Whisper(string targetID, string name, string message)
    {
        TargetID = targetID;
        Name = name;
        Message = message;
    }

    public string TargetID;
    public string Name;
    public string Message;
}


public class All
{
    public All(string playerId, int team, string message)
    {
        PlayerId = playerId;
        Team = team;
        Message = message;
    }

    public string PlayerId;
    public int Team;
    public string Message;
}

public class ServerMessage
{
    public ServerMessage(string playerId, int team, string message)
    {
        PlayerId = playerId;
        Team = team;
        Message = message;
    }

    public string PlayerId;
    public int Team;
    public string Message;
}


public class EndGame
{
    public EndGame(int team)
    {
        Team = team;
    }

    public int Team;
}

public class CT
{
    public CT(int id, string name, int kills, int deaths, string title, string description)
    {
        Id = id;
        Name = name;
        Kills = kills;
        Deaths = deaths;
        Title = title;
        Description = description;
    }

    public int Id;
    public string Name;
    public int Kills;
    public int Deaths;
    public string Title;
    public string Description;
}

public class T
{
    public T(int id, string name, int kills, int deaths, string title, string description)
    {
        Id = id;
        Name = name;
        Kills = kills;
        Deaths = deaths;
        Title = title;
        Description = description;
    }

    public int Id;
    public string Name;
    public int Kills;
    public int Deaths;
    public string Title;
    public string Description;
}

public class ChangeTeam
{
    public ChangeTeam(string playerId, string team)
    {
        PlayerId = playerId;
        Team = team;
    }

    public string PlayerId;
    public string Team;
}

public class Teleport
{
    public Teleport(string playerId, string target)
    {
        PlayerId = playerId;
        Target = target;
    }

    public string PlayerId;
    public string Target;
}

public class ServerWhisper
{
    public ServerWhisper(string playerId, string message)
    {
        PlayerId = playerId;
        Message = message;
    }

    public string PlayerId;
    public string Message;
}

public class RoundStart
{
    public RoundStart(bool roundStart, object obj)
    {
        _RoundStart = roundStart;
        Obj = obj;
    }

    public bool _RoundStart;
    public object Obj;
}

public class Visibility
{
    public Visibility(string playerId, string value)
    {
        PlayerId = playerId;
        Value = value;
    }

    public string PlayerId;
    public string Value;
}

public class Dash
{
    public Dash(Vector2 position) => Position = position;

    public Vector2 Position;
}