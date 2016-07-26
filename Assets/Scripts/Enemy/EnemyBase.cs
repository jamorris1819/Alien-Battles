using UnityEngine;
using System.Collections;

public enum EnemyState { Attacking, Dying, Dead, Normal }

public class EnemyBase : MonoBehaviour {

    public float health;
    public int damage;
    public bool alive = true;
    public float explosionLength = 0;
    public float explosionSpread;
    public Transform splat;

    public Animator animator;

    public bool searchForPlayer;
    public bool playerInSight;
    public bool alwaysKnowLocation = false;
    public Vector3 playerPosition;
    public Quaternion angleToPlayer;
    public EnemyState currentState;
    public LevelManager levelManager;

    public float hitScore;
    public float killScore;

    public float distance;

    bool midDamage = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        angleToPlayer = transform.rotation;
    }

    void FixedUpdate()
    {
        if (health <= 0 && currentState != EnemyState.Dying)
        {
            levelManager.AddScore(killScore);
            if (explosionLength == 0)
            {
                currentState = EnemyState.Dead;
                if (splat != null)
                    Instantiate(splat, transform.position, transform.rotation);
                Object.Destroy(transform.gameObject);
            }
            else
            {
                currentState = EnemyState.Dying;
                StartCoroutine(Dying());
            }
            return;
        }

        if (currentState == EnemyState.Dead || currentState == EnemyState.Dying)
            return;

        if (searchForPlayer)
        {
            Vector2 direction = levelManager.player.transform.position - transform.position;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction);
            if (hit.collider.CompareTag("Player") || alwaysKnowLocation)
            {
                distance = direction.magnitude;
                playerInSight = true;
                playerPosition = hit.collider.transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                angleToPlayer = Quaternion.AngleAxis(angle, Vector3.forward);
            }
            else
                playerInSight = false;
        }
    }

    IEnumerator Dying()
    {
        if (explosionSpread == 0)
        {
            levelManager.Explosion2(transform.position);
        }
        else
        {
            for (int i = 0; i < explosionLength; i++)
            {
                Vector2 position = transform.position;
                position += new Vector2(Random.value * 2 - 1, Random.value * 2 - 1) * explosionSpread;
                levelManager.Explosion(position);
                if (i % 2 == 0)
                {
                    levelManager.audio.Explosion();
                    levelManager.cameraFollow.Shake(0.1f);
                }
                yield return new WaitForSeconds(0.1f);
            }
        }
        currentState = EnemyState.Dead;
        levelManager.Explosion(transform.position);
        if (splat != null)
            Instantiate(splat, transform.position, transform.rotation);
        Object.Destroy(transform.gameObject);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (currentState == EnemyState.Dead || currentState == EnemyState.Dying)
            return;

        if (collision.transform.CompareTag("Player"))
        {
            bool damaged = collision.transform.GetComponent<PlayerController>().Damage(damage);
            if (damaged)
                AnimateAttack();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Bullet"))
        {
            Bullet bullet = collision.transform.transform.GetComponent<Bullet>();
            if (bullet.owner == BulletOwner.Player)
            {
                levelManager.AddScore(hitScore);
                Damage(bullet.damage);
                GameObject.Destroy(bullet.gameObject);
            }
        }
    }

    void AnimateAttack(string name = "attack")
    {
        if (animator != null)
        {
            animator.SetTrigger(name);
            currentState = EnemyState.Attacking;
            StartCoroutine(ResetAttack());
        }
    }

    IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(0.5f);
        currentState = EnemyState.Normal;
        yield return new WaitForEndOfFrame();
    }

    IEnumerator Flash()
    {
        if (!midDamage)
        {
            midDamage = true;
            Renderer r = GetComponent<Renderer>();
            Color nc = r.material.color;
            r.material.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            r.material.color = nc;
            midDamage = false;
        }
    }

    public void Damage(float damage)
    {
        if (currentState == EnemyState.Dead || currentState == EnemyState.Dying)
            return;
        health -= damage;
        // Deal out score points.
        StartCoroutine(Flash());
        levelManager.audio.Damage();
        //System.Threading.Thread.Sleep(30);
    }
}
