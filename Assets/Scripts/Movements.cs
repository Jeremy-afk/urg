using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movements : NetworkBehaviour
{
    private Rigidbody rigidBody;

    //Variables for verticality
    [SerializeField]
    private Transform groundRayPoint;
    [SerializeField]
    private  float groundRayLength = 1.5f;
    [SerializeField]
    private LayerMask raycastTarget;

    // Variables for forward/backward movements
    [SerializeField]
    private AnimationCurve accelerationCurve;
    [SerializeField]
    private float movementsSpeed = 65.0f;
    [SerializeField]
    private float maxSpeed = 50.0f;
    [SyncVar]
    private float accelerationDirection;
    [SyncVar]
    private bool holdingZS;
    [field: SyncVar]
    public float BonusSpeedMult { get; set; } = 1f; // Set to make a boost
    [field: SyncVar]
    public float BonusSpeedMultTime { get; set; } = 0f;

    // Variables for left/right movements
    private Vector3 rotations;
    [SerializeField]
    private float rotationSpeed;
    private bool holdingQD;

    // Variables for drifting
    [SerializeField, Range(0, 1)] private float driftFactor;
    private bool holdingDrift = false;

    private AudioSource klaxonSound;
    [SyncVar]
    private bool canMove = false;
    private Player activePlayer;
    private ParticleSystem smoke;

    // Called by the server to allow the player to move or not
    public void SetMovementActive(bool active)
    {
        canMove = active;
    }

    public void AccelerateDecelerate(InputAction.CallbackContext context)
    {
        // This is called whenever the buttons associated with accelerating/decelerating are pressed (performed) or released (canceled)
        if (context.performed && canMove)
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

    public void TurnLeftRight(InputAction.CallbackContext context)
    {
        // Called whenever a change is detected in the input (stick or key)
        if (context.performed && canMove)
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

    public void Drift(InputAction.CallbackContext context)
    {
        if (context.performed && canMove)
        {
           holdingDrift = true;
        }
        if (context.canceled)
        {
            holdingDrift = false;
        }
    }
    
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
        smoke = GetComponent<ParticleSystem>();
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
        RaycastHit hit;
        if (Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLength, raycastTarget))
        {
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation; Vector3 currentEulerAngles = transform.eulerAngles;
            Vector3 targetEulerAngles = targetRotation.eulerAngles;

            // Only changes rotation on X and Z
            Quaternion adjustedRotation = Quaternion.Euler(targetEulerAngles.x, currentEulerAngles.y, targetEulerAngles.z);

            rigidBody.MoveRotation(Quaternion.Slerp(rigidBody.rotation, adjustedRotation, Time.fixedDeltaTime * 5f));
        }
    }

    private void HandleMovement()
    {
        RaycastHit hit;
        if (Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLength, raycastTarget))
        {
            // Ajuste la direction de déplacement en fonction de la normale du sol
            Vector3 forward = Vector3.ProjectOnPlane(transform.forward, hit.normal).normalized; // ???
        }
        if (holdingZS)
        {
            // If the player is inputing a movement, we want to accelerate towards its max speed using an acceleration curve
            // by using the DIFFERENCE between the current speed and the max speed

            if (BonusSpeedMultTime > 0)
            {
                print("Applying bonus!");
                BonusSpeedMultTime -= Time.fixedDeltaTime;
                activePlayer.SetMaxFOV(150.0f);
            }
            else
            {
                activePlayer.SetMaxFOV(120.0f);
            }

            float currentSpeed = Vector3.Dot(rigidBody.velocity, transform.forward);
            float targetSpeed = accelerationDirection * maxSpeed * (BonusSpeedMultTime > 0 ? BonusSpeedMult : 1);

            float acceleration;
            if (targetSpeed == 0)
            {
                // If the player is not inputing any movement, we want to decelerate to a stop
                acceleration = -currentSpeed;
            }
            else
            {
                // If the player is inputing a movement, we want to accelerate towards its max speed using an acceleration curve

                // This curve allows easy customization of the acceleration response
                // From [-1 to 0]: when the player changes direction (high values = high response = high braking)
                // From [0 to 1]: when the player accelerates (high values = high response = high acceleration)
                // From [1 to 2]: when the player reaches its max speed (high values = agressive clamping to max speed)
                acceleration = accelerationCurve.Evaluate(currentSpeed / targetSpeed) * movementsSpeed * accelerationDirection * (BonusSpeedMultTime > 0 ? BonusSpeedMult : 1);
            }
            rigidBody.AddForce(acceleration * transform.forward, ForceMode.Acceleration);
            smoke.Play();

            //print("acceleration at " + acceleration + " m/s");
        }
        else
        {
            smoke.Clear();
        }

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
            Vector3 driftForce = driftFactor * rotations.y * -transform.right;
            rigidBody.AddForce(driftForce, ForceMode.Acceleration);

            // Slightly increase the turn angle to exaggerate the drift effect
            float driftTurnAmount = rotations.y * rotationSpeed * Time.deltaTime;
            Quaternion driftTurnRotation = Quaternion.Euler(0f, driftTurnAmount, 0f);
            rigidBody.MoveRotation(rigidBody.rotation * driftTurnRotation);
        }
        else
        {
            // TODO: Apply traction to the car's wheels by converting part of the car's speed vector towards the car's forward vector
        }
    }

    public float GetMaxSpeed() {  
        return maxSpeed;
    }
}
