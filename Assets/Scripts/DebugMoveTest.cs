using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class DebugMoveTest : NetworkBehaviour
{
    public float moveSpeed = 10f;
    public float rotationSpeed = 100f;

    private float accelerationInput;
    private float decelerationInput;
    private float moveDirectionInput;

    private Rigidbody rb;

    private InputAction accelerateAction;
    private InputAction decelerateAction;
    private InputAction moveLeftRightAction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>(); // Initialize Rigidbody in Awake
    }

    public override void OnStartLocalPlayer()
    {
        print("OnStartLocalPlayer - Player initialized, setting up input actions.");

        Controls playerInputActions = new Controls();

        accelerateAction = playerInputActions.Player.Accelerate;
        decelerateAction = playerInputActions.Player.Decelerate;
        moveLeftRightAction = playerInputActions.Player.MoveLeftRight;

        accelerateAction.Enable();
        accelerateAction.performed += OnAccelerate;
        accelerateAction.canceled += OnAccelerate;

        decelerateAction.Enable();
        decelerateAction.performed += OnDecelerate;
        decelerateAction.canceled += OnDecelerate;

        moveLeftRightAction.Enable();
        moveLeftRightAction.performed += OnMoveLeftRight;
        moveLeftRightAction.canceled += OnMoveLeftRight;
    }

    private void OnDisable()
    {
        if (!isLocalPlayer) return;

        // Unsubscribe from input events to avoid memory leaks
        accelerateAction.performed -= OnAccelerate;
        accelerateAction.canceled -= OnAccelerate;

        decelerateAction.performed -= OnDecelerate;
        decelerateAction.canceled -= OnDecelerate;

        moveLeftRightAction.performed -= OnMoveLeftRight;
        moveLeftRightAction.canceled -= OnMoveLeftRight;
    }

    public void OnAccelerate(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;

        if (context.performed)
            accelerationInput = 1f;
        else if (context.canceled)
            accelerationInput = 0f;
    }

    public void OnDecelerate(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;

        if (context.performed)
            decelerationInput = -1f;
        else if (context.canceled)
            decelerationInput = 0f;
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
        float forwardMotion = (accelerationInput + decelerationInput) * moveSpeed * Time.fixedDeltaTime;

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
