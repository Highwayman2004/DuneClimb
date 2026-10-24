using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScrapManager : Singleton<ScrapManager>
{
    [SerializeField] TMP_Text scrapText;
    [SerializeField] float scrapIncrementSpeed;

    int totalValue;

    Coroutine incrementAnimation;
    int visualValue;



    public void AddValue(int amount)
    {
        totalValue += amount;
        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        if (incrementAnimation != null)
            return;

        incrementAnimation = StartCoroutine(PlayIncrementAnimation());
    }

    IEnumerator PlayIncrementAnimation()
    {
        while(visualValue < totalValue)
        {
            visualValue++;
            scrapText.text = visualValue.ToString();

            float speedMultiplier = 1 + (totalValue - visualValue) / 10;

            yield return new WaitForSeconds(scrapIncrementSpeed * (1 / speedMultiplier));
        }

        incrementAnimation = null;
    }
}
