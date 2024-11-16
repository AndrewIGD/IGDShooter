using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

public class Weapon : MonoBehaviour
{
    #region Fields

    [SerializeField] GameObject parent;

    [SerializeField] float slowness;

    [SerializeField] int type;

    [SerializeField] float bulletDamage;

    [SerializeField] float bulletSpeed;

    [SerializeField] float bulletReload;

    [SerializeField] GameObject bullet;

    [SerializeField] GameObject bulletPos;

    [SerializeField] bool gun = true;

    [SerializeField] bool automatic = true;

    [SerializeField] bool shotGun = false;

    [SerializeField] bool knife = false;

    [SerializeField] bool heGren = false;

    [SerializeField] bool flashGren = false;

    [SerializeField] bool smokeGren = false;

    [SerializeField] bool wallGren = false;

    [SerializeField] GameObject heGrenade;

    [SerializeField] GameObject flashGrenade;

    [SerializeField] GameObject smokeGrenade;

    [SerializeField] GameObject wallGrenade;

    [SerializeField] int maxBulletsPerRound;

    [SerializeField] int bulletCount;

    [SerializeField] int roundAmmo;

    public float Slowness => slowness;
    public int Team => _team;
    public bool Automatic => automatic;
    public int Type => type;
    public int BulletCount => bulletCount;
    public int RoundAmmo => roundAmmo;
    public bool IsGun => gun;
    public bool IsKnife => knife;
    public bool IsNade => heGren || wallGren || flashGren || smokeGren;
    public bool IsVisionNade => flashGren || smokeGren;
    public bool IsHe => heGren;
    public bool IsFlash => flashGren;
    public bool IsSmoke => smokeGren;
    public bool IsWall => wallGren;

    private bool reloading = false;

    private Player parentScript;

    private int _team = 0;

    private bool canShoot = true;

    #endregion

    public void ThrowGrenade(Vector2 targetPos)
    {
        if (heGren || flashGren || smokeGren || wallGren)
        {
            GameObject grenade = null;

            if (heGren)
            {
                grenade = Instantiate(heGrenade);
            }
            else if (flashGren)
            {
                grenade = Instantiate(flashGrenade);
            }
            else if (smokeGren)
            {
                grenade = Instantiate(smokeGrenade);
            }
            else if (wallGren)
            {
                grenade = Instantiate(wallGrenade);
            }

            grenade.transform.position = bulletPos.transform.position;

            bulletSpeed = Vector2.Distance(grenade.transform.position, targetPos) * 2;

            grenade.transform.localEulerAngles = bulletPos.transform.eulerAngles + new Vector3(0, 0, 180);

            Rigidbody2D grenadeRb = grenade.GetComponent<Rigidbody2D>();

            grenadeRb.linearVelocity = grenade.transform.up * bulletSpeed;

            Grenade grenadeScript = grenade.GetComponent<Grenade>();

            grenadeScript.GetParent(parent);
            grenadeScript.InvokeDetonation();

            Network.Instance.Send(new Play(20, transform.position, parentScript.ID));

            if (heGren)
                Network.Instance.Send(new GrenadePacket(0, grenade.transform.position, grenade.transform.up * bulletSpeed, grenade.transform.localEulerAngles.z));
            else if (flashGren)
                Network.Instance.Send(new GrenadePacket(1, grenade.transform.position, grenade.transform.up * bulletSpeed, grenade.transform.localEulerAngles.z));
            else if (smokeGren)
                Network.Instance.Send(new GrenadePacket(2, grenade.transform.position, grenade.transform.up * bulletSpeed, grenade.transform.localEulerAngles.z));
            else if (wallGren)
                Network.Instance.Send(new GrenadePacket(3, grenade.transform.position, grenade.transform.up * bulletSpeed, grenade.transform.localEulerAngles.z));

            Destroy(gameObject);
        }
    }

    public void ReloadGun()
    {
        if (roundAmmo != maxBulletsPerRound && bulletCount != 0 && reloading == false)
        {
            reloading = true;
            canShoot = false;
            CancelInvoke("CanShoot");
            Invoke("Reload", 3f);
            Invoke("ReloadSound", 1f);
        }
    }

    public void GetTeam(int team)
    {
        _team = team;
    }

    public void Shoot(bool running)
    {
        if (canShoot == true && roundAmmo != 0)
        {
            Network.Instance.Send(new Play(type, transform.position, parentScript.ID));

            if (shotGun)
            {
                float spread = -22.5f;

                for (int i = 0; i < 5; i++)
                {
                    Fire(running, spread, true);


                    spread += 11.25f;
                }
            }
            else
            {
                Fire(running, 0f);
            }

            DecreaseAmmo();
        }
    }

    private void DecreaseAmmo()
    {
        canShoot = false;

        Invoke("CanShoot", bulletReload);

        roundAmmo--;
        if (roundAmmo == 0)
        {
            CancelInvoke("CanShoot");
            Invoke("ReloadSound", 1f);
            Invoke("Reload", 3f);
        }
    }

    private void Fire(bool running, float spread, bool destroy = false)
    {
        GameObject newBullet = Instantiate(bullet);
        newBullet.transform.position = bulletPos.transform.position;
        newBullet.transform.localEulerAngles = bulletPos.transform.eulerAngles + new Vector3(0, 0, spread);

        Hitbox bulletHitbox = newBullet.GetComponent<Hitbox>();

        bulletHitbox.Setup(_team, bulletDamage, type, parent);

        Rigidbody2D bulletRb = newBullet.GetComponent<Rigidbody2D>();

        if (running == false || parentScript.GunRecoil == false)
            bulletRb.linearVelocity = -newBullet.transform.up * bulletSpeed;
        else bulletRb.linearVelocity = -newBullet.transform.up * bulletSpeed + new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0);

        newBullet.transform.name = GameHost.Instance.drop.ToString(CultureInfo.InvariantCulture);

        Network.Instance.Send(new Bullet(newBullet.transform.position, bulletRb.linearVelocity, newBullet.transform.localEulerAngles.z, GameHost.Instance.drop++, destroy));

        if (destroy)
            Destroy(newBullet, 0.5f);
    }

    private void Reload()
    {
        reloading = false;
        canShoot = true;
        if (bulletCount >= (maxBulletsPerRound - roundAmmo))
        {
            bulletCount -= (maxBulletsPerRound - roundAmmo);
            roundAmmo = maxBulletsPerRound;
        }
        else
        {
            roundAmmo += bulletCount;
            bulletCount = 0;
        }
    }

    private void ReloadSound()
    {
        Network.Instance.Send(new Play(7, transform.position, parentScript.ID));
    }

    private void CanShoot()
    {
        canShoot = true;
    }

    private void Start()
    {
        if (transform.parent.CompareTag("Prefabs") == false)
            parentScript = parent.GetComponent<Player>();
    }

    public void Setup(GameObject gameObject, int team, int bulletCount, int roundAmmo)
    {
        parent = gameObject;
        this._team = team;
        this.bulletCount = bulletCount;
        this.roundAmmo = roundAmmo;
    }

    public void SetupAmmo(int bulletCount, int roundAmmo)
    {
        this.bulletCount = bulletCount;
        this.roundAmmo = roundAmmo;
    }

    public void SetupPlayer(GameObject gameObject, int team)
    {
        parent = gameObject;
        this._team = team;
    }

    public void GetParent(GameObject gameObject) => parent = gameObject;
    public void GetBulletPosition(GameObject gameObject) => bulletPos = gameObject;
}
