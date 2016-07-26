using UnityEngine;
using System.Collections;

public class EnemyDash : MonoBehaviour {

    public float speed;
    public float dashFrequency;
    public float dashSpeed;
    bool dashing = false;
    EnemyBase enemyBase;
    Rigidbody2D rbody;

    void Start()
    {
        enemyBase = GetComponent<EnemyBase>();
        rbody = GetComponent<Rigidbody2D>();
        StartCoroutine(Dash());
    }

    void Update()
    {
        enemyBase.searchForPlayer = !dashing;

        if (dashing)
        {

        }
        else
        {
            if (enemyBase.playerInSight)
            {
                Vector2 direction = enemyBase.angleToPlayer * Vector2.right;
                direction.Normalize();
                rbody.velocity = direction * speed;
            }

            transform.rotation = Quaternion.Lerp(transform.rotation, enemyBase.angleToPlayer, 0.1f);
        }
    }

    IEnumerator Dash()
    {
        yield return new WaitForSeconds(dashFrequency);
        dashing = true;
        rbody.velocity = Vector2.zero;
        yield return new WaitForSeconds(0.25f);
        Vector2 direction = enemyBase.angleToPlayer * Vector2.right;
        direction.Normalize();
        rbody.velocity = direction * dashSpeed;
        yield return new WaitForSeconds(1f);
        dashing = false;
        StartCoroutine(Dash());
    }
}
