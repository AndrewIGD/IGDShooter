using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BinaryDeserializer
{
    private static byte[] _bytes;
    private static int _byteIndex;

    public static object[] Deserialize(byte[] bytes)
    {
        List<object> objectList = new List<object>();
        _bytes = bytes;
        _byteIndex = 0;

        while (_byteIndex < bytes.Length)
        {
            BinaryType type = (BinaryType)DeserializeByte();

            switch (type)
            {
                case BinaryType.Integer:
                    {
                        objectList.Add(DeserializeInt());

                        break;
                    }
                case BinaryType.Float:
                    {
                        objectList.Add(DeserializeFloat());

                        break;
                    }
                case BinaryType.String:
                    {
                        objectList.Add(DeserializeString());

                        break;
                    }
                case BinaryType.PlayerCount:
                    {
                        objectList.Add(new PlayerCount(DeserializeInt()));

                        break;
                    }
                case BinaryType.SystemMessage:
                    {
                        objectList.Add(new SystemMessage(DeserializeString()));

                        break;
                    }
                case BinaryType.ClientNewPlayerName:
                    {
                        objectList.Add(new ClientNewPlayerName(DeserializeString()));

                        break;
                    }
                case BinaryType.ServerNewPlayerName:
                    {
                        objectList.Add(new ServerNewPlayerName(DeserializeString(), DeserializeString()));

                        break;
                    }
                case BinaryType.NewPlayerTeam:
                    {
                        objectList.Add(new NewPlayerTeam(DeserializeInt()));

                        break;
                    }
                case BinaryType.PlayerTarget:
                    {
                        objectList.Add(new PlayerTarget(new Vector2(DeserializeFloat(), DeserializeFloat())));

                        break;
                    }
                case BinaryType.Shoot:
                    {
                        objectList.Add(new Shoot(DeserializeFloat()));

                        break;
                    }
                case BinaryType.ThrowNade:
                    {
                        objectList.Add(new ThrowNade(new Vector2(DeserializeFloat(), DeserializeFloat())));

                        break;
                    }
                case BinaryType.Switch:
                    {
                        objectList.Add(new Switch(DeserializeInt()));

                        break;
                    }
                case BinaryType.Process:
                    {
                        objectList.Add(new Process(DeserializeInt()));

                        break;
                    }
                case BinaryType.Reload:
                    {
                        objectList.Add(new Reload());

                        break;
                    }
                case BinaryType.Defuse:
                    {
                        objectList.Add(new Defuse());

                        break;
                    }
                case BinaryType.StopDef:
                    {
                        objectList.Add(new StopDef());

                        break;
                    }
                case BinaryType.DropTarget:
                    {
                        objectList.Add(new DropTarget());

                        break;
                    }
                case BinaryType.Shift:
                    {
                        objectList.Add(new Shift());

                        break;
                    }
                case BinaryType.StopSh:
                    {
                        objectList.Add(new StopSh());

                        break;
                    }
                case BinaryType.Throw:
                    {
                        objectList.Add(new Throw(new Vector2(DeserializeFloat(), DeserializeFloat())));

                        break;
                    }
                case BinaryType.ScrollDown:
                    {
                        objectList.Add(new ScrollDown());

                        break;
                    }
                case BinaryType.ScrollUp:
                    {
                        objectList.Add(new ScrollUp());

                        break;
                    }
                case BinaryType.Graffiti:
                    {
                        objectList.Add(new Graffiti(DeserializeInt()));

                        break;
                    }
                case BinaryType.Msg:
                    {
                        objectList.Add(new Msg(DeserializeByte() == 1, DeserializeInt(), DeserializeString()));

                        break;
                    }
                case BinaryType.Play:
                    {
                        byte paramCount = DeserializeByte();

                        switch (paramCount)
                        {
                            case 0:
                                {
                                    objectList.Add(new Play(DeserializeInt()));

                                    break;
                                }
                            case 1:
                                {
                                    objectList.Add(new Play(DeserializeInt(), new Vector2(DeserializeFloat(), DeserializeFloat())));

                                    break;
                                }
                            case 2:
                                {
                                    objectList.Add(new Play(DeserializeInt(), new Vector2(DeserializeFloat(), DeserializeFloat()), DeserializeString()));

                                    break;
                                }
                        }

                        break;
                    }
                case BinaryType.Destroy:
                    {
                        objectList.Add(new Destroy(DeserializeString()));

                        break;
                    }
                case BinaryType.Kick:
                    {
                        objectList.Add(new Kick(DeserializeString(), DeserializeString()));

                        break;
                    }
                case BinaryType.Ban:
                    {
                        objectList.Add(new Ban(DeserializeString(), DeserializeString()));

                        break;
                    }
                case BinaryType.KillFeed:
                    {
                        objectList.Add(new KillFeedPacket(DeserializeString(), DeserializeString(), DeserializeInt(), DeserializeInt(), DeserializeInt()));

                        break;
                    }
                case BinaryType.SetKills:
                    {
                        objectList.Add(new SetKills(DeserializeString(), DeserializeInt()));

                        break;
                    }
                case BinaryType.SetDeaths:
                    {
                        objectList.Add(new SetDeaths(DeserializeString(), DeserializeInt()));

                        break;
                    }
                case BinaryType.FlashScreen:
                    {
                        objectList.Add(new FlashScreen(new Vector2(DeserializeFloat(), DeserializeFloat())));

                        break;
                    }
                case BinaryType.SmokeVfx:
                    {
                        objectList.Add(new SmokeVfx(new Vector2(DeserializeFloat(), DeserializeFloat())));

                        break;
                    }
                case BinaryType.Wall:
                    {
                        objectList.Add(new WallPacket(new Vector2(DeserializeFloat(), DeserializeFloat()), DeserializeFloat(), DeserializeInt()));

                        break;
                    }
                case BinaryType.Grenade:
                    {
                        objectList.Add(new GrenadePacket(DeserializeInt(), new Vector2(DeserializeFloat(), DeserializeFloat()), new Vector2(DeserializeFloat(), DeserializeFloat()), DeserializeFloat()));

                        break;
                    }
                case BinaryType.Bullet:
                    {
                        objectList.Add(new Bullet(new Vector2(DeserializeFloat(), DeserializeFloat()), new Vector2(DeserializeFloat(), DeserializeFloat()), DeserializeFloat(), DeserializeInt()));

                        break;
                    }
                case BinaryType.Defused:
                    {
                        objectList.Add(new Defused());

                        break;
                    }
                case BinaryType.Warmup:
                    {
                        objectList.Add(new Warmup());

                        break;
                    }
                case BinaryType.BombExplosion:
                    {
                        objectList.Add(new BombExplosion(new Vector2(DeserializeFloat(), DeserializeFloat())));

                        break;
                    }
                case BinaryType.ServerDrop:
                    {
                        objectList.Add(new ServerDrop(DeserializeInt(), new Vector2(DeserializeFloat(), DeserializeFloat()), new Vector2(DeserializeFloat(), DeserializeFloat()), DeserializeInt(), DeserializeInt(), DeserializeInt()));

                        break;
                    }
                case BinaryType.SpawnPlayer:
                    {
                        objectList.Add(new SpawnPlayer(DeserializeString(), DeserializeString(), DeserializeInt(), DeserializeInt(), DeserializeByte(), DeserializeByte(), DeserializeByte(), DeserializeByte(), DeserializeInt(), DeserializeInt(), DeserializeByte(), DeserializeInt(), DeserializeInt()));

                        break;
                    }
                case BinaryType.Disconnect:
                    {
                        objectList.Add(new Disconnect(DeserializeString()));

                        break;
                    }
                case BinaryType.Death:
                    {
                        if (DeserializeByte() == 1)
                            objectList.Add(new Death(DeserializeString(), DeserializeInt(), DeserializeInt(), DeserializeString()));
                        else objectList.Add(new Death(DeserializeString()));

                        break;
                    }
                case BinaryType.PlayerInfo:
                    {
                        objectList.Add(new PlayerInfo(DeserializeString(), new Vector2(DeserializeFloat(), DeserializeFloat()), DeserializeFloat(), DeserializeInt(), new Vector2(DeserializeFloat(), DeserializeFloat()), DeserializeString(), DeserializeInt(), DeserializeInt(), DeserializeInt(), DeserializeInt(), DeserializeByte(), DeserializeByte(), DeserializeByte(), DeserializeByte(), DeserializeInt(), DeserializeInt(), DeserializeInt(), DeserializeByte(), DeserializeByte(), DeserializeByte(), DeserializeByte()));

                        break;
                    }
            }
        }

        return objectList.ToArray();
    }

    public static int GetIntFromBytes(byte[] bytes)
    {
        int num = 0;

        int byteIndex = 0;

        for (int i = 3; i >= 0; i--)
            num += bytes[byteIndex++] << (i * 8);

        return num;
    }

    private static byte DeserializeByte()
    {
        return _bytes[_byteIndex++];
    }

    private static int DeserializeInt()
    {
        int num = 0;

        for (int i = 3; i >= 0; i--)
            num += _bytes[_byteIndex++] << (i * 8);

        return num;
    }

    private static unsafe float DeserializeFloat()
    {
        int val = DeserializeInt();

        return *(float*)&val;
    }

    private static string DeserializeString()
    {
        int size = DeserializeInt();

        string text = "";

        for (int i = 0; i < size; i++)
            text += ((char)_bytes[_byteIndex++]).ToString();

        return text;
    }
}
