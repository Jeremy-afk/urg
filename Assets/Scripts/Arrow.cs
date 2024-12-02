using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float speed;

    private Vector2 direction = new Vector2(1, 0);

    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection;
    }

    public void OnTriggerEnter(Collider collided)
    {
        if (collided.tag == "Player")
        {
            Destroy(gameObject);
        }
        else if (collided.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(direction.normalized * speed * Time.deltaTime);
    }
}
