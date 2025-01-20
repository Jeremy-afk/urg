using System;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public enum ItemType
{
    BOW, //0
    FEATHER, //1
    POTION, //2
    SWORD, //3
    TRAP, //4
    NOTHING //5
}

public class ItemManager : NetworkBehaviour
{
    [field: SyncVar]
    public bool CanUseItem { get; set; } = true;

    public static event Action<ItemType> OnItemChanged;

    [Header("Potion")]
    [SerializeField] private float forceSpeedBoost = 1.5f;
    [SerializeField] private float speedBoostDuration = 0.5f;

    [Header("Arrow")]
    [SerializeField] private Arrow arrowPrefab;
    [SerializeField] private Vector3 offsetArrow = new(0, 0, 0);

    [Header("Trap")]
    [SerializeField] private Trap trapPrefab;
    [SerializeField] private Vector3 offsetTrap = new(0, 0, 0);

    private Movements movementsScript;
    private ItemType itemInHand = ItemType.NOTHING;

    private void Start()
    {
        movementsScript = GetComponent<Movements>();
    }

    public void SetItemInHand(ItemType item)
    {
        itemInHand = item;
        OnItemChanged?.Invoke(item);
    }

    public ItemType GetItemInHand()
    {
        return itemInHand;
    }

    // This will be called on the server when the player tries to use an item
    public void Item(InputAction.CallbackContext context)
    {
        if (context.performed && CanUseItem)
        {
            // creates an instance of the item ans apply its effect
            switch (itemInHand)
            {
                case ItemType.BOW:
                    print("Headshot!");
                    Vector3 spawnPosition = transform.position + offsetArrow;
                    Arrow newArrow = Instantiate(arrowPrefab, transform.position + new Vector3(3.0f, 0, 0.0f), Quaternion.identity);
                    newArrow.SetDirection(transform.forward);
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
                    Instantiate(trapPrefab, transform.position + new Vector3(-2.0f, -transform.position.y, 0.0f), Quaternion.identity);
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
