using UnityEngine;

public class Scrap : Loot
{
    [SerializeField] int value;

    public override void Collect()
    {
        ScrapManager.Instance.AddValue(value);
    }
}
