using System;
using UnityEngine;

public abstract class Loot : MonoBehaviour
{
    [SerializeField] BoxCollider collider;
    [HideInInspector] public int spawnId;

    public event Action<Loot> OnLootRelease;

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

    public virtual void Set()
    {
        collider.enabled = true;
    }

    public virtual void Collect()
    {
        collider.enabled = false;
    }

    protected void Release()
    {
        LootManager.Instance.Release(this);
    }
}
