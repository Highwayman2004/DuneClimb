using UnityEngine;

[CreateAssetMenu]
public class TerrainData : UpdatableData
{
    public float uniformScale = 2.5f;
    public float meshHeighMultiplier;
    public AnimationCurve meshHeightCurve;


    [Header("Player start area (chunk units; world = x uniformScale)")]
    public Vector2 startPosition = Vector2.zero;
    public float startFlatRadius = 30f;
    public float startBlendWidth = 40f;
    public float startClearRadius = 55f;
}
