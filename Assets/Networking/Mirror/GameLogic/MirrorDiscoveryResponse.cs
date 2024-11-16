using System;
using System.Net;
using Mirror;

public struct MirrorDiscoveryResponse : NetworkMessage
{
    public IPEndPoint EndPoint { get; set; }
    
    public Uri uri;

    public int CurrentPlayers;
    public int TotalPlayers;
    public string RoomName;
    
    public long serverId;
}