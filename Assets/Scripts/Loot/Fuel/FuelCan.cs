using System.Collections;
using UnityEngine;

public class FuelCan : Loot
{
    [SerializeField] float refillAmount;
    [Space]
    [SerializeField] Transform visuals;
    [SerializeField] float rotationSpeed;
    [SerializeField] float collectTime;

    float rotation;
    bool collected = false;

    void Update()
    {
        rotation += rotationSpeed * Time.deltaTime;
        visuals.localRotation = Quaternion.Euler(0, rotation, 0);
    }

    public override void Set()
    {
        base.Set();
        collected = false;
    }

    public override void Collect()
    {
        if (collected)
            return;

        base.Collect();

        collected = true;
        FuelController.Instance.Refill(refillAmount);

        StartCoroutine(PlayCollectAnimation());
    }

    IEnumerator PlayCollectAnimation()
    {
        float time = 0;

        while(time < collectTime)
        {
            time += Time.deltaTime;

            float scale = Mathf.Lerp(1.2f, 0.1f, time / collectTime);
            visuals.localScale = new(scale, scale, scale);
            yield return null;
        }

        Release();
    }
}
