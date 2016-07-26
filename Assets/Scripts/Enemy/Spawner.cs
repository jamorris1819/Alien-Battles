using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour {

    public Transform enemyToSpawn;
    public int numberToSpawn;
    public float radius;

    public List<Transform> children;

    public bool boss = false;
    bool active = false;
    public int time;
    float deltaTime;
    public int limit;

    void Start()
    {
        children = new List<Transform>();
        if (boss)
            return;
        int spawned = 0;
        while (spawned < numberToSpawn)
        {
            Vector3 spawnLocation = transform.position + new Vector3(((2f * Random.value) - 1f) * radius, ((2f * Random.value) - 1f) * radius, 0);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, spawnLocation, Vector3.Distance(transform.position, spawnLocation) * 1.2f);
            if (hit == null || hit.transform == null)
            {
                children.Add((Transform)Instantiate(enemyToSpawn, spawnLocation, Quaternion.identity));
                spawned++;
            }
        }
    }
	
    void Update()
    {
        if (boss && active)
        {
            deltaTime -= Time.deltaTime;
            if (deltaTime < 0)
            {
                deltaTime = time;
                if (AliveChildren >= limit)
                    return;
                for (int i = 0; i < numberToSpawn; i++)
                {
                    children.Add((Transform)Instantiate(enemyToSpawn, transform.position + new Vector3(((2f * Random.value) - 1f) * radius, ((2f * Random.value) - 1f) * radius, 0), Quaternion.identity));
                }
            }
        }
    }

    public void Begin()
    {
        active = true;
    }

    public void Stop()
    {
        active = false;
    }

    public int AliveChildren
    {
        get
        {
            return 0;
        }
    }
}
