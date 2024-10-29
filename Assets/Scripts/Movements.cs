using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewBehaviourScript : MonoBehaviour
{
    private Rigidbody rigidBody;

    //Variables for forward/backward movements
    [SerializeField]
    private float speed = 20.0f;
    [SerializeField]
    private float maxSpeed = 30.0f;
    private Vector3 movements;
    private bool holding;

    //Variables for left/right movements
    private Vector3 rotations;

    int counter = 0;

    public void AccelerateDecelerate(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            holding = true;
            movements = new Vector3(0, 0, context.ReadValue<float>()) * speed * Time.deltaTime;
        }
        if (context.canceled)
        {
            holding = false;
        }
    }

    public void MoveLeftRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            rotations = new Vector3(0, context.ReadValue<Vector2>().y, 0) * speed;
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
        if (holding /*&& rigidBody.velocity.x < maxSpeed && rigidBody.velocity.x > -maxSpeed*/)
        {
            rigidBody.velocity += movements;
            counter++;
        }
        rigidBody.velocity = Vector3.ClampMagnitude(rigidBody.velocity, maxSpeed);
    }
}
