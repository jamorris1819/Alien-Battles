using UnityEngine;
using System.Collections;

public class BulletCasing : MonoBehaviour {

    void Start()
    {
        StartCoroutine(Stay());
    }

    IEnumerator Stay()
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(GetComponent<Rigidbody2D>());
        Destroy(GetComponent<BoxCollider2D>());
    }
}
