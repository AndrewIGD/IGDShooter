using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Globalization;
using System;

public class Hitbox : MonoBehaviour
{
    [SerializeField] bool destroyOnHit = true;
    [SerializeField] float _damage;

    public int Team => _team;

    private GameObject _parent;

    private int _team;

    private int _type;

    private string _name;

    private string _playerNumber = "";

    private Player _parentScript;

    public void Setup(int team, float damage, int type, GameObject parent)
    {
        _team = team;
        _damage = damage;
        _type = type;
        _parent = parent;
    }

    public void KnifeSetup(int team, GameObject parent) { _team = team; _parent = parent; }

    public void Destroy()
    {
        if (destroyOnHit)
        {
            Network.Instance.Send(new Destroy(transform.name));
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (_parent != null)
        {
            _parentScript = _parent.GetComponent<Player>();

            _playerNumber = _parentScript.ID;
            _name = _parentScript.Name;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        Wall wall = collision.gameObject.GetComponent<Wall>();

        if (player != null)
        {
            if (player.Team != _team || (GameHost.Instance != null ? Config.FriendlyFire : false))
            {
                if (GameHost.Instance != null)
                {
                    player.DecreaseHp(_parentScript, _damage, _type);
                }

                Destroy();
            }
        }
        else if(wall != null)
        {
            if (GameHost.Instance != null)
                wall.DecreaseHp(_damage);

            Destroy();
        }
    }

    public void GetTeam(int team)
    {
        _team = team;
    }
}
