using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine.UIElements;

public class MapGenerator : MonoBehaviour
{
    public Map[] maps;
    public int mapIndex;
    public Transform tilePrefab;
    public Transform obstaclePrefab;
    public Transform floor1;
    public Transform floor;
    public Transform nevmeshMaskPrefab;
    public Vector2 maxMapSize;
    Transform[,] tileMap;
    

    [Range(0, 1)]
    public float outlinePercent;
    

    List<Cordinate> allTileCoords;
    Queue<Cordinate> shuffledTileCoords;
    Queue<Cordinate> shuffledOpenTileCoords;

    public float tileSize;
    Map CurrentMap;

    void Start()
    {
        FindAnyObjectByType<Spawner>().OnNewWave += MapGenerator_OnNewWave;
    }

    private void MapGenerator_OnNewWave(int waveNum)
    {
        mapIndex = waveNum - 1;
        GenerateMap();
    }

    public void GenerateMap()
    {
        CurrentMap = maps[mapIndex];
        tileMap = new Transform[CurrentMap.mapSize.x, CurrentMap.mapSize.y];
        System.Random randomMapGenerator = new System.Random(CurrentMap.seed);
        

        // Generating Cordinates 
        allTileCoords = new List<Cordinate>();
        for (int x = 0; x < CurrentMap.mapSize.x; x++)
        {
            for (int y = 0; y < CurrentMap.mapSize.y; y++)
            {
                allTileCoords.Add(new Cordinate(x, y));
            }
        }
        shuffledTileCoords = new Queue<Cordinate>(Utlity.ShuffleArray(allTileCoords.ToArray(), CurrentMap.seed));
        
        // Creating MapHolder Object
        string holderName = "Generated Map";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        // Spawing Tiles
        for (int x = 0; x < CurrentMap.mapSize.x; x++)
        {
            for (int y = 0; y < CurrentMap.mapSize.y; y++)
            {
                Vector3 tilePosition = CoordToPosition(x, y);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                newTile.localScale = Vector3.one * (1 - outlinePercent)*tileSize;
                newTile.parent = mapHolder;
                tileMap[x, y] = newTile;
            }
        }

        // Spawning Obstacles
        bool[,] obstacleMap = new bool[(int)CurrentMap.mapSize.x, (int)CurrentMap.mapSize.y];

        int obstacleCount = (int)(CurrentMap.mapSize.x * CurrentMap.mapSize.y * CurrentMap.obsPercent);
        int currentObstacleCount = 0;
        List<Cordinate> allOpenCordinate = new List<Cordinate>(allTileCoords);

        for (int i = 0; i < obstacleCount; i++)
        {
            Cordinate randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount++;

            if (randomCoord != CurrentMap.mapCenter && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                float obsHeight = Mathf.Lerp(CurrentMap.minObsHeight, CurrentMap.maxObsHeight, (float)randomMapGenerator.NextDouble());
                Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);

                Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up *obsHeight/2, Quaternion.identity) as Transform;
                newObstacle.parent = mapHolder;
                newObstacle.localScale = new Vector3((1 - outlinePercent) * tileSize, obsHeight, (1 - outlinePercent) * tileSize);

                Renderer obsRenderer = newObstacle.GetComponent<Renderer>();
                Material obsMaterial = new Material(obsRenderer.sharedMaterial);
                float colorPercent = randomCoord.y / (float)CurrentMap.mapSize.y;
                obsMaterial.color = Color.Lerp(CurrentMap.foreGroundColor, CurrentMap.backeGroundColor, colorPercent);
                obsRenderer.sharedMaterial = obsMaterial;
                allOpenCordinate.Remove(randomCoord);
            }
            else
            {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--;
            }
        }
        shuffledOpenTileCoords = new Queue<Cordinate>(Utlity.ShuffleArray(allOpenCordinate.ToArray(), CurrentMap.seed));

        // Creating NevMesh Mask

        Transform maskLeft = Instantiate(nevmeshMaskPrefab, Vector3.left * (CurrentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity) as Transform;
        maskLeft.parent = mapHolder;
        maskLeft.localScale = new Vector3((maxMapSize.x - CurrentMap.mapSize.x) / 2f, 1, CurrentMap.mapSize.y) * tileSize;

        Transform maskright = Instantiate(nevmeshMaskPrefab, Vector3.right * (CurrentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity) as Transform;
        maskright.parent = mapHolder;
        maskright.localScale = new Vector3((maxMapSize.x -CurrentMap.mapSize.x) / 2f, 1, CurrentMap.mapSize.y) * tileSize;

        Transform masktop = Instantiate(nevmeshMaskPrefab, Vector3.forward * (CurrentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
        masktop.parent = mapHolder;
        masktop.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y -CurrentMap.mapSize.y) / 2f) * tileSize;

        Transform maskbottom = Instantiate(nevmeshMaskPrefab, Vector3.back * (CurrentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
        maskbottom.parent = mapHolder;
        maskbottom.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y -CurrentMap.mapSize.y) / 2f) * tileSize;

        floor.localScale = new Vector3(maxMapSize.x, maxMapSize.y) * tileSize;
        floor1.localScale= new Vector3(CurrentMap.mapSize.x * tileSize, CurrentMap.mapSize.y * tileSize);

    }

    bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount)
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Cordinate> queue = new Queue<Cordinate>();
        queue.Enqueue(CurrentMap.mapCenter);
        mapFlags[CurrentMap.mapCenter.x, CurrentMap.mapCenter.y] = true;

        int accessibleTileCount = 1;

        while (queue.Count > 0)
        {
            Cordinate tile = queue.Dequeue();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int neighbourX = tile.x + x;
                    int neighbourY = tile.y + y;
                    if (x == 0 || y == 0)
                    {
                        if (neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1))
                        {
                            if (!mapFlags[neighbourX, neighbourY] && !obstacleMap[neighbourX, neighbourY])
                            {
                                mapFlags[neighbourX, neighbourY] = true;
                                queue.Enqueue(new Cordinate(neighbourX, neighbourY));
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }
        }

        int targetAccessibleTileCount = (int)(CurrentMap.mapSize.x *CurrentMap.mapSize.y - currentObstacleCount);
        return targetAccessibleTileCount == accessibleTileCount;
    }

    Vector3 CoordToPosition(int x, int y)
    {
        return new Vector3(-CurrentMap.mapSize.x / 2f + 0.5f + x, 0, -CurrentMap.mapSize.y / 2f + 0.5f + y)*tileSize;
    }

    public Transform GetTileFromPos(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x / tileSize + (CurrentMap.mapSize.x - 1) / 2f);
        int y = Mathf.RoundToInt(position.z / tileSize + (CurrentMap.mapSize.y - 1) / 2f);
        x = Mathf.Clamp(x, 0, tileMap.GetLength(0)-1);
        y = Mathf.Clamp(y, 0, tileMap.GetLength(1)-1);
        return tileMap[x, y];
    }
    public Cordinate GetRandomCoord()
    {
        Cordinate randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }

    public Transform GetRandomOpenTile()
    {
        Cordinate randomCoord = shuffledOpenTileCoords.Dequeue();
        shuffledOpenTileCoords.Enqueue(randomCoord);
        return tileMap[randomCoord.x, randomCoord.y];
    }

    [System.Serializable]
    public struct Cordinate
    {
        public int x;
        public int y;

        public Cordinate(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public static bool operator ==(Cordinate c1, Cordinate c2)
        {
            return c1.x == c2.x && c1.y == c2.y;
        }

        public static bool operator !=(Cordinate c1, Cordinate c2)
        {
            return !(c1 == c2);
        }

    }

    [System.Serializable]
    public class Map
    {
        public Cordinate mapSize;
        [Range(0, 1)]
        public float obsPercent;
        public int seed;
        public float minObsHeight;
        public float maxObsHeight;
        public Color foreGroundColor;
        public Color backeGroundColor;

        public Cordinate mapCenter
        {
            get
            {
                return new Cordinate(mapSize.x/2, mapSize.y/2);
            }
        }
    }
}