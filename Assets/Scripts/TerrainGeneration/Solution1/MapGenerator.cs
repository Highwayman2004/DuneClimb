using UnityEngine;
using System;
using System.Threading;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, Mesh }
    public DrawMode drawMode;

    public NoiseData noiseData;
    public TerrainData terrainData;

    public Material terrainMaterial;

    public const int mapChunkSize = 239;
    [Range(0, MeshGenerator.numSupportedLODs-1)] public int editorLODPreview;

    public bool autoUpdate;


    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = 
        new Queue<MapThreadInfo<MapData>>();


    Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue =
        new Queue<MapThreadInfo<MeshData>>();


    void OnValuesUpdated()
    {
        if(!Application.isPlaying)
        {
            GenerateMapInEditor();
        }
    }


    public void GenerateMapInEditor()
    {
        MapData mapData = GenerateMapData(Vector2.zero);

        MapDisplay display = FindAnyObjectByType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightmap(mapData.heightMap));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, terrainData.meshHeighMultiplier, terrainData.meshHeightCurve, editorLODPreview));
        }
    }


    public void RequestMapData(Vector2 centre, Action<MapData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MapDataThread(centre, callback);
        };

        new Thread(threadStart).Start();
    }


    void MapDataThread(Vector2 centre,Action<MapData> callback)
    {
        MapData mapData = GenerateMapData(centre);
        lock(mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }


    public void RequestMeshData(MapData mapData, int lod, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MeshDataThread(mapData, lod, callback);
        };

        new Thread(threadStart).Start();
    }


    void MeshDataThread(MapData mapData, int lod, Action<MeshData> callback)
    {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, terrainData.meshHeighMultiplier, terrainData.meshHeightCurve, lod);
        lock (meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }
    }


    void Update()
    {
        /*if(mapDataThreadInfoQueue.Count > 0)
        {
            for(int i = 0; i < mapDataThreadInfoQueue.Count; i++ )
            {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }*/
        lock (mapDataThreadInfoQueue)
        {
            while (mapDataThreadInfoQueue.Count > 0)
            {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
        /*if (meshDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }*/
        lock (meshDataThreadInfoQueue)
        {
            while (meshDataThreadInfoQueue.Count > 0)
            {
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }


    MapData GenerateMapData(Vector2 centre)
    {
        /*float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize + 2, mapChunkSize + 2, noiseData.seed, noiseData.noiseScale, noiseData.octaves, noiseData.persistance, noiseData.lacunarity, centre + noiseData.offset, noiseData.normalizeMode);

        return new MapData(noiseMap);*/

        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize + 2, mapChunkSize + 2, noiseData.seed,
        noiseData.noiseScale, noiseData.octaves, noiseData.persistance, noiseData.lacunarity,
        centre + noiseData.offset, noiseData.normalizeMode);

        ApplyStartArea(noiseMap, centre);
        return new MapData(noiseMap);
    }


    float SampleStartNoise() => Noise.SampleGlobal(terrainData.startPosition.x, terrainData.startPosition.y, noiseData.seed, noiseData.noiseScale, noiseData.octaves, noiseData.persistance, 
        noiseData.lacunarity, noiseData.offset);


    void ApplyStartArea(float[,] map, Vector2 centre)
    {
        if (noiseData.normalizeMode != Noise.NormalizeMode.Global) return;

        float outer = terrainData.startFlatRadius + terrainData.startBlendWidth;
        int size = map.GetLength(0);
        int half = (size - 1) / 2;
        Vector2 start = terrainData.startPosition;

        float reach = half + 1 + outer;
        if (Mathf.Abs(start.x - centre.x) > reach || Mathf.Abs(start.y - centre.y) > reach) return;

        float target = SampleStartNoise();

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float wx = centre.x + (x - half);
                float wz = centre.y + (half - y);
                float d = Vector2.Distance(new Vector2(wx, wz), start);
                if (d >= outer) continue;

                float t = Mathf.InverseLerp(terrainData.startFlatRadius, outer, d);
                float w = 1f - Mathf.SmoothStep(0f, 1f, t);
                map[x, y] = Mathf.Lerp(map[x, y], target, w);
            }
        }
    }


    public Vector3 GetStartPoint()
    {
        float h = terrainData.meshHeightCurve.Evaluate(SampleStartNoise()) * terrainData.meshHeighMultiplier;
        Vector2 s = terrainData.startPosition;
        return new Vector3(s.x, h, s.y) * terrainData.uniformScale;
    }


    private void OnValidate()
    {
        if(terrainData != null)
        {
            terrainData.OnValuesUpdated -= OnValuesUpdated;
            terrainData.OnValuesUpdated += OnValuesUpdated;
        }
        if (noiseData != null)
        {
            noiseData.OnValuesUpdated -= OnValuesUpdated;
            noiseData.OnValuesUpdated += OnValuesUpdated;
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

public struct MapData
{
    public readonly float[,] heightMap;

    public MapData(float[,] heightMap)
    {
        this.heightMap = heightMap;
    }
}