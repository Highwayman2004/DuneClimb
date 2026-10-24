using Unity.VisualScripting;
using UnityEngine;

public class WheelControl : MonoBehaviour
{
    [SerializeField] bool steerable;
    [SerializeField] bool motorized;

    public bool Steerable => steerable;
    public bool Motorized => motorized;
    public WheelCollider WheelCollider { private set; get; } = null;

    void Start()
    {
        WheelCollider = GetComponent<WheelCollider>();
    }
}