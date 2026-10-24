using UnityEngine;

public class LootCollector : MonoBehaviour
{
    [SerializeField] float collectRadius;

    void Update()
    {
        LootManager.Instance.CollectNearby(transform.position, collectRadius);
    }
}
