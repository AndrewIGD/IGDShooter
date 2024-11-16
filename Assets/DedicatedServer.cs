using kcp2k;
using Mirror;
using UnityEngine;

public class DedicatedServer : MonoBehaviour
{
    private NetworkManager networkManager;
    
    void Start()
    {
        CreateServer();
    }

    void CreateServer()
    {
        var networkManagerObj = new GameObject("NetworkManager");
        networkManagerObj.transform.SetParent(transform);
    }
}
