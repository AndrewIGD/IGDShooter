using System.Collections.Generic;

internal interface INetworkHandler
{
    public void Init();

    public void Host();

    public void Join(SessionData sessionData);

    public void Send(byte[] message);

    public void Process(byte[] message, string senderId);

    public void OnClientConnected(string id);

    public void OnJoinedServer();

    public string GetSelfID();

    public string[] GetConnectionIDs();
}