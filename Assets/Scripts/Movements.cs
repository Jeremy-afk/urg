using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movements : NetworkBehaviour
{
    private Rigidbody rigidBody;

    // Variables for forward/backward movements
    [SerializeField]
    private float movementsSpeed = 500.0f;
    [SerializeField]
    private float maxSpeed = 30.0f;
    private Vector3 movements;
    private bool holdingZS;

    // Variables for left/right movements
    private Vector3 rotations;
    [SerializeField]
    private float rotationSpeed = 45.0f;
    private bool holdingQD;

    // Variables for drifting
    [SerializeField, Range(0, 1)] private float driftFactor;
    private bool holdingDrift = false;

    // Variables for klaxon
    private AudioSource klaxonSound;

    private Controls controls;

    private void Awake()
    {
        controls = new();
    }

    private void OnEnable()
    {
        controls.Player.AccelerateDecelerate.Enable();
        controls.Player.AccelerateDecelerate.performed += AccelerateDecelerate;
        controls.Player.AccelerateDecelerate.canceled += AccelerateDecelerate;

        controls.Player.MoveLeftRight.Enable();
        controls.Player.MoveLeftRight.performed += MoveLeftRight;
        controls.Player.MoveLeftRight.canceled += MoveLeftRight;

        controls.Player.Drift.Enable();
        controls.Player.Drift.performed += MoveLeftRight;
        controls.Player.Drift.canceled += MoveLeftRight;

        // Gotta also to it for Item and Klaxon
    }

    private void OnDisable()
    {
        controls.Player.AccelerateDecelerate.Disable();
        controls.Player.AccelerateDecelerate.performed -= AccelerateDecelerate;
        controls.Player.AccelerateDecelerate.canceled += AccelerateDecelerate;

        controls.Player.MoveLeftRight.Disable();
        controls.Player.MoveLeftRight.performed -= MoveLeftRight;
        controls.Player.MoveLeftRight.canceled -= MoveLeftRight;

        controls.Player.Drift.Disable();
        controls.Player.Drift.performed -= MoveLeftRight;
        controls.Player.Drift.canceled -= MoveLeftRight;

        // Gotta also to it for Item and Klaxon
    }

    public void AccelerateDecelerate(InputAction.CallbackContext context)
    {
        // This is called whenever the buttons associated with accelerating/decelerating are pressed (performed) or released (canceled)
        if (context.performed)
        {
            holdingZS = true;
            float direction = context.ReadValue<float>();
            print(direction);
            movements = direction * movementsSpeed * Time.fixedDeltaTime * transform.forward;
        }
        if (context.canceled)
        {
            holdingZS = false;
        }
    }

    public void MoveLeftRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            holdingQD = true;
            rotations = rotationSpeed * Time.fixedDeltaTime * context.ReadValue<float>() * transform.right;
        }
        if (context.canceled)
        {
            holdingQD = false;
        }
    }

    public void Drift(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
           holdingDrift=true;
        }
        if (context.canceled)
        {
            holdingDrift = true;
        }
    }

    public void Item(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // TODO
            // creates an instance of the item ans apply its effect
        }
    }

    public void Klaxon(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            klaxonSound.Play();
        }
    }

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        klaxonSound = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        // If this is not the local player, don't touch anything
        if (!isLocalPlayer) return;
        // Otherwise, this means this is the local player's car. Thus handle the movement.
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (holdingZS)
        {
            //rigidBody.velocity += movements;
            rigidBody.AddForce(movements, ForceMode.Acceleration);
        }
        rigidBody.velocity = Vector3.ClampMagnitude(rigidBody.velocity, maxSpeed);

        if (holdingQD)
        {
            Quaternion quaternionRotation = Quaternion.Euler(rotations);
            rigidBody.MoveRotation(rigidBody.rotation * quaternionRotation);
        }

        if (holdingDrift)
        {
            // Reduce forward speed while drifting for a looser control feel
            rigidBody.velocity = Vector3.Lerp(rigidBody.velocity, transform.forward * rigidBody.velocity.magnitude * 0.7f, driftFactor * Time.deltaTime);

            // Apply a slight sideways force opposite to the turn direction to enhance sliding
            Vector3 driftForce = driftFactor * movementsSpeed * rotations.y * -transform.right;
            rigidBody.AddForce(driftForce, ForceMode.Acceleration);

            // Slightly increase the turn angle to exaggerate the drift effect
            float driftTurnAmount = rotations.y * rotationSpeed * 1.5f * Time.deltaTime;
            Quaternion driftTurnRotation = Quaternion.Euler(0f, driftTurnAmount, 0f);
            rigidBody.MoveRotation(rigidBody.rotation * driftTurnRotation);
        }
    }
}
