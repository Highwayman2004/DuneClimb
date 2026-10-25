using UnityEngine;

public class RandomStartData : MonoBehaviour
{
    public bool startWithRandomData;
    public NoiseData noiseData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //mapGenerator = new MapGenerator();
        if(startWithRandomData)
        {
            noiseData.LaunchWithRandomData();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
