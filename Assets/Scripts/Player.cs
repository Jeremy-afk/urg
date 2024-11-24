using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    [SyncVar]
    [SerializeField]
    private int lapCount = 0;

    [SyncVar]
    private Movements moves;

    private void OnTriggerEnter(Collider other)
    {
        if (!isServer)
            return;

        if (other.tag == "RaceStartPoint")
        {
            lapCount++;
        }
        if (other.tag == "ItemBox")
        {
            print("CollisionWithBox");
            moves = GetComponent<Movements>();
            moves.SetItemType(Movements.ItemType.POTION); //Not working yet
        }
    }
}
