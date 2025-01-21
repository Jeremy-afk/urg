using Mirror;
using UnityEngine;

public class Trap : NetworkBehaviour
{
    public void OnTriggerEnter(Collider collided)
    {
        if (collided.CompareTag("Player"))
        {
            print("TRAP ACTIVATION");
            Destroy(gameObject);
        }
    }
}
