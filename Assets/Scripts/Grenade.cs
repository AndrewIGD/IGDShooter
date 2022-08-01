using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

public class Grenade : MonoBehaviour
{
    [SerializeField] bool he = false;
    [SerializeField] bool flash = false;
    [SerializeField] bool smoke = false;
    [SerializeField] bool wall = false;

    [SerializeField] GameObject explosionVfx;

    private GameObject _parent;

    private Player _parentScript;

    private long _playerNumber = 0;
    private string _name;
    private int _team;


    public GameObject GetParent(GameObject parent) => _parent = parent;

    public void InvokeDetonation()
    {
        Invoke("Detonate", 1f);
    }

    private void Start()
    {
        if (_parent != null)
        {
            _parentScript = _parent.GetComponent<Player>();

            _playerNumber = _parentScript.ID;
            _name = _parentScript.Name;
            _team = _parentScript.Team;
        }
    }

    private void Detonate()
    {
        if (he)
        {
            GameObject vfx = Instantiate(explosionVfx);
            vfx.transform.position = transform.position;
            Destroy(vfx, 2f);

            if (GameHost.Instance != null)
            {
                GameHost.Instance.message += "Play " + "9" + " " + transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + transform.position.y.ToString(CultureInfo.InvariantCulture) + "\n";

                foreach (Player player in GameClient.Instance.AlivePlayers)
                {
                    float distance = Vector2.Distance(transform.position, player.transform.position);

                    if (distance < 8f && player.Dead == false)
                    {
                        int damage = 200 / ((int)distance + 1);

                        player.DecreaseHp(_parentScript, damage, 6);
                    }
                }

                foreach (Wall wall in GameClient.Instance.Walls)
                {
                    float distance = Vector2.Distance(transform.position, wall.SpriteRenderer.bounds.ClosestPoint(transform.position));

                    if (distance < 8f)
                    {
                        wall.DecreaseHp(2000 / ((int)distance + 1));
                    }
                }
            }
            Destroy(gameObject);
        }
        else if (flash)
        {
            if (GameHost.Instance != null)
            {
                GameHost.Instance.message += "Play " + "10" + " " + transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + transform.position.y.ToString(CultureInfo.InvariantCulture) + "\n";
                Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
                Bounds bounds = GetComponent<SpriteRenderer>().bounds;

                if (GeometryUtility.TestPlanesAABB(planes, bounds))
                {
                    Animator cameraAnimator = Camera.main.GetComponent<Animator>();

                    cameraAnimator.Play("flash", 0, 0);

                    cameraAnimator.speed = ((float)((int)(Vector2.Distance(transform.position, Camera.main.transform.position)) + 1)) / 6f;

                }
                GameHost.Instance.message += "FlashScreen " + transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + transform.position.y.ToString(CultureInfo.InvariantCulture) + "\n";
            }
            Destroy(gameObject);
        }
        else if (smoke)
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

            if (GameHost.Instance != null)
            {
                GameHost.Instance.message += "Play " + "11" + " " + transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + transform.position.y.ToString(CultureInfo.InvariantCulture) + "\n";
                GameObject vfx = Instantiate(explosionVfx);
                vfx.transform.position = transform.position;
                vfx.GetComponent<Smoke>().Activate();
                GameHost.Instance.message += "SmokeVfx " + vfx.transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + vfx.transform.position.y.ToString(CultureInfo.InvariantCulture) + "\n";
            }

            Destroy(gameObject, 1f);
        }
        else if (wall)
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

            if (GameHost.Instance != null)
            {
                GameHost.Instance.message += "Play " + "24" + " " + transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + transform.position.y.ToString(CultureInfo.InvariantCulture) + "\n";
                GameHost.Instance.message += "Wall " + transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + transform.position.y.ToString(CultureInfo.InvariantCulture) + " " + transform.localEulerAngles.z.ToString(CultureInfo.InvariantCulture) + " " + "wall" + GameHost.Instance.drop++.ToString(CultureInfo.InvariantCulture) + "\n";
            }

            Destroy(gameObject);
        }
    }
}
