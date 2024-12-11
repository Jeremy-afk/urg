using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    [SerializeField] private Transform initialCamPos;

    private void Start()
    {
        if (isLocalPlayer)
        {
            GameObject mainCamera = Camera.main.gameObject;
            mainCamera.transform.SetParent(gameObject.transform);
            mainCamera.transform.localPosition = initialCamPos.localPosition;
            mainCamera.transform.rotation = initialCamPos.rotation;

            // Register to the game manager
            GameManager.Instance.RegisterPlayer(GetComponent<NetworkIdentity>());
        }
    }
}
