using System.Collections.Generic;
using UnityEngine;

public class LootManager : Singleton<LootManager>
{
    List<Loot> activeLoot = new();

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

    public void RegisterLoot(Loot loot)
    {
        activeLoot.Add(loot);
    }

    public void CollectScrap(Loot loot)
    {
        activeLoot.Remove(loot);
        loot.gameObject.SetActive(false);
        loot.Collect();
    }
}
