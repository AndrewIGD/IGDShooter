using System;
using Mirror;

public class MirrorNetworkManager : NetworkManager
{
    public Action OnJoinedServer;
    public Action<NetworkConnectionToClient> OnClientConnected;
    
    public override void OnClientConnect()
    {
        base.OnClientConnect();
        
        OnJoinedServer?.Invoke();
    }
    
    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn);
        
        OnClientConnected?.Invoke(conn);
    }
}
