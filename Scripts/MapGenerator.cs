using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Transform tilePrefab;
    public Vector2 mapSize;
   public Transform obsPrefab;

    [Range(0, 1)]
    public float outLinePercent;
    public int seed = 10;

    List<Cordinate> allTilesCordinates;
    Queue<Cordinate> shuffledTilesCordinate;
    private void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        allTilesCordinates = new List<Cordinate>();
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                allTilesCordinates.Add(new Cordinate(x, y));
            }
        }
        shuffledTilesCordinate = new Queue<Cordinate>(Utlity.ShuffleArray(allTilesCordinates.ToArray(),seed));
        string holderName = "Generated Map";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }
        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        for(int x = 0; x < mapSize.x; x++)
        {
            for(int y = 0; y < mapSize.y; y++)
            {
                Vector3 tilePos = CordinateToPosition(x, y);
                Transform newTile = Instantiate(tilePrefab, tilePos, Quaternion.Euler(Vector3.right * 90)) as Transform;
                newTile.localScale = Vector3.one*(1 - outLinePercent);
                newTile.parent = mapHolder;
            }
        }
        int obsCount = 10;
        for(int i = 0; i < obsCount; i++)
        {
            Cordinate randomCordinate = GetrandomCordinate();
            Vector3 obsPos = CordinateToPosition(randomCordinate.x, randomCordinate.y);
            Transform newObs = Instantiate(obsPrefab, obsPos+Vector3.up *.5f, Quaternion.identity) as Transform;
            newObs.parent = mapHolder;
        }
    }

    Vector3 CordinateToPosition(int x,int y)
    {
        return new Vector3(-mapSize.x / 2 + 0.5f + x, 0, -mapSize.y / 2 + 0.5f + y);
    }

    public Cordinate GetrandomCordinate()
    {
        Cordinate randomCordinate = shuffledTilesCordinate.Dequeue();
        shuffledTilesCordinate.Enqueue(randomCordinate);
        return randomCordinate;
    }
    public struct Cordinate
    {
        public int x;
        public int y;

        public Cordinate(int _x,int _y)
        {
            x = _x;
            y = _y;
        }
    }
}
