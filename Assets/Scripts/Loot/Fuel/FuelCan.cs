using UnityEngine;

public class FuelCan : Loot
{
    [SerializeField] float refillAmount;
    [Space]
    [SerializeField] Transform visuals;
    [SerializeField] float rotationSpeed;

    float rotation;

    void Update()
    {
        rotation += rotationSpeed;
        visuals.localRotation = Quaternion.Euler(-90, rotation, 0);
    }

    public override void Collect()
    {
        FuelController.Instance.Refill(refillAmount);
    }
}
