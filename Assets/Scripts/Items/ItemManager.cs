using Mirror;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum ItemType
{
    NOTHING, //0
    BOW, //1
    FEATHER, //2
    POTION, //3
    SWORD, //4
    TRAP, //5
}

public class ItemManager : NetworkBehaviour
{
    [field: SyncVar]
    public bool CanUseItem { get; set; } = true;

    public static event Action<ItemType> OnItemChanged;

    [Header("Potion")]
    [SerializeField] private float forceSpeedBoost = 1.5f;
    [SerializeField] private float speedBoostDuration = 2.0f;

    [Header("Arrow")]
    [SerializeField] private Arrow arrowPrefab;
    [SerializeField] private float arrowSpeed = 1f;
    [SerializeField] private Vector3 offsetArrow = new(0, 0, 0);

    [Header("Trap")]
    [SerializeField] private Trap trapPrefab;
    [SerializeField] private Vector3 offsetTrap = new(0, 0, 0);

    private Movements movementsScript;

    [SyncVar(hook = nameof(OnItemInHandChanged))]
    private ItemType itemInHand = ItemType.NOTHING;

    private void Start()
    {
        movementsScript = GetComponent<Movements>();
    }

    private void OnItemInHandChanged(ItemType oldValue, ItemType newValue)
    {
        if (isLocalPlayer)
        {
            OnItemChanged?.Invoke(itemInHand);
        }
    }

    public void SetItemInHand(ItemType item)
    {
        print("Setting item in hand");
        itemInHand = item;
    }

    public ItemType GetItemInHand()
    {
        return itemInHand;
    }

    public void RequestItemUse(InputAction.CallbackContext context)
    {
        if (context.performed && CanUseItem && isLocalPlayer)
        {
            if (itemInHand != ItemType.NOTHING)
            {
                print("Asking server to use item");
                UseItem();
            }
            else
            {
                print("You have no item!");
            }
        }
    }

    // This will be called by the server when the player tries to use an item
    [Command]
    public void UseItem()
    {
        if (CanUseItem)
        {
            // creates an instance of the item ans apply its effect
            switch (itemInHand)
            {
                case ItemType.BOW:
                    print("Headshot!");
                    Vector3 spawnPosition = transform.position + offsetArrow;
                    Arrow newArrow = Instantiate(arrowPrefab, transform.position + new Vector3(3.0f, 0, 0.0f), Quaternion.identity);
                    newArrow.SetDirection(transform.forward * arrowSpeed);
                    NetworkServer.Spawn(newArrow.gameObject);
                    break;
                case ItemType.FEATHER:
                    print("Yahoo!");
                    break;
                case ItemType.POTION:
                    print("Glou glou!");
                    movementsScript.BonusSpeedMult = forceSpeedBoost;
                    movementsScript.BonusSpeedMultTime = speedBoostDuration;
                    break;
                case ItemType.SWORD:
                    print("Chling!");
                    break;
                case ItemType.TRAP:
                    Trap trap = Instantiate(trapPrefab, transform.position + new Vector3(-2.0f, -transform.position.y, 0.0f), Quaternion.identity);
                    NetworkServer.Spawn(trap.gameObject);
                    print("Trapped loser!");
                    break;
                case ItemType.NOTHING:
                    print("You have no item!");
                    break;
                default:
                    print("Error : not an item!");
                    break;
            }
            SetItemInHand(ItemType.NOTHING);
        }
    }

}
