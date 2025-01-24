using Mirror;
using UnityEngine;

public class Trap : NetworkBehaviour
{
    [SerializeField]
    private Transform groundRayPoint;
    [SerializeField]
    private float groundRayLength = 1.5f;
    [SerializeField]
    private LayerMask raycastTarget;

    public void OnTriggerEnter(Collider collided)
    {
        if (collided.CompareTag("Player"))
        {
            print("TRAP ACTIVATION");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        RaycastHit hit;
        if (Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLength, raycastTarget))
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
