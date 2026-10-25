using UnityEngine;

[CreateAssetMenu]
public class TerrainData : UpdatableData
{
    public float uniformScale = 2.5f;
    public float meshHeighMultiplier;
    public AnimationCurve meshHeightCurve;

    /*public float minHeight
    {
        get
        {
            return uniformScale * meshHeighMultiplier * meshHeightCurve.Evaluate(0);
        }
    }


    public float maxHeight
    {
        get
        {
            return uniformScale * meshHeighMultiplier * meshHeightCurve.Evaluate(1);
        }
    }*/
}
