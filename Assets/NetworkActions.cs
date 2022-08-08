using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkActions : MonoBehaviour
{
    public void Host()
    {
        Network.Instance.SetNetworkingLibrary(MatchData.NetworkingLibrary);

        Network.Instance.Host();

        SceneManager.LoadScene("OnlineWaitMenu");
    }

    public void Join()
    {
        return;
    }

    public void BrowseRoomsScene()
    {
        Network.Instance.SetNetworkingLibrary(MatchData.NetworkingLibrary);

        SceneManager.LoadScene("BrowseRooms");
    }
}
