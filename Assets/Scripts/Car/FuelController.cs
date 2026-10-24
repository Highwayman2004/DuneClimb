using UnityEngine;

public class FuelController : Singleton<FuelController>
{
    [SerializeField] float maxFuel;
    [SerializeField] float minFuelUse;
    [SerializeField] float maxFuelUse;
    [SerializeField] float maxUseSpeed;
    [SerializeField] Transform fuelDisplay;

    float fuel;

    public bool HasFuel => fuel > 0;

    void Awake()
    {
        fuel = maxFuel;
        UpdateVisuals();
    }

    public void UseFuel(float speed)
    {
        fuel -= Mathf.Lerp(minFuelUse, maxFuelUse, Mathf.InverseLerp(0, maxUseSpeed, speed)) * Time.deltaTime;
        UpdateVisuals();
    }

    public void Refill(float amount)
    {
        fuel += amount;
        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        fuelDisplay.transform.localRotation = Quaternion.Euler(0, 0, (2 - Mathf.Max(0, fuel / maxFuel)) * -180f);
    }
}
