using UnityEngine;
using Mirror;


public class ItemBox : MonoBehaviour
{
    private float timer = 0.0f;
    private Renderer rend;
    [SerializeField] private bool isBow;
    [SerializeField] private bool isPotion;
    [SerializeField] private bool isTrap;
    private ItemManager itemManager;


    private void OnTriggerEnter(Collider other)
    {
        if ( other.TryGetComponent(out NetworkIdentity id))
        {
            if (id.isLocalPlayer)
            {
                if (other.CompareTag("Player"))
                {
                    itemManager = other.GetComponent<ItemManager>();
                    if (itemManager.GetItemInHand() == ItemType.NOTHING)
                    {
                        print("CollisionWithBox");
                        if (isBow)
                        {
                            print("You got a bow!");
                            itemManager.SetItemInHand(ItemType.BOW);
                        }
                        else if (isPotion)
                        {
                            print("You got a potion!");
                            itemManager.SetItemInHand(ItemType.POTION);
                        }
                        else if (isTrap)
                        {
                            print("You got a trap!");
                            itemManager.SetItemInHand(ItemType.TRAP);
                        }

                    }
                    rend.enabled = false;
                    timer = 0.0f;
                    Debug.Log("ReloadingBox");
                }
            }
        }
    }

    private void Start()
    {
        rend = GetComponent<Renderer>();
    }

    private void Update()
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
