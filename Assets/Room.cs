using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI title, playerCount;

    private SessionData _session;

    public void Setup(SessionData session)
    {
        _session = session;

        title.text = _session.roomName;

        playerCount.text = _session.playerCount + "/" + _session.maxPlayerCount;
    }

    public void Join()
    {
        Network.Instance.Join(_session);
    }
}
