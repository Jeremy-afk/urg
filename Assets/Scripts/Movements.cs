using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movements : NetworkBehaviour
{
    [SerializeField] private float naturalBreak = 0.5f;
    private Rigidbody rigidBody;

    [Header("Verticality")]
    [SerializeField] private Transform groundRayPoint;
    [SerializeField] private float groundRayLength = 1.5f;
    [SerializeField] private LayerMask raycastTarget;

    [Header("Acceleration")]
    [SerializeField] private AnimationCurve accelerationCurve;
    [SerializeField] private float movementsSpeed = 65.0f;
    [SerializeField] private float maxSpeedNoDrifting = 100.0f;
    [SerializeField] private float maxSpeedDrifting = 75.0f;
    private float maxSpeed = 100f;

    [SyncVar] private float accelerationDirection;
    [SyncVar] private bool holdingZS;
    [SyncVar] private float bonusSpeedMult = 1f;
    [SyncVar] private float bonusSpeedMultTime = 0f;

    [Header("Turn movements")]
    private Vector3 rotations;
    [SerializeField] private float rotationSpeed;
    private bool holdingQD;
    [SerializeField] private float turnDrag = 0.25f;
    [SerializeField] private float normalDrag = 0.1f;

    [Header("Drifting")]
    [SerializeField, Range(0, 1)] private float driftFactor;
    [SerializeField] private ParticleSystem rightWheelPart;
    [SerializeField] private ParticleSystem leftWheelPart;
    private bool holdingDrift = false;

    [Header("Animations")]
    [SerializeField] private GameObject smokeObject;

    private AudioSource klaxonSound;
    private Player activePlayer;
    [SyncVar] private bool canMove = false;

    public float GetMaxSpeed() => maxSpeed;
    public Vector3 GetRotations() => rotations;
    public bool GetHoldingDrift() => holdingDrift;

    public override void OnStartAuthority()
    {
        enabled = true; // Enable input processing only for the local player
    }

    [Server]
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

        if (isLocalPlayer) HandleLooks();
    }

    private void FixedUpdate()
    {
        if (!isServer) return; // Physics only runs on the server
        if (!canMove) return;  // Player must be allowed to move

        HandleMovement();
        HandleVerticality();
    }

    private void HandleLooks()
    {
        smokeObject.SetActive(holdingZS);

        if (canMove && holdingQD && holdingDrift)
        {
            rightWheelPart.Play();
            leftWheelPart.Play();
        }
        else
        {
            rightWheelPart.Stop();
            leftWheelPart.Stop();
        }
    }

    private void HandleVerticality()
    {
        if (Physics.Raycast(groundRayPoint.position, -transform.up, out RaycastHit hit, groundRayLength, raycastTarget))
        {
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            Vector3 currentEulerAngles = transform.eulerAngles;
            Vector3 targetEulerAngles = targetRotation.eulerAngles;

            Quaternion adjustedRotation = Quaternion.Euler(targetEulerAngles.x, currentEulerAngles.y, targetEulerAngles.z);
            rigidBody.MoveRotation(Quaternion.Slerp(rigidBody.rotation, adjustedRotation, Time.fixedDeltaTime * 5f));
        }
    }

    private void HandleMovement()
    {
        rigidBody.drag = normalDrag;
        activePlayer.SetMaxFOV(bonusSpeedMultTime > 0 ? 150.0f : 120.0f);

        float currentSpeed = Vector3.Dot(rigidBody.velocity, transform.forward);
        float targetSpeed = accelerationDirection * maxSpeed * (bonusSpeedMultTime > 0 ? bonusSpeedMult : 1);
        float acceleration = accelerationCurve.Evaluate(currentSpeed / targetSpeed) * movementsSpeed * accelerationDirection * (bonusSpeedMultTime > 0 ? bonusSpeedMult : 1);

        rigidBody.AddForce(acceleration * transform.forward, ForceMode.Acceleration);

        if (holdingQD)
        {
            if (!holdingDrift) rigidBody.drag = turnDrag;
            if (Mathf.Abs(currentSpeed) > 0.1f)
            {
                Quaternion quaternionRotation = Quaternion.Euler(rotations);
                rigidBody.MoveRotation(rigidBody.rotation * quaternionRotation);
            }
        }

        if (holdingDrift)
        {
            maxSpeed = maxSpeedDrifting;
            rigidBody.velocity = Vector3.Lerp(rigidBody.velocity, 0.7f * rigidBody.velocity.magnitude * transform.forward, driftFactor * Time.deltaTime);

            Vector3 driftForce = driftFactor * rotations.y * -transform.right;
            rigidBody.AddForce(driftForce, ForceMode.Acceleration);

            float driftTurnAmount = rotations.y * rotationSpeed * Time.deltaTime;
            Quaternion driftTurnRotation = Quaternion.Euler(0f, driftTurnAmount, 0f);
            rigidBody.MoveRotation(rigidBody.rotation * driftTurnRotation);
        }
        else
        {
            maxSpeed = maxSpeedNoDrifting;
        }
    }

    [Command]
    private void CmdSetAcceleration(float accelerationDir)
    {
        accelerationDirection = accelerationDir;
        holdingZS = accelerationDir != 0;
    }

    [Command]
    private void CmdSetTurn(float turnDir)
    {
        rotations.y = turnDir;
        holdingQD = turnDir != 0;
    }

    [Command]
    private void CmdSetDrift(bool drift)
    {
        holdingDrift = drift;
    }

    [Client]
    public void OnMove(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;
        float moveInput = context.ReadValue<float>();
        holdingZS = moveInput != 0;
        CmdSetAcceleration(moveInput);
    }

    [Client]
    public void OnTurn(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;
        float turnInput = context.ReadValue<float>();
        holdingQD = turnInput != 0;
        CmdSetTurn(turnInput);
    }

    [Client]
    public void OnDrift(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;
        bool drift = context.performed && holdingQD;
        holdingDrift = drift;
        CmdSetDrift(drift);
    }
}
