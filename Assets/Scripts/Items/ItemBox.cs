using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ItemBox : MonoBehaviour
{
    private float timer = 0.0f;
    private Renderer rend;
    [SerializeField] private bool isBow;
    [SerializeField] private bool isPotion;
    [SerializeField] private bool isTrap;

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
            if (ItemManager.Instance.GetItemInHand() == ItemType.NOTHING)
            {
                print("CollisionWithBox");
                if (isBow)
                {
                    print("You got a bow!");
                    ItemManager.Instance.SetItemInHand(ItemType.BOW);
                }
                else if (isPotion) 
                {
                    print("You got a potion!");
                    ItemManager.Instance.SetItemInHand(ItemType.POTION);
                }
                else if (isTrap)
                {
                    print("You got a trap!");
                    ItemManager.Instance.SetItemInHand(ItemType.TRAP);
                }
                    
            }
            rend.enabled = false;
            timer = 0.0f;
            Debug.Log("ReloadingBox");
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
            timer += Time.deltaTime;
            if (timer > 3.0f)
            {
                rend.enabled = true;
                Debug.Log("ItemBox reloaded");
            }
        }
    }
}
