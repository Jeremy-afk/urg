using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ItemBox : MonoBehaviour
{
    private float timer = 0.0f;
    Renderer rend;
    private void OnTriggerEnter(Collider other)
    {
        /*if (!isServer)
            return;*/
        if (other.tag == "Player")
        {
            //gameObject.SetActive(false);
            rend.enabled = false;
            timer = 0.0f;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rend = this.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!rend.enabled)
        {
            print("reloading box");
            timer += Time.deltaTime;
            if (timer > 3.0f)
            {
                //gameObject.SetActive(true);
                rend.enabled = true;
            }
        }
    }
}
