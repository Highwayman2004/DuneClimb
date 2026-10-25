using UnityEngine;

[CreateAssetMenu]
public class NoiseData : UpdatableData
{
    public Noise.NormalizeMode normalizeMode;
    public float noiseScale;
    public int octaves;
    [Range(0, 1)] public float persistance;
    public float lacunarity;
    public int seed;
    public Vector2 offset;

    public void LaunchWithRandomData()
    {
        noiseScale = Random.Range(60f, 100f);
        seed = Random.Range(1, 100);
    }

#if UNITY_EDITOR

    protected override void OnValidate()
    {
        if (octaves < 0)
        {
            octaves = 0;
        }
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }

        base.OnValidate();
    }
#endif
}
