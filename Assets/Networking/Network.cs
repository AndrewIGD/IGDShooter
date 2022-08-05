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

    public int MTU => (int)Mathf.Ceil(PacketSize * UpdateRate);

    public delegate void MessagesReceived(object[] objects, string senderId);

    public event MessagesReceived OnMessagesReceived = delegate { };

    public delegate void ClientConnected(string id);

    public event ClientConnected OnClientConnected = delegate { };

    public delegate void JoinedServer();

    public event JoinedServer OnJoinedServer = delegate { };


    [HideInInspector]
    public NetworkingLibrary _library;

    public static Network Instance;

    private INetworkHandler _networkHandler;

    private float _timeStep;

    private List<object> _objects = new List<object>();

    JsonSerializerSettings serializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };

    public void Host()
    {
        IsHost = true;

        _networkHandler.Host();
    }

    public void Join(SessionData sessionData)
    {
        _networkHandler.Join(sessionData);
    }

    public void Send(object obj)
    {
        BinarySerializer.Add(obj);

        if (IsHost)
            ProcessObject(obj, _networkHandler.GetSelfID()); // The host doesn't have a client processing this data, so might as well just do it here
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

    public void SetNetworkingLibrary(NetworkingLibrary library) => _library = library;

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

            byte[] bytes = BinarySerializer.Get();

            BinarySerializer.Clear();

            _networkHandler.Send(bytes);
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        if (level != 1) //Room Scene
            return;

        GameObject networkHandler = new GameObject();

        switch (_library)
        {
            case NetworkingLibrary.Bolt:
                {
                    _networkHandler = networkHandler.AddComponent<PhotonNetworkHandler>();
                    networkHandler.name = "Bolt";

                    break;
                }
            case NetworkingLibrary.Lidgren:
                {
                    _networkHandler = networkHandler.AddComponent<LidgrenNetworkHandler>();
                    networkHandler.name = "Lidgren";

                    break;
                }
        }

        _networkHandler.Init();

        DontDestroyOnLoad(networkHandler);

        StartCoroutine(MessageLoop());
    }
}