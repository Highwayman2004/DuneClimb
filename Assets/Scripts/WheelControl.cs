using Unity.VisualScripting;
using UnityEngine;

public class WheelControl : MonoBehaviour
{
    [SerializeField] Transform wheelModel;

    [SerializeField] bool steerable;
    [SerializeField] bool motorized;

    public bool Steerable => steerable;
    public bool Motorized => motorized;
    public WheelCollider WheelCollider { private set; get; } = null;

    void Start()
    {
        WheelCollider = GetComponent<WheelCollider>();
    }

    void Update()
    {
        WheelCollider.GetWorldPose(out Vector3 position, out Quaternion rotation);
        wheelModel.transform.position = position;
        wheelModel.transform.rotation = rotation * Quaternion.Euler(90, 90, 0);
    }
}