using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ItemBox : MonoBehaviour
{
    private float timer = 0.0f;
    private Renderer rend;
    private Movements moves;

    public enum ItemType
    {
        BOW, //0
        FEATHER, //1
        POTION, //2
        SWORD, //3
        TRAP, //4
        NOTHING //5
    }

    private void OnTriggerEnter(Collider other)
    {
        /*if (!isServer)
            return;*/
        if (other.tag == "Player")
        {
            moves = GetComponent<Movements>();//Not working yet
            if (moves.GetItemType() == ItemType.NOTHING)
            {
                print("CollisionWithBox");
                moves.SetItemType(ItemType.POTION);
            }
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
                rend.enabled = true;
            }
        }
    }
}
