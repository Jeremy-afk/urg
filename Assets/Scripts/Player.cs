using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    /*[SyncVar]
    [SerializeField] 
    private int lapCount = 0;
    */

    [SerializeField] private Transform initialCamPos;

    private void Start()
    {
        if (isLocalPlayer)
        {
            GameObject mainCamera = Camera.main.gameObject;
            mainCamera.transform.SetParent(gameObject.transform);
            mainCamera.transform.localPosition = initialCamPos.localPosition;
            mainCamera.transform.rotation = initialCamPos.rotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isServer)
            return;

        /*if (other.tag == "RaceStartPoint")
        {
            lapCount++;
        }
        }*/
    }
}
