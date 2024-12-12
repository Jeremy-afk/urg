using UnityEngine;


public class ItemBox : MonoBehaviour
{
    private float timer = 0.0f;
    private Renderer rend;
    [SerializeField] private bool isBow;
    [SerializeField] private bool isPotion;
    [SerializeField] private bool isTrap;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
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
