using System.Collections.Generic;
using UnityEngine;

public static class LootRoute
{
    public struct RoutePoint
    {
        public Vector2 worldPos;
        public bool isFuel;
        public int id;
        public float yaw;
        public int variant;
    }


    static int NonZero(int v) => v == 0 ? 1 : v;
    public static int NodeId(int seed, int cx, int cy) => NonZero(System.HashCode.Combine(seed, cx, cy, 999));

    public static Vector2 NodePos(int seed, int cx, int cy, float cell)
    {
        var rng = new System.Random(System.HashCode.Combine(seed, cx, cy, 1234));
        return new Vector2((cx + 0.15f + 0.7f * (float)rng.NextDouble()) * cell,
                           (cy + 0.15f + 0.7f * (float)rng.NextDouble()) * cell);
    }

    public static bool NodeHasFuel(int seed, int cx, int cy, float chance)
    {
        var rng = new System.Random(System.HashCode.Combine(seed, cx, cy, 777));
        return rng.NextDouble() < chance;
    }

    static bool EdgeExists(int seed, int cx, int cy, int dir, float chance)
    {
        var rng = new System.Random(System.HashCode.Combine(seed, cx, cy, dir, 555));
        return rng.NextDouble() < chance;
    }

    static bool Inside(Vector2 p, Vector2 min, Vector2 max) =>
        p.x >= min.x && p.x < max.x && p.y >= min.y && p.y < max.y;


    public static IEnumerable<RoutePoint> PointsInChunk(Vector2 coord, int chunkSize, int seed, LootData data)
    {
        float cell = data.cellSizeInChunks * chunkSize;
        Vector2 centre = coord * chunkSize;
        Vector2 min = centre - Vector2.one * (chunkSize * 0.5f);
        Vector2 max = centre + Vector2.one * (chunkSize * 0.5f);

        int x0 = Mathf.FloorToInt(min.x / cell) - 1, x1 = Mathf.FloorToInt(max.x / cell) + 1;
        int y0 = Mathf.FloorToInt(min.y / cell) - 1, y1 = Mathf.FloorToInt(max.y / cell) + 1;

        for (int cy = y0; cy <= y1; cy++)
            for (int cx = x0; cx <= x1; cx++)
            {
                Vector2 a = NodePos(seed, cx, cy, cell);

                if (Inside(a, min, max) && NodeHasFuel(seed, cx, cy, data.nodeFuelChance))
                {
                    var nrng = new System.Random(System.HashCode.Combine(seed, cx, cy, 4242));
                    yield return new RoutePoint
                    {
                        worldPos = a,
                        isFuel = true,
                        id = NodeId(seed, cx, cy),
                        yaw = (float)nrng.NextDouble() * 360f,
                        variant = nrng.Next()
                    };
                }

                for (int dir = 0; dir < 2; dir++)
                {
                    if (!EdgeExists(seed, cx, cy, dir, data.edgeChance)) continue;

                    Vector2 b = NodePos(seed, cx + (dir == 0 ? 1 : 0), cy + (dir == 1 ? 1 : 0), cell);
                    Vector2 along = (b - a).normalized;
                    Vector2 perp = new Vector2(-along.y, along.x);

                    var rng = new System.Random(System.HashCode.Combine(seed, cx, cy, dir, 31337));
                    Vector2 ctrl = (a + b) * 0.5f + perp * ((float)rng.NextDouble() * 2f - 1f) * data.pathWobble * 2f;

                    int count = Mathf.Max(2, Mathf.CeilToInt((b - a).magnitude * 1.1f / data.spacing));
                    for (int k = 1; k < count; k++)
                    {
                        float side = (float)rng.NextDouble() * 2f - 1f;
                        float jitter = (float)rng.NextDouble() * 2f - 1f;
                        float yaw = (float)rng.NextDouble() * 360f;
                        int variant = rng.Next();

                        float t = (k + jitter * data.alongJitter) / count;
                        float u = 1f - t;
                        Vector2 p = u * u * a + 2f * u * t * ctrl + t * t * b + perp * side * data.sideJitter;

                        if (!Inside(p, min, max)) continue;
                        yield return new RoutePoint
                        {
                            worldPos = p,
                            isFuel = false,
                            yaw = yaw,
                            variant = variant,
                            id = NonZero(System.HashCode.Combine(seed, cx, cy, dir, k))
                        };
                    }
                }
            }
    }


    public static bool TryGetNearestFuelNode(Vector2 worldXZ, int seed, LootData data, int chunkSize,
        float uniformScale, out Vector2 resultWorldXZ)
    {
        resultWorldXZ = default;
        if (data == null) return false;

        float cell = data.cellSizeInChunks * chunkSize;
        Vector2 p = worldXZ / uniformScale;
        int cx0 = Mathf.FloorToInt(p.x / cell), cy0 = Mathf.FloorToInt(p.y / cell);

        float best = float.MaxValue;
        bool found = false;
        for (int cy = cy0 - 3; cy <= cy0 + 3; cy++)
            for (int cx = cx0 - 3; cx <= cx0 + 3; cx++)
            {
                if (!NodeHasFuel(seed, cx, cy, data.nodeFuelChance)) continue;
                if (LootManager.Instance != null && LootManager.Instance.IsCollected(NodeId(seed, cx, cy))) continue;

                Vector2 n = NodePos(seed, cx, cy, cell);
                float d = (n - p).sqrMagnitude;
                if (d < best) { best = d; resultWorldXZ = n * uniformScale; found = true; }
            }
        return found;
    }
}
