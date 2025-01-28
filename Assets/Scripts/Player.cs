using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Player : NetworkBehaviour, IDamageable
{
    [Header("Collisions")]
    [SerializeField] private float collisionForce = 5.0f;
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private float repulsiveForce = 5.0f;
    private float repulsionTimer = 0.0f;
    private float repulsiveDuration = 0.5f;
    private bool collideWithPlayer = false;

    [Header("Camera")]
    [SerializeField] private Transform initialCamPos; // Position initiale de la caméra
    [SerializeField] private float minFOV = 60.0f;
    [SerializeField] private float maxFOV = 120.0f;
    [SerializeField] private float cameraLerpSpeedRotation = 5f; // Vitesse de transition de la caméra
    [SerializeField] private float cameraLerpSpeedPosition = 50f; // Vitesse de transition de la caméra
    private Camera mainCamera;
    private Quaternion targetCameraRotation; // Rotation cible de la caméra
    private Vector3 targetCameraPosition;   // Position cible de la caméra

    [Header("Stun")]
    [SerializeField] private UnityEvent<float> onStun;
    private float stunTimer = 0.0f;
    private bool isStunned = false;

    private Movements moves;
    private uint team;

    public void SetMaxFOV(float newMaxFOV)
    {
        maxFOV = newMaxFOV;
    }

    private void Start()
    {
        moves = GetComponent<Movements>();

        // Enregistrer le joueur dans le GameManager
        SetTeam(GameManager.Instance.RegisterPlayer(GetComponent<NetworkIdentity>()));
        print("registered player with team " + team);

        if (!isLocalPlayer) return;

        mainCamera = Camera.main;

        // Configurer la position et rotation initiales
        mainCamera.transform.SetParent(null); // Découpler temporairement la caméra pour gérer la rotation correctement
        mainCamera.transform.SetPositionAndRotation(initialCamPos.position, initialCamPos.rotation);
        targetCameraRotation = mainCamera.transform.rotation;
        targetCameraPosition = mainCamera.transform.position;

        mainCamera.fieldOfView = minFOV;
    }

    private void OnCollisionEnter(Collision other)
    {
        // TODO: Play a sound when colliding with something

        var speed = rigidBody.velocity.magnitude;

        if (other.gameObject.TryGetComponent(out Rigidbody rb))
        {
            rb.AddForce(collisionForce * speed * (rb.position - transform.position).normalized, ForceMode.Impulse);
            AudioManager.Instance.PlaySoundEffect(AudioManager.Instance.collisionSounds, speed / 10);
        }
    }

    private void FixedUpdate()
    {
        if (collideWithPlayer)
        {
            rigidBody.AddForce(100 * -repulsiveForce * new Vector3(transform.forward.x, 0, transform.forward.z), ForceMode.Acceleration);
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
        HandleStun();

        if (!isLocalPlayer) return;

        AdjustCameraView();
    }

    private void AdjustCameraView()
    {
        // Ajuster le FOV en fonction de la vitesse
        float ratio = rigidBody.velocity.magnitude / moves.GetMaxSpeed();
        mainCamera.fieldOfView = Mathf.Lerp(minFOV, maxFOV, ratio);

        if (moves.GetHoldingDrift())
        {
            // Si la voiture tourne, garder une rotation fixe mais orientée vers l'avant
            Vector3 driftOffset = 0.75f * moves.GetRotations().y * transform.right; // Décalage selon la direction du drift
            targetCameraRotation = Quaternion.LookRotation(transform.forward + driftOffset, Vector3.up);
        }
        else
        {
            targetCameraRotation = Quaternion.LookRotation(transform.forward, Vector3.up);
        }

        // Appliquer une interpolation fluide à la rotation et à la position de la caméra
        targetCameraPosition = initialCamPos.position;
        mainCamera.transform.SetPositionAndRotation(
            Vector3.Lerp(mainCamera.transform.position, targetCameraPosition, Time.deltaTime * cameraLerpSpeedPosition),
            Quaternion.Lerp(mainCamera.transform.rotation, targetCameraRotation, Time.deltaTime * cameraLerpSpeedRotation));
    }

    private void HandleStun()
    {
        if (isStunned)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0)
            {
                isStunned = false;
                moves.SetMovementActive(true);
                print("************ Stun ended");
            }
        }
    }

    [Server]
    public void Stun(float duration)
    {
        print("stun!");
        isStunned = true;
        stunTimer = duration;
        moves.SetMovementActive(false);
        RpcStunFeedback(duration);
    }

    [ClientRpc]
    private void RpcStunFeedback(float duration)
    {
        onStun.Invoke(duration);
    }

    public uint GetTeam()
    {
        return team;
    }

    public void SetTeam(uint team)
    {
        this.team = team;
    }
}