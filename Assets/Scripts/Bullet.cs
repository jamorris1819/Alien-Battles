using UnityEngine;
using System.Collections;

public enum BulletOwner { Player, Enemy, None }

public class Bullet : MonoBehaviour {

    public float damage;
    public BulletOwner owner;

    void OnCollisionEnter2D(Collision2D c)
    {
        if (c.transform.tag == "Wall")
        {
            GameObject.Find("LevelManager").GetComponent<LevelManager>().Impact(c.contacts[0].point);
            Object.Destroy(transform.gameObject);
        }
        if (c.transform.tag == "Breakable")
        {
            if (c.transform.GetComponent<DropBox>() != null)
            {
                Instantiate(c.transform.GetComponent<DropBox>().DropItem, transform.position, Quaternion.identity);
            }
            Object.Destroy(c.gameObject);
            Object.Destroy(transform.gameObject);
        }
        if (c.transform.tag == "Enemy" && owner == BulletOwner.Enemy)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D c)
    {
        if (c.transform.tag == "Wall")
        {
            GameObject.Find("LevelManager").GetComponent<LevelManager>().Impact(c.transform.position);
            Object.Destroy(transform.gameObject);
        }
        if (c.transform.tag == "Enemy" && owner == BulletOwner.None)
        {
            c.GetComponent<EnemyBase>().Damage(damage);
        }
        if (c.transform.tag == "Breakable")
        {
            if (c.transform.GetComponent<DropBox>() != null)
            {
                Instantiate(c.transform.GetComponent<DropBox>().DropItem, transform.position, Quaternion.identity);
            }
            Object.Destroy(c.gameObject);
            Object.Destroy(transform.gameObject);
        }
    }
}
