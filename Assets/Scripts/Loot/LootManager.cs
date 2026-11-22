using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class LootManager : Singleton<LootManager>
{
    [SerializeField] Transform collectPosition;
    [SerializeField] Transform pickupPosition;

    List<Loot> activeLoot = new();
    HashSet<int> collectedIds = new();

    public event Action<Loot> OnLootSpawn;
    public event Action<Loot> OnLootRemove;

    public Vector3 CollectPosition => collectPosition.position;
    public Vector3 PickupPosition => pickupPosition.position;

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
        loot.Collect();
    }

    public void Release(Loot loot)
    {
        loot.gameObject.SetActive(false);
        if (loot.spawnId != 0) collectedIds.Add(loot.spawnId);
        OnLootRemove?.Invoke(loot);
    }
}
