using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    [SerializeField] 
    private Transform initialCamPos;
    [SerializeField]
    private Rigidbody rigidBody;
    [SerializeField] 
    private float repulsiveForce = 5.0f;
    private float repulsionTimer = 0.0f;
    private float repulsiveDuration = 0.5f;

    private bool collideWithPlayer = false;

    private Movements moves;

    private Camera mainCamera;
    [SerializeField]
    private float minFOV = 60.0f;
    [SerializeField]
    private float maxFOV = 120.0f;

    public void SetMaxFOV(float newMaxFOV)
    {
        maxFOV = newMaxFOV;
    }

    private void Start()
    {
        if (isLocalPlayer)
        {
            //rigidBody = GetComponent<Rigidbody>();
            //Debug.Log(rigidBody == null);

            mainCamera = Camera.main;
            mainCamera.transform.SetParent(gameObject.transform);
            mainCamera.transform.localPosition = initialCamPos.localPosition;
            mainCamera.transform.rotation = initialCamPos.rotation;
            mainCamera.fieldOfView = 60.0f;

            moves = GetComponent<Movements>();

            // Register to the game manager
            GameManager.Instance.RegisterPlayer(GetComponent<NetworkIdentity>());
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent(out NetworkIdentity id))
        {
            if (id.isLocalPlayer)
            {
                if (other.gameObject.CompareTag("Player"))
                {
                    collideWithPlayer = true;
                    Debug.Log("Collide with another player");
                }
                else
                {

                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (collideWithPlayer == true)
        {
            rigidBody.AddForce(-repulsiveForce * new Vector3(transform.forward.x, 0, transform.forward.z) * 100, ForceMode.Acceleration);
            rigidBody.rotation.SetFromToRotation(new Vector3(0, transform.forward.y, 0), new Vector3(0, transform.forward.y + 135, 0));
            repulsionTimer += Time.deltaTime;
            if (repulsionTimer > repulsiveDuration)
            {
                collideWithPlayer = false;
            }
        }
    }

    private void Update()
    {
        float ratio = rigidBody.velocity.magnitude / moves.GetMaxSpeed();
        mainCamera.fieldOfView = Mathf.Lerp(minFOV, maxFOV, ratio);
    }
}
