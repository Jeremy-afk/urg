using Mirror;
using UnityEngine;


public class ItemBox : NetworkBehaviour
{
    [Header("Box")]
    [SerializeField] private float reloadTime = 3.0f;

    [Header("Item")]
    [SerializeField] ItemType item;
    private ItemManager itemManager;

    public Renderer rend;
    private float timer = 0.0f;
    [SyncVar(hook = nameof(OnBoxActiveChanged))]
    private bool isBoxActive = true;


    // The server should be the only one to handle the collision (for now it's not the case)
    private void OnTriggerEnter(Collider other)
    {
        if (!isServer || !isBoxActive) return;

        if (other.TryGetComponent(out NetworkIdentity id))
        {
            if (other.CompareTag("Player"))
            {
                itemManager = other.GetComponent<ItemManager>();
                if (itemManager.GetItemInHand() == ItemType.NOTHING)
                {
                    print($"You got a {item}");
                    // Update the item manager on the server and the client
                    itemManager.SetItemInHand(item);
                    print("Set the item in the item manager of the client " + id);
                }

                // Disable the box
                EnterBoxCooldown();
            }
        }
    }

    [Server]
    private void EnterBoxCooldown()
    {
        isBoxActive = false;
        timer = 0.0f;
    }

    [Client]
    private void OnBoxActiveChanged(bool oldValue, bool newValue)
    {
        print("box active changed to " + newValue);
        rend.enabled = newValue;
        timer = 0.0f;
    }

    private void Awake()
    {
        //rend = GetComponent<Renderer>();
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
