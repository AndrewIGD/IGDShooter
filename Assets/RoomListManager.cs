using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomListManager : MonoBehaviour
{
    [SerializeField] Transform parent;
    [SerializeField] GameObject roomPrefab;

    void Start()
    {
        Network.Instance.OnSessionListUpdated += UpdateList;

        Network.Instance.StartClient();
    }

    private void UpdateList(List<SessionData> sessionList)
    {
        for (int i = 0; i < sessionList.Count; i++)
        {
            GameObject room = Instantiate(roomPrefab, parent);

            room.GetComponent<Room>().Setup(sessionList[0]);
        }
    }
}
