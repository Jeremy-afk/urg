using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movements : NetworkBehaviour
{
    [SerializeField] private float naturalBreak = 0.5f;
    private Rigidbody rigidBody;

    [Header("Verticality")]
    //Variables for verticality
    [SerializeField] private Transform groundRayPoint;
    [SerializeField] private float groundRayLength = 1.5f;
    [SerializeField] private LayerMask raycastTarget;

    [Header("Acceleration")]
    // Variables for forward/backward movements
    [SerializeField] private AnimationCurve accelerationCurve;
    [SerializeField] private float movementsSpeed = 65.0f;
    [SerializeField] private float maxSpeedNoDrifting = 100.0f;
    [SerializeField] private float maxSpeedDrifting = 100 - 25;
    private float maxSpeed;
    [SyncVar]
    private float accelerationDirection;
    [SyncVar]
    private bool holdingZS;

    [SyncVar]
    private float bonusSpeedMult = 1f; // Set to make a boost
    [SyncVar]
    private float bonusSpeedMultTime = 0f;

    [Header("Turn movements")]
    // Variables for left/right movements
    private Vector3 rotations;
    [SerializeField] private float rotationSpeed;
    private bool holdingQD;
    [SerializeField] private float turnDrag = 0.25f;
    [SerializeField] private float normalDrag = 0.1f;

    [Header("Drifting")]
    // Variables for drifting
    [SerializeField, Range(0, 1)] private float driftFactor;
    [SerializeField] private ParticleSystem rightWheelPart;
    [SerializeField] private ParticleSystem leftWheelPart;
    private bool holdingDrift = false;

    [Header("Animations")]
    [SerializeField] public GameObject smokeObject;

    private AudioSource klaxonSound;
    private Player activePlayer;
    [SyncVar] private bool canMove = false;

    // Called by the server to allow the player to move or not
    public void SetMovementActive(bool active)
    {
        canMove = active;

        if (!active)
        {
            rightWheelPart.Stop();
            leftWheelPart.Stop();
        }
    }

    [Server]
    public void ApplySpeedBoost(float bonus, float duration)
    {
        bonusSpeedMult = bonus;
        bonusSpeedMultTime = duration;
    }

    [Client]
    public void AccelerateDecelerate(InputAction.CallbackContext context)
    {
        // This is called whenever the buttons associated with accelerating/decelerating are pressed (performed) or released (canceled)
        if (context.performed)
        {
            holdingZS = true;
            float direction = context.ReadValue<float>();
            accelerationDirection = direction;
        }
        if (context.canceled)
        {
            holdingZS = false;
            accelerationDirection = 0;
        }
    }

    [Client]
    public void TurnLeftRight(InputAction.CallbackContext context)
    {
        // Called whenever a change is detected in the input (stick or key)
        if (context.performed)
        {
            holdingQD = true;
            rotations = rotationSpeed * Time.fixedDeltaTime * context.ReadValue<float>() * transform.up;
        }
        if (context.canceled)
        {
            holdingQD = false;
            rotations = Vector3.zero;
        }
    }

    [Client]
    public void Drift(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            holdingDrift = true;
        }
        if (context.canceled)
        {
            holdingDrift = false;
        }
    }

    [Client]
    // TODO: Move this to an AUDIO manager script
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
        activePlayer = GetComponent<Player>();
    }

    private void Update()
    {
        if (bonusSpeedMultTime > 0)
        {
            bonusSpeedMultTime -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        // If this is not the local player, or the player isn't allowed to move don't touch anything
        if (!isLocalPlayer || !canMove) return;
        // Otherwise, this means this is the local player's car. Thus handle the movement.
        HandleMovement();
        HandleVerticality();
    }

    private void HandleVerticality()
    {
        if (Physics.Raycast(groundRayPoint.position, -transform.up, out RaycastHit hit, groundRayLength, raycastTarget))
        {
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            Vector3 currentEulerAngles = transform.eulerAngles;
            Vector3 targetEulerAngles = targetRotation.eulerAngles;

            // Only changes rotation on X and Z
            Quaternion adjustedRotation = Quaternion.Euler(targetEulerAngles.x, currentEulerAngles.y, targetEulerAngles.z);

            rigidBody.MoveRotation(Quaternion.Slerp(rigidBody.rotation, adjustedRotation, Time.fixedDeltaTime * 5f));
        }
    }

    private void HandleMovement()
    {
        if (Physics.Raycast(groundRayPoint.position, -transform.up, out RaycastHit hit, groundRayLength, raycastTarget))
        {
            // Ajuste la direction de déplacement en fonction de la normale du sol
            Vector3 forward = Vector3.ProjectOnPlane(transform.forward, hit.normal).normalized; // ???
        }
        if (holdingZS)
        {
            // If the player is inputing a movement, we want to accelerate towards its max speed using an acceleration curve
            // by using the DIFFERENCE between the current speed and the max speed
            rigidBody.drag = normalDrag;
            if (bonusSpeedMultTime > 0)
            {
                activePlayer.SetMaxFOV(150.0f);
            }
            else
            {
                activePlayer.SetMaxFOV(120.0f);
            }

            float currentSpeed = Vector3.Dot(rigidBody.velocity, transform.forward);
            float targetSpeed = accelerationDirection * maxSpeed * (bonusSpeedMultTime > 0 ? bonusSpeedMult : 1);

            float acceleration;
            if (targetSpeed == 0)
            {
                // If the player is not inputing any movement, we want to decelerate to a stop
                acceleration = -currentSpeed * naturalBreak;
            }
            else
            {
                // If the player is inputing a movement, we want to accelerate towards its max speed using an acceleration curve

                // This curve allows easy customization of the acceleration response
                // From [-1 to 0]: when the player changes direction (high values = high response = high braking)
                // From [0 to 1]: when the player accelerates (high values = high response = high acceleration)
                // From [1 to 2]: when the player reaches its max speed (high values = agressive clamping to max speed)
                acceleration = accelerationCurve.Evaluate(currentSpeed / targetSpeed) * movementsSpeed * accelerationDirection * (bonusSpeedMultTime > 0 ? bonusSpeedMult : 1);
            }
            rigidBody.AddForce(acceleration * transform.forward, ForceMode.Acceleration);
            smokeObject.SetActive(true);

            //print("acceleration at " + acceleration + " m/s");
        }
        else
        {
            smokeObject.SetActive(false);
        }

        if (holdingQD)
        {
            if (!holdingDrift)
            {
                rigidBody.drag = turnDrag;
            }
            float currentSpeed = Vector3.Dot(rigidBody.velocity, transform.forward);
            if (currentSpeed < -0.1 || currentSpeed > 0.1)
            {
                Quaternion quaternionRotation = Quaternion.Euler(rotations);
                rigidBody.MoveRotation(rigidBody.rotation * quaternionRotation);
            }
        }

        if (holdingDrift)
        {
            maxSpeed = maxSpeedDrifting;
            // Reduce forward speed while drifting for a looser control feel
            rigidBody.velocity = Vector3.Lerp(rigidBody.velocity, 0.7f * rigidBody.velocity.magnitude * transform.forward, driftFactor * Time.deltaTime);

            // Apply a slight sideways force opposite to the turn direction to enhance sliding
            Vector3 driftForce = driftFactor * rotations.y * -transform.right;
            rigidBody.AddForce(driftForce, ForceMode.Acceleration);

            // Slightly increase the turn angle to exaggerate the drift effect
            float driftTurnAmount = rotations.y * rotationSpeed * Time.deltaTime;
            Quaternion driftTurnRotation = Quaternion.Euler(0f, driftTurnAmount, 0f);
            rigidBody.MoveRotation(rigidBody.rotation * driftTurnRotation);

            if (canMove && holdingQD)
            {
                rightWheelPart.Play();
                leftWheelPart.Play();
            }
        }
        else
        {
            rightWheelPart.Stop();
            leftWheelPart.Stop();
            maxSpeed = maxSpeedNoDrifting;
        }
    }

    public float GetMaxSpeed()
    {
        return maxSpeed;
    }

    public bool GetHoldingDrift()
    {
        return holdingDrift;
    }

    public Vector3 GetRotations()
    {
        return rotations;
    }

    public bool GetHoldingQD()
    {
        return holdingQD;
    }
}
