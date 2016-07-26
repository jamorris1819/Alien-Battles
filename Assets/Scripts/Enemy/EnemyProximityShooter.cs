using UnityEngine;
using System.Collections;

public class EnemyProximityShooter : MonoBehaviour {

    public float shootInterval;
    public float shootDistance;
    public Transform bullet;
    public int bulletSpeed;

    EnemyBase enemyBase;
    EnemyChase enemyChase;
    Animator animator;
    bool canShoot = true;

    void Start()
    {
        enemyBase = GetComponent<EnemyBase>();
        enemyChase = GetComponent<EnemyChase>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        animator.SetBool("moving", enemyChase.chase);
        if (enemyBase.distance < shootDistance && enemyBase.distance != 0)
        {
            if (canShoot)
            {
                canShoot = false;
                animator.SetTrigger("shoot");
                Transform b = (Transform)Instantiate(bullet, transform.position, enemyBase.angleToPlayer);
                b.position += transform.rotation * new Vector2(0.3f, 0);
                b.GetComponent<Rigidbody2D>().velocity = ((Vector3)enemyBase.playerPosition - transform.position).normalized * bulletSpeed;
                StartCoroutine(Reset());
            }
            
            enemyChase.chase = false;
        }
        else
            enemyChase.chase = true;
    }

    IEnumerator Reset()
    {
        yield return new WaitForSeconds(shootInterval + Random.Range(-1, 1));
        canShoot = true;
    }
}
