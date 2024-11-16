using Lidgren.Network;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class LidgrenNetworkHandler : MonoBehaviour, INetworkHandler
{
    public bool IsReady() => true;

    private NetClient _client;
    private NetServer _server;

    private const int Port = 25565;

    private List<SessionData> _sessionList = new List<SessionData>();

    public void Init()
    {
        _client = new NetClient(GetClientConfig());
        _client.Start();

        _client.DiscoverLocalPeers(Port);

        StartCoroutine(MessageCheck());
    }

    public void Host()
    {
        if (_client != null)
            _client.Shutdown("Kaput");
        _client = null;

        _server = new NetServer(GetServerConfig());
        _server.Start();

        StartCoroutine(MessageCheck());
    }

    public void Join(SessionData sessionData)
    {
        _client.Connect((sessionData as LidgrenSessionData).endPoint);

        OnJoinedServer();
    }

    public void Send(byte[] message)
    {
        if (message == null)
            return;
        if (message.Length == 0)
            return;

        if (_client != null)
        {
            if (_client.ConnectionStatus != NetConnectionStatus.Connected)
                return;

            var outGoingMessage = _client.CreateMessage();
            outGoingMessage.Write(message);

            _client.SendMessage(outGoingMessage, NetDeliveryMethod.ReliableOrdered);
        }
        else
        {
            if (_server.ConnectionsCount == 0)
                return;

            var outGoingMessage = _server.CreateMessage();
            outGoingMessage.Write(message);
            _server.SendMessage(outGoingMessage, _server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
        }
    }

    public void Process(byte[] message, string senderId)
    {
        Network.Instance.Process(message, senderId);
    }

    private IEnumerator MessageCheck()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(1f / Network.Instance.UpdateRate);

            List<NetIncomingMessage> messages = new List<NetIncomingMessage>();

            if (_client != null)
                _client.ReadMessages(messages);
            else _server.ReadMessages(messages);

            if (messages.Count != 0)
            {
                for (int i = 0; i < messages.Count; i++)
                {
                    try
                    {
                        NetIncomingMessage message = messages[i];

                        switch (message.MessageType)
                        {
                            case NetIncomingMessageType.DiscoveryRequest:
                                {
                                    Debug.Log("Discovery Request");

                                    NetOutgoingMessage response = _server.CreateMessage();
                                    response.Write(Guid.NewGuid().ToString() + " " + (_server.ConnectionsCount + 1) + " " + _server.Configuration.MaximumConnections);

                                    // Send the response to the sender of the request
                                    _server.SendDiscoveryResponse(response, message.SenderEndPoint);

                                    break;
                                }

                            case NetIncomingMessageType.DiscoveryResponse:
                                {
                                    Debug.Log("Received");

                                    string data = message.ReadString();

                                    string[] parameters = data.Split(' ');

                                    _sessionList.Add(new LidgrenSessionData(parameters[0], int.Parse(parameters[1]), int.Parse(parameters[2]), message.SenderEndPoint));

                                    Network.Instance.UpdateSessionList(_sessionList);

                                    break;
                                }

                            case NetIncomingMessageType.StatusChanged:
                                {
                                    if(message.SenderConnection.Status == NetConnectionStatus.Connected)
                                        OnClientConnected(message.SenderConnection.RemoteUniqueIdentifier.ToString());

                                    break;
                                }

                            case NetIncomingMessageType.Data:
                                {
                                    List<byte> countBytes = new List<byte>();

                                    for (int j = 0; j < 4; j++)
                                        countBytes.Add(message.ReadByte());

                                    int count = BinaryDeserializer.GetIntFromBytes(countBytes.ToArray());

                                    byte[] bytes;

                                    message.ReadBytes(count, out bytes);

                                    Process(bytes, message.SenderConnection.RemoteUniqueIdentifier.ToString());

                                    break;
                                }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex);
                    }
                }
            }
        }
    }

    private NetPeerConfiguration GetClientConfig()
    {
        NetPeerConfiguration config = new NetPeerConfiguration("IGDShooter")
        {
            PingInterval = 1f,
            ResendHandshakeInterval = 1f,
            MaximumHandshakeAttempts = 15,
            ConnectionTimeout = 1000f,
            ReceiveBufferSize = Network.Instance.PacketSize,
            SendBufferSize = Network.Instance.PacketSize
        };

        config.EnableMessageType(NetIncomingMessageType.StatusChanged);
        config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);

        return config;
    }

    private NetPeerConfiguration GetServerConfig()
    {
        NetPeerConfiguration config = new NetPeerConfiguration("IGDShooter")
        {
            Port = Port,
            PingInterval = 1f,
            ResendHandshakeInterval = 1f,
            MaximumHandshakeAttempts = 15,
            ConnectionTimeout = 1000f,
            ReceiveBufferSize = Network.Instance.PacketSize,
            SendBufferSize = Network.Instance.PacketSize
        };

        config.EnableMessageType(NetIncomingMessageType.StatusChanged);
        config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);

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
        if (_server != null)
            return _server.UniqueIdentifier.ToString();
        else return _client.UniqueIdentifier.ToString();
    }

    public string[] GetConnectionIDs()
    {
        if (_server != null)
        {
            string[] arr = new string[_server.ConnectionsCount];

            for (int i = 0; i < arr.Length; i++)
                arr[i] = _server.Connections[i].RemoteUniqueIdentifier.ToString();

            return arr;
        }
        else
        {
            string[] arr = new string[_client.ConnectionsCount];

            for (int i = 0; i < arr.Length; i++)
                arr[i] = _client.Connections[i].RemoteUniqueIdentifier.ToString();

            return arr;
        }
    }

    public bool HasConnections()
    {
        if (_server != null)
            return _server.ConnectionsCount != 0;

        return _client.ConnectionsCount != 0;
    }

    public void Stop()
    {
        if (_server != null)
        {
            _server.Shutdown("Shutdown");
        }
        else _client.Shutdown("Shutdown");
    }

    public int ConnectionCount()
    {
        if (_server != null)
            return _server.ConnectionsCount;

        return _client.ConnectionsCount;
    }

    public int Ping()
    {
        if (_server != null)
            return 0;

        return (int)(_client.ServerConnection.AverageRoundtripTime * 1000);
    }

    public void SendTo(string id, byte[] bytes)
    {
        if (_server == null)
            return;

        for (int i = 0; i < _server.ConnectionsCount; i++)
        {
            if(id == _server.Connections[i].RemoteUniqueIdentifier.ToString())
            {
                var outGoingMessage = _server.CreateMessage();
                outGoingMessage.Write(bytes);

                _server.SendMessage(outGoingMessage, _server.Connections[i], NetDeliveryMethod.ReliableOrdered);

                return;
            }
        }
    }

    public void JoinIP(string ip)
    {
        _client = new NetClient(GetClientConfig());
        _client.Start();

        _client.Connect(ip, Port);

        StartCoroutine(MessageCheck());
    }
}

public class LidgrenSessionData : SessionData
{
    public IPEndPoint endPoint;

    public LidgrenSessionData(string roomName, int playerCount, int maxPlayerCount, IPEndPoint endPoint) : base(roomName, playerCount, maxPlayerCount)
    {
        this.endPoint = endPoint;
    }
}

