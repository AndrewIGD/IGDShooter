using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

public class Buy : MonoBehaviour
{
    [SerializeField] int type;
    [SerializeField] int price;

    private Player _player;

    public void AssignPlayer(Player player) => _player = player;

    public void BuyItem()
    {
        GameClient.Instance.Send("Process " + _player.ID.ToString(CultureInfo.InvariantCulture) + " " + type.ToString(CultureInfo.InvariantCulture) + "\n");

        SoundArchive.Instance.Play(22, _player.transform);
    }
}
