using System;
using UnityEngine;

public enum ItemType
{
    BOW, //0
    FEATHER, //1
    POTION, //2
    SWORD, //3
    TRAP, //4
    NOTHING //5
}

public class ItemManager : MonoBehaviour
{
    public static ItemType itemInHand = ItemType.NOTHING;
    public static ItemManager Instance;

    public static event Action<ItemType> OnItemChanged;

    public void SetItemInHand(ItemType item)
    {
        itemInHand = item;
        OnItemChanged?.Invoke(item);
    }

    public ItemType GetItemInHand()
    {
        return itemInHand;
    }

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
