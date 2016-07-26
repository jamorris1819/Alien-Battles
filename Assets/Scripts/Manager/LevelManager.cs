using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {

    public AudioClip song;

    public RuntimeAnimatorController[] animators;
    public Transform[] bullets;
    public Sprite[] weaponIcon;

    public Camera camera;
    public CameraFollow cameraFollow;

    public Transform flash;

    public PlayerController player;

    public AudioManager audio;

    public Transform[] effects;

    Weapon pistol;
    Weapon rifle;
    Weapon plasma;
    Weapon flamethrower;
    Weapon laser;

    void Awake()
    {
        pistol = new Weapon(0.3f, "Pistol", 0.25f, animators[0], bullets[0], 8f, weaponIcon[0], WeaponType.Pistol, 2);
        rifle = new Weapon(0.1f, "Rifle", 0.35f, animators[1], bullets[1], 12f, weaponIcon[1], WeaponType.Rifle, 5);
        plasma = new Weapon(0.1f, "Plasma", 0.3f, animators[2], bullets[2], 12f, weaponIcon[2], WeaponType.Plasma, 3);
        flamethrower = new Weapon(0.5f, "Flamethrower", 0.5f, animators[3], bullets[3], 4f, weaponIcon[3], WeaponType.Flamethrower, 0);
        laser = new Weapon(0.035f, "Laser", 0.2f, animators[4], bullets[4], 16f, weaponIcon[4], WeaponType.Laser, 2);
    }

    void Start()
    {
        audio = GetComponent<AudioManager>();
        audio.PlaySong(song);
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        camera = Camera.main;
        cameraFollow = camera.GetComponent<CameraFollow>();
    }

    public Weapon GetWeapon(WeaponType type)
    {
        switch (type)
        {
            case WeaponType.Pistol:
                return pistol;
            case WeaponType.Rifle:
                return rifle;
            case WeaponType.Plasma:
                return plasma;
            case WeaponType.Flamethrower:
                return flamethrower;
            case WeaponType.Laser:
                return laser;
            default:
                return pistol;
        }
    }

    public void Shoot()
    {
        if (player.currentWeapon.type == WeaponType.Laser)
            audio.Laser();
        else
            audio.Shoot();
    }

    public bool playerLocked
    {
        get { return player.locked; }
        set { player.locked = value; }
    }

    public void CreateFlash(Vector2 position, float duration)
    {
        Transform f = (Transform)Instantiate(flash, position, Quaternion.identity);
        f.GetComponent<Flash>().duration = duration;
    }

    public void AddScore(float score)
    {
        player.score += score;
    }

    public void Impact(Vector2 position)
    {
        Instantiate(effects[Random.Range(0, 3)], position, Quaternion.identity);
    }

    public void Explosion(Vector2 position)
    {
        Instantiate(effects[3], position, Quaternion.identity);
    }

    public void Explosion2(Vector2 position)
    {
        Instantiate(effects[4], position, Quaternion.identity);
    }
}
