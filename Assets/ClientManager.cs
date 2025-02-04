using kcp2k;
using Mirror;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
    private string ip = "157.159.195.98";
    private string port = "7777";
    private int maxConnectionAttempt = 30;
    private float delayBetweenConnectionAttempts = 1f;

    [SerializeField] private TMP_InputField sessionCodeInput;
    [SerializeField] private GameObject buttonsUI;
    [SerializeField] private TextMeshProUGUI connectionStatusText;
    [SerializeField] private LoadingIcon connectionStatusLoadingIcon;

    private MyNetworkRoomManager room;
    private MyNetworkRoomManager Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as MyNetworkRoomManager;
        }
    }

    private void Start()
    {
        connectionStatusLoadingIcon.gameObject.SetActive(false);
        connectionStatusText.text = "";
        MyNetworkRoomManager.singleton.networkAddress = ip;
    }

    // requestRoom == true -> utilisateur a cliqué sur create room
    // requestRoom == false -> utilisateur a cliqué sur join room
    private async Task GetServerInfoAndConnect(bool requestRoom)
    {
        Debug.Log("Connexion au script Python pour récupérer les informations du serveur...");

        try
        {
            using Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //LiveLogger.Log("Socket créée. : " + clientSocket);
            clientSocket.ReceiveTimeout = 5000;
            Debug.Log($"Tentative de connexion à {ip}:{port}...");

            await Task.Run(() => clientSocket.Connect(new IPEndPoint(IPAddress.Parse(ip), int.Parse(port))));
            Debug.Log("Connecté au serveur Python.");

            // Envoi d'une requête pour récupérer les informations du serveur
            string requestMessage = requestRoom ? "createRoom" : $"joinRoom {sessionCodeInput.text}";
            byte[] requestBytes = Encoding.UTF8.GetBytes(requestMessage);
            clientSocket.Send(requestBytes);

            // Réception de la réponse du serveur Python
            byte[] buffer = new byte[1024];
            int bytesReceived = await Task.Run(() => clientSocket.Receive(buffer));
            string serverInfo = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
            string[] serverInfoParts = serverInfo.Split(' ');

            //LiveLogger.Log("Server info  : " + serverInfo);

            Debug.Log($"Informations du serveur reçues : {serverInfo}");

            if (serverInfoParts[0] == "erreur")
            {
                if (serverInfoParts[1] == "roomNotFound")
                {
                    connectionStatusText.color = Color.red;
                    connectionStatusText.text = "Erreur: code de session associé à aucune room existante.";
                    connectionStatusLoadingIcon.gameObject.SetActive(false);
                    buttonsUI.SetActive(true);
                }
                return;
            }

            // Convertir la réponse en entier (port du serveur Mirror)
            int serverPort = int.Parse(serverInfoParts[0]);

            // Configurer et connecter au serveur Mirror
            KcpTransport transport = MyNetworkRoomManager.singleton.GetComponent<KcpTransport>();
            transport.port = (ushort)serverPort;

            Debug.Log($"Connexion au serveur sur {ip}:{serverPort}...");
            StartCoroutine(TryToStartClient());
        }
        catch (Exception e)
        {
            Debug.LogError($"Erreur de connexion : {e.Message}");
            connectionStatusText.color = Color.red;
            connectionStatusText.text = "Erreur lors de la création d'une room.";
            connectionStatusLoadingIcon.gameObject.SetActive(false);
            buttonsUI.SetActive(true);
        }
    }

    public void CreateRoom()
    {
        buttonsUI.SetActive(false);
        connectionStatusText.color = Color.black;
        connectionStatusText.text = "Creating a room...";
        connectionStatusLoadingIcon.gameObject.SetActive(true);
        GetServerInfoAndConnect(true);
    }

    public void JoinRoom()
    {
        buttonsUI.SetActive(false);
        connectionStatusText.color = Color.black;
        connectionStatusText.text = "Joining a room...";
        connectionStatusLoadingIcon.gameObject.SetActive(true);
        GetServerInfoAndConnect(false);
    }

    private IEnumerator TryToStartClient()
    {
        int connectionAttempts = 0;
        int maxConnectionAttempts = maxConnectionAttempt;

        Debug.Log("Connection attempt n°" + connectionAttempts);

        while (connectionAttempts < maxConnectionAttempts)
        {
            MyNetworkRoomManager.singleton.StartClient();
            yield return new WaitForSeconds(delayBetweenConnectionAttempts);
            connectionAttempts++;
        }

        Debug.LogError("Maximum connection attempts reached.");
    }
}
