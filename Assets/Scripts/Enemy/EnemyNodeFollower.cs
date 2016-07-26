using UnityEngine;
using System.Collections;

public class EnemyNodeFollower : MonoBehaviour {

    public float speed;
    public Transform[] nodes;
    public float pauseAtNode;
    public bool enabled = true;

    EnemyBase enemyBase;
    int nodeIndex = 0;
    Rigidbody2D rbody;
    Quaternion angleToPlayer;

    void Start()
    {
        enemyBase = GetComponent<EnemyBase>();
        rbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (enabled)
        {
            Vector2 direction = nodes[nodeIndex].position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            angleToPlayer = Quaternion.AngleAxis(angle, Vector3.forward);

            direction = angleToPlayer * Vector2.right;
            direction.Normalize();
            rbody.velocity = direction * speed;

            transform.rotation = Quaternion.Lerp(transform.rotation, angleToPlayer, 0.1f);

            if (Vector3.Distance(transform.position, nodes[nodeIndex].position) < 0.1f)
            {
                enabled = false;
                StartCoroutine(NextNode());
            }
        }
        else
            transform.rotation = Quaternion.Lerp(transform.rotation, enemyBase.angleToPlayer, 0.1f);
    }

    IEnumerator NextNode()
    {
        rbody.velocity = Vector2.zero;
        yield return new WaitForSeconds(pauseAtNode);
        if (nodeIndex < nodes.Length - 1)
            nodeIndex++;
        else
            nodeIndex = 0;
        enabled = true;
    }
}
