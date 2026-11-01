using UnityEngine;

public class RandomStartData : MonoBehaviour
{
    public bool startWithRandomData;
    public NoiseData noiseData;

    void Awake()
    {
        if (startWithRandomData)
        {
            noiseData.LaunchWithRandomData();
        }
    }
}
