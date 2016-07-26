using UnityEngine;
using System.Collections;

public class Planet : MonoBehaviour {

    void Start()
    {
        GetComponent<Rigidbody>().angularVelocity = Vector3.down * 0.2f;
    }
}
