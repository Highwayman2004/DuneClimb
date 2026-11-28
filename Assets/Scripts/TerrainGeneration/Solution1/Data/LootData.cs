using UnityEngine;

[CreateAssetMenu]
public class LootData : ScriptableObject
{
    [Header("Prefabs")]
    public GameObject[] scrapPrefabs;
    public GameObject[] fuelPrefabs;


    [Header("Route layout")]
    public float cellSizeInChunks = 2f;
    [Range(0, 1)] public float edgeChance = 0.8f;
    [Range(0, 1)] public float nodeFuelChance = 1f;
    public float pathWobble = 40f;


    [Header("Scrap along route")]
    public float spacing = 14f;
    public float sideJitter = 4f;
    [Range(0, 1)] public float alongJitter = 0.3f;

    [Header("Fuel placement")]
    [Min(0)] public float cellGrowth = 0.5f;
    public float maxCellSizeInChunks = 6f;

    [Header("Placement")]
    public float lootRadius = 1.5f;
    public float maxSlope = 35f;
    public float heightOffset = 0.5f;
    public float scrapHeightOffset = 0.1f;
    public float fuelHeightOffset = 0.3f;


    [Header("Around the player start (world units)")]
    public float startClearRadius = 15f;
    public int starterScrapCount = 4;
    public float starterDistance = 25f;
}
