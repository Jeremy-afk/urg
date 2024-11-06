using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewBehaviourScript : MonoBehaviour
{
    private Rigidbody rigidBody;

    //Variables for forward/backward movements
    [SerializeField]
    private float movementsSpeed = 500.0f;
    [SerializeField]
    private float maxSpeed = 30.0f;
    private Vector3 movements;
    private bool holdingZS;

    //Variables for left/right movements
    private Vector3 rotations;
    [SerializeField]
    private float rotationSpeed = 45.0f;
    private bool holdingQD;

    //Variables for drifting
    [SerializeField, Range(0, 1)] private float driftFactor;
    private bool holdingDrift = false;

    public void AccelerateDecelerate(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            holdingZS = true;
            movements = new Vector3(0, 0, context.ReadValue<float>()) * movementsSpeed * Time.deltaTime;
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
            rotations = new Vector3(0, context.ReadValue<Vector2>().y, 0) * rotationSpeed * Time.deltaTime;
        }
        if (context.canceled)
        {
            holdingQD=false;
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

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
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
            Vector3 driftForce = -transform.right * rotations.y * movementsSpeed * driftFactor;
            rigidBody.AddForce(driftForce, ForceMode.Acceleration);

            // Slightly increase the turn angle to exaggerate the drift effect
            float driftTurnAmount = rotations.y * rotationSpeed * 1.5f * Time.deltaTime;
            Quaternion driftTurnRotation = Quaternion.Euler(0f, driftTurnAmount, 0f);
            rigidBody.MoveRotation(rigidBody.rotation * driftTurnRotation);
        }

        
    }
}
