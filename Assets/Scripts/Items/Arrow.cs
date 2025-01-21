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

    public void OnTriggerEnter(Collider collided)
    {
        if (collided.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
        else if (collided.CompareTag("Wall"))
        {
            // Bounce in the future?
            Destroy(this.gameObject);
        }
    }

    private void FixedUpdate()
    {
        transform.Translate(speed * Time.deltaTime * direction);
    }
}
