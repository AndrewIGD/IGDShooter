using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class Network : MonoBehaviour
{
    public int PacketSize;

    public float UpdateRate;

    public bool IsHost = false;

    public bool HasConnections => _networkHandler.HasConnections();
    public int ConnectionCount => _networkHandler.ConnectionCount();
    public int Ping => _networkHandler.Ping();

    public int MTU => (int)Mathf.Ceil(PacketSize * UpdateRate);

    public delegate void MessagesReceived(object[] objects, string senderId);

    public event MessagesReceived OnMessagesReceived = delegate { };

    public delegate void ClientConnected(string id);

    public event ClientConnected OnClientConnected = delegate { };

    public delegate void JoinedServer();

    public event JoinedServer OnJoinedServer = delegate { };

    public delegate void SessionListUpdate(List<SessionData> sessionList);

    public event SessionListUpdate OnSessionListUpdated = delegate { };


    [HideInInspector]
    public NetworkingLibrary _library;

    public static Network Instance;

    private INetworkHandler _networkHandler;

    private float _timeStep;

    private List<object> _objects = new List<object>();

    JsonSerializerSettings serializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };

    public void UpdateSessionList(List<SessionData> sessionList)
    {
        OnSessionListUpdated.Invoke(sessionList);
    }

    public void Shutdown()
    {
        _networkHandler.Stop();
    }

    public void ForceSendPackets()
    {
        byte[] bytes = BinarySerializer.Get();

        BinarySerializer.Clear();

        _networkHandler.Send(bytes);
    }

    public void Host()
    {
        IsHost = true;

        _networkHandler.Host();

        StartCoroutine(MessageLoop());
    }

    public IEnumerator SelfConnect()
    {
        yield return new WaitUntil(() => _networkHandler.IsReady());

        OnClientConnected.Invoke(_networkHandler.GetSelfID());
    }

    public void Join(SessionData sessionData)
    {
        _networkHandler.Join(sessionData);
    }

    public void JoinIP(string ip)
    {
        _networkHandler.JoinIP(ip);

        StartCoroutine(MessageLoop());
    }

    public void Send(object obj)
    {
        BinarySerializer.Add(obj);

        if (IsHost && !_networkHandler.HasManualProcessing())
            ProcessObject(obj, _networkHandler.GetSelfID()); // The host doesn't have a client processing this data, so might as well just do it here
    }

    public void SendTo(string id, object obj)
    {
        if (id == _networkHandler.GetSelfID() && !_networkHandler.HasManualProcessing())
            ProcessObject(obj, _networkHandler.GetSelfID());

        _networkHandler.SendTo(id, BinarySerializer.GetDirect(obj));
    }

    public void Process(byte[] message, string senderId)
    {
        object[] objects = BinaryDeserializer.Deserialize(message);
        
        OnMessagesReceived.Invoke(objects, senderId);
    }

    public void ProcessObject(object obj, string senderId)
    {
        OnMessagesReceived.Invoke(new object[1] { obj }, senderId);
    }

    public void InvokeClientConnected(string id)
    {
        OnClientConnected.Invoke(id);
    }

    public void InvokeJoinedServer()
    {
        OnJoinedServer.Invoke();
    }

    public void SetNetworkingLibrary(NetworkingLibrary library)
    {
        _library = library;

        GameObject networkHandler = new GameObject();

        switch (_library)
        {
            case NetworkingLibrary.Lidgren:
                {
                    _networkHandler = networkHandler.AddComponent<LidgrenNetworkHandler>();
                    networkHandler.name = "Lidgren";

                    break;
                }
            
            case NetworkingLibrary.LiteNetLib:
            {
                _networkHandler = networkHandler.AddComponent<LiteNetLibNetworkingHandler>();
                networkHandler.name = "LiteNetLib";

                break;
            }
        }

        DontDestroyOnLoad(networkHandler);
    }

    public void StartClient()
    {
        _networkHandler.Init();

        StartCoroutine(MessageLoop());
    }

    public string GetSelfID() => _networkHandler.GetSelfID();

    public string[] GetConnectionIDs() => _networkHandler.GetConnectionIDs();

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        _timeStep = 1 / UpdateRate;
    }

    private IEnumerator MessageLoop()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(_timeStep);
            
            ForceSendPackets();
        }
    }
}