using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit;

public class Phone : MonoBehaviour
{
    [SerializeField] XRSimpleInteractable interactable;
    [SerializeField] Transform car;
    [SerializeField] Transform marker;
    [SerializeField] Transform sleepScreen;
    [SerializeField] TMP_Text distanceText;
    [SerializeField] float sleepTime;

    [SerializeField] SpawningData spawningData; 
    [SerializeField] NoiseData noiseData; 
    [SerializeField] TerrainData terrainData;

    Coroutine sleepCoroutine;
    bool useKeyboard = false;

    void Start()
    {
        interactable.selectExited.AddListener(OnHandRelease);
    }

    void Update()
    {
        List<Loot> loot = LootManager.Instance.GetLoot(l => l is FuelCan);
        (Loot l, float distance) = GetNearestLoot(loot);
        Vector2 carXZ = new(car.position.x, car.position.z);

        if (l != null)
        {
            //UpdateDirection(l);
            UpdateDirection(l.transform.position);
            UpdateDistance(distance);
        }
        else if (LootRoute.TryGetNearestFuelNode(carXZ, noiseData.seed, spawningData.lootData,
             MapGenerator.mapChunkSize - 1, terrainData.uniformScale, out Vector2 target))
        {
            UpdateDirection(new Vector3(target.x, 0, target.y));
            UpdateDistance(Vector2.Distance(carXZ, target));
        }

        if (useKeyboard && Keyboard.current.qKey.wasPressedThisFrame)
            WakeUp();
    }

    public void SetForKeyboard()
    {
        useKeyboard = true;
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

    /*void UpdateDirection(Loot loot)
    {
        Vector3 carDir = Vector3.ProjectOnPlane(car.forward, Vector3.up).normalized;
        Vector3 lootDir = Vector3.ProjectOnPlane((loot.transform.position - car.position), Vector3.up).normalized;

        float angle = Vector3.SignedAngle(carDir, lootDir, Vector3.up);
        marker.localRotation = Quaternion.Euler(0f, 0f, -angle);
    }*/

    void UpdateDirection(Vector3 targetPos)
    {
        Vector3 carDir = Vector3.ProjectOnPlane(car.forward, Vector3.up).normalized;
        Vector3 dir = Vector3.ProjectOnPlane(targetPos - car.position, Vector3.up).normalized;
        float angle = Vector3.SignedAngle(carDir, dir, Vector3.up);
        marker.localRotation = Quaternion.Euler(0f, 0f, -angle);
    }

    void UpdateDistance(float distance)
    {
        distanceText.text = Mathf.RoundToInt(distance) + " m";
    }

    void OnHandRelease(SelectExitEventArgs args)
    {
        WakeUp();
    }

    void WakeUp()
    {
        if (sleepCoroutine != null)
            return;

        sleepScreen.gameObject.SetActive(false);
        sleepCoroutine = StartCoroutine(WaitToSleep());
    }

    IEnumerator WaitToSleep()
    {
        yield return new WaitForSeconds(sleepTime);
        sleepScreen.gameObject.SetActive(true);
        sleepCoroutine = null;
    }
}
