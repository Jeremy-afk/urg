using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemHolder : MonoBehaviour
{
    [SerializeField] private Image itemHolderImage;
    [Space]
    [SerializeField] private Sprite emptyHolder;

    [Header("Item Sprites")]
    [SerializeField] private Sprite bowSprite;
    [SerializeField] private Sprite potionSprite;
    [SerializeField] private Sprite featherSprite;
    [SerializeField] private Sprite swordSprite;
    [SerializeField] private Sprite trapSprite;

    private Dictionary<ItemType, Sprite> itemSprites = new();

    private void Start()
    {
        itemSprites.Add(ItemType.NOTHING, emptyHolder);

        itemSprites.Add(ItemType.BOW, bowSprite);
        itemSprites.Add(ItemType.FEATHER, featherSprite);
        itemSprites.Add(ItemType.POTION, potionSprite);
        itemSprites.Add(ItemType.SWORD, swordSprite);
        itemSprites.Add(ItemType.TRAP, trapSprite);

        // Subscribe to the event of item change
    }

    private void OnEnable()
    {
        ItemManager.OnItemChanged += UpdateItem;
    }

    private void OnDisable()
    {
        ItemManager.OnItemChanged -= UpdateItem;
    }

    public void UpdateItem(ItemType currentItem)
    {
        if (itemSprites.TryGetValue(currentItem, out Sprite sprite))
        {
            itemHolderImage.sprite = sprite;
        }
        else
        {
            itemHolderImage.sprite = emptyHolder; // Use default sprite if no match is found
        }
    }
}
