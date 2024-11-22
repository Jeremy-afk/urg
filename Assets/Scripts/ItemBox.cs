using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ItemBox : MonoBehaviour
{
    private float timer = 0.0f;
    private void OnTriggerEnter(Collider other)
    {
        /*if (!isServer)
            return;*/
        if (other.tag == "Player")
        {
            gameObject.SetActive(false);
            timer = 0.0f;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameObject.activeSelf)
        {
            timer += Time.deltaTime;
            if (timer > 5.0f)
            {
                gameObject.SetActive(true);
            }
        }
    }
}
