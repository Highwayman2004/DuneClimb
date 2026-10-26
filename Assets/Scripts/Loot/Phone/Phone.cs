using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class Phone : MonoBehaviour
{
    [SerializeField] Transform car;
    [SerializeField] Transform marker;
    [SerializeField] TMP_Text distanceText;

    void Update()
    {
        List<Loot> loot = LootManager.Instance.GetLoot(l => l is FuelCan);
        (Loot l, float distance) = GetNearestLoot(loot);

        if(l != null)
        {
            UpdateDirection(l);
            UpdateDistance(distance);
        }
    }

    (Loot, float) GetNearestLoot(List<Loot> loot)
    {
        Loot nearestLoot = null;
        float distance = 9999f;
        Vector2 carPos = new(car.position.x, car.position.z);

        foreach (var l in loot)
        {
            Vector2 lootPos = new(l.transform.position.x, l.transform.position.z);

            float d = Vector2.Distance(carPos, lootPos);

            if(d < distance)
            {
                nearestLoot = l;
                distance = d;
            }
        }

        return (nearestLoot, distance);
    }

    void UpdateDirection(Loot loot)
    {
        Vector3 carDir = Vector3.ProjectOnPlane(car.forward, Vector3.up).normalized;
        Vector3 lootDir = Vector3.ProjectOnPlane((loot.transform.position - car.position), Vector3.up).normalized;

        float angle = Vector3.SignedAngle(carDir, lootDir, Vector3.up);
        marker.localRotation = Quaternion.Euler(0f, 0f, -angle);
    }

    void UpdateDistance(float distance)
    {
        distanceText.text = Mathf.RoundToInt(distance) + " m";
    }
}
