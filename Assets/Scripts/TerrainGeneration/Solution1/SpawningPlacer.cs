using System.Collections.Generic;
using UnityEngine;

public class SpawningPlacer : MonoBehaviour
{
    public struct Placement
    {
        public GameObject prefab;
        public Vector3 localPosition;
        public Quaternion localRotation;
        public float scale;
        public bool ignoreLOD;
    }


    public static List<Placement> Generate(MapData mapData, Vector2 coord, int worldSeed,
        SpawningData spawningData, TerrainData terrain, int chunkSize)
    {
        var result = new List<Placement>();
        if (spawningData == null) return result;

        float[,] hm = mapData.heightMap;
        int size = hm.GetLength(0);
        int half = (size - 1) / 2;
        float H(int x, int y) => terrain.meshHeightCurve.Evaluate(hm[x, y]) * terrain.meshHeighMultiplier;
        Vector3 Normal(int x, int y) => new Vector3(
            H(x - 1, y) - H(x + 1, y), 2f, H(x, y - 1) - H(x, y + 1)).normalized;

        var exclusions = new List<Vector3>();
        for (int r = 0; r < spawningData.rules.Length; r++)
            if (spawningData.rules[r].mode == SpawnMode.Landmark)
                GenerateLandmark(spawningData.rules[r], r);

        for (int r = 0; r < spawningData.rules.Length; r++)
            if (spawningData.rules[r].mode == SpawnMode.Scatter)
                GenerateScatter(spawningData.rules[r], r);

        return result;

        // ---------------- local functions ----------------
        void GenerateScatter(SpawningRule rule, int ruleIndex)
        {
            if (rule.prefabs == null || rule.prefabs.Length == 0) return;
            var rng = new System.Random(System.HashCode.Combine(worldSeed, (int)coord.x, (int)coord.y, ruleIndex));
            var placed = new List<Vector2>();

            for (int i = 0; i < rule.attemptsPerChunk; i++)
            {
                int x = rng.Next(2, size - 2);
                int y = rng.Next(2, size - 2);
                float yaw = (float)rng.NextDouble() * 360f;
                float scale = Mathf.Lerp(rule.scaleRange.x, rule.scaleRange.y, (float)rng.NextDouble());
                int variant = rng.Next(rule.prefabs.Length);

                float h = H(x, y);
                if (rule.filterByActualHeight)
                {
                    if (h < rule.minHeight || h > rule.maxHeight) continue;
                }
                else if (hm[x, y] < rule.minNoiseHeight || hm[x, y] > rule.maxNoiseHeight) continue;

                Vector3 normal = Normal(x, y);
                float slope = Vector3.Angle(Vector3.up, normal);
                if (slope < rule.minSlope || slope > rule.maxSlope) continue;

                Vector2 xz = new Vector2(x - half, half - y);

                if (rule.useClusterNoise)
                {
                    float wx = coord.x * chunkSize + xz.x;
                    float wz = coord.y * chunkSize + xz.y;
                    float cn = Mathf.PerlinNoise(wx / rule.clusterScale + 10000f + worldSeed * 0.13f * (ruleIndex + 1),
                                                 wz / rule.clusterScale + 10000f);
                    if (cn < rule.clusterThreshold) continue;
                }

                bool blocked = false;
                foreach (Vector3 e in exclusions)
                    if ((new Vector2(e.x, e.y) - xz).sqrMagnitude < e.z * e.z) { blocked = true; break; }
                if (blocked) continue;

                foreach (Vector2 p in placed)
                    if ((p - xz).sqrMagnitude < rule.minSpacing * rule.minSpacing) { blocked = true; break; }
                if (blocked) continue;
                placed.Add(xz);

                Quaternion rot = Quaternion.Euler(0, yaw, 0);
                if (rule.alignToSurface) rot = Quaternion.FromToRotation(Vector3.up, normal) * rot;

                result.Add(new Placement
                {
                    prefab = rule.prefabs[variant],
                    localPosition = new Vector3(xz.x, h - rule.sinkAmount, xz.y),
                    localRotation = rot,
                    scale = scale,
                    ignoreLOD = false
                });
            }
        }

        void GenerateLandmark(SpawningRule rule, int ruleIndex)
        {
            int rs = Mathf.Max(1, rule.regionSizeInChunks);
            int regionX = Mathf.FloorToInt(coord.x / rs);
            int regionY = Mathf.FloorToInt(coord.y / rs);

            var rng = new System.Random(System.HashCode.Combine(worldSeed, regionX, regionY, ruleIndex, 7919));
            if (rng.NextDouble() > rule.spawnChance) return;
            int targetX = regionX * rs + rng.Next(rs);
            int targetY = regionY * rs + rng.Next(rs);
            if ((int)coord.x != targetX || (int)coord.y != targetY) return;

            int margin = Mathf.CeilToInt(rule.footprintRadius) + 3;
            if (margin * 2 >= size) return;

            for (int attempt = 0; attempt < rule.searchAttempts; attempt++)
            {
                int cx = rng.Next(margin, size - margin);
                int cy = rng.Next(margin, size - margin);

                float min = H(cx, cy), max = min, sum = min;
                for (int k = 0; k < 8; k++)
                {
                    float a = k * Mathf.PI / 4f;
                    float h = H(cx + Mathf.RoundToInt(Mathf.Cos(a) * rule.footprintRadius),
                                cy + Mathf.RoundToInt(Mathf.Sin(a) * rule.footprintRadius));
                    min = Mathf.Min(min, h); max = Mathf.Max(max, h); sum += h;
                }
                if (max - min > rule.maxHeightDifference) continue;

                Vector2 centre = new Vector2(cx - half, half - cy);
                float baseHeight = sum / 9f - rule.sinkAmount;

                if (rule.prefabs != null && rule.prefabs.Length > 0)
                    result.Add(new Placement
                    {
                        prefab = rule.prefabs[rng.Next(rule.prefabs.Length)],
                        localPosition = new Vector3(centre.x, baseHeight, centre.y),
                        localRotation = Quaternion.Euler(0, (float)rng.NextDouble() * 360f, 0),
                        scale = Mathf.Lerp(rule.scaleRange.x, rule.scaleRange.y, (float)rng.NextDouble()),
                        ignoreLOD = true
                    });

                if (rule.pieces != null && rule.pieces.Length > 0)
                {
                    for (int p = 0; p < rule.pieceCount; p++)
                    {
                        float ang = (float)rng.NextDouble() * Mathf.PI * 2f;
                        float dist = Mathf.Sqrt((float)rng.NextDouble()) * rule.footprintRadius;
                        int px = cx + Mathf.RoundToInt(Mathf.Cos(ang) * dist);
                        int py = cy + Mathf.RoundToInt(Mathf.Sin(ang) * dist);
                        result.Add(new Placement
                        {
                            prefab = rule.pieces[rng.Next(rule.pieces.Length)],
                            localPosition = new Vector3(px - half, H(px, py) - 0.3f, half - py),
                            localRotation = Quaternion.Euler(0, (float)rng.NextDouble() * 360f, 0),
                            scale = Mathf.Lerp(0.8f, 1.2f, (float)rng.NextDouble()),
                            ignoreLOD = true
                        });
                    }
                }

                exclusions.Add(new Vector3(centre.x, centre.y, rule.footprintRadius + 4f));
                return;
            }
        }
    }
}
