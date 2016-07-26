using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Delaunay;
using Delaunay.Geo;

public enum GenerationStage { GeneratingRooms, SpacingRooms, ChoosingRooms, GeneratingCorridors, GeneratingWalls, PopulatingRooms, Finished }

public class GenerateDungeon : MonoBehaviour {

    public GenerationStage generationStage;
    public Transform roomPrefab;
    public Transform tilePrefab;
    public Transform wallPrefab;
    public Transform spawnerPrefab;
    public int numberOfRooms;
    public Vector2 minRoomSize;
    public Vector2 maxRoomSize;
    public int xSpread;
    public int ySpread;
    public string seed;
    [Range(0, 1)]
    public float edgesToReturn;
    [HideInInspector]
    public Vector2 startingPoint;
    public List<Transform> enemies;
    public Transform turret;
    public List<Transform> pickups;

    List<Transform> rooms;
    List<Transform> chosenRooms;
    List<Vector2> points;
    List<LineSegment> spanningTree;
    List<LineSegment> corridors;
    List<Transform> corridorTiles;
    List<Transform> tilesToCheck;

    public delegate void EventOccur();
    public event EventOccur Generated;

    public void StartGeneration(int size)
    {
        numberOfRooms = size;
        StartGeneration();
    }

    public void StartGeneration()
    {
        rooms = new List<Transform>();
        chosenRooms = new List<Transform>();
        points = new List<Vector2>();
        spanningTree = new List<LineSegment>();
        corridors = new List<LineSegment>();
        corridorTiles = new List<Transform>();
        tilesToCheck = new List<Transform>();
        StartCoroutine(Generate());
    }

    IEnumerator Generate()
    {
        Time.timeScale = 10f;
        generationStage = GenerationStage.GeneratingRooms;
        // Generate the rooms.
        for (int i = 0; i < numberOfRooms; i++)
        {
            Transform room = (Transform)Instantiate(roomPrefab, new Vector3(Random.Range(-xSpread, xSpread), Random.Range(-ySpread, ySpread)), Quaternion.identity);
            room.GetComponent<GenerateRoom>().BeginGeneration(new Vector3(Random.Range(minRoomSize.x, maxRoomSize.x), Random.Range(minRoomSize.y, maxRoomSize.y)));
            rooms.Add(room);
        }

        // Wait for rooms to be generated.
        bool waitOnRooms = false;
        while (!waitOnRooms)
        {
            waitOnRooms = true;
            foreach (Transform room in rooms)
            {
                if (!room.GetComponent<GenerateRoom>().generated)
                {
                    waitOnRooms = false;
                    break;
                }
            }
            yield return new WaitForEndOfFrame();
        }

        generationStage = GenerationStage.SpacingRooms;
        yield return new WaitForSeconds(2f);
        generationStage = GenerationStage.ChoosingRooms;
        Vector2 mean = Vector2.zero;
        foreach (Transform room in rooms)
            mean += room.GetComponent<GenerateRoom>().dimensions;
        mean /= rooms.Count;

        foreach (Transform room in rooms)
            if (room.GetComponent<GenerateRoom>().dimensions.x >= mean.x && room.GetComponent<GenerateRoom>().dimensions.y >= mean.y)
                chosenRooms.Add(room);

        generationStage = GenerationStage.GeneratingCorridors;

        List<uint> colours = new List<uint>();
        for (int i = 0; i < chosenRooms.Count; i++)
            colours.Add(0);

        foreach (Transform room in rooms)
            if (chosenRooms.Contains(room))
                points.Add(room.position);

        Voronoi v = new Voronoi(points, colours, new Rect(-xSpread, -ySpread, xSpread * 2, ySpread * 2));
        spanningTree = v.SpanningTree();
        List<LineSegment> triangles = v.DelaunayTriangulation();

        int returned = 0;
        while(returned < (int)(edgesToReturn * triangles.Count))
        {
            LineSegment segment = triangles[Random.Range(0, triangles.Count)];
            if (!spanningTree.Contains(segment))
            {
                spanningTree.Add(segment);
                returned++;
            }
        }

        foreach (LineSegment segment in spanningTree)
        {
            Transform rooma = FindRoom(segment.p0);
            Transform roomb = FindRoom(segment.p1);

            Vector2? aa = rooma.transform.position;
            Vector2? ab = new Vector2(rooma.transform.position.x, roomb.transform.position.y);
            LineSegment a = new LineSegment(aa, ab);

            Vector2? ba = new Vector2(rooma.transform.position.x, roomb.transform.position.y);
            Vector2? bb = roomb.position; ;
            LineSegment b = new LineSegment(ba, bb);

            corridors.Add(a);
            corridors.Add(b);
        }

        foreach (LineSegment segment in corridors)
        {
            Transform target = FindRoom(segment.p1);
            Vector2 from = (Vector2)segment.p0;
            Vector2 direction = (Vector2)segment.p1 - (Vector2)segment.p0;
            float distance = Vector2.Distance((Vector2)segment.p0, (Vector2)segment.p1);

            for (int i = 0; i < 10; i++)
            {
                RaycastHit2D hit = Physics2D.Raycast(from, direction, distance);
                if (!chosenRooms.Contains(hit.transform))
                {
                    chosenRooms.Add(hit.transform);
                }
                else
                    break;
            }
        }

        foreach (Transform roomTransform in rooms)
            if (!chosenRooms.Contains(roomTransform))
                roomTransform.gameObject.SetActive(false);

        foreach (LineSegment segment in corridors)
        {
            for (float f = Vector2.Distance((Vector2)segment.p0, (Vector2)segment.p1) + 0.64f; f > 0; f -= 0.32f)
            {
                Vector2 direction = ((Vector2)segment.p1 - (Vector2)segment.p0).normalized;
                Transform t;
                t = (Transform)Instantiate(tilePrefab, (Vector2)segment.p0 + direction * f, Quaternion.identity);
                corridorTiles.Add(t);
                t = (Transform)Instantiate(tilePrefab, (Vector2)segment.p0 + (PerpendicularClockwise(direction).normalized * 0.32f) + (direction * f), Quaternion.identity);
                corridorTiles.Add(t);
                t = (Transform)Instantiate(tilePrefab, (Vector2)segment.p0 - (PerpendicularClockwise(direction).normalized * 0.32f) + (direction * f), Quaternion.identity);
                corridorTiles.Add(t);
                t = (Transform)Instantiate(tilePrefab, (Vector2)segment.p0 + (PerpendicularClockwise(direction).normalized * 0.32f) * 2f + (direction * f), Quaternion.identity);
                corridorTiles.Add(t);
                t = (Transform)Instantiate(tilePrefab, (Vector2)segment.p0 - (PerpendicularClockwise(direction).normalized * 0.32f) * 2f + (direction * f), Quaternion.identity);
                corridorTiles.Add(t);
            }
        }

        foreach (Transform r in rooms)
        {
            r.GetComponent<BoxCollider2D>().enabled = false;
            foreach (Transform t in r.GetComponent<GenerateRoom>().tiles)
                t.GetComponent<BoxCollider2D>().enabled = true;
        }

        foreach (Transform r in corridorTiles)
            if (r.GetComponent<StemTile>().check)
                r.GetComponent<BoxCollider2D>().enabled = true;

        foreach (Transform r in chosenRooms)
        {
            if (r != null)
            {
                if (r.GetComponent<GenerateRoom>() != null)
                    r.GetComponent<GenerateRoom>().Differentiate();
            }
        }

        generationStage = GenerationStage.GeneratingWalls;

        foreach (Transform r in corridorTiles)
            if (r.GetComponent<StemTile>().check)
                r.GetComponent<StemTile>().FormRoom();

        bool advance = false;

        while (!advance)
        {
            advance = true;
            foreach (Transform roomTransform in chosenRooms)
                if (roomTransform != null)
                    foreach (Transform t in roomTransform.GetComponent<GenerateRoom>().tilesToCheck)
                        if (t != null)
                        {
                            if (t.GetComponent<StemTile>().complete == false)
                            {
                                advance = false;
                                break;
                            }
                        }
            foreach (Transform t in corridorTiles)
                if (t != null)
                {
                    if (t.GetComponent<StemTile>().complete == false)
                    {
                        advance = false;
                        break;
                    }
                }

            yield return new WaitForEndOfFrame();
        }

        foreach (Transform roomTransform in chosenRooms)
            if (roomTransform != null)
                foreach (Transform t in roomTransform.GetComponent<GenerateRoom>().tiles)
                    if (t != null)
                    {
                        t.GetComponent<BoxCollider2D>().enabled = false;
                    }
        foreach (Transform t in corridorTiles)
            if (t != null)
            {
                t.GetComponent<BoxCollider2D>().enabled = false;
            }
        Time.timeScale = 1f;
        generationStage = GenerationStage.PopulatingRooms;
        startingPoint = Vector2.zero;
        foreach (Vector2 point in points)
        {
            if (startingPoint == Vector2.zero)
            {
                startingPoint = point;
                continue;
            }
            if (Random.Range(0, 10) < 7)
            {
                Transform spawner = (Transform)Instantiate(spawnerPrefab, point, Quaternion.identity);
                spawner.GetComponent<Spawner>().radius = FindRoom((Vector2?)point).GetComponent<GenerateRoom>().widthInUnits / 2;
                spawner.GetComponent<Spawner>().enemyToSpawn = enemies[Random.Range(0, enemies.Count)];
                spawner.GetComponent<Spawner>().numberToSpawn = Random.Range(2, 5);
            }
            else
                Instantiate(turret, point, Quaternion.identity);
            if (Random.Range(0, 10) >= 7)
            {
                Transform room = FindRoom(point);
                bool placed = false;
                while (!placed)
                {
                    Vector2 location = point + new Vector2(Random.Range(-(room.GetComponent<GenerateRoom>().widthInUnits / 2), room.GetComponent<GenerateRoom>().widthInUnits / 2), Random.Range(-(room.GetComponent<GenerateRoom>().heightInUnits / 2), room.GetComponent<GenerateRoom>().heightInUnits / 2));
                    RaycastHit2D hit = Physics2D.Raycast(point, location, Vector2.Distance(point, location));
                    if (hit == null || hit.transform == null)
                    {
                        Transform pickup = (Transform)Instantiate(pickups[Random.Range(0, pickups.Count)], location, Quaternion.identity);
                        placed = true;
                    }

                }
            }
        }

        generationStage = GenerationStage.Finished;
        TriggerEvent();
        yield return new WaitForEndOfFrame();
    }

    Transform FindRoom(Vector2? position)
    {
        foreach (Transform rm in rooms)
        {
            if (rm.position == (Vector3)position)
                return rm;
        }
        return null;
    }

    protected virtual void TriggerEvent()
    {
        if (Generated != null)
            Generated();
    }

    Vector2 PerpendicularClockwise(Vector2 vector2)
    {
        return new Vector2(-vector2.y, vector2.x);
    }
}
