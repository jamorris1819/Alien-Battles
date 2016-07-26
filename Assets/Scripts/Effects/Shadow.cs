using UnityEngine;
using System.Collections;

public class Shadow : MonoBehaviour {

    public Vector2 offset = new Vector2(0.015f, -0.015f);
    GameObject shadow;

    void Start()
    {
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        shadow = new GameObject(transform.name + " shadow");
        shadow.transform.parent = GameObject.Find("Shadows").transform;
        shadow.AddComponent<SpriteRenderer>();
        shadow.GetComponent<SpriteRenderer>().sprite = sprite;
        shadow.GetComponent<SpriteRenderer>().color = Color.black;
        shadow.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder - 1;
    }

    void Update()
    {
        shadow.transform.position = transform.position + (Vector3)offset;
        shadow.transform.rotation = transform.rotation;
        shadow.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
    }

    void OnDestroy()
    {
        Destroy(shadow);
    }
}
