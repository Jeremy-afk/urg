using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class DebugMoveTest : NetworkBehaviour
{
    public float moveSpeed = 10f;
    public float rotationSpeed = 100f;

    private float accelerationInput;
    private float moveDirectionInput;

    private Rigidbody rb;

    private InputAction accelerateDecelerateAction;
    private InputAction moveLeftRightAction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>(); // Initialize Rigidbody in Awake
    }

    public override void OnStartLocalPlayer()
    {
        print("OnStartLocalPlayer - Player initialized, setting up input actions.");

        Controls playerInputActions = new Controls();

        accelerateDecelerateAction = playerInputActions.Player.AccelerateDecelerate;
        moveLeftRightAction = playerInputActions.Player.TurnLeftRight;

        accelerateDecelerateAction.Enable();
        accelerateDecelerateAction.performed += OnAccelerateDecelerate;
        accelerateDecelerateAction.canceled += OnAccelerateDecelerate;

        moveLeftRightAction.Enable();
        moveLeftRightAction.performed += OnMoveLeftRight;
        moveLeftRightAction.canceled += OnMoveLeftRight;
    }

    private void OnDisable()
    {
        if (!isLocalPlayer) return;

        // Unsubscribe from input events to avoid memory leaks
        accelerateDecelerateAction.performed -= OnAccelerateDecelerate;
        accelerateDecelerateAction.canceled -= OnAccelerateDecelerate;

        moveLeftRightAction.performed -= OnMoveLeftRight;
        moveLeftRightAction.canceled -= OnMoveLeftRight;
    }

    public void OnAccelerateDecelerate(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;

        if (context.performed)
            accelerationInput = context.ReadValue<float>();
        else if (context.canceled)
            accelerationInput = 0f;
    }

    public void OnMoveLeftRight(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;

        moveDirectionInput = context.ReadValue<float>();
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer) return;

        // Forward motion (acceleration/deceleration)
        float forwardMotion = (accelerationInput) * moveSpeed * Time.fixedDeltaTime;

        // Apply forward movement
        Vector3 move = transform.forward * forwardMotion;
        rb.MovePosition(rb.position + move);

        // Rotate based on left/right input (moveDirectionInput)
        float rotation = moveDirectionInput * rotationSpeed * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0, rotation, 0));

        // Tell the server to synchronize the movement for other players
        //CmdSyncMovement(rb.position, rb.rotation);
    }

    [Command]
    private void CmdSyncMovement(Vector3 position, Quaternion rotation)
    {
        // Server updates all clients with the new position/rotation
        RpcSyncMovement(position, rotation);
    }

    [ClientRpc]
    private void RpcSyncMovement(Vector3 position, Quaternion rotation)
    {
        if (!isLocalPlayer) // Only update for remote clients
        {
            rb.MovePosition(position);
            rb.MoveRotation(rotation);
        }
    }
}
