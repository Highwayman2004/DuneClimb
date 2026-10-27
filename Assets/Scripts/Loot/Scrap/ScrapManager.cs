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

    void Start()
    {
        UpdateScrapText();
    }

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
            UpdateScrapText();

            float speedMultiplier = 1 + (totalValue - visualValue) / 10;

            yield return new WaitForSeconds(scrapIncrementSpeed * (1 / speedMultiplier));
        }

        incrementAnimation = null;
    }

    void UpdateScrapText()
    {
        if (visualValue <= 0)
        {
            scrapText.text = "00000";
            return;
        }

        string text = "";

        int left = 10000;

        while (left > visualValue)
        {
            text += "0";
            left /= 10;
        }

        scrapText.text = text + visualValue;
    }
}
