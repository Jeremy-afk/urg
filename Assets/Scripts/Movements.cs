using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField]
    private Rigidbody rigidBody;

    //Variables for movements
    [SerializeField]
    private float speed = 5.0f;
    private float maxSpeed = 8.0f;

    public void Accelerate(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            rigidBody.AddForce(new Vector3(1.0f, 0.0f, 0.0f) * speed);
        }
    }

    public void Dcelerate(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            rigidBody.AddForce(new Vector3(-1.0f, 0.0f, 0.0f) * speed);
        }
    }

    public void MoveLeftRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            rigidBody.AddForce(new Vector3(0.0f, 0.0f, context.ReadValue<Vector3>().y) * speed);
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
        
    }

    // Update is called once per frame
    void Update()
    {
        rigidBody.velocity = Vector2.ClampMagnitude(rigidBody.velocity, maxSpeed);
    }
}
