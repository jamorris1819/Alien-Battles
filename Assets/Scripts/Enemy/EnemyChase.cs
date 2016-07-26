using UnityEngine;
using System.Collections;

public class EnemyChase : MonoBehaviour {

    public float speed;
    public bool chase = true;
    public float proximity;
    EnemyBase enemyBase;
    Rigidbody2D rbody;

    void Start()
    {
        enemyBase = GetComponent<EnemyBase>();
        rbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (chase)
        {
            if (enemyBase.playerInSight)
            {
                if (proximity > 0)
                {
                    if (Vector3.Distance(transform.position, enemyBase.playerPosition) > proximity)
                    {
                        Vector2 direction = enemyBase.angleToPlayer * Vector2.right;
                        direction.Normalize();
                        rbody.velocity = direction * speed;
                    }
                }
                else
                {
                    Vector2 direction = enemyBase.angleToPlayer * Vector2.right;
                    direction.Normalize();
                    rbody.velocity = direction * speed;
                }
            }
        }
        else
            rbody.velocity = Vector2.zero;

        transform.rotation = Quaternion.Lerp(transform.rotation, enemyBase.angleToPlayer, 0.1f);
    }
}
