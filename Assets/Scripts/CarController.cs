using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    [Header("Car Settings")]
    [SerializeField] private float acceleration;
    [SerializeField] private float decceleration;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField, Range(0, 1)] private float driftFactor;

    private Rigidbody rb;
    private bool isDrifting;
    private float moveInput = 0f;
    private float turnInput = 0f;
    private bool driftInput = false;

    private AudioSource hornSound;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        hornSound = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        // Update moveInput based on input action
        moveInput = context.ReadValue<float>();
    }

    public void OnTurn(InputAction.CallbackContext context)
    {
        // Update turnInput based on input action
        Vector2 turnVector = context.ReadValue<Vector2>();
        turnInput = turnVector.x;
    }

    public void OnDrift(InputAction.CallbackContext context)
    {
        // Update driftInput based on input action
        driftInput = context.ReadValueAsButton();
    }

    public void OnHorn(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            hornSound.Play();
        }
    }

    public void OnItem(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //add behaviour
        }
    }

    private void HandleMovement()
    {
        // Apply forward/backward force based on moveInput
        if (moveInput > 0)
        {
            rb.AddForce(transform.forward * moveInput * acceleration, ForceMode.Acceleration);
        }
        else if (moveInput < 0)
        {
            rb.AddForce(transform.forward * moveInput * decceleration, ForceMode.Acceleration);
        }

        // Clamp the speed to maxSpeed
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);

        // Handle turning
        if (moveInput != 0)
        {
            float turnAmount = turnInput * turnSpeed * Time.deltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, turnAmount, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }

        // Handle drifting
        if (driftInput)
        {
            isDrifting = true;
            rb.velocity = Vector3.Lerp(rb.velocity, transform.forward * rb.velocity.magnitude * 0.7f, driftFactor * Time.deltaTime);

            // Apply a slight sideways force opposite to the turn direction to enhance sliding
            Vector3 driftForce = -transform.right * turnInput * acceleration * driftFactor;
            rb.AddForce(driftForce, ForceMode.Acceleration);

            // Slightly increase the turn angle to exaggerate the drift effect
            float driftTurnAmount = turnInput * turnSpeed * 1.5f * Time.deltaTime;
            Quaternion driftTurnRotation = Quaternion.Euler(0f, driftTurnAmount, 0f);
            rb.MoveRotation(rb.rotation * driftTurnRotation);
        }
        else
        {
            isDrifting = false;
        }
    }
}

