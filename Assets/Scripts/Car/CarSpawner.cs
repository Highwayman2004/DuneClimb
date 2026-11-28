using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [SerializeField] Transform car;
    [SerializeField] Rigidbody carBody;
    [SerializeField] MapGenerator mapGenerator;
    [SerializeField] float startYaw;
    [SerializeField] float dropHeight = 0.5f;
    [SerializeField] float timeout = 15f;


    Vector3 startPoint;
    float timer;
    bool placed;


    void Start()
    {
        if (mapGenerator == null) mapGenerator = FindAnyObjectByType<MapGenerator>();
        startPoint = mapGenerator.GetStartPoint();

        carBody.isKinematic = true;
        car.SetPositionAndRotation(startPoint + Vector3.up * 5f, Quaternion.Euler(0, startYaw, 0));
    }


    void FixedUpdate()
    {
        if (placed) return;
        timer += Time.fixedDeltaTime;

        RaycastHit[] hits = Physics.RaycastAll(startPoint + Vector3.up * 500f, Vector3.down,
            1000f, ~0, QueryTriggerInteraction.Ignore);

        foreach (RaycastHit h in hits)
        {
            if (h.collider.name != "Terrain Chunk") continue;
            Release(h.point);
            return;
        }

        if (timer > timeout && timer - Time.fixedDeltaTime <= timeout)
        {
            Debug.LogWarning("Start collider never appeared; releasing the car anyway.");
            //Release(startPoint);
        }
    }


    void Release(Vector3 groundPoint)
    {
        car.SetPositionAndRotation(groundPoint + Vector3.up * dropHeight, Quaternion.Euler(0, startYaw, 0));
        carBody.linearVelocity = Vector3.zero;
        carBody.angularVelocity = Vector3.zero;
        carBody.isKinematic = false;
        placed = true;
        enabled = false;
    }
}
