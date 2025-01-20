using UnityEngine;
using Mirror;


public class ItemBox : MonoBehaviour
{
    [Header("Box")]
    [SerializeField] private float reloadTime = 3.0f;

    [Header("Item")]
    [SerializeField] ItemType item;
    private ItemManager itemManager;

    private Renderer rend;
    private float timer = 0.0f;
    private bool isBoxActive = true;

    // The server should be the only one to handle the collision (for now it's not the case)
    private void OnTriggerEnter(Collider other)
    {
        if (!isBoxActive) return;

        if (other.TryGetComponent(out NetworkIdentity id))
        {
            if (other.CompareTag("Player"))
            {
                if (id.isLocalPlayer)
                {
                    itemManager = other.GetComponent<ItemManager>();
                    if (itemManager.GetItemInHand() == ItemType.NOTHING)
                    {
                        switch (item)
                        {
                            case ItemType.BOW:
                                print("You got a bow!");
                                itemManager.SetItemInHand(ItemType.BOW);
                                break;
                            case ItemType.POTION:
                                print("You got a potion!");
                                itemManager.SetItemInHand(ItemType.POTION);
                                break;
                            case ItemType.TRAP:
                                print("You got a trap!");
                                itemManager.SetItemInHand(ItemType.TRAP);
                                break;
                        }
                    }
                    
                }

                // Disable the box
                isBoxActive = false;
                rend.enabled = false;
                timer = 0.0f;
                Debug.Log("ReloadingBox");
            }
        }
    }

    private void Start()
    {
        rend = GetComponent<Renderer>();
    }

    private void Update()
    {
        if (!isBoxActive)
        {
            if (timer > reloadTime)
            {
                rend.enabled = true;
                isBoxActive = true;
                Debug.Log("ItemBox reloaded");
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
    }
}
