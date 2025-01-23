using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField]
    private Transform initialCamPos; // Position initiale de la caméra
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

    private Quaternion targetCameraRotation; // Rotation cible de la caméra
    private Vector3 targetCameraPosition;   // Position cible de la caméra
    private bool isTurning = false; // Indique si la voiture tourne

    [SerializeField]
    private float cameraLerpSpeedRotation = 5f; // Vitesse de transition de la caméra
    [SerializeField]
    private float cameraLerpSpeedPosition = 50f; // Vitesse de transition de la caméra

    public void SetMaxFOV(float newMaxFOV)
    {
        maxFOV = newMaxFOV;
    }

    private void Start()
    {
        if (!isLocalPlayer) return;

        mainCamera = Camera.main;

        // Configurer la position et rotation initiales
        mainCamera.transform.SetParent(null); // Découpler temporairement la caméra pour gérer la rotation correctement
        mainCamera.transform.position = initialCamPos.position;
        mainCamera.transform.rotation = initialCamPos.rotation;

        targetCameraRotation = mainCamera.transform.rotation;
        targetCameraPosition = mainCamera.transform.position;

        mainCamera.fieldOfView = minFOV;

        moves = GetComponent<Movements>();

        // Enregistrer le joueur dans le GameManager
        GameManager.Instance.RegisterPlayer(GetComponent<NetworkIdentity>());
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
            }
        }
    }

    private void FixedUpdate()
    {
        if (collideWithPlayer)
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
        if (!isLocalPlayer) return;

        // Ajuster le FOV en fonction de la vitesse
        float ratio = rigidBody.velocity.magnitude / moves.GetMaxSpeed();
        mainCamera.fieldOfView = Mathf.Lerp(minFOV, maxFOV, ratio);

        if (moves.GetHoldingDrift())
        {
            // Si la voiture tourne, garder une rotation fixe mais orientée vers l'avant
            Vector3 driftOffset = transform.right * 0.75f * moves.GetRotations().y; // Décalage selon la direction du drift
            targetCameraRotation = Quaternion.LookRotation(transform.forward + driftOffset, Vector3.up);
        }
        else
        {
            targetCameraRotation = Quaternion.LookRotation(transform.forward, Vector3.up);
        }

        // Appliquer une interpolation fluide à la rotation et à la position de la caméra
        targetCameraPosition = initialCamPos.position;
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetCameraPosition, Time.deltaTime * cameraLerpSpeedPosition);
        mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, targetCameraRotation, Time.deltaTime * cameraLerpSpeedRotation);
    }
}