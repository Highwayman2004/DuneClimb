using JetBrains.Annotations;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, ColorMap, Mesh}
    public DrawMode drawMode;

    const int mapChunkSize = 241;
    [Range(0, 6)] public int levelOfDetail;
    //public int mapWidth;
    //public int mapHeight;
    public float noiseScale;

    public int octaves;
    [Range(0, 1)]  public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;


    public float meshHeighMultiplier;

    public AnimationCurve meshHeightCurve;

    public bool autoUpdate;


    public TerrainType[] regions;

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, 
            noiseScale,octaves, persistance, lacunarity, offset);

        Color[] colorMap = new Color[mapChunkSize * mapChunkSize];
        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];
                for(int i = 0; i < regions.Length; i++)
                {
                    if(currentHeight <= regions[i].height)
                    {
                        colorMap[y * mapChunkSize + x] = regions[i].color;
                        break;
                    }
                }
            }
        }

        MapDisplay display = FindAnyObjectByType<MapDisplay>();
        if(drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightmap(noiseMap));
        }
        else if(drawMode == DrawMode.ColorMap)
        {
            //display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, 
            //    mapWidth, mapHeight));
            display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, 
                mapChunkSize, mapChunkSize));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, 
                meshHeighMultiplier,meshHeightCurve, levelOfDetail), 
                TextureGenerator.TextureFromColorMap(colorMap,mapChunkSize, mapChunkSize));
        }
        //display.DrawNoiseMap(noiseMap);
    }


    private void OnValidate()
    {
        /*if(mapWidth < 1)
        {
            mapWidth = 1;
        }
        if(mapHeight < 1)
        {
            mapHeight = 1;
        }*/
        if (octaves < 0)
        {
            octaves = 0;
        }
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
    }
}


[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}
