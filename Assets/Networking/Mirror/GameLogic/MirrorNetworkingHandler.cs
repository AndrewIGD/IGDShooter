using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using kcp2k;
using Mirror;
using UnityEngine;

public class MirrorNetworkingHandler : MonoBehaviour, INetworkHandler
{
    private MirrorNetworkManager networkManager;

    private List<SessionData> _sessionList = new List<SessionData>();

    private MirrorDiscovery _discovery;

    public async void Init()
    {
        Stop();
        
        await CreateNetworkManager();
        
        CreateDiscovery();
        
        _discovery.StartDiscovery();
    }

    private void OnServerDiscoveryResponse(MirrorDiscoveryResponse response)
    {
        _sessionList.Add(new MirrorSessionData(response.RoomName, response.CurrentPlayers, response.TotalPlayers, response.EndPoint));

        Network.Instance.UpdateSessionList(_sessionList);
    }

    private MirrorDiscoveryResponse CreateServerDiscoveryResponse()
    {
        return new MirrorDiscoveryResponse
        {
            RoomName = Guid.NewGuid().ToString(),
            CurrentPlayers = NetworkServer.connections.Count,
            TotalPlayers = NetworkServer.maxConnections,
        };
    }

    public async void Host()
    {
        await CreateNetworkManager();
        
        Stop();

        networkManager.networkAddress = "localhost";

        networkManager.StartHost();
        
        CreateDiscovery();
        
        _discovery.AdvertiseServer();
    }

    public void Join(SessionData sessionData)
    {
        var mirrorSessionData = (MirrorSessionData)sessionData;
        
        JoinIP(mirrorSessionData.endPoint.PrettyAddress());
    }
    
    public async void JoinIP(string ip)
    {
        Stop();
        
        await CreateNetworkManager();
        
        networkManager.networkAddress = ip;
        
        networkManager.StartClient();
    }

    public void Send(byte[] message)
    {
        if (message == null)
            return;
        
        if (message.Length == 0)
            return;
        
        MirrorByteArray byteArray = new MirrorByteArray(message);

        if (NetworkServer.active)
        {
            NetworkServer.SendToAll(byteArray);
            return;
        }

        NetworkClient.Send(byteArray);
    }
    
    public void SendTo(string id, byte[] message)
    {
        if (message == null)
            return;
        
        if (message.Length == 0)
            return;
        
        if (!NetworkServer.active)
            return;
        
        for (int i = 0; i < NetworkServer.connections.Count; i++)
            if (NetworkServer.connections.Values.ElementAt(i).connectionId.ToString() == id)
            {
                MirrorByteArray byteArray = new MirrorByteArray(message);
                NetworkServer.connections.Values.ElementAt(i).Send(byteArray);
                return;
            }
    }

    public void Process(byte[] message, string senderId)
    {
        Network.Instance.Process(message, senderId);
    }

    public void OnClientConnected(string id)
    {
        Network.Instance.InvokeClientConnected(id);
    }

    public void OnJoinedServer()
    {
        Network.Instance.InvokeJoinedServer();
    }

    private void CreateDiscovery()
    {
        if(_discovery != null)
            return;
        
        var discoveryObj = new GameObject("Discovery");
        discoveryObj.transform.SetParent(transform);
        
        _discovery = discoveryObj.AddComponent<MirrorDiscovery>();
        _discovery.BroadcastAddress = "127.0.0.1";
        _discovery.OnServerFound.AddListener(OnServerDiscoveryResponse);
        _discovery.ServerDiscoveryHandler = CreateServerDiscoveryResponse;
    }

    private async Task CreateNetworkManager()
    {
        if(networkManager != null)
            return;
        
        var networkManagerObj = new GameObject("NetworkManager");
        networkManagerObj.transform.SetParent(transform);
        
        var transport = networkManagerObj.AddComponent<ThreadedKcpTransport>();
        
        networkManager = networkManagerObj.AddComponent<MirrorNetworkManager>();
        networkManager.transport = transport;
        networkManager.autoCreatePlayer = false;
        
        networkManager.OnJoinedServer += () => OnJoinedServer();
        networkManager.OnClientConnected += conn => OnClientConnected(conn.connectionId.ToString());

        await Task.Delay(0);
        
        NetworkClient.RegisterHandler<MirrorByteArray>(OnClientReceiveMessage);
        NetworkServer.RegisterHandler<MirrorByteArray>(OnServerReceiveMessage);
    }

    private void OnClientReceiveMessage(MirrorByteArray obj)
    {
        Process(obj.data, NetworkClient.connection.connectionId.ToString());
    }
    
    private void OnServerReceiveMessage(NetworkConnectionToClient conn, MirrorByteArray obj)
    {
        Process(obj.data, conn.connectionId.ToString());
    }

    public void Stop()
    {
        if (NetworkManager.singleton == null)
            return;
        
        if (NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopClient();
        }

        if (NetworkServer.active)
        {
            NetworkManager.singleton.StopHost();
        }
    }
    
    public string GetSelfID()
    {
        return NetworkClient.connection.connectionId.ToString();
    }

    public int ConnectionCount()
    {
        if (!NetworkServer.active)
            return 1;
        
        return NetworkServer.connections.Count - 1;
    }

    public string[] GetConnectionIDs()
    {
        if (!NetworkServer.active)
            return new string[1] { NetworkClient.connection.connectionId.ToString() };
        
        var connections = new string[NetworkServer.connections.Count];

        for (int i = 0; i < NetworkServer.connections.Count; i++)
        {
            connections[i] = NetworkServer.connections.Values.ElementAt(i).connectionId.ToString();
            Debug.Log(connections[i]);
        }

        return connections;
    }
    
    public bool IsReady()
    {
        return true;
    }
    
    public bool HasConnections()
    {
        if (!NetworkServer.active)
            return NetworkClient.isConnected;
        
        return NetworkServer.connections.Count > 0;
    }
    
    public int Ping()
    {
        return Mathf.RoundToInt((float)NetworkTime.rtt * 1000);
    }
}

public class MirrorSessionData : SessionData
{
    public IPEndPoint endPoint;

    public MirrorSessionData(string roomName, int playerCount, int maxPlayerCount, IPEndPoint endPoint) : base(roomName, playerCount, maxPlayerCount)
    {
        this.endPoint = endPoint;
    }
}