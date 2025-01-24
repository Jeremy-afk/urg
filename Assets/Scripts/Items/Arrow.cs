using Mirror;
using UnityEngine;

public class Arrow : NetworkBehaviour
{
    [SerializeField] private float speed;

    private Vector3 direction;

    public void SetDirection(Vector3 newDirection)
    { 
        direction = newDirection.normalized;
    }

    public void SetOrientation(Vector3 shootDirection)
    {
        transform.rotation = Quaternion.LookRotation(shootDirection, Vector3.up);
    }


    public void OnTriggerEnter(Collider collided)
    {
        if (collided.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
        else if (collided.CompareTag("Ground"))
        {
            // Bounce in the future?
            Destroy(this.gameObject);
        }
    }

    private void FixedUpdate()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
}
