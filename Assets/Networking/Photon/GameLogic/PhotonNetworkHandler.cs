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
    private Dictionary<BoltConnection, string> messagesFromClients = new Dictionary<BoltConnection, string>();

    private string messageFromServer;

    public void Init()
    {
        BoltLauncher.SetUdpPlatform(new PhotonPlatform());

        BoltLauncher.StartClient(GetConfig());
    }

    public void Host()
    {
        BoltLauncher.Shutdown();

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

        while (currentLen < message.Length)
        {
            string content = stringFromBytes.Substring(currentLen, Mathf.Min(currentLen + stringLengthLimit, stringFromBytes.Length));
            bool isFinal = section++ == sectionCount;
            currentLen += stringLengthLimit;

            if (BoltNetwork.IsServer)
            {
                ServerPacket packet = ServerPacket.Create(targets: GlobalTargets.Others);
                packet.Content = content;
                packet.IsFinal = isFinal;
                packet.Send();
            }
            else
            {
                ClientPacket packet = ClientPacket.Create(targets: GlobalTargets.OnlyServer);
                packet.Content = content;
                packet.IsFinal = isFinal;
                packet.Send();
            }
        }
    }

    public override void OnEvent(ClientPacket packet)
    {
        if (messagesFromClients.ContainsKey(packet.RaisedBy) == false)
            messagesFromClients.Add(packet.RaisedBy, packet.Content);
        else messagesFromClients[packet.RaisedBy] += packet.Content;

        if (packet.IsFinal)
        {
            string message = messagesFromClients[packet.RaisedBy];

            byte[] bytes = Convert.FromBase64String(message);

            if (bytes.Length > 4)
            {
                string msg = "Received ";

                for (int i = 0; i < bytes.Length; i++)
                    msg += bytes[i] + " ";

                Debug.Log(msg);
            }

            Process(bytes.Skip(4).ToArray(), packet.RaisedBy.RemoteEndPoint.SteamId.Id.ToString());

            messagesFromClients[packet.RaisedBy] = "";
        }
    }

    public override void OnEvent(ServerPacket packet)
    {
        messageFromServer += packet.Content;

        if (packet.IsFinal)
        {
            string message = messageFromServer;

            byte[] bytes = Convert.FromBase64String(message);

            if (bytes.Length > 4)
            {
                string msg = "Received ";

                for (int i = 0; i < bytes.Length; i++)
                    msg += bytes[i] + " ";

                Debug.Log(msg);
            }

            Process(bytes.Skip(4).ToArray(), packet.RaisedBy.RemoteEndPoint.SteamId.Id.ToString());

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

        //RoomListManager.Instance.UpdateRoomList(sessions);
    }

    public override bool PersistBetweenStartupAndShutdown()
    {
        return true;
    }

    private BoltConfig GetConfig()
    {
        var config = BoltRuntimeSettings.instance.GetConfigCopy();

        config.serverConnectionLimit = 2;

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
}

public class BoltSessionData : SessionData
{
    public UdpSession udpSession;

    public BoltSessionData(string roomName, int playerCount, int maxPlayerCount, UdpSession udpSession) : base(roomName, playerCount, maxPlayerCount)
    {
        this.udpSession = udpSession;
    }
}
