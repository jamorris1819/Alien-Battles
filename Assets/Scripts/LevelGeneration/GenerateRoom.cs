using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenerateRoom : MonoBehaviour {

    public Transform floorTile;
    public Vector2 dimensions;

    [HideInInspector]
    public bool generated;
    [HideInInspector]
    public float widthInUnits;
    [HideInInspector]
    public float heightInUnits;
    [HideInInspector]
    public Transform[,] tiles;
    [HideInInspector]
    public List<Transform> tilesToCheck;

    public void BeginGeneration(Vector2 size)
    {
        dimensions = size;
        tilesToCheck = new List<Transform>();
        StartCoroutine(Generate());
    }

    IEnumerator Generate()
    {
        int width = (int)dimensions.x;
        int height = (int)dimensions.y;
        tiles = new Transform[width, height];

        // We want the centre of the object to be the actual centre.
        // So if it's an even number, we offset.
        float xOffset = width % 2 == 0 ? -0.16f : 0;
        float yOffset = height % 2 == 0 ? -0.16f : 0;
        widthInUnits = (width * 0.32f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Transform tile = (Transform)Instantiate(floorTile, transform.position, Quaternion.identity);
                tile.parent = transform;
                tile.localPosition = (new Vector3(x, y) - new Vector3(width / 2, height / 2)) * 0.32f;
                if (!(x > 0 && x < width - 1 && y > 0 && y < height - 1))
                    tilesToCheck.Add(tile);
                tiles[x, y] = tile;
            }
        }
        GetComponent<BoxCollider2D>().size = new Vector2(width, height) * 0.32f;
        generated = true;
        StartCoroutine(Freeze());
        yield return new WaitForEndOfFrame();
    }

    IEnumerator Freeze()
    {
        yield return new WaitForSeconds(1f);
        Destroy(GetComponent<Rigidbody2D>());
        transform.position = new Vector3(Mathf.Ceil(transform.position.x * 100f / 32f), Mathf.Ceil(transform.position.y * 100f / 32f)) * 32f / 100f;
    }

    public void Differentiate()
    {
        foreach (Transform tile in tilesToCheck)
            tile.GetComponent<StemTile>().FormRoom();
    }
}
