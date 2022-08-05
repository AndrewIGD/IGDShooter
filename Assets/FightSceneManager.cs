using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FightSceneManager : MonoBehaviour
{
    [SerializeField] WinLoseAnim winLose;
    [SerializeField] GameObject bomb;
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject he;
    [SerializeField] GameObject flash;
    [SerializeField] GameObject smoke;
    [SerializeField] GameObject wallGren;
    [SerializeField] GameObject wall;
    [SerializeField] GameObject wallVfx;
    [SerializeField] GameObject smokeVfx;
    [SerializeField] GameObject armorObject;
    [SerializeField] GameObject heExplosion;
    [SerializeField] Animator cameraAnimator;
    [SerializeField] Animator canvasAnimator;
    [SerializeField] GameObject[] weaponObjects;
    [SerializeField] GameObject[] graffities;
    [SerializeField] GameObject[] drops;
    [SerializeField] Text bulletText;
    [SerializeField] Text blueRounds;
    [SerializeField] Text redRounds;    
    [SerializeField] Text blueCount;
    [SerializeField] Text redCount;
    [SerializeField] GameObject boxMD;
    [SerializeField] GameObject ctPrefab;
    [SerializeField] GameObject tPrefab;
    [SerializeField] GameObject[] healthChildren;
    [SerializeField] GameObject[] armorChildren;
    [SerializeField] Buy[] buyButtons;
    [SerializeField] GameObject canBuyText;
    [SerializeField] GameObject roundTime;
    [SerializeField] GameObject warmup;
    [SerializeField] GameObject bulletImage;
    [SerializeField] GameObject megamapDetect;

    [SerializeField] PlayerFollow playerFollow;

    [SerializeField] GameObject characterInfo;

    [SerializeField] Text buyMenuTeamCash;

    [SerializeField] GameObject deathObj;

    [SerializeField] Text healthText;

    [SerializeField] Text armorText;

    [SerializeField] GameObject weaponList;

    [SerializeField] Image[] weaponImages;

    [SerializeField] GameObject cashText;

    [SerializeField] Text cashTextScript;

    [SerializeField] GameObject[] guns;

    [SerializeField] GameObject buyMenu;

    [SerializeField] GameObject[] activateAfterRoundStart;

    public static FightSceneManager Instance;

    public WinLoseAnim WinLose => winLose;
    public GameObject Bomb => bomb;
    public GameObject BulletPrefab => bullet;
    public GameObject HePrefab => he;
    public GameObject FlashPrefab => flash;
    public GameObject SmokePrefab => smoke;
    public GameObject WallPrefab => wallGren;
    public Animator CameraAnimator => cameraAnimator;
    public GameObject SmokeVfx => smokeVfx;
    public GameObject Wall => wall;
    public GameObject WallVfx => wallVfx;
    public GameObject ArmorObject => armorObject;
    public GameObject HeExplosion => heExplosion;
    public Animator CanvasAnimator => canvasAnimator;
    public GameObject[] WeaponUIObjects => weaponObjects;
    public GameObject[] Drops => drops;
    public GameObject[] Graffities => graffities;
    public GameObject BoxMD => boxMD;
    public GameObject Warmup => warmup;
    public Text BulletText => bulletText;
    public Text BlueRounds => blueRounds;
    public Text RedRounds => redRounds;
    public Text BlueCount => blueCount;
    public Text RedCount => redCount;
    public GameObject CTPrefab => ctPrefab;
    public GameObject TPrefab => tPrefab;
    public Text BuyMenuTeamCash => buyMenuTeamCash;
    public GameObject[] HealthChildren => healthChildren;
    public GameObject[] ArmorChildren => armorChildren;
    public GameObject CanBuyText => canBuyText;
    public GameObject BulletImage => bulletImage;
    public Buy[] BuyButtons => buyButtons;
    public GameObject MegamapDetect => megamapDetect;
    public PlayerFollow PlayerFollow => playerFollow;
    public Image[] WeaponImages => weaponImages;
    public GameObject DeathInfo => deathObj;
    public GameObject WeaponList => weaponList;
    public GameObject BuyMenu => buyMenu;
    public GameObject[] Guns => guns;
    public GameObject CashText => cashText;
    public GameObject CharacterInfo => characterInfo;
    public Text CashTextScript => cashTextScript;
    public Text HealthText => healthText;
    public Text ArmorText => armorText;
    public GameObject[] ActivateAfterRoundStart => activateAfterRoundStart;

    private void Awake()
    {
        Instance = this;
    }
}
