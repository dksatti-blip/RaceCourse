using UnityEngine;

public class CarSuspension : MonoBehaviour
{
    [Header("Suspension Points")]
    public Transform[] suspensionPoints = new Transform[4];

    [Header("Suspension Settings")]
    public float restLength = 0.5f;
    public float springStrength = 30000f;
    public float damperStrength = 3500f;
    public float wheelRadius = 0.35f;

    private Rigidbody carBody;

    void Awake()
    {
        carBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (carBody == null || suspensionPoints == null)
            return;

        foreach (Transform point in suspensionPoints)
        {
            if (point == null)
                continue;

            Vector3 origin = point.position;
            Vector3 direction = -transform.up;
            float rayLength = restLength + wheelRadius;

            if (Physics.Raycast(origin, direction,
                out RaycastHit hit, rayLength))
            {
                float compression =
                    restLength - (hit.distance - wheelRadius);

                compression = Mathf.Clamp(
                    compression, 0f, restLength);

                Vector3 pointVelocity =
                    carBody.GetPointVelocity(origin);

                float verticalVelocity =
                    Vector3.Dot(pointVelocity, transform.up);

                float springForce =
                    compression * springStrength;

                float damperForce =
                    verticalVelocity * damperStrength;

                float force =
                    Mathf.Max(0f, springForce - damperForce);

                carBody.AddForceAtPosition(
                    transform.up * force,
                    origin,
                    ForceMode.Force);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (suspensionPoints == null)
            return;

        Gizmos.color = Color.yellow;

        foreach (Transform point in suspensionPoints)
        {
            if (point == null)
                continue;

            Gizmos.DrawLine(
                point.position,
                point.position - transform.up *
                (restLength + wheelRadius));
        }
    }
}
