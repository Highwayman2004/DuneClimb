using UnityEngine;

public class FuelCan : Loot
{
    [SerializeField] float refillAmount;

    public override void Collect()
    {
        FuelController.Instance.Refill(refillAmount);
    }
}
