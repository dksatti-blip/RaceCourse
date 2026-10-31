
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

[RequireComponent(typeof(Rigidbody))]
public class CarController : NetworkBehaviour
{
    public float acceleration = 20f;
    public float steeringSpeed = 50f;
    public float maxSpeed = 8f;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearDamping = 2f;
        rb.angularDamping = 3f;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            enabled = false;
        }
    }

    void FixedUpdate()
    {
        if (!IsOwner || rb == null || Keyboard.current == null)
            return;

        float moveInput = 0f;
        float steerInput = 0f;

        if (Keyboard.current.wKey.isPressed ||
            Keyboard.current.upArrowKey.isPressed)
            moveInput = 1f;
        else if (Keyboard.current.sKey.isPressed ||
                 Keyboard.current.downArrowKey.isPressed)
            moveInput = -1f;

        if (Keyboard.current.aKey.isPressed ||
            Keyboard.current.leftArrowKey.isPressed)
            steerInput = -1f;
        else if (Keyboard.current.dKey.isPressed ||
                 Keyboard.current.rightArrowKey.isPressed)
            steerInput = 1f;

        Vector3 horizontalVelocity =
            new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (Mathf.Abs(moveInput) > 0.01f &&
            horizontalVelocity.magnitude < maxSpeed)
        {
            rb.AddForce(
                transform.forward * moveInput * acceleration,
                ForceMode.Acceleration);
        }

        if (Mathf.Abs(moveInput) > 0.01f &&
            Mathf.Abs(steerInput) > 0.01f)
        {
            float direction = moveInput > 0f ? 1f : -1f;
            float turn = steerInput * steeringSpeed *
                         direction * Time.fixedDeltaTime;

            rb.MoveRotation(
                rb.rotation * Quaternion.Euler(0f, turn, 0f));
        }
    }
}
