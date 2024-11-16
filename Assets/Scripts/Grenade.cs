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

    private string _playerNumber = "";
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
                Network.Instance.Send(new Play(9, transform.position));

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
                Network.Instance.Send(new Play(10, transform.position));

                Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
                Bounds bounds = GetComponent<SpriteRenderer>().bounds;

                if (GeometryUtility.TestPlanesAABB(planes, bounds))
                {
                    Animator cameraAnimator = Camera.main.GetComponent<Animator>();

                    cameraAnimator.Play("flash", 0, 0);

                    cameraAnimator.speed = ((float)((int)(Vector2.Distance(transform.position, Camera.main.transform.position)) + 1)) / 6f;
                }

                Network.Instance.Send(new FlashScreen(transform.position));
            }
            Destroy(gameObject);
        }
        else if (smoke)
        {
            GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0, 0);

            if (GameHost.Instance != null)
            {
                Network.Instance.Send(new Play(11, transform.position));

                GameObject vfx = Instantiate(explosionVfx);
                vfx.transform.position = transform.position;
                vfx.GetComponent<Smoke>().Activate();

                Network.Instance.Send(new SmokeVfx(vfx.transform.position));
            }

            Destroy(gameObject, 1f);
        }
        else if (wall)
        {
            GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0, 0);

            if (GameHost.Instance != null)
            {
                Network.Instance.Send(new Play(24, transform.position));
                Network.Instance.Send(new WallPacket(transform.position, transform.localEulerAngles.z, GameHost.Instance.drop++));
            }

            Destroy(gameObject);
        }
    }
}
