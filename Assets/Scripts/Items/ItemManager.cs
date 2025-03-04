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
    [SerializeField] private Transform arrowSpawnPosition;

    [Header("Trap")]
    [SerializeField] private Trap trapPrefab;
    [SerializeField] private Transform trapSpawnPosition;

    private Movements movementsScript;
    private uint playerTeam;
    private bool playedSound = false;

    [SyncVar(hook = nameof(OnItemInHandChanged))]
    private ItemType itemInHand = ItemType.NOTHING;

    private void Start()
    {
        playerTeam = GetComponent<Player>().GetTeam();
        movementsScript = GetComponent<Movements>();
    }

    [Client]
    private void OnItemInHandChanged(ItemType oldValue, ItemType newValue)
    {
        if (isLocalPlayer)
        {
            playedSound = false;
            OnItemChanged?.Invoke(itemInHand);
        }
    }

    [Server]
    public void SetItemInHand(ItemType item)
    {
        itemInHand = item;
    }

    public ItemType GetItemInHand()
    {
        return itemInHand;
    }

    [Client]
    public void RequestItemUse(InputAction.CallbackContext context)
    {
        if (context.performed && CanUseItem && isLocalPlayer)
        {
            if (itemInHand != ItemType.NOTHING)
            {
                print("Asking server to use item");
                UseItem();
                PlayItemSound();
            }
            else
            {
                print("You have no item!");
            }
        }
    }

    [Client]
    private void PlayItemSound()
    {
        if (playedSound) return;

        switch (itemInHand)
        {
            case ItemType.BOW:
                AudioManager.Instance.PlaySoundEffect(AudioManager.Instance.itemBowSound);
                break;
            case ItemType.FEATHER:
                break;
            case ItemType.POTION:
                AudioManager.Instance.PlaySoundEffect(AudioManager.Instance.itemPotionUseSound);
                break;
            case ItemType.SWORD:
                break;
            case ItemType.TRAP:
                AudioManager.Instance.PlaySoundEffect(AudioManager.Instance.itemTrapUseSound);
                break;
            case ItemType.NOTHING:
                break;
            default:
                break;
        }

        playedSound = true;
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
                    Vector3 spawnPosition = arrowSpawnPosition.position;
                    Arrow newArrow = Instantiate(arrowPrefab, spawnPosition, Quaternion.identity);
                    Vector3 shootDirection = transform.forward;
                    newArrow.SetDirection(shootDirection * arrowSpeed);
                    newArrow.SetOrientation(shootDirection);
                    newArrow.SetTeam(playerTeam);
                    NetworkServer.Spawn(newArrow.gameObject);
                    break;
                case ItemType.FEATHER:
                    print("Yahoo!");
                    break;
                case ItemType.POTION:
                    print("Glou glou!");
                    movementsScript.ApplySpeedBoost(forceSpeedBoost, speedBoostDuration);
                    break;
                case ItemType.SWORD:
                    print("Chling!");
                    break;
                case ItemType.TRAP:
                    Trap trap = Instantiate(trapPrefab, trapSpawnPosition.position, Quaternion.identity);
                    trap.SetTeam(playerTeam);
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
