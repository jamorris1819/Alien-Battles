using UnityEngine;
using System.Collections;

public class EnemyLauncher : MonoBehaviour {

    EnemyNodeFollower enf;
    public Transform grenade;
    Transform ourGrenade;
    bool waiting = false;

    void Start()
    {
        enf = GetComponent<EnemyNodeFollower>();
    }

    IEnumerator Land()
    {
        if (enf != null)
            enf.enabled = true;
        yield return new WaitForSeconds(0.8f);
        waiting = false;;
        ourGrenade.GetComponent<Rigidbody2D>().drag = 5f;
    }

    void Update()
    {
        if (!enf.enabled && !waiting)
        {
            ourGrenade = (Transform)Instantiate(grenade, transform.position, Quaternion.identity);
            Vector3 target = GameObject.Find("Player").transform.position;
            ourGrenade.GetComponent<Rigidbody2D>().velocity = (target - transform.position);
            GetComponent<Animator>().SetTrigger("throw");
            StartCoroutine(Land());
            waiting = true;
        }
    }
}
