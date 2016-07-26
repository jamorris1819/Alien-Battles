using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.name == "Player")
        {
            collider.GetComponent<PlayerController>().health = 35;
            if (GetComponent<GunPickup>() != null)
            {
                collider.GetComponent<PlayerController>().ChangeWeapon(GetComponent<GunPickup>().type);
            }
            GameObject.Find("LevelManager").GetComponent<AudioManager>().Pickup();
            Object.Destroy(transform.gameObject);
        }
    }
}
