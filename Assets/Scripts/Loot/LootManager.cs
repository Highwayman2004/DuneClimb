using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LootManager : Singleton<LootManager>
{
    List<Loot> activeLoot = new();

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

    public List<Loot> GetLoot(Predicate<Loot> filter)
    {
        return activeLoot.Where(l => filter(l)).ToList();
    }

    public void RegisterLoot(Loot loot)
    {
        activeLoot.Add(loot);
        OnLootSpawn?.Invoke(loot);
    }

    public void CollectScrap(Loot loot)
    {
        activeLoot.Remove(loot);
        loot.gameObject.SetActive(false);
        loot.Collect();
        OnLootRemove?.Invoke(loot);
    }
}
