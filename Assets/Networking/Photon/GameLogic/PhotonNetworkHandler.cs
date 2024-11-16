using System;
using UnityEngine;
using Photon.Bolt;
using Photon.Bolt.Matchmaking;
using UdpKit;
using System.Collections.Generic;
using UdpKit.Platform;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using Photon.Bolt.Utils;

public class PhotonNetworkHandler : GlobalEventListener, INetworkHandler
{
    public bool IsReady() => BoltNetwork.UdpSocket != null;

    private Dictionary<BoltConnection, string> messagesFromClients = new Dictionary<BoltConnection, string>();

    private Dictionary<PacketData, string> packets = new Dictionary<PacketData, string>();

    private int _packetID = 0;

    private string messageFromServer;

    public void Init()
    {
        BoltLauncher.SetUdpPlatform(new PhotonPlatform());

        BoltLauncher.StartClient(GetConfig());
    }

    public void Host()
    {
        BoltLauncher.SetUdpPlatform(new PhotonPlatform());

        BoltLauncher.StartServer(GetConfig());
    }

    public void Join(SessionData sessionData)
    {
        BoltMatchmaking.JoinSession((sessionData as BoltSessionData).udpSession);
    }

    public void Send(byte[] message)
    {
        if (BoltNetwork.IsConnected == false)
            return;
        if (message == null)
            return;
        if (message.Length == 0)
            return;

        /*if(BoltNetwork.IsClient)
            BoltNetwork.Server.StreamBytes(Data, message);
        else
        {
            BoltConnection[] clients = BoltNetwork.Clients.ToArray();

            for(int i=0;i<clients.Length;i++)
                clients[i].StreamBytes(Data,message);
        }*/

        int currentLen = 0;
        int stringLengthLimit = 140;

        int section = 1;
        int sectionCount = (int)Mathf.Ceil(message.Length / (float)stringLengthLimit);

        string stringFromBytes = Convert.ToBase64String(message);

        while (currentLen < stringFromBytes.Length)
        {
            string content = stringFromBytes.Substring(currentLen, Mathf.Min(stringLengthLimit, stringFromBytes.Length - currentLen));
            currentLen += stringLengthLimit;

            bool isFinal = currentLen >= stringFromBytes.Length;

            if (BoltNetwork.IsServer)
            {
                ServerPacket packet = ServerPacket.Create(targets: GlobalTargets.Others, ReliabilityModes.ReliableOrdered);
                packet.Content = content;
                packet.PacketID = _packetID;
                packet.IsFinal = isFinal;
                packet.Send();
            }
            else
            {
                ClientPacket packet = ClientPacket.Create(targets: GlobalTargets.OnlyServer, ReliabilityModes.ReliableOrdered);
                packet.Content = content;
                packet.PacketID = _packetID;
                packet.IsFinal = isFinal;
                packet.Send();
            }
        }

        _packetID++;
    }

    public override void OnEvent(ClientPacket packet)
    {
        PacketData data = new PacketData(packet.PacketID, packet.RaisedBy);

        if (packets.ContainsKey(data) == false)
            packets[data] = packet.Content;
        else
            packets[data] += packet.Content;

        if (packet.IsFinal)
        {
            try
            {
                string message = packets[data];

                packets.Remove(data);

                byte[] bytes = Convert.FromBase64String(message);

                Process(bytes.Skip(4).ToArray(), packet.RaisedBy.RemoteEndPoint.SteamId.Id.ToString());
            }
            catch
            {

            }

            messagesFromClients[packet.RaisedBy] = "";
        }
    }

    public override void OnEvent(ServerPacket packet)
    {
        PacketData data = new PacketData(packet.PacketID, packet.RaisedBy);

        if (packets.ContainsKey(data) == false)
            packets[data] = packet.Content;
        else
            packets[data] += packet.Content;

        if (packet.IsFinal)
        {
            try
            {
                string message = packets[data];

                packets.Remove(data);

                byte[] bytes = Convert.FromBase64String(message);

                Process(bytes.Skip(4).ToArray(), packet.RaisedBy.RemoteEndPoint.SteamId.Id.ToString());
            }
            catch
            {

            }

            messageFromServer = "";
        }
    }

    public void Process(byte[] message, string senderId)
    {
        Network.Instance.Process(message, senderId);
    }

    //-----BOLT API------

    /*private const string DataChannelName = "Data";

    private static UdpKit.UdpChannelName Data;

    public override void BoltStartBegin()
    {
        Data = BoltNetwork.CreateStreamChannel(DataChannelName, UdpKit.UdpChannelMode.Reliable, 4);
    }

    public override void StreamDataReceived(BoltConnection connection, UdpStreamData data)
    {
        Process(data.Data.Skip(4).ToArray(), connection.RemoteEndPoint.SteamId.Id.ToString());
    }*/

    public override void BoltStartDone()
    {
        if (BoltNetwork.IsServer)
        {
            string matchName = Guid.NewGuid().ToString();

            BoltMatchmaking.CreateSession(
                sessionID: matchName
            );
        }
    }

    public override void SessionConnected(UdpSession session, IProtocolToken token)
    {
        OnJoinedServer();
    }

    public override void Connected(BoltConnection connection)
    {
        OnClientConnected(connection.RemoteEndPoint.SteamId.Id.ToString());

        //connection.SetStreamBandwidth(Network.Instance.MTU);
    }

    public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
    {
        List<SessionData> sessions = new List<SessionData>();

        foreach (var session in sessionList)
        {
            UdpSession photonSession = session.Value as UdpSession;

            if (photonSession.Source == UdpSessionSource.Photon)
            {
                sessions.Add(new BoltSessionData(photonSession.HostName, photonSession.ConnectionsCurrent, photonSession.ConnectionsMax, photonSession));
            }
        }

        Network.Instance.UpdateSessionList(sessions);
    }

    public override bool PersistBetweenStartupAndShutdown()
    {
        return true;
    }

    private BoltConfig GetConfig()
    {
        var config = BoltRuntimeSettings.instance.GetConfigCopy();

        config.serverConnectionLimit = 32;

        return config;
    }

    public void OnClientConnected(string id)
    {
        Network.Instance.InvokeClientConnected(id);
    }

    public void OnJoinedServer()
    {
        Network.Instance.InvokeJoinedServer();
    }

    public string GetSelfID()
    {
        return BoltNetwork.UdpSocket.WanEndPoint.SteamId.Id.ToString();
    }

    public string[] GetConnectionIDs()
    {
        BoltConnection[] connections = BoltNetwork.Connections.ToArray();

        string[] arr = new string[connections.Length];

        for (int i = 0; i < arr.Length; i++)
            arr[i] = connections[i].RemoteEndPoint.SteamId.Id.ToString();

        return arr;
    }

    public bool HasConnections()
    {
        return BoltNetwork.Connections.Count() != 0;
    }

    public void Stop()
    {
        BoltNetwork.Shutdown();
    }

    public int ConnectionCount()
    {
        return BoltNetwork.Connections.Count();
    }

    public int Ping()
    {
        if (BoltNetwork.IsServer)
            return 0;

        return (int)(BoltNetwork.Server.PingNetwork * 1000);
    }

    public static string ByteArrayToString(byte[] ba)
    {
        StringBuilder hex = new StringBuilder(ba.Length * 2);
        foreach (byte b in ba)
            hex.AppendFormat("{0:x2}", b);
        return hex.ToString();
    }

    public static byte[] StringToByteArray(String hex)
    {
        int NumberChars = hex.Length;
        byte[] bytes = new byte[NumberChars / 2];
        for (int i = 0; i < NumberChars; i += 2)
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        return bytes;
    }

    public void SendTo(string id, byte[] message)
    {
        if (BoltNetwork.IsConnected == false)
            return;
        if (message == null)
            return;
        if (message.Length == 0)
            return;
        if (BoltNetwork.IsServer == false)
            return;

        int currentLen = 0;
        int stringLengthLimit = 140;

        int section = 1;
        int sectionCount = (int)Mathf.Ceil(message.Length / (float)stringLengthLimit);

        string stringFromBytes = Convert.ToBase64String(message);

        while (currentLen < stringFromBytes.Length)
        {
            string content = stringFromBytes.Substring(currentLen, Mathf.Min(stringLengthLimit, stringFromBytes.Length - currentLen));
            currentLen += stringLengthLimit;

            bool isFinal = currentLen >= stringFromBytes.Length;

            BoltConnection[] connections = BoltNetwork.Connections.ToArray();

            for (int i = 0; i < connections.Length; i++)
                if (id == connections[i].RemoteEndPoint.SteamId.Id.ToString())
                {
                    ServerPacket packet = ServerPacket.Create(connections[i], ReliabilityModes.ReliableOrdered);
                    packet.Content = content;
                    packet.PacketID = _packetID;
                    packet.IsFinal = isFinal;
                    packet.Send();

                    break;
                }
        }

        _packetID++;
    }

    public void JoinIP(string ip)
    {
        return;
    }

    public bool ShouldSelfConnect()
    {
        return true;
    }

    public bool HasManualProcessing() => false;
}

public class BoltSessionData : SessionData
{
    public UdpSession udpSession;

    public BoltSessionData(string roomName, int playerCount, int maxPlayerCount, UdpSession udpSession) : base(roomName, playerCount, maxPlayerCount)
    {
        this.udpSession = udpSession;
    }
}

public struct PacketData
{
    public PacketData(int ID, BoltConnection connection)
    {
        this.ID = ID;
        Connection = connection;
    }

    public int ID;
    public BoltConnection Connection;
}