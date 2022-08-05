using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField] GameObject dot;

    [SerializeField] SpriteRenderer[] limbs;

    [SerializeField] Sprite[] tSprites;

    [SerializeField] Sprite[] ctSprites;

    [SerializeField] GameObject box;

    [SerializeField] GameObject healthBar;

    [SerializeField] GameObject setHealthBar;

    [SerializeField] GameObject setArmorBar;

    [SerializeField] GameObject name;

    [SerializeField] GameObject gunParent;

    [SerializeField] GameObject grenBltPos;

    [SerializeField] GameObject limbsObj;

    [SerializeField] float health;

    [SerializeField] float armor;

    [SerializeField] float maxHealth;

    [SerializeField] float speed;

    #endregion

    #region Public Variables

    public GameObject HealthBar => healthBar;
    public TextMeshPro NameText => _nameText;
    public int GunType => _gunType;
    public float Cash => _cash;
    public float Health => health;
    public float Armor => armor;
    public Weapon GunScript => _gunScript;
    public bool HasPistol => _hasPistol;
    public bool HasHe => _hasHe;
    public bool HasFlash => _hasFlash;
    public bool HasSmoke => _hasSmoke;
    public bool HasBomb => _hasBomb;
    public bool CanBuy => _canBuy;
    public bool InBuyZone => _inBuyZone;

    public bool IsAnimation(string name) => _animator.GetCurrentAnimatorStateInfo(0).IsName(name);

    public Vector2 MoveDir => (_onlineNewPos - _onlineOldPos) / Time.deltaTime;

    public float FullHealth => health + armor;

    public float TimeAlive => _timeAlive;

    public bool Dead => _dead;

    public GameObject Gun
    {
        get
        {
            return _gun;
        }
        set
        {
            _gun = value;
            _gunHitbox = _gun.GetComponent<Hitbox>();
            _gunBoxCollider = _gun.GetComponent<BoxCollider2D>();
            _gunScript = _gun.GetComponent<Weapon>();

            _gunScript.GetParent(gameObject);
        }
    }

    public void IncreaseConsecutiveKills()
    {
        _consecutiveKills++;
    }

    public long ID => _playerNumber;

    public string Name => _nameText.text;

    public int Team => _team;

    private bool _canMove = true;

    public bool IsAbleToGraffiti => _canGrafiti;

    public bool HasWall => _hasWall;

    public bool GunRecoil = true;

    public bool Controllable => _controllable;
    public bool Controlling => _controlling;

    #endregion

    #region Private Variables

    private int _visibilityCount = 0;

    private bool _hasPistol = false;
    private int _gunType = -1;

    private bool _hasHe = false;
    private bool _hasFlash = false;
    private bool _hasSmoke = false;

    private GameObject _target;

    private int _cash;

    private bool _controlling = false;

    private Vector2 _onlineNewPos;
    private Vector2 _onlineOldPos;


    private int _gunBulletCount;
    private int _gunRoundAmmo;

    private int _pistolBulletCount;
    private int _pistolRoundAmmo;


    private List<int> _gunPrices;

    private bool _canGrafiti = true;

    private bool _shifting = false;

    private bool _hasWall = false;

    private int _currentGunID = 2;

    private Dictionary<Player, float> _playersDamaged;

    private GameObject _canBuyText;

    private bool _canBuy = true;

    private bool _inBuyZone = false;

    private float _timeAlive = 0;

    private long _playerNumber = -1;

    private bool _controllable = false;

    private bool _freezed = true;

    private int _currentRoundsSurvived;
    private int _damageDealtToPlayers;
    private int _bombDefusals;

    private int _bombPlants;
    private int _silentSteps;
    private int _consecutiveKills;
    private int _thrownWeapons;
    private int _heDamage;
    private int _roundsSurvived;
    private int _nadesThrown;

    private TextMeshPro _nameText;

    private bool _hasBomb = false;

    private int _team;

    private bool _dead = false;

    private int _buyZoneCount = 0;

    private Vector2 _grenTargetPos;

    private bool _defusing = false;

    private bool _hasShot = false;

    private BoxCollider2D _boxCollider2D;
    private Weapon _gunScript;
    private Hitbox _gunHitbox;
    private Animator _animator;

    private bool _canPlay = true;

    private bool _chatPress = false;

    private int _graffiti1;
    private int _graffiti2;
    private int _graffiti3;
    private int _graffiti4;

    private bool _showInv = false;
    private bool _mouseScroll = false;

    private GameObject _gun;

    private Vector3 _movementDirection;

    private Vector3 _interpolationCurrentPos;
    private Vector3 _interpolationTarget;

    private float _interpolationTime = -1;

    private BoxCollider2D _gunBoxCollider;

    private Rigidbody2D _rb;
    #endregion

    #region Public Methods

    public void SetSpeed(float v) => speed = v;

    public void Setup(long id, string name, int gunID, bool hasPistol, bool hasHe, bool hasFlash, bool hasSmoke, bool hasWall, int armor, int cash, int team)
    {
        _team = team;

        _playerNumber = id;

        if (GameHost.Instance != null)
            _currentRoundsSurvived = MatchData.PlayerData[_playerNumber].RoundsSurvived;

        EnableControl();
        NameText.text = name;

        _gunType = gunID;

        if (_gunType != -1)
        {
            _gunBulletCount = FightSceneManager.Instance.Guns[_gunType].GetComponent<Weapon>().BulletCount;
            _gunRoundAmmo = FightSceneManager.Instance.Guns[_gunType].GetComponent<Weapon>().RoundAmmo;
        }

        _hasPistol = hasPistol;

        if (_hasPistol)
        {
            _pistolBulletCount = FightSceneManager.Instance.Guns[6].GetComponent<Weapon>().BulletCount;
            _pistolRoundAmmo = FightSceneManager.Instance.Guns[6].GetComponent<Weapon>().RoundAmmo;
        }

        _hasHe = hasHe;
        _hasFlash = hasFlash;
        _hasSmoke = hasSmoke;
        _hasWall = hasWall;
        this.armor = armor;

        FightSceneManager.Instance.ArmorObject.SetActive(armor != 0);

        _cash = cash;

        RespawnWeapon();
    }

    public void Control()
    {
        _controlling = true;

        _graffiti1 = PlayerPrefs.GetInt("Graf1", 0);
        _graffiti2 = PlayerPrefs.GetInt("Graf2", 1);
        _graffiti3 = PlayerPrefs.GetInt("Graf3", 2);
        _graffiti4 = PlayerPrefs.GetInt("Graf4", 3);

        if (PlayerPrefs.GetInt("ShowInv") == 0)
            _showInv = false;
        else _showInv = true;

        if (PlayerPrefs.GetInt("MouseScroll") == 0)
            _mouseScroll = false;
        else _mouseScroll = true;

        FightSceneManager.Instance.CharacterInfo.SetActive(true);
        FightSceneManager.Instance.BulletImage.SetActive(true);
        FightSceneManager.Instance.WeaponList.gameObject.SetActive(true);
        FightSceneManager.Instance.ArmorText.text = armor.ToString(CultureInfo.InvariantCulture);

        foreach (Buy button in FightSceneManager.Instance.BuyButtons)
            button.AssignPlayer(this);

        foreach (GameObject child in FightSceneManager.Instance.HealthChildren)
            child.SetActive(true);

        foreach (GameObject child in FightSceneManager.Instance.ArmorChildren)
            child.SetActive(true);
    }

    public void EnableControl() => _controllable = true;

    public void EnableBuying() => _canBuy = true;
    public void DisableBuying() => _canBuy = false;

    public void SlowWalk() => _shifting = true;
    public void StopSlowWalk() => _shifting = false;

    public void Freeze() => _freezed = true;
    public void Unfreeze() => _freezed = false;

    public void SetupInterpolation(Vector3 moveDir, Vector3 currentPos, Vector3 target, float time)
    {
        _movementDirection = moveDir;
        _interpolationCurrentPos = currentPos;
        _interpolationTarget = target;
        _interpolationTime = time;
    }

    public void AssignBomb()
    {
        _hasBomb = true;
    }

    public void EnableBuyZone()
    {
        _buyZoneCount++;

        _inBuyZone = true;
    }

    public void DisableBuyZone()
    {
        _buyZoneCount--;

        if (_buyZoneCount == 0)
            _inBuyZone = false;
    }

    public void ChangeTeam(string team)
    {
        if (team.Trim() == "ct")
        {
            for (int i = 0; i < limbs.Length; i++)
            {
                limbs[i].sprite = ctSprites[i];
                Tab.Instance.ChangeTeam(this, 0);
                this._team = 0;
                if (Gun != null)
                {
                    _gunScript.GetTeam(this._team);
                    if (_gunHitbox != null)
                    {
                        _gunHitbox.GetTeam(this._team);
                    }
                }
            }

            if (GameHost.Instance != null)
                MatchData.PlayerData[_playerNumber - 1].Team = 0;
        }
        else if (team.Trim() == "t")
        {
            for (int i = 0; i < limbs.Length; i++)
            {
                limbs[i].sprite = tSprites[i];
                Tab.Instance.ChangeTeam(this, 1);
                this._team = 1;
                if (Gun != null)
                {
                    _gunScript.GetTeam(this._team);
                    if (_gunHitbox != null)
                    {
                        _gunHitbox.GetTeam(this._team);
                    }
                }
            }
            if (GameHost.Instance != null)
                MatchData.PlayerData[_playerNumber - 1].Team = 1;
        }
    }

    public void RoundEnd()
    {
        MatchData.PlayerData[_playerNumber].DamageDealt += _damageDealtToPlayers;
        MatchData.PlayerData[_playerNumber].BombDefusals += _bombDefusals;
        MatchData.PlayerData[_playerNumber].BombPlants += _bombPlants;
        MatchData.PlayerData[_playerNumber].SilentSteps += _silentSteps;
        MatchData.PlayerData[_playerNumber].ThrownWeapons += _thrownWeapons;
        MatchData.PlayerData[_playerNumber].HeDamage += _heDamage;
        MatchData.PlayerData[_playerNumber].RoundsSurvived += _roundsSurvived;
        MatchData.PlayerData[_playerNumber].NadesThrown += _nadesThrown;

        if (_consecutiveKills > MatchData.PlayerData[_playerNumber].ConsecutiveKills)
            MatchData.PlayerData[_playerNumber].ConsecutiveKills = _consecutiveKills;

        if (_currentRoundsSurvived > MatchData.PlayerData[_playerNumber].MaximumRoundsSurvived)
        {
            MatchData.PlayerData[_playerNumber].MaximumRoundsSurvived = _currentRoundsSurvived;
        }
    }

    public void ActivateBox()
    {
        box.SetActive(true);
    }

    public void Visible()
    {
        _visibilityCount++;

        dot.SetActive(true);
    }

    public void Invisible()
    {
        _visibilityCount--;

        if (_visibilityCount == 0)
        {
            dot.SetActive(false);
        }
    }

    public void DeathData(string damageDealt, string damageReceived, string killerName)
    {
        FightSceneManager.Instance.DeathInfo.GetComponentInChildren<Text>().text = "You were killed by <b><color=red>" + killerName + "</color></b>\n" + "Damage Dealt: <b><color=orange>" + damageDealt + "</color></b>\n" + "Damage Received: <b><color=orange>" + damageReceived + "</color></b>";
        FightSceneManager.Instance.DeathInfo.SetActive(true);
    }

    public void DealtDamage(Player player, float damage, bool he = false)
    {
        _damageDealtToPlayers += (int)damage;

        if (he)
            _heDamage += (int)damage;

        if (_playersDamaged.ContainsKey(player) == false)
        {
            _playersDamaged.Add(player, damage);
        }
        else
        {
            _playersDamaged[player] += damage;
        }
    }


    public void TriggerDeath()
    {
        if (_dead == false)
        {
            if (_controlling)
            {
                FightSceneManager.Instance.CharacterInfo.SetActive(true);
                foreach (Transform child in FightSceneManager.Instance.CharacterInfo.transform)
                {
                    if (child.name != "MatchInfo" && child.name != "DeathData")
                        child.gameObject.SetActive(false);
                }
                FightSceneManager.Instance.BuyMenu.SetActive(false);
            }

            if (GameHost.Instance != null)
            {
                StopDefuse();
            }
            healthBar.SetActive(false);
            _dead = true;
            Invoke("Death", 5f);
            _boxCollider2D.enabled = false;
            _animator.enabled = false;
            if (GameHost.Instance != null)
            {
                bool found = false;
                foreach (Player player in GameClient.Instance.AlivePlayers)
                {
                    if ((player.Team == _team && player._controllable && player.gameObject != gameObject && player._dead == false) || (_team == 1 && GameHost.Instance._bomb))
                        found = true;
                }
                if (found == false && GameHost.Instance._roundOver == false)
                {
                    GameHost.Instance.message += "Play " + "15" + "\n";
                }

                GameHost gameHost = GameHost.Instance;
                MatchData.PlayerData[_playerNumber].GunID = -1;
                MatchData.PlayerData[_playerNumber].HasPistol = false;
                MatchData.PlayerData[_playerNumber].HasHe = false;
                MatchData.PlayerData[_playerNumber].HasFlash = false;
                MatchData.PlayerData[_playerNumber].HasSmoke = false;
                MatchData.PlayerData[_playerNumber].HasWall = false;
                MatchData.PlayerData[_playerNumber].Armor = 0;
                MatchData.PlayerData[_playerNumber].DamageDealt += _damageDealtToPlayers;
                MatchData.PlayerData[_playerNumber].BombDefusals += _bombDefusals;
                MatchData.PlayerData[_playerNumber].BombPlants += _bombPlants;
                MatchData.PlayerData[_playerNumber].SilentSteps += _silentSteps;
                if (_consecutiveKills > MatchData.PlayerData[_playerNumber].ConsecutiveKills)
                    MatchData.PlayerData[_playerNumber].ConsecutiveKills = _consecutiveKills;
                MatchData.PlayerData[_playerNumber].ThrownWeapons += _thrownWeapons;
                MatchData.PlayerData[_playerNumber].HeDamage += _heDamage;
                MatchData.PlayerData[_playerNumber].RoundsSurvived = 0;
                MatchData.PlayerData[_playerNumber].NadesThrown += _nadesThrown;

                if (_gunType != -1)
                {
                    GameHost.Instance.message += "Drop " + _gunType.ToString(CultureInfo.InvariantCulture) + " " + transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + transform.position.y.ToString(CultureInfo.InvariantCulture) + " " + Random.Range(-5f, 5f).ToString(CultureInfo.InvariantCulture) + " " + Random.Range(-5f, 5f).ToString(CultureInfo.InvariantCulture) + " " + GameHost.Instance.drop++.ToString(CultureInfo.InvariantCulture) + " " + _gunBulletCount.ToString(CultureInfo.InvariantCulture) + " " + _gunRoundAmmo.ToString(CultureInfo.InvariantCulture) + "\n";
                }
                if (_hasPistol)
                {
                    GameHost.Instance.message += "Drop " + "0" + " " + transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + transform.position.y.ToString(CultureInfo.InvariantCulture) + " " + Random.Range(-5f, 5f).ToString(CultureInfo.InvariantCulture) + " " + Random.Range(-5f, 5f).ToString(CultureInfo.InvariantCulture) + " " + GameHost.Instance.drop++.ToString(CultureInfo.InvariantCulture) + " " + _pistolBulletCount.ToString(CultureInfo.InvariantCulture) + " " + _pistolRoundAmmo.ToString(CultureInfo.InvariantCulture) + "\n";
                }
                if (_hasHe)
                {
                    GameHost.Instance.message += "Drop " + "6" + " " + transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + transform.position.y.ToString(CultureInfo.InvariantCulture) + " " + Random.Range(-5f, 5f).ToString(CultureInfo.InvariantCulture) + " " + Random.Range(-5f, 5f).ToString(CultureInfo.InvariantCulture) + " " + GameHost.Instance.drop++.ToString(CultureInfo.InvariantCulture) + " 0 0" + "\n";
                }
                if (_hasFlash)
                {
                    GameHost.Instance.message += "Drop " + "7" + " " + transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + transform.position.y.ToString(CultureInfo.InvariantCulture) + " " + Random.Range(-5f, 5f).ToString(CultureInfo.InvariantCulture) + " " + Random.Range(-5f, 5f).ToString(CultureInfo.InvariantCulture) + " " + GameHost.Instance.drop++.ToString(CultureInfo.InvariantCulture) + " 0 0" + "\n";
                }
                if (_hasFlash)
                {
                    GameHost.Instance.message += "Drop " + "8" + " " + transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + transform.position.y.ToString(CultureInfo.InvariantCulture) + " " + Random.Range(-5f, 5f).ToString(CultureInfo.InvariantCulture) + " " + Random.Range(-5f, 5f).ToString(CultureInfo.InvariantCulture) + " " + GameHost.Instance.drop++.ToString(CultureInfo.InvariantCulture) + " 0 0" + "\n";
                }
                if (_hasBomb)
                {
                    GameHost.Instance.message += "Drop " + "9" + " " + transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + transform.position.y.ToString(CultureInfo.InvariantCulture) + " " + Random.Range(-5f, 5f).ToString(CultureInfo.InvariantCulture) + " " + Random.Range(-5f, 5f).ToString(CultureInfo.InvariantCulture) + " " + GameHost.Instance.drop++.ToString(CultureInfo.InvariantCulture) + " 0 0" + "\n";
                }
                if (_hasWall)
                {
                    GameHost.Instance.message += "Drop " + "10" + " " + transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + transform.position.y.ToString(CultureInfo.InvariantCulture) + " " + Random.Range(-5f, 5f).ToString(CultureInfo.InvariantCulture) + " " + Random.Range(-5f, 5f).ToString(CultureInfo.InvariantCulture) + " " + GameHost.Instance.drop++.ToString(CultureInfo.InvariantCulture) + " 0 0" + "\n";
                }
                _gunType = -1;
                _hasPistol = false;
                _hasHe = false;
                _hasFlash = false;
                _hasSmoke = false;
                _hasWall = false;
                armor = 0;
                _hasBomb = false;
            }
            foreach (Transform child in limbsObj.transform)
            {
                if (child.name != "Gun")
                {
                    try
                    {
                        child.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-5f, 5f), Random.Range(-5f, 5f));
                        child.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-360, 360);
                        child.GetComponent<BoxCollider2D>().enabled = true;
                    }
                    catch
                    {

                    }
                }
                else Destroy(child.gameObject);
            }
        }
    }

    public void DecreaseHp(Player attacker, float damage, int damageType)
    {
        GameHost gameHost = GameHost.Instance;
        gameHost.message += "Play " + "16" + " " + transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + transform.position.y.ToString(CultureInfo.InvariantCulture) + " " + _playerNumber.ToString(CultureInfo.InvariantCulture) + "\n";
        if (armor != 0)
        {
            if (damage > armor)
            {
                damage -= armor;
                armor = 0;
                if (gameHost != null && _controlling)
                    FightSceneManager.Instance.ArmorText.text = "0";
            }
            else
            {
                armor -= damage;
                damage = 0;
                if (gameHost != null && _controlling)
                    FightSceneManager.Instance.ArmorText.text = armor.ToString(CultureInfo.InvariantCulture);
            }
        }

        health -= damage;

        if(attacker != null)
            attacker.DealtDamage(this, damage, damageType == 6);

        if (health <= 0)
        {
            if (gameHost.Warmup == false || damage == 9999)
            {
                _canMove = false;
                if (gameHost != null && _controlling)
                    FightSceneManager.Instance.HealthText.text = "0";

                float damageDealt = 0f;
                if (attacker != null)
                {
                    if (_playersDamaged.ContainsKey(attacker))
                    {
                        damageDealt = _playersDamaged[attacker];
                    }
                }
                float damageReceived = 0f;

                if (attacker != null)
                {
                    damageReceived = attacker.GetDamageDealt(this);
                }

                MatchData.PlayerData[_playerNumber].Deaths++;

                gameHost.message += "SetDeaths " + _playerNumber.ToString(CultureInfo.InvariantCulture) + " " + MatchData.PlayerData[_playerNumber].Deaths.ToString(CultureInfo.InvariantCulture) + "\n";

                if (attacker != null)
                    gameHost.message += "Death " + _playerNumber.ToString(CultureInfo.InvariantCulture) + " " + damageDealt.ToString(CultureInfo.InvariantCulture) + " " + damageReceived.ToString(CultureInfo.InvariantCulture) + " " + attacker.Name + "\n";
                else gameHost.message += "Death " + _playerNumber.ToString(CultureInfo.InvariantCulture) + "\n";
            }
            else
            {
                health = 100;
                if (gameHost != null && _controlling)
                    FightSceneManager.Instance.HealthText.text = "100";
                if (_team == 0)
                    transform.position = GameObject.Find("ctSpawn" + (Random.Range(0, 99999) % 9).ToString(CultureInfo.InvariantCulture)).transform.position;
                else if (_team == 1)
                    transform.position = GameObject.Find("tSpawn" + (Random.Range(0, 99999) % 9).ToString(CultureInfo.InvariantCulture)).transform.position;

                MatchData.PlayerData[_playerNumber].Deaths++;

                gameHost.message += "SetDeaths " + _playerNumber.ToString(CultureInfo.InvariantCulture) + " " + MatchData.PlayerData[_playerNumber].Deaths.ToString(CultureInfo.InvariantCulture) + "\n";
            }

            if (attacker != null)
                GameHost.Instance.RegisterKill(attacker.ID, this, damageType);
        }
        else if (gameHost != null && _controlling)
            FightSceneManager.Instance.HealthText.text = health.ToString(CultureInfo.InvariantCulture);
    }

    public float GetDamageDealt(Player player)
    {
        if (_playersDamaged.ContainsKey(player))
            return _playersDamaged[player];

        return 0;
    }

    public void ActivateSword()
    {
        if (_gunScript.Type == 0)
            _gunBoxCollider.enabled = true;
    }
    public void DeactivateSword()
    {
        try
        {
            if (_gunScript.Type == 0)
                _gunBoxCollider.enabled = false;
        }
        catch
        {

        }
    }

    public void Grafitied()
    {
        _canGrafiti = false;
        Invoke("CanGrafiti", 10f);
    }

    public void Initialize()
    {
        _controllable = true;

        _rb = GetComponent<Rigidbody2D>();

        _boxCollider2D = GetComponent<BoxCollider2D>();
        _animator = GetComponent<Animator>();

        _playersDamaged = new Dictionary<Player, float>();

        _nameText = name.GetComponent<TextMeshPro>();

        if (_controlling)
        {
            name.transform.localPosition = new Vector3(0, 0.1115999f, 0);
            healthBar.SetActive(false);
            Invoke("OffWeaponList", 5f);
        }
        maxHealth = health;

        _gunPrices = new List<int>() { 200, 1200, 2400, 4400, 7000, 10000, 600, 600, 600, 600, 600 };

        GameClient.Instance.megamap = FightSceneManager.Instance.MegamapDetect.gameObject;
    }

    public bool GetDrop(int type, int bulletCount, int roundAmmo)
    {
        bool ok = false;

        if (type == 0 && _hasPistol == false)
        {
            _hasPistol = true;
            _pistolBulletCount = bulletCount;
            _pistolRoundAmmo = roundAmmo;
            if (_gunScript.Type == 0)
            {
                Destroy(Gun);
                GameObject newGun = Instantiate(FightSceneManager.Instance.Guns[6], gunParent.transform);
                Gun = newGun;
                Gun.GetComponent<SpriteRenderer>().enabled = limbsObj.transform.GetChild(0).gameObject.activeInHierarchy;

                _currentGunID = 1;

                _animator.Play("idle_gun", 0, 0);

                _gunScript.Setup(gameObject, _team, bulletCount, roundAmmo);

                GameHost.Instance.message += "Play " + "20" + " " + transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + transform.position.y.ToString(CultureInfo.InvariantCulture) + " " + GetComponent<Player>()._playerNumber.ToString(CultureInfo.InvariantCulture) + "\n";
            }
            ok = true;
        }
        else if (type <= 5 && type != 0 && _gunType == -1)
        {
            _gunType = type;
            _gunBulletCount = bulletCount;
            _gunRoundAmmo = roundAmmo;
            if (_gunScript.Type == 0 || _gunScript.Type == 6)
            {
                RespawnWeapon();
                GameHost.Instance.message += "Play " + "20" + " " + transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + transform.position.y.ToString(CultureInfo.InvariantCulture) + " " + GetComponent<Player>()._playerNumber.ToString(CultureInfo.InvariantCulture) + "\n";
            }
            ok = true;
        }
        else if (type == 6 && _hasHe == false)
        {
            _hasHe = true;
            ok = true;
        }
        else if (type == 7 && _hasFlash == false)
        {
            _hasFlash = true;
            ok = true;
        }
        else if (type == 8 && _hasSmoke == false)
        {
            _hasSmoke = true;
            ok = true;
        }
        else if (type == 9 && _hasBomb == false)
        {
            _hasBomb = true;
            ok = true;
        }
        else if (type == 10 && _hasWall == false)
        {
            _hasWall = true;
            ok = true;
        }

        UpdateCashText();

        return ok;
    }

    private void UpdateCashText()
    {
        if (GameHost.Instance != null && _controlling)
            FightSceneManager.Instance.CashTextScript.text = "$" + _cash.ToString(CultureInfo.InvariantCulture);
    }

    public void ProcessPurchase(int type)
    {
        if (_cash >= _gunPrices[type])
        {
            if (type == 0 && _hasPistol == false)
            {
                _hasPistol = true;

                _cash -= _gunPrices[type];
                _pistolBulletCount = FightSceneManager.Instance.Guns[6].GetComponent<Weapon>().BulletCount;
                _pistolRoundAmmo = FightSceneManager.Instance.Guns[6].GetComponent<Weapon>().RoundAmmo;
                if (_gunScript.Type == 0)
                {
                    RespawnWeapon();
                }

                ActivateWeaponList();
            }
            else if (type <= 5 && type != 0 && type != _gunType)
            {
                if (_gunType >= 1 && _gunType <= 5)
                {
                    GameHost.Instance.message += "Drop " + _gunType.ToString(CultureInfo.InvariantCulture) + " " + transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + transform.position.y.ToString(CultureInfo.InvariantCulture) + " " + "0" + " " + "0" + " " + GameHost.Instance.drop++.ToString(CultureInfo.InvariantCulture) + " " + _gunBulletCount.ToString(CultureInfo.InvariantCulture) + " " + _gunRoundAmmo.ToString(CultureInfo.InvariantCulture) + "\n";
                }

                _gunType = type;

                _cash -= _gunPrices[type];
                _gunBulletCount = FightSceneManager.Instance.Guns[_gunType].GetComponent<Weapon>().BulletCount;
                _gunRoundAmmo = FightSceneManager.Instance.Guns[_gunType].GetComponent<Weapon>().RoundAmmo;

                if (_gunScript.Type != 7 && _gunScript.Type != 8 && _gunScript.Type != 9)
                {
                    RespawnWeapon();
                    GameHost.Instance.message += "Play " + "20" + " " + transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + transform.position.y.ToString(CultureInfo.InvariantCulture) + " " + GetComponent<Player>()._playerNumber.ToString(CultureInfo.InvariantCulture) + "\n";
                }
                ActivateWeaponList();
            }
            else if (type == 6 && _hasHe == false)
            {
                _hasHe = true;

                _cash -= _gunPrices[type];

                ActivateWeaponList();
            }
            else if (type == 7 && _hasFlash == false)
            {
                _hasFlash = true;

                _cash -= _gunPrices[type];

                ActivateWeaponList();
            }
            else if (type == 8 && _hasSmoke == false)
            {
                _hasSmoke = true;

                _cash -= _gunPrices[type];

                ActivateWeaponList();
            }
            else if (type == 9 && armor < 100)
            {
                armor += 50;
                if (armor > 100)
                    armor = 100;

                _cash -= _gunPrices[type];
                if (GameHost.Instance != null && _controlling)
                    FightSceneManager.Instance.ArmorText.text = armor.ToString(CultureInfo.InvariantCulture);
            }
            else if (type == 10 && _hasWall == false)
            {
                _hasWall = true;

                _cash -= _gunPrices[type];

                ActivateWeaponList();
            }

            UpdateCashText();
            MatchData.PlayerData[_playerNumber].Cash = _cash;
        }
    }

    private void ActivateWeaponList()
    {
        if (_controlling)
        {
            FightSceneManager.Instance.WeaponList.SetActive(true);
            CancelInvoke("OffWeaponList");
            Invoke("OffWeaponList", 5f);
        }
    }

    public void Defuse()
    {
        if (_defusing == false && GameHost.Instance.bombObject != null)
        {
            if (Vector2.Distance(transform.position, GameHost.Instance.bombObject.transform.position) < 1)
            {
                GameHost.Instance.message += "Play " + "13" + " " + transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + transform.position.y.ToString(CultureInfo.InvariantCulture) + " " + GetComponent<Player>()._playerNumber.ToString(CultureInfo.InvariantCulture) + "\n";

                if (_gunScript.IsGun)
                    _animator.Play("idle_gun");
                else _animator.Play("idle_knife");

                _defusing = true;

                GameHost.Instance.message += "Defusing " + _playerNumber.ToString(CultureInfo.InvariantCulture) + "\n";
                Invoke("Win", 5f);
            }
        }
    }

    public void StopDefuse()
    {
        GameHost.Instance.message += "StopDef " + _playerNumber.ToString(CultureInfo.InvariantCulture) + "\n";
        _defusing = false;
        CancelInvoke("Win");

        NullTargetPosition();
    }

    public void NullTargetPosition()
    {
        if (_target != null)
        {
            Destroy(_target);
            _target = null;
        }
    }

    public void StopMovement()
    {
        _canMove = false;
    }

    public void AddCash(int value)
    {
        _cash += value;
        FightSceneManager.Instance.CashTextScript.text = "$" + _cash.ToString(CultureInfo.InvariantCulture);
    }

    public void ActivateMovement()
    {
        _canMove = true;
        if (Gun == null)
            RespawnWeapon();
    }

    public void SetWeapon(int type)
    {
        Destroy(Gun);
        GameObject newGun = Instantiate(FightSceneManager.Instance.Guns[type], gunParent.transform);
        Gun = newGun;
        Gun.GetComponent<SpriteRenderer>().enabled = limbsObj.transform.GetChild(0).gameObject.activeInHierarchy;
        _gunScript.SetupPlayer(gameObject, _team);
        if (type == 6)
        {
            _currentGunID = 1;
            _gunScript.SetupAmmo(_pistolBulletCount, _pistolRoundAmmo);
        }
        else if (type <= 5 && type != 0)
        {
            _currentGunID = 0;
            _gunScript.SetupAmmo(_gunBulletCount, _gunRoundAmmo);
        }

        if (type == 0)
        {
            _currentGunID = 2;
            newGun.transform.name = "sword";
            _gunHitbox.KnifeSetup(_team, gameObject);
        }
        else if (type == 7 || type == 8 || type == 9 || type == 11)
        {
            _currentGunID = type - 4;
            _gunScript.GetBulletPosition(grenBltPos);
        }
        else if (type == 10)
            _currentGunID = 7;
    }

    public void UpdateInfo(Vector2 playerPos, float angle, float health, string armorText, int canBuy, int inBuyZone)
    {
        if (Vector2.Distance(transform.position, playerPos) > 0.1f)
        {
            if (_timeAlive > 3)
            {
                _interpolationCurrentPos = transform.position;
                _interpolationTime = 0;
                _interpolationTarget = playerPos;
            }
            else transform.position = playerPos;
        }

        transform.localEulerAngles = new Vector3(0, 0, angle);

        this.health = health;
        if (_controlling)
        {
            FightSceneManager.Instance.HealthText.text = health.ToString(CultureInfo.InvariantCulture);
            FightSceneManager.Instance.ArmorText.text = armorText;

            if (canBuy == 1)
                _canBuy = true;
            else _canBuy = false;

            if (inBuyZone == 1)
                _inBuyZone = true;
            else _inBuyZone = false;
        }
    }

    public void SwitchWeapon(int type)
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("attack_knife") == false)
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("bomb_plant"))
                _animator.Play("idle_knife");
            if (type == 0)
            {
                if (_gunType != -1 && _gunScript.Type != _gunType)
                {
                    Destroy(Gun);
                    GameObject newGun = Instantiate(FightSceneManager.Instance.Guns[_gunType], gunParent.transform);
                    Gun = newGun;
                    Gun.GetComponent<SpriteRenderer>().enabled = limbsObj.transform.GetChild(0).gameObject.activeInHierarchy;
                    _currentGunID = 0;

                    _animator.Play("idle_gun", 0, 0);

                    _gunScript.Setup(gameObject, _team, _gunBulletCount, _gunRoundAmmo);
                    GameHost.Instance.message += "Play " + "20" + " " + transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + transform.position.y.ToString(CultureInfo.InvariantCulture) + " " + _playerNumber.ToString(CultureInfo.InvariantCulture) + "\n";
                }
            }
            else if (type == 1)
            {
                if (_hasPistol && _gunScript.Type != 6)
                {
                    Destroy(Gun);
                    GameObject newGun = Instantiate(FightSceneManager.Instance.Guns[6], gunParent.transform);
                    Gun = newGun;
                    Gun.GetComponent<SpriteRenderer>().enabled = limbsObj.transform.GetChild(0).gameObject.activeInHierarchy;
                    _currentGunID = 1;

                    _animator.Play("idle_gun", 0, 0);

                    _gunScript.Setup(gameObject, _team, _pistolBulletCount, _pistolRoundAmmo);
                    GameHost.Instance.message += "Play " + "20" + " " + transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + transform.position.y.ToString(CultureInfo.InvariantCulture) + " " + _playerNumber.ToString(CultureInfo.InvariantCulture) + "\n";
                }
            }
            else if (type == 2 && _gunScript.Type != 0)
            {
                Destroy(Gun);
                GameObject knife = Instantiate(FightSceneManager.Instance.Guns[0], gunParent.transform);
                Gun = knife;
                Gun.GetComponent<SpriteRenderer>().enabled = limbsObj.transform.GetChild(0).gameObject.activeInHierarchy;
                _currentGunID = 2;

                _animator.Play("idle_knife", 0, 0);

                knife.transform.name = "sword";
                _gunHitbox.KnifeSetup(_team, gameObject);
                GameHost.Instance.message += "Play " + "21" + " " + transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + transform.position.y.ToString(CultureInfo.InvariantCulture) + " " + _playerNumber.ToString(CultureInfo.InvariantCulture) + "\n";
            }
            else if (type == 3)
            {
                if (_hasHe && _gunScript.Type != 7)
                {
                    Destroy(Gun);
                    GameObject newGun = Instantiate(FightSceneManager.Instance.Guns[7], gunParent.transform);
                    Gun = newGun;
                    Gun.GetComponent<SpriteRenderer>().enabled = limbsObj.transform.GetChild(0).gameObject.activeInHierarchy;
                    _currentGunID = 3;


                    _animator.Play("idle_knife", 0, 0);

                    _gunScript.GetParent(gameObject);
                    _gunScript.GetBulletPosition(grenBltPos);
                    GameHost.Instance.message += "Play " + "20" + " " + transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + transform.position.y.ToString(CultureInfo.InvariantCulture) + " " + _playerNumber.ToString(CultureInfo.InvariantCulture) + "\n";
                }
            }
            else if (type == 4)
            {
                if (_hasFlash && _gunScript.Type != 8)
                {
                    Destroy(Gun);
                    GameObject newGun = Instantiate(FightSceneManager.Instance.Guns[8], gunParent.transform);
                    Gun = newGun;
                    Gun.GetComponent<SpriteRenderer>().enabled = limbsObj.transform.GetChild(0).gameObject.activeInHierarchy;
                    _currentGunID = 4;


                    _animator.Play("idle_knife", 0, 0);

                    _gunScript.GetParent(gameObject);
                    _gunScript.GetBulletPosition(grenBltPos);
                    GameHost.Instance.message += "Play " + "20" + " " + transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + transform.position.y.ToString(CultureInfo.InvariantCulture) + " " + _playerNumber.ToString(CultureInfo.InvariantCulture) + "\n";
                }
            }
            else if (type == 5)
            {
                if (_hasSmoke && _gunScript.Type != 9)
                {
                    Destroy(Gun);
                    GameObject newGun = Instantiate(FightSceneManager.Instance.Guns[9], gunParent.transform);
                    Gun = newGun;
                    Gun.GetComponent<SpriteRenderer>().enabled = limbsObj.transform.GetChild(0).gameObject.activeInHierarchy;
                    _currentGunID = 5;


                    _animator.Play("idle_knife", 0, 0);

                    _gunScript.GetParent(gameObject);
                    _gunScript.GetBulletPosition(grenBltPos);
                    GameHost.Instance.message += "Play " + "20" + " " + transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + transform.position.y.ToString(CultureInfo.InvariantCulture) + " " + _playerNumber.ToString(CultureInfo.InvariantCulture) + "\n";
                }
            }
            else if (type == 6)
            {
                if (_hasBomb && _gunScript.Type != 10)
                {
                    Destroy(Gun);
                    GameObject newGun = Instantiate(FightSceneManager.Instance.Guns[10], gunParent.transform);
                    Gun = newGun;
                    Gun.GetComponent<SpriteRenderer>().enabled = limbsObj.transform.GetChild(0).gameObject.activeInHierarchy;
                    _currentGunID = 7;


                    _animator.Play("idle_knife", 0, 0);

                    _gunScript.GetParent(gameObject);
                    GameHost.Instance.message += "Play " + "20" + " " + transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + transform.position.y.ToString(CultureInfo.InvariantCulture) + " " + _playerNumber.ToString(CultureInfo.InvariantCulture) + "\n";
                }
            }
            else if (type == 7)
            {
                if (_hasWall && _gunScript.Type != 11)
                {
                    Destroy(Gun);
                    GameObject newGun = Instantiate(FightSceneManager.Instance.Guns[11], gunParent.transform);
                    Gun = newGun;
                    Gun.GetComponent<SpriteRenderer>().enabled = limbsObj.transform.GetChild(0).gameObject.activeInHierarchy;
                    _currentGunID = 6;


                    _animator.Play("idle_knife", 0, 0);

                    _gunScript.GetParent(gameObject);
                    _gunScript.GetBulletPosition(grenBltPos);
                    GameHost.Instance.message += "Play " + "20" + " " + transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + transform.position.y.ToString(CultureInfo.InvariantCulture) + " " + _playerNumber.ToString(CultureInfo.InvariantCulture) + "\n";
                }
            }
        }
    }

    public void UpdateTargetCoordinates(Vector2 vector2)
    {
        if (_target == null)
            _target = new GameObject();

        _target.transform.position = vector2;
    }

    public void Shoot()
    {
        if (_freezed == false && GameHost.Instance != null && _animator.GetCurrentAnimatorStateInfo(0).IsName("attack_knife") == false)
        {
            bool ok = false;

            if (_target != null)
            {
                Destroy(_target);
                _target = null;

                ok = true;
            }

            if (_freezed == false && _gunScript.Type == 10)
            {
                //Planting

                if (_animator.GetCurrentAnimatorStateInfo(0).IsName("bomb_plant") == false)
                {
                    Collider2D[] hit = Physics2D.OverlapBoxAll(Gun.transform.position, new Vector2(0.1f, 0.1f), 0f);
                    foreach (Collider2D collider in hit)
                    {
                        if (collider.CompareTag("BombSite"))
                        {
                            GameHost.Instance.message += "Play " + "12" + " " + transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + transform.position.y.ToString(CultureInfo.InvariantCulture) + " " + GetComponent<Player>()._playerNumber.ToString(CultureInfo.InvariantCulture) + "\n";
                            _animator.Play("bomb_plant");
                        }
                    }
                }
                return;
            }
            else if (_freezed == false && _gunScript.IsKnife == true)
            {
                _grenTargetPos = Cursor.Position;
                if (GameHost.Instance != null)
                {
                    _animator.Play("attack_knife");
                    GameHost.Instance.message += "Play " + "8" + " " + transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + transform.position.y.ToString(CultureInfo.InvariantCulture) + " " + GetComponent<Player>()._playerNumber.ToString(CultureInfo.InvariantCulture) + "\n";
                }
            }
            else if (_freezed == false && _gunScript.IsGun)
            {
                _gunScript.Shoot(ok);
            }
        }
    }

    public void PlantBomb()
    {
        Invoke("PlantFinish", 0.5f);
        Destroy(Gun);
        _hasBomb = false;
        GameHost.Instance._bombTimeLeft = Config.BombTime;
        GameHost.Instance._bomb = true;
        GameHost.Instance.message += "Play " + "23" + "\n";
        GameHost.Instance.message += "Plant " + Gun.transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + Gun.transform.position.y.ToString(CultureInfo.InvariantCulture) + "\n";
        _bombPlants++;
    }

    public void ThrowNade(Vector3 mousePos)
    {
        if (_freezed == false && _gunScript.IsNade)
        {
            if (_gunScript.IsVisionNade)
                _nadesThrown++;

            NullTargetPosition();

            Vector2 dif = mousePos - transform.position;
            dif.Normalize();

            float rot = Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg;
            transform.localEulerAngles = new Vector3(0, 0, rot + 90);

            _grenTargetPos = mousePos;
            if (GameHost.Instance != null)
            {
                _animator.Play("attack_knife");
                GameHost.Instance.message += "Play " + "8" + " " + transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + transform.position.y.ToString(CultureInfo.InvariantCulture) + " " + GetComponent<Player>()._playerNumber.ToString(CultureInfo.InvariantCulture) + "\n";
            }
        }
        else if (_freezed == false && (_gunScript.Type == 10 && _animator.GetCurrentAnimatorStateInfo(0).IsName("bomb_plant")))
        {
            _canMove = true;
            _animator.Play("idle_knife");
        }
    }

    public void AddHealth(float value, bool roundStart)
    {
        health += value;

        if (health > maxHealth)
            maxHealth = health;
        else if (health <= 0)
        {
            MatchData.PlayerData[ID].Deaths++;

            GameHost.Instance.message += (roundStart ? "RoundStart " : "") + "SetDeaths " + ID.ToString(CultureInfo.InvariantCulture) + " " + MatchData.PlayerData[ID].Deaths.ToString(CultureInfo.InvariantCulture) + "\n";

            GameHost.Instance.message += (roundStart ? "RoundStart " : "") + "Death " + ID.ToString(CultureInfo.InvariantCulture) + "\n";
        }

        if (GameClient.Instance.players[GameClient.Instance.playerId] == this)
        {
            FightSceneManager.Instance.HealthText.text = health.ToString(CultureInfo.InvariantCulture);
        }
    }

    public void Throw(Vector2 pos)
    {
        if (_freezed)
            _thrownWeapons++;

        if (_gunScript.Type == 6)
        {
            _hasPistol = false;

            if (_controlling)
                FightSceneManager.Instance.WeaponUIObjects[1].SetActive(false);

            Vector2 dif = pos - (Vector2)transform.position;
            dif.Normalize();

            float rot = Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg;
            transform.localEulerAngles = new Vector3(0, 0, rot + 90);

            NullTargetPosition();

            if (_target != null)
                GameHost.Instance.message += "Drop " + "0" + " " + (transform.position - (transform.position - grenBltPos.transform.position) * 2).x.ToString(CultureInfo.InvariantCulture) + " " + (transform.position - (transform.position - grenBltPos.transform.position) * 2).y.ToString(CultureInfo.InvariantCulture) + " " + (-transform.up * (5 + speed)).x.ToString(CultureInfo.InvariantCulture) + " " + (-transform.up * (5 + speed)).y.ToString(CultureInfo.InvariantCulture) + " " + GameHost.Instance.drop++.ToString(CultureInfo.InvariantCulture) + " " + _pistolBulletCount.ToString(CultureInfo.InvariantCulture) + " " + _pistolRoundAmmo.ToString(CultureInfo.InvariantCulture) + "\n";
            else GameHost.Instance.message += "Drop " + "0" + " " + (transform.position - (transform.position - grenBltPos.transform.position) * 2).x.ToString(CultureInfo.InvariantCulture) + " " + (transform.position - (transform.position - grenBltPos.transform.position) * 2).y.ToString(CultureInfo.InvariantCulture) + " " + (-transform.up * 5).x.ToString(CultureInfo.InvariantCulture) + " " + (-transform.up * 5).y.ToString(CultureInfo.InvariantCulture) + " " + GameHost.Instance.drop++.ToString(CultureInfo.InvariantCulture) + " " + _pistolBulletCount.ToString(CultureInfo.InvariantCulture) + " " + _pistolRoundAmmo.ToString(CultureInfo.InvariantCulture) + "\n";

            RespawnWeapon();

            if (_gunScript.IsKnife)
                GameHost.Instance.message += "Play " + "21" + " " + transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + transform.position.y.ToString(CultureInfo.InvariantCulture) + " " + GetComponent<Player>()._playerNumber.ToString(CultureInfo.InvariantCulture) + "\n";
            else GameHost.Instance.message += "Play " + "20" + " " + transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + transform.position.y.ToString(CultureInfo.InvariantCulture) + " " + GetComponent<Player>()._playerNumber.ToString(CultureInfo.InvariantCulture) + "\n";

            _boxCollider2D.enabled = false;
            _boxCollider2D.enabled = true;
        }
        else if (_gunScript.Type == 10)
        {
            _hasBomb = false;

            if (_controlling)
                FightSceneManager.Instance.WeaponUIObjects[6].SetActive(false);

            Vector2 dif = pos - (Vector2)transform.position;
            dif.Normalize();

            float rot = Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg;
            transform.localEulerAngles = new Vector3(0, 0, rot + 90);

            NullTargetPosition();

            if (_target != null)
                GameHost.Instance.message += "Drop " + "9" + " " + (transform.position - (transform.position - grenBltPos.transform.position) * 2).x.ToString(CultureInfo.InvariantCulture) + " " + (transform.position - (transform.position - grenBltPos.transform.position) * 2).y.ToString(CultureInfo.InvariantCulture) + " " + (-transform.up * (5 + speed)).x.ToString(CultureInfo.InvariantCulture) + " " + (-transform.up * (5 + speed)).y.ToString(CultureInfo.InvariantCulture) + " " + GameHost.Instance.drop++.ToString(CultureInfo.InvariantCulture) + " " + "0" + " " + "0" + "\n";
            else GameHost.Instance.message += "Drop " + "9" + " " + (transform.position - (transform.position - grenBltPos.transform.position) * 2).x.ToString(CultureInfo.InvariantCulture) + " " + (transform.position - (transform.position - grenBltPos.transform.position) * 2).y.ToString(CultureInfo.InvariantCulture) + " " + (-transform.up * 5).x.ToString(CultureInfo.InvariantCulture) + " " + (-transform.up * 5).y.ToString(CultureInfo.InvariantCulture) + " " + GameHost.Instance.drop++.ToString(CultureInfo.InvariantCulture) + " " + "0" + " " + "0" + "\n";

            RespawnWeapon();

            if (_gunScript.IsKnife)
                GameHost.Instance.message += "Play " + "21" + " " + transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + transform.position.y.ToString(CultureInfo.InvariantCulture) + " " + GetComponent<Player>()._playerNumber.ToString(CultureInfo.InvariantCulture) + "\n";
            else GameHost.Instance.message += "Play " + "20" + " " + transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + transform.position.y.ToString(CultureInfo.InvariantCulture) + " " + GetComponent<Player>()._playerNumber.ToString(CultureInfo.InvariantCulture) + "\n";

            _boxCollider2D.enabled = false;
            _boxCollider2D.enabled = true;
        }
        else
        {
            if (_gunScript.Type >= 1 && _gunScript.Type <= 5)
            {

                if (_controlling)
                    FightSceneManager.Instance.WeaponUIObjects[0].SetActive(false);

                Vector2 dif = pos - (Vector2)transform.position;
                dif.Normalize();

                float rot = Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg;
                transform.localEulerAngles = new Vector3(0, 0, rot + 90);

                NullTargetPosition();

                if (_target != null)
                    GameHost.Instance.message += "Drop " + _gunType.ToString(CultureInfo.InvariantCulture) + " " + (transform.position - (transform.position - grenBltPos.transform.position) * 2).x.ToString(CultureInfo.InvariantCulture) + " " + (transform.position - (transform.position - grenBltPos.transform.position) * 2).y.ToString(CultureInfo.InvariantCulture) + " " + (-transform.up * (5 + speed)).x.ToString(CultureInfo.InvariantCulture) + " " + (-transform.up * (5 + speed)).y.ToString(CultureInfo.InvariantCulture) + " " + GameHost.Instance.drop++.ToString(CultureInfo.InvariantCulture) + " " + _gunBulletCount.ToString(CultureInfo.InvariantCulture) + " " + _gunRoundAmmo.ToString(CultureInfo.InvariantCulture) + "\n";
                else GameHost.Instance.message += "Drop " + _gunType.ToString(CultureInfo.InvariantCulture) + " " + (transform.position - (transform.position - grenBltPos.transform.position) * 2).x.ToString(CultureInfo.InvariantCulture) + " " + (transform.position - (transform.position - grenBltPos.transform.position) * 2).y.ToString(CultureInfo.InvariantCulture) + " " + (-transform.up * 5).x.ToString(CultureInfo.InvariantCulture) + " " + (-transform.up * 5).y.ToString(CultureInfo.InvariantCulture) + " " + GameHost.Instance.drop++.ToString(CultureInfo.InvariantCulture) + " " + _gunBulletCount.ToString(CultureInfo.InvariantCulture) + " " + _gunRoundAmmo.ToString(CultureInfo.InvariantCulture) + "\n";

                _gunType = -1;

                RespawnWeapon();

                if (_gunScript.IsKnife)
                    GameHost.Instance.message += "Play " + "21" + " " + transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + transform.position.y.ToString(CultureInfo.InvariantCulture) + " " + GetComponent<Player>()._playerNumber.ToString(CultureInfo.InvariantCulture) + "\n";
                else GameHost.Instance.message += "Play " + "20" + " " + transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + transform.position.y.ToString(CultureInfo.InvariantCulture) + " " + GetComponent<Player>()._playerNumber.ToString(CultureInfo.InvariantCulture) + "\n";

                _boxCollider2D.enabled = false;
                _boxCollider2D.enabled = true;
            }
        }
    }


    public void ScrollUp()
    {
        int aux = _currentGunID;

        while (_currentGunID < 8)
        {
            _currentGunID++;

            if (_currentGunID == 7 && _hasBomb)
                break;
            if (_currentGunID == 6 && _hasWall)
                break;
            if (_currentGunID == 5 && _hasSmoke)
                break;
            if (_currentGunID == 4 && _hasFlash)
                break;
            if (_currentGunID == 3 && _hasHe)
                break;
            if (_currentGunID == 2)
                break;
            if (_currentGunID == 1 && _hasPistol)
                break;
            if (_currentGunID == 0 && _gunType != -1)
                break;
        }

        if (_currentGunID > 7)
            _currentGunID = aux;
        else
        {
            if (_currentGunID <= 5)
            {
                SwitchWeapon(_currentGunID);
            }
            else if (_currentGunID == 6)
                SwitchWeapon(7);
            else if (_currentGunID == 7)
                SwitchWeapon(6);
        }
    }

    public void ScrollDown()
    {
        int aux = _currentGunID;

        while (_currentGunID >= 0)
        {
            _currentGunID--;

            if (_currentGunID == 7 && _hasBomb)
                break;
            if (_currentGunID == 6 && _hasWall)
                break;
            if (_currentGunID == 5 && _hasSmoke)
                break;
            if (_currentGunID == 4 && _hasFlash)
                break;
            if (_currentGunID == 3 && _hasHe)
                break;
            if (_currentGunID == 2)
                break;
            if (_currentGunID == 1 && _hasPistol)
                break;
            if (_currentGunID == 0 && _gunType != -1)
                break;
        }


        if (_currentGunID < 0)
            _currentGunID = aux;
        else
        {

            if (_currentGunID <= 5)
            {
                SwitchWeapon(_currentGunID);
            }
            else if (_currentGunID == 6)
                SwitchWeapon(7);
            else if (_currentGunID == 7)
                SwitchWeapon(6);
        }
    }

    public void SetHealth(float v, bool roundStart)
    {
        health += v;

        if (health > maxHealth)
            maxHealth = health;

        if (GameClient.Instance.players[GameClient.Instance.playerId] == this)
        {
            FightSceneManager.Instance.HealthText.text = health.ToString(CultureInfo.InvariantCulture);
        }
    }

    public void UpdateGunInfo(float v1, string v2, int bulletCount, int roundAmmo, string v3, string v4)
    {
        if (_controlling)
        {
            if (v1 == 0)
                FightSceneManager.Instance.ArmorObject.SetActive(false);
            else FightSceneManager.Instance.ArmorObject.SetActive(true);
        }
        else
        {
            if (health > maxHealth)
                maxHealth = health;

            setHealthBar.transform.localScale = new Vector3(float.Parse(v3, CultureInfo.InvariantCulture) / maxHealth, 1, 0);
            setArmorBar.transform.localScale = new Vector3(float.Parse(v4, CultureInfo.InvariantCulture) / maxHealth, 1, 0);
        }


        if (v2 == "6")
        {
            _pistolBulletCount = bulletCount;
            _pistolRoundAmmo = roundAmmo;
        }
        else if (int.Parse(v2, CultureInfo.InvariantCulture) > 0 && int.Parse(v2, CultureInfo.InvariantCulture) <= 5)
        {
            _gunBulletCount = bulletCount;
            _gunRoundAmmo = roundAmmo;
        }
    }

    public void UpdateOther(int cash, int gunType, Vector2 moveDir, string anim)
    {
        _cash = cash;

        if (GunScript.Type != gunType)
        {
            SetWeapon(gunType);
        }

        _movementDirection = moveDir;

        _animator.Play(anim);
    }

    #endregion

    #region Private Methods
    void Death()
    {
        if (_controlling)
            FightSceneManager.Instance.DeathInfo.SetActive(false);

        GameClient.Instance.AlivePlayers.Remove(this);

        gameObject.SetActive(false);
    }

    void OffWeaponList()
    {
        if (_showInv == false)
            FightSceneManager.Instance.WeaponList.SetActive(false);
    }

    void CanGrafiti()
    {
        _canGrafiti = true;
    }

    void Win()
    {
        GameClient.Instance.Send("StopDef " + _playerNumber.ToString(CultureInfo.InvariantCulture) + "\n");
        CancelInvoke("Win");
        _defusing = false;
        StopDefuse();

        GameHost.Instance.message += "Play " + "15" + "\n";

        GameHost.Instance.DefuseBomb();

        _bombDefusals++;

        NullTargetPosition();
    }

    void ThrowGren()
    {
        if (GameHost.Instance != null)
        {
            if (_gunScript.IsHe)
                _hasHe = false;
            else if (_gunScript.IsFlash)
                _hasFlash = false;
            else if (_gunScript.IsSmoke)
                _hasSmoke = false;
            else if (_gunScript.IsWall)
                _hasWall = false;

            _gunScript.ThrowGrenade(_grenTargetPos);
            _boxCollider2D.enabled = false;
            _boxCollider2D.enabled = true;
        }
    }

    void RespawnWeapon()
    {
        if (Gun != null)
            Destroy(Gun);

        if (_gunType != -1)
        {
            GameObject newGun = Instantiate(FightSceneManager.Instance.Guns[_gunType], gunParent.transform);
            Gun = newGun;
            Gun.GetComponent<SpriteRenderer>().enabled = limbsObj.transform.GetChild(0).gameObject.activeInHierarchy;
            _currentGunID = 0;

            _animator.Play("idle_gun", 0, 0);

            _gunScript.Setup(gameObject, _team, _gunBulletCount, _gunRoundAmmo);
        }
        else if (_hasPistol)
        {
            GameObject newGun = Instantiate(FightSceneManager.Instance.Guns[6], gunParent.transform);
            Gun = newGun;
            Gun.GetComponent<SpriteRenderer>().enabled = limbsObj.transform.GetChild(0).gameObject.activeInHierarchy;
            _currentGunID = 1;

            _animator.Play("idle_gun", 0, 0);

            _gunScript.Setup(gameObject, _team, _pistolBulletCount, _pistolRoundAmmo);
        }
        else
        {
            GameObject knife = Instantiate(FightSceneManager.Instance.Guns[0], gunParent.transform);
            Gun = knife;
            Gun.GetComponent<SpriteRenderer>().enabled = limbsObj.transform.GetChild(0).gameObject.activeInHierarchy;
            _currentGunID = 2;
            knife.transform.name = "sword";

            _animator.Play("idle_knife", 0, 0);

            _gunHitbox.KnifeSetup(_team, gameObject);
        }
    }

    void PlantFinish()
    {
        _animator.Play("idle_knife");
        RespawnWeapon();
    }

    void CantBuy()
    {
        _canBuyText.SetActive(true);

        Invoke("DeactivateCantBuy", 3f);
    }

    void DeactivateCantBuy()
    {
        _canBuyText.SetActive(false);
    }

    void CanPlay()
    {
        _canPlay = true;
    }

    void Update()
    {
        if (_controllable == false)
            return;

        bool attackKnife = _animator.GetCurrentAnimatorStateInfo(0).IsName("attack_knife");
        bool bombPlant = _animator.GetCurrentAnimatorStateInfo(0).IsName("bomb_plant");
        bool runGun = _animator.GetCurrentAnimatorStateInfo(0).IsName("run_gun");
        bool runKnife = _animator.GetCurrentAnimatorStateInfo(0).IsName("run_knife");

        _timeAlive += Time.deltaTime;

        _rb.velocity = new Vector2(0, 0);

        if (_controlling)
        {
            if ((_canBuy == false || _inBuyZone == false) && FightSceneManager.Instance.BuyMenu.activeInHierarchy)
            {
                bool active = true;
                FightSceneManager.Instance.BuyMenu.SetActive(!active);
                FightSceneManager.Instance.CharacterInfo.SetActive(active);

                CantBuy();
            }

            foreach (Image obj in FightSceneManager.Instance.WeaponImages)
            {
                obj.color = new Color32(255, 255, 255, 128);
            }

            if (Gun != null)
            {
                if (_gunScript.Type == 6)
                {
                    FightSceneManager.Instance.WeaponImages[1].color = new Color32(255, 255, 255, 255);
                }
                else if (_gunScript.Type == 0)
                {
                    FightSceneManager.Instance.WeaponImages[2].color = new Color32(255, 255, 255, 255);
                }
                else if (_gunScript.Type <= 5 && _gunScript.Type >= 1)
                {
                    FightSceneManager.Instance.WeaponImages[0].color = new Color32(255, 255, 255, 255);
                }
                else if (_gunScript.Type == 7)
                {
                    FightSceneManager.Instance.WeaponImages[3].color = new Color32(255, 255, 255, 255);
                }
                else if (_gunScript.Type == 8)
                {
                    FightSceneManager.Instance.WeaponImages[4].color = new Color32(255, 255, 255, 255);
                }
                else if (_gunScript.Type == 9)
                {
                    FightSceneManager.Instance.WeaponImages[5].color = new Color32(255, 255, 255, 255);
                }
                else if (_gunScript.Type == 10)
                {
                    FightSceneManager.Instance.WeaponImages[6].color = new Color32(255, 255, 255, 255);
                }
                else if (_gunScript.Type == 11)
                {
                    FightSceneManager.Instance.WeaponImages[7].color = new Color32(255, 255, 255, 255);
                }
            }
        }

        _onlineOldPos = _onlineNewPos;
        _onlineNewPos = transform.position;
        if (GameHost.Instance != null && _target != null && _shifting == false && _freezed == false && _defusing == false && (runKnife || runGun))
        {
            if (_canPlay)
            {
                _canPlay = false;
                Invoke("CanPlay", 0.35f);
                GameHost.Instance.message += "Play " + "14" + " " + transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + transform.position.y.ToString(CultureInfo.InvariantCulture) + " " + GetComponent<Player>()._playerNumber.ToString(CultureInfo.InvariantCulture) + "\n";
            }
            else
            {
                _silentSteps++;
            }
        }

        if (GameHost.Instance == null && _controllable && _timeAlive > 3 && _dead == false)
        {
            transform.position += _movementDirection * Time.deltaTime;
        }

        if (_interpolationTime != -1 && _controllable)
        {
            _interpolationTime += Time.deltaTime;

            float time = _interpolationTime / (0.05f);
            if (time > 1)
                time = 1;

            transform.position = Vector3.Lerp(_interpolationCurrentPos, _interpolationTarget, time);

            if (time == 1)
                _interpolationTime = -1;
        }

        if (_dead == false)
        {
            if (_controllable)
            {
                if (_defusing == false && attackKnife == false && bombPlant == false && _target != null && GameHost.Instance != null)
                {
                    if (_freezed)
                    {
                        Vector2 dif = _target.transform.position - transform.position;
                        dif.Normalize();

                        float rot = Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg;
                        transform.localEulerAngles = new Vector3(0, 0, rot + 90);

                        NullTargetPosition();
                    }
                    else
                    {
                        if (Vector2.Distance(transform.position, _target.transform.position) < 0.5f)
                        {
                            NullTargetPosition();

                            if (_gunScript.IsGun)
                                _animator.Play("idle_gun");
                            else _animator.Play("idle_knife");
                        }
                        else
                        {
                            if (_gunScript.IsGun)
                                _animator.Play("run_gun");
                            else _animator.Play("run_knife");

                            if (_shifting)
                                transform.position = Vector2.MoveTowards(transform.position, _target.transform.position, (speed - _gunScript.Slowness) / 2 * Time.deltaTime);
                            else transform.position = Vector2.MoveTowards(transform.position, _target.transform.position, (speed - _gunScript.Slowness) * Time.deltaTime);

                            Vector2 dif = _target.transform.position - transform.position;
                            dif.Normalize();

                            float rot = Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg;
                            transform.localEulerAngles = new Vector3(0, 0, rot + 90);
                        }
                    }
                }
                else if (_defusing == false && attackKnife == false && bombPlant == false && GameHost.Instance != null)
                {
                    if (_gunScript.IsGun)
                        _animator.Play("idle_gun");
                    else _animator.Play("idle_knife");
                }
            }

            if (_controlling)
            {
                if (Chat.Instance.Focused == false && ConsoleCanvas.Instance.Content.activeInHierarchy == false)
                {
                    if (Input.GetKeyDown(KeyCode.B) && _inBuyZone)
                    {
                        if (_canBuy)
                        {
                            bool active = FightSceneManager.Instance.BuyMenu.activeInHierarchy;
                            FightSceneManager.Instance.BuyMenu.SetActive(!active);
                            FightSceneManager.Instance.CharacterInfo.SetActive(active);
                            GameClient.Instance.Send("DropTarget" + " " + _playerNumber.ToString(CultureInfo.InvariantCulture));
                        }
                        else CantBuy();
                    }
                    if (attackKnife == false)
                    {
                        if (Input.GetKeyDown(KeyCode.Alpha1))
                        {
                            FightSceneManager.Instance.WeaponList.SetActive(true);
                            CancelInvoke("OffWeaponList");
                            Invoke("OffWeaponList", 5f);
                            GameClient.Instance.Send("Switch " + _playerNumber.ToString(CultureInfo.InvariantCulture) + " " + "0" + "\n");
                        }
                        if (Input.GetKeyDown(KeyCode.Alpha2))
                        {
                            FightSceneManager.Instance.WeaponList.SetActive(true);
                            CancelInvoke("OffWeaponList");
                            Invoke("OffWeaponList", 5f);
                            GameClient.Instance.Send("Switch " + _playerNumber.ToString(CultureInfo.InvariantCulture) + " " + "1" + "\n");

                        }
                        if (Input.GetKeyDown(KeyCode.Alpha3))
                        {
                            FightSceneManager.Instance.WeaponList.SetActive(true);
                            CancelInvoke("OffWeaponList");
                            Invoke("OffWeaponList", 5f);
                            GameClient.Instance.Send("Switch " + _playerNumber.ToString(CultureInfo.InvariantCulture) + " " + "2" + "\n");
                        }
                        if (Input.GetKeyDown(KeyCode.Alpha4))
                        {
                            FightSceneManager.Instance.WeaponList.SetActive(true);
                            CancelInvoke("OffWeaponList");
                            Invoke("OffWeaponList", 5f);
                            GameClient.Instance.Send("Switch " + _playerNumber.ToString(CultureInfo.InvariantCulture) + " " + "3" + "\n");

                        }
                        if (Input.GetKeyDown(KeyCode.Alpha5))
                        {
                            FightSceneManager.Instance.WeaponList.SetActive(true);
                            CancelInvoke("OffWeaponList");
                            Invoke("OffWeaponList", 5f);
                            GameClient.Instance.Send("Switch " + _playerNumber.ToString(CultureInfo.InvariantCulture) + " " + "4" + "\n");

                        }
                        if (Input.GetKeyDown(KeyCode.Alpha6))
                        {
                            FightSceneManager.Instance.WeaponList.SetActive(true);
                            CancelInvoke("OffWeaponList");
                            Invoke("OffWeaponList", 5f);
                            GameClient.Instance.Send("Switch " + _playerNumber.ToString(CultureInfo.InvariantCulture) + " " + "5" + "\n");

                        }
                        if (Input.GetKeyDown(KeyCode.Alpha8))
                        {
                            FightSceneManager.Instance.WeaponList.SetActive(true);
                            CancelInvoke("OffWeaponList");
                            Invoke("OffWeaponList", 5f);
                            GameClient.Instance.Send("Switch " + _playerNumber.ToString(CultureInfo.InvariantCulture) + " " + "6" + "\n");

                        }
                        if (Input.GetKeyDown(KeyCode.Alpha7))
                        {
                            FightSceneManager.Instance.WeaponList.SetActive(true);
                            CancelInvoke("OffWeaponList");
                            Invoke("OffWeaponList", 5f);
                            GameClient.Instance.Send("Switch " + _playerNumber.ToString(CultureInfo.InvariantCulture) + " " + "7" + "\n");

                        }

                        if (Input.GetKeyDown(KeyCode.Z))
                        {
                            GameClient.Instance.Send("Graffiti " + _playerNumber.ToString(CultureInfo.InvariantCulture) + " " + _graffiti1.ToString(CultureInfo.InvariantCulture) + "\n");
                        }
                        if (Input.GetKeyDown(KeyCode.X))
                        {
                            GameClient.Instance.Send("Graffiti " + _playerNumber.ToString(CultureInfo.InvariantCulture) + " " + _graffiti2.ToString(CultureInfo.InvariantCulture) + "\n");
                        }
                        if (Input.GetKeyDown(KeyCode.C))
                        {
                            GameClient.Instance.Send("Graffiti " + _playerNumber.ToString(CultureInfo.InvariantCulture) + " " + _graffiti3.ToString(CultureInfo.InvariantCulture) + "\n");
                        }
                        if (Input.GetKeyDown(KeyCode.V))
                        {
                            GameClient.Instance.Send("Graffiti " + _playerNumber.ToString(CultureInfo.InvariantCulture) + " " + _graffiti4.ToString(CultureInfo.InvariantCulture) + "\n");
                        }

                        if (_mouseScroll)
                        {
                            if (Input.GetAxis("Mouse ScrollWheel") < 0f)
                            {
                                GameClient.Instance.Send("ScrollUp " + _playerNumber.ToString(CultureInfo.InvariantCulture) + "\n");
                            }
                            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                            {
                                GameClient.Instance.Send("ScrollDown " + _playerNumber.ToString(CultureInfo.InvariantCulture) + "\n");
                            }
                        }

                        if (Input.GetKeyDown(KeyCode.R))
                        {
                            GameClient.Instance.Send("Reload " + _playerNumber.ToString(CultureInfo.InvariantCulture) + "\n");
                        }

                        if (Input.GetKeyDown(KeyCode.G))
                        {
                            GameClient.Instance.Send("Throw " + _playerNumber.ToString(CultureInfo.InvariantCulture) + " " + Cursor.Position.x.ToString(CultureInfo.InvariantCulture) + " " + Cursor.Position.y.ToString(CultureInfo.InvariantCulture) + "\n");
                        }

                        if (_team == 0)
                        {
                            if (Input.GetKey(KeyCode.E))
                            {
                                GameClient.Instance.Send("Defuse " + _playerNumber.ToString(CultureInfo.InvariantCulture) + "\n");
                            }
                            if (Input.GetKeyUp(KeyCode.E))
                            {
                                GameClient.Instance.Send("StopDef " + _playerNumber.ToString(CultureInfo.InvariantCulture) + "\n");
                            }
                        }
                    }
                    if (Input.GetKeyDown(KeyCode.LeftShift))
                    {
                        GameClient.Instance.Send("Shift " + _playerNumber.ToString(CultureInfo.InvariantCulture) + "\n");
                    }
                    if (Input.GetKeyUp(KeyCode.LeftShift))
                    {
                        GameClient.Instance.Send("StopSh " + _playerNumber.ToString(CultureInfo.InvariantCulture) + "\n");
                    }
                }
                if (_defusing == false && FightSceneManager.Instance.BuyMenu.activeInHierarchy == false)
                {
                    if (attackKnife == false && bombPlant == false && Input.GetMouseButton(0) && Chat.Instance.Focused == false && ConsoleCanvas.Instance.Content.activeInHierarchy == false && (DragMegamap.Instance != null ? DragMegamap.Instance.gameObject.activeInHierarchy == false : true))
                    {
                        if ((_hasShot == false && _gunScript.Automatic == false) || _gunScript.Automatic)
                        {
                            _hasShot = true;

                            Vector2 dif = (Vector3)Cursor.Position - transform.position;
                            dif.Normalize();

                            float rot = Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg;

                            GameClient.Instance.Send("Shoot " + _playerNumber.ToString(CultureInfo.InvariantCulture) + " " + (rot + 90).ToString(CultureInfo.InvariantCulture) + "\n");
                        }
                    }
                    else _hasShot = false;


                    if (Chat.Instance.Focused == false && ConsoleCanvas.Instance.Content.activeInHierarchy == false)
                    {
                        if (Input.GetMouseButtonUp(0) && DragMegamap.Instance == null)
                        {
                            GameClient.Instance.Send("ThrowNade " + _playerNumber.ToString(CultureInfo.InvariantCulture) + " " + Cursor.Position.x.ToString(CultureInfo.InvariantCulture) + " " + Cursor.Position.y.ToString(CultureInfo.InvariantCulture) + "\n");
                        }

                        if (attackKnife == false && bombPlant == false && Input.GetMouseButton(1))
                        {
                            GameClient.Instance.Send("PlayerTarget" + " " + _playerNumber.ToString(CultureInfo.InvariantCulture) + " " + Cursor.Position.x.ToString(CultureInfo.InvariantCulture) + " " + Cursor.Position.y.ToString(CultureInfo.InvariantCulture));
                        }
                    }
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space) && _controlling && Chat.Instance.Focused == false && ConsoleCanvas.Instance.Content.activeInHierarchy == false)
            {
                FightSceneManager.Instance.PlayerFollow.AddException(gameObject);
                FightSceneManager.Instance.PlayerFollow.ClearTarget();
                _controlling = false;
            }
            if (GameHost.Instance != null)
            {
                _rb.velocity = new Vector2(0, 0);
            }
        }
    }

    public void SetName(string v)
    {
        _nameText.text = v;
    }

    public void SetVisibility(bool visibility)
    {
        foreach (Transform child in limbsObj.transform)
        {
            if (child.name.Contains("Gun"))
            {
                foreach (Transform child2 in child)
                {
                    if (Team == GameClient.Instance.Team)
                        child2.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 128);
                    else child2.GetComponent<SpriteRenderer>().enabled = visibility;
                }
            }
            else
            {
                if (Team == GameClient.Instance.Team)
                    child.gameObject.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 128);
                else child.gameObject.SetActive(visibility);
            }
        }
    }

    #endregion
}
