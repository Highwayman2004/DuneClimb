using UnityEngine;

public abstract class Loot : MonoBehaviour
{
    void Start()
    {
        LootManager.Instance.RegisterLoot(this);
    }

    public abstract void Collect();
}
