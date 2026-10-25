using UnityEngine;
using System;
using System.Threading;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    //public enum DrawMode { NoiseMap, ColorMap, Mesh}
    public enum DrawMode { NoiseMap, Mesh }
    public DrawMode drawMode;

    public NoiseData noiseData;
    public TerrainData terrainData;
    //public TextureData textureData;

    public Material terrainMaterial;

    public const int mapChunkSize = 239;
    [Range(0, 6)] public int editorLODPreview;

    public bool autoUpdate;


    //public TerrainType[] regions;

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


    /*void OnTextureValuesUpdated()
    {
        textureData.ApplyToMaterial(terrainMaterial);
    }*/


    public void GenerateMapInEditor()
    {
        MapData mapData = GenerateMapData(Vector2.zero);

        MapDisplay display = FindAnyObjectByType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightmap(mapData.heightMap));
        }
        /*else if (drawMode == DrawMode.ColorMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColorMap(mapData.colorMap,
                mapChunkSize, mapChunkSize));
        }*/
        else if (drawMode == DrawMode.Mesh)
        {
            //display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap,terrainData.meshHeighMultiplier, terrainData.meshHeightCurve, editorLODPreview), TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
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
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize + 2, mapChunkSize + 2, noiseData.seed, noiseData.noiseScale, noiseData.octaves, noiseData.persistance, noiseData.lacunarity, centre + noiseData.offset, noiseData.normalizeMode);

        return new MapData(noiseMap);
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
        /*if(textureData != null)
        {
            textureData.OnValuesUpdated -= OnTextureValuesUpdated;
            textureData.OnValuesUpdated += OnTextureValuesUpdated;
        }*/
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


/*[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}*/


public struct MapData
{
    public readonly float[,] heightMap;
    //public readonly Color[] colorMap;

    /*public MapData(float[,] heightMap, Color[] colorMap)
    {
        this.heightMap = heightMap;
        this.colorMap = colorMap;
    }*/
    public MapData(float[,] heightMap)
    {
        this.heightMap = heightMap;
    }
}