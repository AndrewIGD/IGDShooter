using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkActions : MonoBehaviour
{
    public void Host()
    {
        if (Directory.Exists(Application.dataPath + "\\Maps\\" + GameHost.Map) == false)
            return;
        
        Network.Instance.SetNetworkingLibrary(MatchData.NetworkingLibrary);

        Network.Instance.Host();

        SceneManager.LoadScene("OnlineWaitMenu");
    }

    public void Join()
    {
        Network.Instance.SetNetworkingLibrary(MatchData.NetworkingLibrary);

        GameObject obj = new GameObject();

        obj.AddComponent<GameClient>().Initialize();
        obj.name = "GameClient";

        Network.Instance.JoinIP(GameObject.Find("JoinInput").GetComponent<InputField>().text);
    }

    public void BrowseRoomsScene()
    {
        Network.Instance.SetNetworkingLibrary(MatchData.NetworkingLibrary);

        GameObject obj = new GameObject();

        obj.AddComponent<GameClient>().Initialize();
        obj.name = "GameClient";

        SceneManager.LoadScene("BrowseRooms");
    }
}
