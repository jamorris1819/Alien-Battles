using UnityEngine;
using System.Collections;

enum TurretState { Searching, Targetting, Destroyed }

public class Turret : MonoBehaviour {

    public float shootInterval;
    public Transform bullet;
    public float turretExtension;

    PlayerController player;
    LevelManager levelManager;

    EnemyBase enemyBase;

    Quaternion target;
    bool canShoot = true;

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        enemyBase = GetComponent<EnemyBase>();
    }
	
    void Update()
    {
        SetTarget(enemyBase.angleToPlayer);
       
        if (enemyBase.playerInSight)
        {
            if (canShoot && transform.GetComponent<Renderer>().isVisible)
            {
                Transform newBullet = (Transform)Instantiate(bullet, transform.position, Quaternion.identity);
                Vector3 direction = target * Vector2.right;
                direction.Normalize();
                newBullet.position = transform.position + (direction * turretExtension);
                newBullet.GetComponent<Rigidbody2D>().velocity = direction * 4f;
                newBullet.GetComponent<Rigidbody2D>().angularVelocity = 10f;
                canShoot = false;
                levelManager.audio.Shoot();
                StartCoroutine(ResetShoot());
            }
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, target, 0.1f);
    }

    IEnumerator ResetShoot()
    {
        yield return new WaitForSeconds(shootInterval);
        canShoot = true;
    }

    void SetTarget(Quaternion angle)
    {
        target = angle;
    }
}
