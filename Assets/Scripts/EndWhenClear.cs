using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EndWhenClear : MonoBehaviour {

    public List<Transform> enemies;
    bool loop = true;

    void Update()
    {
        if (!loop)
            return;
        int alive = 0;
        foreach (Transform t in enemies)
            if (t != null)
                alive++;
        if (alive == 0)
        {
            GameObject.Find("LevelManager").GetComponent<EndLevel>().TriggerEnd();
            loop = false;
        }
    }
}
