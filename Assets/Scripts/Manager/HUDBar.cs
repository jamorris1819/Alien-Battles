using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUDBar : MonoBehaviour {

    public Sprite[] healthSprites;
    public Sprite[] ammoSprites;
    public Image healthImage;
    public Image ammoImage;
    public Text ammo;
    public Text lives;
    public Text score;
    public Image gunSprite;
    PlayerController player;


    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        healthImage.sprite = healthSprites[Mathf.Clamp(healthSprites.Length - (int)player.health - 1, 0, healthSprites.Length)];
        ammoImage.sprite = ammoSprites[Mathf.Clamp(ammoSprites.Length - (int)player.bullets - 1, 0, ammoSprites.Length)];
        lives.text = "Lives: " + PlayerController.lives.ToString();
        score.text = "Score: " + (int)player.score;
        gunSprite.sprite = player.currentWeapon.icon;
    }
}
