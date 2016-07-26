using UnityEngine;
using System.Collections;

public class EnemyStrategic : MonoBehaviour {

    public PolygonCollider2D area;
    public float span;
    public float minDistance;

    public Transform[] ppoints;

    EnemyBase enemyBase;
    Rigidbody2D rbody;

    Vector3 targetLocation;
    Vector3 searchBaseLocation;

    void Start()
    {
        enemyBase = GetComponent<EnemyBase>();
        rbody = GetComponent<Rigidbody2D>();
    }
	
	void Update () {
        if (targetLocation == Vector3.zero)
        {
            FindLocation();
        }

        if (!SafeDistance(transform.position))
        {
            Debug.Log("Search");
            StartCoroutine(FindNewLocationNearby());
        }
        GetComponent<EnemyChase>().chase = SafeDistance(transform.position);
        if(!SafeDistance(transform.position))
            rbody.velocity = ((Vector3)(targetLocation - transform.position)).normalized * 4f;
	}

    void FindLocation()
    {
        bool validLocation = false;
        while (!validLocation)
        {
            float x = Random.Range(-span, span);
            float y = Random.Range(-span, span);
            if (Physics2D.OverlapPoint(transform.position + new Vector3(x, y)) == area.GetComponent<Collider2D>())
            {
                Debug.Log("found");
                validLocation = true;
                targetLocation = transform.position + new Vector3(x, y);
                transform.position = targetLocation;
            }
        }
    }

    IEnumerator FindNewLocationNearby()
    {
        searchBaseLocation = enemyBase.playerPosition;
        yield return new WaitForSeconds(0f);
        float searchRange = 0.1f;
        bool foundLocation = false;
        while (!foundLocation)
        {
            Vector3[] points = new Vector3[] { new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1), new Vector2(-1, -1), new Vector2(-1, 0), new Vector2(-1, 1) };
            for (int i = 0; i < points.Length; i++)
            {
                float multiplier = (i % 2 == 0) ? searchRange * 2f : Mathf.Sqrt(2 * Mathf.Pow(searchRange, 2));
                Vector3 actualPoint = transform.position + (points[i] * multiplier);
                //yield return new WaitForSeconds(0.3f);
                if (Physics2D.OverlapPoint(actualPoint) == area.GetComponent<Collider2D>() && SafeDistance(actualPoint))
                {
                    //Instantiate(ppoints[1], actualPoint, Quaternion.identity);
                    foundLocation = true;
                    targetLocation = actualPoint;
                    break;
                }
            }
            searchRange += 0.1f;
        }
    }

    bool SafeDistance(Vector3 pos)
    {
        return Vector3.Distance(pos, enemyBase.playerPosition) >= minDistance;
    }
}
