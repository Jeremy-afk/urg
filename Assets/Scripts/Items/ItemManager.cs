using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemBox.ItemType itemInHand = ItemBox.ItemType.NOTHING;
    public static ItemManager Instance;

    public void SetItemInHand(ItemBox.ItemType item)
    {
        itemInHand = item;
    }

    public ItemBox.ItemType GetItemInHand()
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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
