using UnityEngine;
using System.Collections;

public class EnemyExplode : MonoBehaviour {

    public float speed;
    EnemyBase enemyBase;
    Rigidbody2D rbody;

    void Start()
    {
        enemyBase = GetComponent<EnemyBase>();
        rbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (enemyBase.playerInSight)
        {
            Vector2 direction = enemyBase.angleToPlayer * Vector2.right;
            direction.Normalize();
            rbody.velocity = direction * speed;
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, enemyBase.angleToPlayer, 0.1f);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            StartCoroutine(Explode());       
        }
    }

    IEnumerator Explode()
    {
        yield return new WaitForSeconds(0.1f);
        GameObject.Find("Player").GetComponent<PlayerController>().Damage(enemyBase.damage);
        GameObject.Find("LevelManager").GetComponent<AudioManager>().Explosion();
        enemyBase.explosionLength = 1;
        enemyBase.explosionSpread = 0;
        enemyBase.health = 0;
    }
}
