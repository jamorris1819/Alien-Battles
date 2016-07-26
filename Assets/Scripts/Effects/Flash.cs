using UnityEngine;
using System.Collections;

public class Flash : MonoBehaviour {

    public float duration;

    void Start()
    {
        StartCoroutine(End());
    }

    IEnumerator End()
    {
        Renderer r = GetComponent<Renderer>();
        r.material.color = Color.gray;
        yield return new WaitForSeconds(duration / 2f);
        r.material.color = Color.white;
        yield return new WaitForSeconds(duration / 2f);
        Destroy(gameObject);
    }
}
