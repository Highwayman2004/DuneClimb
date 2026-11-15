using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LootManager : Singleton<LootManager>
{
    List<Loot> activeLoot = new();
    HashSet<int> collectedIds = new();

    public event Action<Loot> OnLootSpawn;
    public event Action<Loot> OnLootRemove;

    public void CollectNearby(Vector3 position, float radius)
    {
        for (int i = 0; i < activeLoot.Count; i++)
        {
            if (Vector3.Distance(position, activeLoot[i].transform.position) <= radius)
            {
                CollectScrap(activeLoot[i]);
                i--;
            }
        }
    }

    public bool IsCollected(int id) => collectedIds.Contains(id);

    public List<Loot> GetLoot(Predicate<Loot> filter)
    {
        return activeLoot.Where(l => filter(l)).ToList();
    }

    public void RegisterLoot(Loot loot)
    {
        activeLoot.Add(loot);
        OnLootSpawn?.Invoke(loot);
    }


    public void UnregisterLoot(Loot loot)
    {
        if (activeLoot.Remove(loot)) OnLootRemove?.Invoke(loot);
    }

    public void CollectScrap(Loot loot)
    {
        activeLoot.Remove(loot);
        loot.gameObject.SetActive(false);
        if (loot.spawnId != 0) collectedIds.Add(loot.spawnId);
        loot.Collect();
        OnLootRemove?.Invoke(loot);
    }
}
