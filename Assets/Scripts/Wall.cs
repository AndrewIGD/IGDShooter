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
            GameHost.Instance.message += "Play " + "25" + " " + transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + transform.position.y.ToString(CultureInfo.InvariantCulture) + "\n";
        }
        if (_health <= 0 && GameHost.Instance != null)
        {
            GameHost.Instance.message += "Play " + "26" + " " + transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + transform.position.y.ToString(CultureInfo.InvariantCulture) + "\n";
            GameHost.Instance.message += "Destroy " + transform.name + "\n";
        }
    }
}
