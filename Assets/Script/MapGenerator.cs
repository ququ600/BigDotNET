using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject tilePrefab;

    public GameObject obsPrefab;
    // public float obsCount;
    public Vector2 mapSize;
    public Transform mapHolder;
    public float outlinePercent;

    static public List<GameObject> triggerTile = new List<GameObject>();
    public List<Coord> allTilesCoord = new List<Coord>();
    static private Queue<Coord> shuffledQuene;
    Transform[,] tilemap;

    public Color foregroundColor, backgroundColor;
    public float minObsHeight, maxObsHeight;

    [Range(0, 1)] public float obsPercent;
    private Coord mapCenter;

    bool[,] mapObstacles;

    public Map[] maps;
    public int mapIndex;
    Map currentMap;


    public float tileSize;// 调整地图大小
    public Vector2 mapMaxSize;
    public GameObject navMeshObs;


    private void Start()
    {
        GenerateMap();
    }
    private void GenerateMap()
    {
        tilemap = new Transform[(int)mapSize.x, (int)mapSize.y];

        for (int i = 0; i < mapSize.x; i++)
        {
            for (int j = 0; j < mapSize.y; j++)
            {

                Vector3 newPos = new Vector3(-mapSize.x / 2 + 0.5f + i, 0, -mapSize.y / 2 + 0.5f + j) * tileSize;
                GameObject spawnTile = Instantiate(tilePrefab, newPos, Quaternion.Euler(90, 0, 0));
                spawnTile.transform.SetParent(mapHolder);
                spawnTile.transform.localScale *= (1 - outlinePercent) * tileSize;
                // 触发器tile
                if (i == 0 || i == mapSize.x - 1 || j == 0 || j == mapSize.y - 1)
                {
                    triggerTile.Add(spawnTile);
                }
                allTilesCoord.Add(new Coord(i, j));
                tilemap[i, j] = spawnTile.transform;
            }
        }
        shuffledQuene = new Queue<Coord>(Utilities.ShuffleCoords(allTilesCoord.ToArray()));
        // 生成障碍
        int obsCount = (int)(mapSize.x * mapSize.y * obsPercent);
        mapCenter = new Coord((int)mapSize.x / 2, (int)mapSize.y / 2);
        mapObstacles = new bool[(int)mapSize.x, (int)mapSize.y];
        int currentObscount = 0;
        for (int i = 0; i < obsCount; i++)
        {
            Coord randomCoord = GetRandomCoord();
            mapObstacles[randomCoord.x, randomCoord.y] = true;
            currentObscount++;
            //Debug.Log("456");
            if (randomCoord != mapCenter && MapIsFullyAccessible(mapObstacles, currentObscount))
            {

                float obsHeight = Random.Range(minObsHeight, maxObsHeight);
                Vector3 newPos = new Vector3(-mapSize.x / 2 + 0.5f + randomCoord.x, obsHeight / 2, -mapSize.y / 2 + 0.5f + randomCoord.y) * tileSize;
                GameObject spawnObs = Instantiate(obsPrefab, newPos, Quaternion.identity);
                spawnObs.transform.SetParent(mapHolder);
                spawnObs.transform.localScale = new Vector3(1 - outlinePercent, obsHeight, 1 - outlinePercent) * tileSize;

                #region 
                MeshRenderer meshRender = spawnObs.GetComponent<MeshRenderer>();
                Material material = meshRender.material;
                float colorPercent = randomCoord.y / mapSize.y;
                material.color = Color.Lerp(foregroundColor, backgroundColor, colorPercent);
                meshRender.material = material;
                #endregion
            }
            else
            {
                mapObstacles[randomCoord.x, randomCoord.y] = false;
                currentObscount--;
            }

        }


    }
    private bool MapIsFullyAccessible(bool[,] _mapObstacles, int _currentObscount)
    {
        bool[,] mapFlags = new bool[_mapObstacles.GetLength(0), _mapObstacles.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();// 可行走瓦片，进行筛选可信走到

        queue.Enqueue(mapCenter);
        mapFlags[mapCenter.x, mapCenter.y] = true;// 已检测

        int accessibleCount = 1; //mapcenter可以走
        while (queue.Count > 0)
        {
            Coord currentTile = queue.Dequeue();
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int neighborX = currentTile.x + x;
                    int neighborY = currentTile.y + y;
                    if (x == 0 || y == 0)
                    {
                        if (neighborX >= 0 && neighborX < _mapObstacles.GetLength(0) && neighborY >= 0 && neighborY < _mapObstacles.GetLength(1))
                        {
                            if (!mapFlags[neighborX, neighborY] && !_mapObstacles[neighborX, neighborY])
                            {
                                mapFlags[neighborX, neighborY] = true;
                                accessibleCount++;
                                queue.Enqueue(new Coord(neighborX, neighborY));


                            }
                        }
                    }
                }
            }
        }
        int obsTargetCount = (int)(mapSize.x * mapSize.y - _currentObscount);
        // Debug.Log(accessibleCount);
        // Debug.Log(obsTargetCount);
        // Debug.Log(accessibleCount == obsTargetCount);
        return accessibleCount == obsTargetCount;
    }
    private Coord GetRandomCoord()
    {
        Coord randomCoord = shuffledQuene.Dequeue();
        shuffledQuene.Enqueue(randomCoord);
        return randomCoord;
    }
    static public void dynamicCreateTile(GameObject tile)
    {
        var tempTile = tile;
        Vector3 pos = new Vector3(tile.transform.position.x - 2f, 0, tile.transform.position.z);
        var newTile = Instantiate(tempTile, pos, Quaternion.Euler(90, 0, 0));
        triggerTile.Add(newTile);

    }
    public Transform GetRandomOpenTile()
    {
        Coord randomCoord = shuffledQuene.Dequeue();
        shuffledQuene.Enqueue(randomCoord);
        return tilemap[randomCoord.x, randomCoord.y];
    }

}



[System.Serializable]
public struct Coord
{
    public int x;
    public int y;
    public Coord(int _x, int _y)
    {
        this.x = _x;
        this.y = _y;
    }

    public static bool operator !=(Coord _c1, Coord _c2)
    {
        return !(_c1 == _c2);
    }
    public static bool operator ==(Coord _c1, Coord _c2)
    {
        return (_c1.x == _c2.x) && (_c1.y == _c2.y);
    }
}

[System.Serializable]
public class Map
{
    public Vector2 mapSize;



}