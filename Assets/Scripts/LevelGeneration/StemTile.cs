using UnityEngine;
using System.Collections;

public class StemTile : MonoBehaviour {

    public Transform wall;
    [HideInInspector]
    public bool check = true;
    public bool complete = false;
    public Transform hit;
    public Transform[] boxes;

    public void FormRoom()
    {
        if (check)
            StartCoroutine(FormWalls());
    }

    IEnumerator FormWalls()
    {
        yield return new WaitForSeconds(0f);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(1, 0), 0.32f);
        this.hit = hit.transform;

        if (this.hit == null)
        {
            Instantiate(wall, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        hit = Physics2D.Raycast(transform.position, new Vector2(-1, 0), 0.32f);
        this.hit = hit.transform;

        if (this.hit == null)
        {
            Instantiate(wall, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        hit = Physics2D.Raycast(transform.position, new Vector2(0, 1), 0.32f);
        this.hit = hit.transform;

        if (this.hit == null)
        {
            Instantiate(wall, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        hit = Physics2D.Raycast(transform.position, new Vector2(0, -1), 0.32f);
        this.hit = hit.transform;

        if (this.hit == null)
        {
            Instantiate(wall, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        int num = 0;

        hit = Physics2D.Raycast(transform.position, new Vector2(1, 0), 0.32f);
        if (hit.transform != null)
            if (hit.transform.tag == "Wall")
                num++;
        hit = Physics2D.Raycast(transform.position, new Vector2(-1, 0), 0.32f);
        if (hit.transform != null)
            if (hit.transform.tag == "Wall")
                num++;
        hit = Physics2D.Raycast(transform.position, new Vector2(0, 1), 0.32f);
        if (hit.transform != null)
            if (hit.transform.tag == "Wall")
                num++;
        hit = Physics2D.Raycast(transform.position, new Vector2(0, -1), 0.32f);
        if (hit.transform != null)
            if (hit.transform.tag == "Wall")
                num++;

        if (num == 2)
        {
            Instantiate(wall, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        if (Random.Range(0, 100) == 1)
        {
            Instantiate(boxes[Random.Range(0, boxes.Length)], transform.position, Quaternion.identity);
        }
        complete = true;
    }
}
