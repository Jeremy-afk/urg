using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{

    public void OnTriggerEnter(Collider collided)
    {
        if (collided.tag == "Player")
        {
            Destroy(gameObject);
        }
    }

        // Start is called before the first frame update
        void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
