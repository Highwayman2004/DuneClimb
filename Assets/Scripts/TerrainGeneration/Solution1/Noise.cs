using UnityEngine;

public static class Noise
{
    public enum NormalizeMode { Local, Global};

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, 
        float scale, int octaves, float persistance, float lacunarity, 
        Vector2 offset, NormalizeMode normalizeMode)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i <octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) - offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= persistance;
        }


        if(scale <= 0)
            scale = 0.0001f;

        float minLocalNoiseHeight = float.MaxValue;
        float maxLocalNoiseHeight = float.MinValue;

        float halfWitdh = mapWidth / 2;
        float halfHeight = mapHeight / 2;

        for (int y = 0; y < mapHeight; y++)
        {
            for(int x = 0; x < mapWidth; x++)
            {
                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                for(int i = 0;  i < octaves; i++)
                {
                    float sampleX = (x - halfWitdh +
                        octaveOffsets[i].x) / scale * frequency;
                    float sampleY = (y - halfHeight +
                        octaveOffsets[i].y) / scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 -1;
                    noiseHeight += perlinValue * amplitude;
                    //noiseMap[x, y] = perlinValue;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if(noiseHeight > maxLocalNoiseHeight)
                {
                    maxLocalNoiseHeight = noiseHeight;
                }
                else if(noiseHeight < minLocalNoiseHeight)
                {
                    minLocalNoiseHeight = noiseHeight;
                }

                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if(normalizeMode == NormalizeMode.Local)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight,
                    noiseMap[x, y]);
                }
                else if(normalizeMode == NormalizeMode.Global)
                {
                    float normalizedHeight = (noiseMap[x, y] + 1)/
                        (2f*maxPossibleHeight / 1.4f);
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
            }
        }

        return noiseMap;
    }


    public static float SampleGlobal(float worldX, float worldZ, int seed, float scale,
    int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        System.Random prng = new System.Random(seed);
        if (scale <= 0) scale = 0.0001f;

        float maxPossibleHeight = 0, amplitude = 1, frequency = 1, noiseHeight = 0;
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) - offset.y;

            float sampleX = (worldX + offsetX) / scale * frequency;
            float sampleY = (-worldZ + offsetY) / scale * frequency;
            noiseHeight += (Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1) * amplitude;

            maxPossibleHeight += amplitude;
            amplitude *= persistance;
            frequency *= lacunarity;
        }
        float normalized = (noiseHeight + 1) / (2f * maxPossibleHeight / 1.4f);
        return Mathf.Clamp(normalized, 0, int.MaxValue);
    }
}
