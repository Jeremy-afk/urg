using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    [SyncVar]
    [SerializeField]
    private int lapCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (!isServer)
            return;

        if (other.tag == "RaceStartPoint")
        {
            lapCount++;
        }
    }
}
