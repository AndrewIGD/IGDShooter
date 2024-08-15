using System.Collections.Generic;

internal interface INetworkHandler
{
    public bool HasConnections();

    public void Init();

    public void Host();

    public void Join(SessionData sessionData);

    public void Send(byte[] message);

    public void Process(byte[] message, string senderId);

    public void OnClientConnected(string id);

    public void OnJoinedServer();

    public string GetSelfID();

    public int ConnectionCount();

    public string[] GetConnectionIDs();

    public int Ping();

    public void Stop();

    public void SendTo(string id, byte[] bytes);

    public bool IsReady();
    public void JoinIP(string ip);
}