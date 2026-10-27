using UnityEngine;

public class FuelController : Singleton<FuelController>
{
    [SerializeField] float maxFuel;
    [SerializeField] float minFuelUse;
    [SerializeField] float maxFuelUse;
    [SerializeField] float maxUseSpeed;
    [SerializeField] Transform Needle;

    [SerializeField] float fullFuelAngle = -45f;
    [SerializeField] float emptyFuelAngle = -135f;

    float fuel;
    Quaternion initialNeedleRotation;

    public bool HasFuel => fuel > 0;

    void Awake()
    {
        fuel = maxFuel;
        initialNeedleRotation = Needle.localRotation;
        UpdateVisuals();
    }

    public void UseFuel(float speed)
    {
        fuel -= Mathf.Lerp(
            minFuelUse,
            maxFuelUse,
            Mathf.InverseLerp(0, maxUseSpeed, speed)
        ) * Time.deltaTime;

        UpdateVisuals();
    }

    public void Refill(float amount)
    {
        fuel = Mathf.Min(fuel + amount, maxFuel);
        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        float fuelPercent = maxFuel > 0
            ? Mathf.Clamp01(fuel / maxFuel)
            : 0f;

        float angle = Mathf.Lerp(
            emptyFuelAngle,
            fullFuelAngle,
            fuelPercent
        );

        Needle.localRotation = initialNeedleRotation
            * Quaternion.AngleAxis(angle, Vector3.up);
    }
}