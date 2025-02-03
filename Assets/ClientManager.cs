using kcp2k;
using Mirror;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TMPro;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
    // private string ip = "127.0.0.1";
    private string ip = "157.159.195.98";
    private string port = "7777";
    public int maxConnectionAttempt = 30;
    // Delay in seconds
    public float delayBetweenConnectionAttempt = 1;

    private float connectionAttempts = 0;

    [SerializeField] private GameObject sessionCodeHolder;
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
    IEnumerator GetServerInfoAndConnect(bool requestRoom)
    {
        yield return null;
        Debug.Log("Connexion au script Python pour récupérer les informations du serveur...");

        using (Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        {
            try
            {
                Debug.Log($"Tentative de connexion à {ip}:{port}...");
                // Connexion au serveur Python
                clientSocket.Connect(new IPEndPoint(IPAddress.Parse(ip), int.Parse(port)));
                Debug.Log("Connecté au serveur Python.");

                // Envoi d'une requête pour récupérer les informations du serveur (par exemple, le port du serveur Mirror)
                string requestMessage = "createRoom";

                if (!requestRoom)
                {
                    requestMessage = "joinRoom " + sessionCodeInput.text;
                }

                byte[] requestBytes = Encoding.UTF8.GetBytes(requestMessage);
                clientSocket.Send(requestBytes);

                // Réception de la réponse du serveur Python
                byte[] buffer = new byte[1024];
                int bytesReceived = clientSocket.Receive(buffer);
                string serverInfo = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
                string[] serverInfoParts = serverInfo.Split(' ');
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

                    yield break;
                }

                // Convertir la réponse en entier (port du serveur Mirror)
                int serverPort = int.Parse(serverInfoParts[0]);

                if (serverInfoParts.Length > 1)
                {
                    GameObject.FindFirstObjectByType<SessionCodeHolder>().CmdSetSessionCode(serverInfoParts[1]);
                }

                // Connecter au serveur Mirror avec le port récupéré
                KcpTransport transport = MyNetworkRoomManager.singleton.GetComponent<KcpTransport>();
                transport.port = (ushort)serverPort;

                // Démarrer la connexion client
                Debug.Log($"Connexion au serveur sur {ip}:{serverPort}...");
                StartCoroutine(TryToStartClient());

            }
            catch (Exception e)
            {
                //Debug.LogError($"Erreur de connexion : {e.Message}");
                connectionStatusText.color = Color.red;
                connectionStatusText.text = "Erreur lors de la création d'une room.";
                connectionStatusLoadingIcon.gameObject.SetActive(false);
                buttonsUI.SetActive(true);
            }
        }
    }

    public void CreateRoom()
    {
        buttonsUI.SetActive(false);
        connectionStatusText.color = Color.black;
        connectionStatusText.text = "Creating a room...";
        connectionStatusLoadingIcon.gameObject.SetActive(true);
        StartCoroutine(GetServerInfoAndConnect(true));
    }

    public void JoinRoom()
    {
        buttonsUI.SetActive(false);
        connectionStatusText.color = Color.black;
        connectionStatusText.text = "Joining a room...";
        connectionStatusLoadingIcon.gameObject.SetActive(true);
        StartCoroutine(GetServerInfoAndConnect(false));
    }

    private IEnumerator TryToStartClient()
    {
        connectionAttempts++;
        Debug.Log("Connection attempt n°" + connectionAttempts);

        MyNetworkRoomManager.singleton.StartClient();
        yield return new WaitForSeconds(1);

        if (connectionAttempts >= maxConnectionAttempt)
        {
            Debug.LogError("Maximum connection attempts reached, abort create room process.");
            yield break;
        }

        StartCoroutine(TryToStartClient());
    }
}
