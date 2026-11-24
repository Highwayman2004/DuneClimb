using System.Collections;
using UnityEngine;

public class Scrap : Loot
{
    [SerializeField] Transform holder;
    [SerializeField] int value;
    [Space]
    [SerializeField] float flyUpTime;
    [SerializeField] float flyUpSpeed;
    [Space]
    [SerializeField] float collectSpeed;
    [SerializeField] float pickupSpeed;
    [SerializeField] float speedIncrease;
    [SerializeField] float rotationSpeed;

    bool collected = false;


    public override void Set()
    {
        base.Set();
        holder.localScale = new(1f, 1f, 1f);
        collected = false;

        transform.SetParent(null);
    }

    public override void Collect()
    {
        if (collected)
            return;

        base.Collect();
        collected = true;
        transform.SetParent(ScrapManager.Instance.ScrapHolder);

        ScrapManager.Instance.AddValue(value);
        StartCoroutine(PlayCollectionAnimation());
        StartCoroutine(PlayRotationAnimation());
    }

    IEnumerator PlayCollectionAnimation()
    {
        float time = 0;

        holder.localScale = new(1.15f, 1.15f, 1.15f);

        while(time < flyUpTime)
        {
            time += Time.deltaTime;
            transform.position += new Vector3(0, flyUpSpeed * Time.deltaTime);
            yield return null;
        }

        float speedMult = 1;

        while(Vector3.Distance(transform.position, LootManager.Instance.CollectPosition) > 0.6f)
        {
            speedMult += speedIncrease * Time.deltaTime;

            SetScale(LootManager.Instance.CollectPosition);
            transform.position = Vector3.MoveTowards(transform.position, LootManager.Instance.CollectPosition, collectSpeed * speedMult * Time.deltaTime);
            yield return null;
        }

        speedMult = 1 + (speedMult - 1) * 0.5f;

        while (Vector3.Distance(transform.position, LootManager.Instance.PickupPosition) > 0.25f)
        {
            speedMult += speedIncrease * Time.deltaTime;

            SetScale(LootManager.Instance.PickupPosition);
            transform.position = Vector3.MoveTowards(transform.position, LootManager.Instance.PickupPosition, pickupSpeed * speedMult * Time.deltaTime);
            yield return null;
        }

        Release();
    }

    IEnumerator PlayRotationAnimation()
    {
        Vector3 rotationDirection = Random.onUnitSphere;

        while(true)
        {
            transform.Rotate(rotationSpeed * Time.deltaTime * rotationDirection, Space.Self);
            yield return null;
        }
    }

    void SetScale(Vector3 position)
    {
        float pos = Mathf.InverseLerp(0.15f, 3.5f, Vector3.Distance(transform.position, position));
        float scale = Mathf.Lerp(0.25f, 1.15f, pos);
        holder.localScale = new(scale, scale, scale);
        Debug.Log(scale);
    }
}
