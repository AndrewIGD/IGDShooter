using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

public class Wall : MonoBehaviour
{
    [SerializeField] float _health;

    public SpriteRenderer SpriteRenderer => _spriteRenderer;

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void DecreaseHp(float damage)
    {
        _health -= damage;
        if (GameHost.Instance != null && _health > 0)
        {
            Network.Instance.Send(new Play(25, transform.position));
        }
        if (_health <= 0 && GameHost.Instance != null)
        {
            Network.Instance.Send(new Play(26, transform.position));

            Network.Instance.Send(new Destroy(transform.name));
        }
    }
}
