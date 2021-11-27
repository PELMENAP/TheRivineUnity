using UnityEngine;
using System;
using System.Threading;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    public GameObject player;
    public enum DrawMode { NoiseMap, ColourMap };
    public DrawMode drawMode;
    public Noise.NormalizeMode normalizeMode;
    public const int mapChunkSize = 21;
    public float noiseScale;
    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;
    public int seed;
    public Vector2 offset;
    public bool autoUpdate;
    public TerrainType[] regions;
    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();

    private void Start()
    {
        seed = UnityEngine.Random.Range(-1000, 1000);
    }
    public void DrawMapInEditor()
    {
        MapData mapData = GenerateMapData(Vector2.zero);

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
        }
        else if (drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColourMap(mapData.colourMap, mapChunkSize, mapChunkSize));
        }
    }

    public void RequestMapData(Vector2 centre, Action<MapData> callback)
    {
        MapData mapData = GenerateMapData(centre);
        ThreadStart threadStart = delegate
        {
            MapDataThread(mapData, callback);
        };

        new Thread(threadStart).Start();
    }

    void MapDataThread(MapData mapData, Action<MapData> callback)
    {
        lock (mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }

    void Update()
    {
        if (player.transform.position.x > 100000 || player.transform.position.y > 100000)
        {
            normalizeMode = Noise.NormalizeMode.Local;
        }
        if (mapDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < mapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    MapData GenerateMapData(Vector2 centre)
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, centre + offset, normalizeMode);
        Color[] colourMap = new Color[mapChunkSize * mapChunkSize];
        int[,] objectMap = new int[mapChunkSize, mapChunkSize];
        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight >= regions[i].height)
                    {
                        colourMap[y * mapChunkSize + x] = regions[i].colour;
                        objectMap[x, y] = i;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        return new MapData(noiseMap, colourMap, objectMap);
    }

    void OnValidate()
    {
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }
    }

    struct MapThreadInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }

    }

}

[System.Serializable]
public struct TerrainType
{
    public float height;
    public Color colour;

}

public struct MapData
{
    public readonly float[,] heightMap;
    public readonly Color[] colourMap;
    public readonly int[,] objectMap;
    public MapData(float[,] heightMap, Color[] colourMap, int[,] objectMap)
    {
        this.heightMap = heightMap;
        this.colourMap = colourMap;
        this.objectMap = objectMap;
    }
}