using UnityEngine;

public abstract class Loot : MonoBehaviour
{
    void Awake()
    {
        LootManager.Instance.RegisterLoot(this);
    }

    public abstract void Collect();
}
