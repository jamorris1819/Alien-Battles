using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DestroyWhenClear : MonoBehaviour {

    public List<Transform> enemies;

    void Update()
    {
        int alive = 0;
        foreach (Transform t in enemies)
            if (t != null)
                alive++;
        if (alive == 0)
            Destroy(gameObject);
    }
}
