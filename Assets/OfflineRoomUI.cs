using System;
using UnityEngine;
using UnityEngine.UI;

public class OfflineRoomUI : MonoBehaviour
{
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button joinRoomButton;

    void Start()
    {
        createRoomButton.onClick.AddListener(StartServer);
        joinRoomButton.onClick.AddListener(StartClient);
    }
    private void StartServer()
    {
        throw new NotImplementedException();
    }

    private void StartClient()
    {
        // Récupérer l'adresse du serveur
        string address = MyNetworkRoomManager.singleton.networkAddress;

        // Démarrer le client
        MyNetworkRoomManager.singleton.StartClient();
    }

}
