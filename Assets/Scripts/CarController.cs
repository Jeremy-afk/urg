using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    [Header("Car Settings")]
    [SerializeField] private float acceleration; 
    [SerializeField] private float decceleration;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField, Range(0,1)] private float driftFactor;

    private Rigidbody rb;
    private Gamepad gamepad;
    private bool isDrifting;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        gamepad = Gamepad.current;
        if (gamepad == null)
        {
            Debug.LogWarning("No gamepad connected!");
            return;
        }

        HandleMovement();
    }
    
    private void HandleMovement()
    {
        float moveInput = 0f;
        if (gamepad.buttonSouth.isPressed)  // B button for moving backward
        {
            moveInput = -1f;
        }
        else if (gamepad.buttonEast.isPressed)  // A button for moving forward
        {
            moveInput = 1f;
        }

        float turnInput = gamepad.leftStick.x.ReadValue();
        bool driftInput = gamepad.rightTrigger.isPressed;

        if (moveInput > 0)
        {
            rb.AddForce(transform.forward * moveInput * acceleration, ForceMode.Acceleration);
        }
        else if (moveInput < 0)
        {
            rb.AddForce(transform.forward * moveInput * decceleration, ForceMode.Acceleration);
        }

        
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
            print("Drifting");
            isDrifting = true;
            rb.velocity = Vector3.Lerp(rb.velocity, transform.forward * rb.velocity.magnitude, driftFactor * Time.deltaTime);
        }
        else
        {
            isDrifting = false;
        }
    }
}
