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
    private float translationAcceleration;
    private bool holdingZS;

    // Variables for left/right movements
    private Vector3 rotations;
    [SerializeField]
    private float rotationSpeed;
    private bool holdingQD;

    // Variables for drifting
    [SerializeField, Range(0, 1)] private float driftFactor;
    private bool holdingDrift = false;

    //Variables for Item 
    [SerializeField]  private float forceSpeedBoost = 5.0f;
    private float speedBoostTimer = 0.0f;
    private float speedBoostDuration = 0.5f;
    private bool usedPotion = false;

    [SerializeField] private Arrow arrowPrefab;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, 0);

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

        controls.Player.TurnLeftRight.Enable();
        controls.Player.TurnLeftRight.performed += TurnLeftRight;
        controls.Player.TurnLeftRight.canceled += TurnLeftRight;

        controls.Player.Drift.Enable();
        controls.Player.Drift.performed += Drift;
        controls.Player.Drift.canceled += Drift;

        // Gotta also do it for Item and Klaxon
        controls.Player.Item.Enable();
        controls.Player.Item.performed += Item;
        controls.Player.Item.canceled += Item;

        controls.Player.Klaxon.Enable();
        controls.Player.Klaxon.performed += Klaxon;
        controls.Player.Klaxon.canceled += Klaxon;
    }

    private void OnDisable()
    {
        controls.Player.AccelerateDecelerate.Disable();
        controls.Player.AccelerateDecelerate.performed -= AccelerateDecelerate;
        controls.Player.AccelerateDecelerate.canceled -= AccelerateDecelerate;

        controls.Player.TurnLeftRight.Disable();
        controls.Player.TurnLeftRight.performed -= TurnLeftRight;
        controls.Player.TurnLeftRight.canceled -= TurnLeftRight;

        controls.Player.Drift.Disable();
        controls.Player.Drift.performed -= Drift;
        controls.Player.Drift.canceled -= Drift;

        // Gotta also to it for Item and Klaxon
        controls.Player.Item.Disable();
        controls.Player.Item.performed -= Item;
        controls.Player.Item.canceled -= Item;

        controls.Player.Klaxon.Disable();
        controls.Player.Klaxon.performed -= Klaxon;
        controls.Player.Klaxon.canceled -= Klaxon;
    }

    public void AccelerateDecelerate(InputAction.CallbackContext context)
    {
        // This is called whenever the buttons associated with accelerating/decelerating are pressed (performed) or released (canceled)
        if (context.performed)
        {
            holdingZS = true;
            float direction = context.ReadValue<float>();
            print(direction);
            translationAcceleration = direction * movementsSpeed * Time.fixedDeltaTime;
        }
        if (context.canceled)
        {
            holdingZS = false;
        }
    }

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

    public void Item(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // TODO
            // creates an instance of the item ans apply its effect
            switch (ItemManager.Instance.GetItemInHand())
            {
                case ItemBox.ItemType.BOW:
                    print("Headshot!");
                    Vector3 spawnPosition = transform.position + offset;
                    Arrow newArrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
                    newArrow.SetDirection(transform.forward);
                    break;
                case ItemBox.ItemType.FEATHER:
                    print("Yahoo!");
                    break;
                case ItemBox.ItemType.POTION:
                    print("Glou glou!");
                    translationAcceleration *= forceSpeedBoost;
                    usedPotion = true;
                    break;
                case ItemBox.ItemType.SWORD:
                    print("Chling!");
                    break;
                case ItemBox.ItemType.TRAP:
                    print("Trapped loser!");
                    break;
                case ItemBox.ItemType.NOTHING:
                    print("You have no item!");
                    break;
                default:
                    print("Error : not an item!");
                    break;
            }
            ItemManager.Instance.SetItemInHand(ItemBox.ItemType.NOTHING);
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
        if (!usedPotion)
        {
            rigidBody.velocity = Vector3.ClampMagnitude(rigidBody.velocity, maxSpeed);
        }
        if (holdingZS)
        {
            rigidBody.AddForce(translationAcceleration * transform.forward, ForceMode.Acceleration);
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
        if (usedPotion)
        {
            rigidBody.velocity = Vector3.ClampMagnitude(rigidBody.velocity, maxSpeed*2);
            speedBoostTimer += Time.deltaTime;
            if(speedBoostTimer > speedBoostDuration)
            {
                translationAcceleration /= forceSpeedBoost;
                speedBoostTimer = 0f;
                usedPotion = false;
            }
        }
    }
}
