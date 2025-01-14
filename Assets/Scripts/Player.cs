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

    private void Start()
    {
        if (isLocalPlayer)
        {
            //rigidBody = GetComponent<Rigidbody>();
            //Debug.Log(rigidBody == null);

            GameObject mainCamera = Camera.main.gameObject;
            mainCamera.transform.SetParent(gameObject.transform);
            mainCamera.transform.localPosition = initialCamPos.localPosition;
            mainCamera.transform.rotation = initialCamPos.rotation;

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
            rigidBody.AddForce(-repulsiveForce * transform.forward * 100, ForceMode.Acceleration);
            rigidBody.rotation.SetFromToRotation(transform.forward, transform.forward + new Vector3(0, 45, 0));
            repulsionTimer += Time.deltaTime;
            if (repulsionTimer > repulsiveDuration)
            {
                collideWithPlayer = false;
            }
        }
    }
}
