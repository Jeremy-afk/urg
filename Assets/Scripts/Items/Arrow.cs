using UnityEngine;

public class Arrow : ThrowableItem
{
    [Header("Arrow")]
    [SerializeField] private float stunDuration = 1.5f;
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

    protected override void OnHit(Collider collision)
    {
        if (collision.TryGetComponent(out Player player))
        {
            player.Stun(stunDuration);
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        transform.position += speed * Time.deltaTime * direction;
    }
}
