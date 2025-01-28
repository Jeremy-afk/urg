using UnityEngine;

public class Trap : ThrowableItem
{
    [Header("Trap")]
    [SerializeField]
    private float stunDuration = 1.5f;

    [Header("Placement")]
    [SerializeField]
    private Transform groundRayPoint;
    [SerializeField]
    private float groundRayLength = 1.5f;
    [SerializeField]
    private LayerMask raycastTarget;

    protected override void OnHit(Collider damageable)
    {
        Debug.Log("Trap triggered");

        if (damageable.TryGetComponent(out Player player))
        {
            AudioManager.Instance.PlaySoundEffect(AudioManager.Instance.itemTrapActivatedSound);
            player.Stun(stunDuration);
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (Physics.Raycast(groundRayPoint.position, -transform.up, out RaycastHit hit, groundRayLength, raycastTarget))
        {
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            Vector3 currentEulerAngles = transform.eulerAngles;
            Vector3 targetEulerAngles = targetRotation.eulerAngles;

            // Only changes rotation on X and Z
            Quaternion adjustedRotation = Quaternion.Euler(targetEulerAngles.x, currentEulerAngles.y, targetEulerAngles.z);

            transform.rotation = adjustedRotation;
        }
    }
}
