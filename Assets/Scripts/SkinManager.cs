using UnityEngine;

class SkinManager : MonoBehaviour
{
    [SerializeField] private int defaultSkin;
    [SerializeField] private GameObject skinsContainer;

    private GameObject[] skins;

    public void SetSkin(int index)
    {
        if (index >= skins.Length)
        {
            Debug.LogError($"Skin {index} is not a skin.");
        }

        for (int i = 0; i < skins.Length; i++)
        {
            skins[i].SetActive(i == index);
        }
    }

    private void Awake()
    {
        RegisterSkins();
        SetSkin(defaultSkin);
    }

    private void RegisterSkins()
    {
        skins = new GameObject[skinsContainer.transform.childCount];
        for (int i = 0; i < skinsContainer.transform.childCount; i++)
        {
            skins[i] = skinsContainer.transform.GetChild(i).gameObject;
        }
    }

}