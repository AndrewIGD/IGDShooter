using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Lidgren.Network;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;
using NetPeer = LiteNetLib.NetPeer;

public class LiteNetLibNetworkingHandler : MonoBehaviour, INetworkHandler, INetEventListener
{
    public bool IsReady() => true;

    private const int Port = 11225;

    private List<SessionData> _sessionList = new List<SessionData>();

    private NetManager _manager;
    private Coroutine _broadcastCoroutine;
    private Coroutine _messageLoopCoroutine;

    private bool _isServer = false;
    
    private const float BroadcastInterval = 0.5f;
    private string Key;

    private const int ServerPeerID = 1024;

    private int _id = -1;

    public void Init()
    {
        Key = "IGDShooter " + Application.version;
        
        _manager = new NetManager(this)
        {
            UnconnectedMessagesEnabled = true,
            UpdateTime = (int)(1f / Network.Instance.UpdateRate * 1000)
        };

        _manager.Start();

        _broadcastCoroutine ??= StartCoroutine(BroadcastLoop());
        _messageLoopCoroutine ??= StartCoroutine(MessageLoop());
    }

    private IEnumerator BroadcastLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(BroadcastInterval);
            
            _manager.SendBroadcast(new byte[] {1}, 5000);
        }
    }

    private IEnumerator MessageLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f / Network.Instance.UpdateRate);

            if (_manager is { IsRunning: true })
            {
                _manager.PollEvents();
            }
        }
    }

    public void Host()
    {
        EnsureManagerStopped();

        _manager.BroadcastReceiveEnabled = true;
        _manager.Start(Port);

        _isServer = true;
    }

    public void Join(SessionData sessionData)
    {
        EnsureIsClient();
        
        var liteNetLibSessionData = sessionData as LiteNetLibSessionData;
        
        if(liteNetLibSessionData == null)
            return;
        
        _manager.Connect(liteNetLibSessionData.endPoint, Key);
    }
    
    public void JoinIP(string ip)
    {
        EnsureIsClient();
        
        _manager.Connect(ip, Port, Key);
    }

    private void EnsureManagerStopped()
    {
        if(_manager == null)
            Init();
        
        if (_manager is { IsRunning: true })
            _manager.Stop();
        
        if (_broadcastCoroutine != null)
        {
            StopCoroutine(_broadcastCoroutine);
            _broadcastCoroutine = null;
        }
    }
    
    private void EnsureIsClient()
    {
        EnsureManagerStopped();

        _manager.BroadcastReceiveEnabled = false;
        _manager.Start();

        _isServer = false;
    }
    
    public void OnClientConnected(string id)
    {
        Network.Instance.InvokeClientConnected(id);
    }

    public void OnJoinedServer()
    {
        Network.Instance.InvokeJoinedServer();
    }
    
    public void Stop()
    {
        if (_manager is { IsRunning: true })
            _manager.Stop();
    }

    public void Send(byte[] message)
    {
        if (message == null)
            return;
        
        if (message.Length == 0)
            return;

        if (!_isServer)
        {
            _manager.FirstPeer.Send(message, DeliveryMethod.ReliableOrdered);
            return;
        }
        
        _manager.SendToAll(message, DeliveryMethod.ReliableOrdered);
    }
    
    
    public void SendTo(string id, byte[] bytes)
    {
        if (bytes == null)
            return;
        
        if (bytes.Length == 0)
            return;
        
        if (!_isServer)
            return;

        var peer = _manager.ConnectedPeerList.FirstOrDefault();
        if (peer == null)
            return;
        
        peer.Send(bytes, DeliveryMethod.ReliableOrdered);
    }

    public void Process(byte[] message, string senderId)
    {
        Network.Instance.Process(message, senderId);
    }

    public string GetSelfID()
    {
        if (_isServer)
            return ServerPeerID.ToString();
        
        return _manager.FirstPeer.GetPeerID();
    }

    public string[] GetConnectionIDs()
    {
        return _manager.ConnectedPeerList.Select(x => x.GetPeerID()).ToArray();
    }

    public bool HasConnections()
    {
        return _manager.ConnectedPeerList.Count > 0;
    }

    public int ConnectionCount()
    {
        return _manager.ConnectedPeerList.Count;
    }

    public int Ping()
    {
        if (_isServer)
            return 0;
        
        return _manager.FirstPeer.Ping;
    }
    
    public bool HasManualProcessing() => false;

    #region LiteNetLib Interface
    public void OnPeerConnected(NetPeer peer)
    {
        if (!_isServer)
        {
            OnJoinedServer();
            return;
        }
        
        OnClientConnected(peer.GetPeerID());
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        return;
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
    {
        return;
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
    {
        var bytes = new byte[reader.UserDataSize];
        Array.Copy(reader.RawData, reader.UserDataOffset, bytes, 0, reader.UserDataSize);
        Process(bytes, peer.GetPeerID());
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {
        return;
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
        return;
    }

    public void OnConnectionRequest(ConnectionRequest request)
    {
        if (!_isServer)
        {
            request.Reject();
            return;
        }

        request.AcceptIfKey(Key);
    }
    #endregion
}


public class LiteNetLibSessionData : SessionData
{
    public IPEndPoint endPoint;

    public LiteNetLibSessionData(string roomName, int playerCount, int maxPlayerCount, IPEndPoint endPoint) : base(roomName, playerCount, maxPlayerCount)
    {
        this.endPoint = endPoint;
    }
}
