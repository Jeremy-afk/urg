using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewBehaviourScript : MonoBehaviour
{
    private Rigidbody rigidBody;

    //Variables for forward/backward movements
    [SerializeField]
    private float movementsSpeed = 20.0f;
    [SerializeField]
    private float maxSpeed = 30.0f;
    private Vector3 movements;
    private bool holdingZS;

    //Variables for left/right movements
    private Vector3 rotations;
    [SerializeField]
    private float rotationSpeed = 5.0f;
    private bool holdingQD;

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
            // TODO
            //drift + starts a timer (check the shmup game to see how it works)
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
    void Update()
    {
        if (holdingZS)
        {
            rigidBody.velocity += movements;
        }
        rigidBody.velocity = Vector3.ClampMagnitude(rigidBody.velocity, maxSpeed);

        if (holdingQD)
        {
            Quaternion quaternionRotation = Quaternion.Euler(rotations);
            rigidBody.MoveRotation(quaternionRotation);
        }
    }
}
