using UnityEngine;
using System.Collections;

public class Explosive : MonoBehaviour {

    public bool triggered = false;
    public bool autoExplode;
    public int damage;
    public float delay;
    public AudioClip explosionSound;
    public Transform grenadeShadow;
    Transform shadow;

    void Start()
    {
        if (autoExplode)
        {
            StartCoroutine(Explode());
        }
        if (grenadeShadow != null)
        {
            shadow = (Transform)Instantiate(grenadeShadow, transform.position, Quaternion.identity);
        }
    }

    void Update()
    {
        if (shadow != null)
        {
            shadow.position = transform.position;
            shadow.position -= new Vector3(0, (transform.localScale.x - 1)) * 0.25f;
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Player")
        {
            if (!triggered)
            {
                triggered = true;
                StartCoroutine(Explode());
            }
        }
    }

    IEnumerator Explode()
    {
        yield return new WaitForSeconds(delay);
        if(GetComponent<CircleCollider2D>().IsTouching(GameObject.Find("Player").GetComponent<CircleCollider2D>()))
            GameObject.Find("LevelManager").GetComponent<LevelManager>().player.Damage(damage);
        GameObject.Find("LevelManager").GetComponent<AudioManager>().Play(explosionSound);
        GameObject.Find("LevelManager").GetComponent<LevelManager>().Explosion2(transform.position);
        GetComponent<SpriteRenderer>().enabled = false;
        if (shadow != null)
            shadow.GetComponent<SpriteRenderer>().enabled = false;
        for (int i = 0; i < 5; i++)
        {
            GameObject.Find("LevelManager").GetComponent<LevelManager>().cameraFollow.Shake(0.04f);
            yield return new WaitForSeconds(0.1f);
        }
        if(shadow != null)
            Destroy(shadow.gameObject);
        Destroy(gameObject);
    }
}
