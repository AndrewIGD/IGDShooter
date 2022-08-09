using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BinarySerializer
{
    public static void Add(object obj)
    {
        switch (obj)
        {
            case int i:
                {
                    _byteList.Add((byte)BinaryType.Integer);
                    _byteList.AddRange(SerializeInt(i));

                    break;
                }
            case float f:
                {
                    _byteList.Add((byte)BinaryType.Float);
                    _byteList.AddRange(SerializeFloat(f));

                    break;
                }
            case string s:
                {
                    _byteList.Add((byte)BinaryType.String);
                    _byteList.AddRange(SerializeString(s));

                    break;
                }

            case PlayerCount playerCount:
                {
                    _byteList.Add((byte)BinaryType.PlayerCount);
                    _byteList.AddRange(SerializeInt(playerCount.Number));

                    break;
                }

            case SystemMessage systemMessage:
                {
                    _byteList.Add((byte)BinaryType.SystemMessage);
                    _byteList.AddRange(SerializeString(systemMessage.Data));

                    break;
                }

            case ClientNewPlayerName clientNewPlayerName:
                {
                    _byteList.Add((byte)BinaryType.ClientNewPlayerName);
                    _byteList.AddRange(SerializeString(clientNewPlayerName.Data));

                    break;
                }

            case ServerNewPlayerName serverNewPlayerName:
                {
                    _byteList.Add((byte)BinaryType.ClientNewPlayerName);
                    _byteList.AddRange(SerializeString(serverNewPlayerName.ID));
                    _byteList.AddRange(SerializeString(serverNewPlayerName.Data));

                    break;
                }

            case NewPlayerTeam newPlayerTeam:
                {
                    _byteList.Add((byte)BinaryType.NewPlayerTeam);
                    _byteList.AddRange(SerializeInt(newPlayerTeam.TeamID));

                    break;
                }

            case PlayerTarget playerTarget:
                {
                    _byteList.Add((byte)BinaryType.PlayerTarget);
                    _byteList.AddRange(SerializeFloat(playerTarget.Position.x));
                    _byteList.AddRange(SerializeFloat(playerTarget.Position.y));

                    break;
                }

            case Shoot shoot:
                {
                    _byteList.Add((byte)BinaryType.Shoot);
                    _byteList.AddRange(SerializeFloat(shoot.Angle));

                    break;
                }

            case ThrowNade throwNade:
                {
                    _byteList.Add((byte)BinaryType.ThrowNade);
                    _byteList.AddRange(SerializeFloat(throwNade.Position.x));
                    _byteList.AddRange(SerializeFloat(throwNade.Position.y));

                    break;
                }

            case Switch _switch:
                {
                    _byteList.Add((byte)BinaryType.Switch);
                    _byteList.AddRange(SerializeInt(_switch.InventoryID));

                    break;
                }
            case Process process:
                {
                    _byteList.Add((byte)BinaryType.Process);
                    _byteList.AddRange(SerializeInt(process.BuyID));

                    break;
                }
            case Reload reload:
                {
                    _byteList.Add((byte)BinaryType.Reload);

                    break;
                }

            case Defuse defuse:
                {
                    _byteList.Add((byte)BinaryType.Defuse);

                    break;
                }

            case StopDef stopDef:
                {
                    _byteList.Add((byte)BinaryType.StopDef);

                    break;
                }

            case DropTarget dropTarget:
                {
                    _byteList.Add((byte)BinaryType.DropTarget);

                    break;
                }
            case Shift shift:
                {
                    _byteList.Add((byte)BinaryType.Shift);

                    break;
                }
            case StopSh stopSh:
                {
                    _byteList.Add((byte)BinaryType.StopSh);

                    break;
                }
            case Throw thr:
                {
                    _byteList.Add((byte)BinaryType.Throw);
                    _byteList.AddRange(SerializeFloat(thr.Position.x));
                    _byteList.AddRange(SerializeFloat(thr.Position.y));

                    break;
                }
            case ScrollUp scrollUp:
                {
                    _byteList.Add((byte)BinaryType.ScrollUp);

                    break;
                }
            case ScrollDown scrollDown:
                {
                    _byteList.Add((byte)BinaryType.ScrollDown);

                    break;
                }
            case Graffiti graffiti:
                {
                    _byteList.Add((byte)BinaryType.Graffiti);
                    _byteList.AddRange(SerializeInt(graffiti.GraffitiID));

                    break;
                }
            case Msg msg:
                {
                    _byteList.Add((byte)BinaryType.Msg);
                    _byteList.Add((byte)(msg.All ? 1 : 0));
                    _byteList.AddRange(SerializeInt(msg.Team));
                    _byteList.AddRange(SerializeString(msg.Data));

                    break;
                }
            case Play play:
                {
                    byte paramCount = 0;

                    if (play.Position.HasValue)
                        paramCount++;

                    if (play.PlayerID != null)
                        paramCount++;

                    _byteList.Add((byte)BinaryType.Play);
                    _byteList.Add(paramCount);
                    _byteList.AddRange(SerializeInt(play.SoundID));

                    if (play.Position.HasValue)
                    {
                        _byteList.AddRange(SerializeFloat(play.Position.Value.x));
                        _byteList.AddRange(SerializeFloat(play.Position.Value.y));
                    }

                    if (play.PlayerID != null)
                        _byteList.AddRange(SerializeString(play.PlayerID));

                    break;
                }
            case Destroy destroy:
                {
                    _byteList.Add((byte)BinaryType.Destroy);
                    _byteList.AddRange(SerializeString(destroy.ItemName));

                    break;
                }
            case Kick kick:
                {
                    _byteList.Add((byte)BinaryType.Kick);
                    _byteList.AddRange(SerializeString(kick.PlayerID));
                    _byteList.AddRange(SerializeString(kick.Message));

                    break;
                }
            case Ban ban:
                {
                    _byteList.Add((byte)BinaryType.Ban);
                    _byteList.AddRange(SerializeString(ban.PlayerID));
                    _byteList.AddRange(SerializeString(ban.Message));

                    break;
                }
            case KillFeedPacket killFeed:
                {
                    _byteList.Add((byte)BinaryType.KillFeed);
                    _byteList.AddRange(SerializeString(killFeed.KillerName));
                    _byteList.AddRange(SerializeString(killFeed.TargetName));
                    _byteList.AddRange(SerializeInt(killFeed.KillType));
                    _byteList.AddRange(SerializeInt(killFeed.KillerTeam));
                    _byteList.AddRange(SerializeInt(killFeed.TargetTeam));

                    break;
                }
            case SetKills setKills:
                {
                    _byteList.Add((byte)BinaryType.SetKills);
                    _byteList.AddRange(SerializeString(setKills.KillerId));
                    _byteList.AddRange(SerializeInt(setKills.Kills));

                    break;
                }
            case SetDeaths setDeaths:
                {
                    _byteList.Add((byte)BinaryType.SetDeaths);
                    _byteList.AddRange(SerializeString(setDeaths.Target));
                    _byteList.AddRange(SerializeInt(setDeaths.Deaths));

                    break;
                }
            case FlashScreen flashScreen:
                {
                    _byteList.Add((byte)BinaryType.FlashScreen);
                    _byteList.AddRange(SerializeFloat(flashScreen.Position.x));
                    _byteList.AddRange(SerializeFloat(flashScreen.Position.y));

                    break;
                }
            case SmokeVfx smokeVfx:
                {
                    _byteList.Add((byte)BinaryType.SmokeVfx);
                    _byteList.AddRange(SerializeFloat(smokeVfx.Position.x));
                    _byteList.AddRange(SerializeFloat(smokeVfx.Position.y));

                    break;
                }
            case WallPacket wall:
                {
                    _byteList.Add((byte)BinaryType.Wall);
                    _byteList.AddRange(SerializeFloat(wall.Position.x));
                    _byteList.AddRange(SerializeFloat(wall.Position.y));
                    _byteList.AddRange(SerializeFloat(wall.Angle));
                    _byteList.AddRange(SerializeInt(wall.ID));

                    break;
                }
            case GrenadePacket grenade:
                {
                    _byteList.Add((byte)BinaryType.Grenade);
                    _byteList.AddRange(SerializeInt(grenade.GrenadeID));
                    _byteList.AddRange(SerializeFloat(grenade.Position.x));
                    _byteList.AddRange(SerializeFloat(grenade.Position.y));
                    _byteList.AddRange(SerializeFloat(grenade.Velocity.y));
                    _byteList.AddRange(SerializeFloat(grenade.Velocity.y));
                    _byteList.AddRange(SerializeFloat(grenade.Angle));

                    break;
                }
            case Bullet bullet:
                {
                    _byteList.Add((byte)BinaryType.Bullet);
                    _byteList.AddRange(SerializeInt(bullet.ID));
                    _byteList.AddRange(SerializeFloat(bullet.Position.x));
                    _byteList.AddRange(SerializeFloat(bullet.Position.y));
                    _byteList.AddRange(SerializeFloat(bullet.Velocity.y));
                    _byteList.AddRange(SerializeFloat(bullet.Velocity.y));
                    _byteList.AddRange(SerializeFloat(bullet.Angle));

                    break;
                }
            case Defused defused:
                {
                    _byteList.Add((byte)BinaryType.Defused);

                    break;
                }
            case Warmup warmup:
                {
                    _byteList.Add((byte)BinaryType.Warmup);

                    break;
                }
            case BombExplosion bombExplosion:
                {
                    _byteList.Add((byte)BinaryType.BombExplosion);
                    _byteList.AddRange(SerializeFloat(bombExplosion.Position.x));
                    _byteList.AddRange(SerializeFloat(bombExplosion.Position.y));

                    break;
                }
            case ServerDrop serverDrop:
                {
                    _byteList.Add((byte)BinaryType.ServerDrop);
                    _byteList.AddRange(SerializeInt(serverDrop.Type)); 
                    _byteList.AddRange(SerializeFloat(serverDrop.Position.x));
                    _byteList.AddRange(SerializeFloat(serverDrop.Position.y));
                    _byteList.AddRange(SerializeFloat(serverDrop.Velocity.y));
                    _byteList.AddRange(SerializeFloat(serverDrop.Velocity.y));
                    _byteList.AddRange(SerializeInt(serverDrop.ID));
                    _byteList.AddRange(SerializeFloat(serverDrop.BulletCount));
                    _byteList.AddRange(SerializeFloat(serverDrop.RoundAmmo));

                    break;
                }
            case SpawnPlayer spawnPlayer:
                {
                    _byteList.Add((byte)BinaryType.SpawnPlayer);
                    _byteList.AddRange(SerializeString(spawnPlayer.PlayerID));
                    _byteList.AddRange(SerializeString(spawnPlayer.Name));
                    _byteList.AddRange(SerializeInt(spawnPlayer.Team));
                    _byteList.AddRange(SerializeInt(spawnPlayer.GunID));
                    _byteList.Add((byte)(spawnPlayer.Pistol));
                    _byteList.Add((byte)(spawnPlayer.HE));
                    _byteList.Add((byte)(spawnPlayer.Flash));
                    _byteList.Add((byte)(spawnPlayer.Smoke));
                    _byteList.AddRange(SerializeInt(spawnPlayer.Armor));
                    _byteList.AddRange(SerializeInt(spawnPlayer.Cash));
                    _byteList.Add((byte)(spawnPlayer.Wall));
                    _byteList.AddRange(SerializeInt(spawnPlayer.Kills));
                    _byteList.AddRange(SerializeInt(spawnPlayer.Deaths));

                    break;
                }
            case Disconnect disconnect:
                {
                    _byteList.Add((byte)BinaryType.Disconnect);
                    _byteList.AddRange(SerializeString(disconnect.ID));

                    break;
                }
            case Death death:
                {
                    _byteList.Add((byte)BinaryType.Death);
                    _byteList.Add((byte)(death.Dealt.HasValue ? 1 : 0));
                    _byteList.AddRange(SerializeString(death.Id));

                    if (death.Dealt.HasValue)
                    {
                        _byteList.AddRange(SerializeInt(death.Dealt.Value));
                        _byteList.AddRange(SerializeInt(death.Received.Value));
                        _byteList.AddRange(SerializeString(death.Attacker));
                    }

                    break;
                }
            case PlayerInfo playerInfo:
                {
                    _byteList.Add((byte)BinaryType.PlayerInfo);
                    _byteList.AddRange(SerializeString(playerInfo.PlayerID));
                    _byteList.AddRange(SerializeFloat(playerInfo.Position.x));
                    _byteList.AddRange(SerializeFloat(playerInfo.Position.y));
                    _byteList.AddRange(SerializeFloat(playerInfo.Angle));
                    _byteList.AddRange(SerializeInt(playerInfo.Health));
                    _byteList.AddRange(SerializeFloat(playerInfo.MoveDir.x));
                    _byteList.AddRange(SerializeFloat(playerInfo.MoveDir.y));
                    _byteList.AddRange(SerializeString(playerInfo.Anim));
                    _byteList.AddRange(SerializeInt(playerInfo.GunScriptType));
                    _byteList.AddRange(SerializeInt(playerInfo.Armor));
                    _byteList.AddRange(SerializeInt(playerInfo.Cash));
                    _byteList.AddRange(SerializeInt(playerInfo.GunType));
                    _byteList.Add((byte)(playerInfo.Pistol));
                    _byteList.Add((byte)(playerInfo.HE));
                    _byteList.Add((byte)(playerInfo.Flash));
                    _byteList.Add((byte)(playerInfo.Smoke));
                    _byteList.AddRange(SerializeInt(playerInfo.GunRoundAmmo));
                    _byteList.AddRange(SerializeInt(playerInfo.GunBulletCount));
                    _byteList.AddRange(SerializeInt(playerInfo.RoundTime));
                    _byteList.Add((byte)(playerInfo.CanBuy));
                    _byteList.Add((byte)(playerInfo.InBuyZone));
                    _byteList.Add((byte)(playerInfo.HasBomb));
                    _byteList.Add((byte)(playerInfo.HasWall));

                    break;
                }
        }
    }

    public static byte[] Get()
    {
        _byteList.InsertRange(0, SerializeInt(_byteList.Count));

        return _byteList.ToArray();
    }

    public static void Clear()
    {
        _byteList.Clear();
    }

    private static List<byte> SerializeInt(int num)
    {
        List<byte> bytes = new List<byte>();

        for (int i = 3; i >= 0; i--)
            bytes.Add((byte)(((num >> (i * 8))) & 0xFF));

        return bytes;
    }

    private static unsafe List<byte> SerializeFloat(float num)
    {
        int val = *((int*)&num);

        return SerializeInt(val);
    }

    private static List<byte> SerializeString(string text)
    {
        List<byte> bytes = new List<byte>();

        bytes.AddRange(SerializeInt(text.Length));

        for (int i = 0; i < text.Length; i++)
            bytes.Add((byte)text[i]);

        return bytes;
    }

    private static List<byte> _byteList = new List<byte>();
}