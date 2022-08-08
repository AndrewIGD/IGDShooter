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
    Death
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

public class Throw
{
    public Throw(Vector2 position) => Position = position;

    public Vector2 Position;
}

public class ScrollUp
{ }

public class ScrollDown
{ }

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
    public Bullet(Vector2 position, Vector2 velocity, float angle, int id)
    {
        Position = position;
        Velocity = velocity;
        Angle = angle;
        ID = id;
    }

    public int ID;
    public Vector2 Position;
    public Vector2 Velocity;
    public float Angle;
}

public class Defused { }
public class Warmup { }

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
    public SpawnPlayer(string playerID, string name, int team, int gunID, int pistol, int he, int flash, int smoke, int armor, int cash, int wall, int kills, int deaths)
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
