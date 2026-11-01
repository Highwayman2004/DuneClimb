using UnityEngine;

public enum SpawnMode { Scatter, Landmark }

[CreateAssetMenu]
public class SpawningData : ScriptableObject
{
    public SpawningRule[] rules;
}

[System.Serializable]
public class SpawningRule
{
    public string name;
    public SpawnMode mode;

    [Header("For all")]
    public GameObject[] prefabs;
    public Vector2 scaleRange = new Vector2(0.8f, 1.2f);
    public float sinkAmount = 0.1f;
    public bool alignToSurface;

    [Header("Scatter")]
    public int attemptsPerChunk = 60;
    public bool filterByActualHeight;
    public float minNoiseHeight = 0f, maxNoiseHeight = 100f;
    public float minHeight = 0f, maxHeight = 1000f;
    [Range(0, 90)] public float minSlope = 0f;
    [Range(0, 90)] public float maxSlope = 30f;
    public float minSpacing = 3f;
    public bool useClusterNoise;
    public float clusterScale = 120f;
    [Range(0, 1)] public float clusterThreshold = 0.6f;


    [Header("Landmark")]
    public int regionSizeInChunks = 5;
    [Range(0, 1)] public float spawnChance = 0.6f;
    public float footprintRadius = 30f;
    public float maxHeightDifference = 6f;
    public int searchAttempts = 25;
    public GameObject[] pieces;
    public int pieceCount = 0;
}