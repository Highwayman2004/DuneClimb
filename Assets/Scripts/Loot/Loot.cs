using UnityEngine;

public abstract class Loot : MonoBehaviour
{
    [HideInInspector] public int spawnId;

    private void Awake()
    {
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    void Start()
    {
        LootManager.Instance.RegisterLoot(this);
    }

    void OnDestroy()
    {
        if (LootManager.Instance != null) LootManager.Instance.UnregisterLoot(this);
    }

    public abstract void Collect();
}
