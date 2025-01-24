using kcp2k;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ClientManager : MonoBehaviour
{
    public string ip = "127.0.0.1";
    public string port = "7777";
    public int maxConnectionAttempt = 30;
    // Delay in seconds
    public float delayBetweenConnectionAttempt = 1;

    private float connectionAttempts = 0;

    [SerializeField] private GameObject sessionCodeHolder;
    [SerializeField] private TMP_InputField sessionCodeInput;

    // requestRoom == true -> utilisateur a cliqué sur create room
    // requestRoom == false -> utilisateur a cliqué sur join room
    IEnumerator GetServerInfoAndConnect(bool requestRoom)
    {
        Debug.Log("Connexion au script Python pour récupérer les informations du serveur...");

        using (Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        {
            try
            {
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

                // Convertir la réponse en entier (port du serveur Mirror)
                int serverPort = int.Parse(serverInfoParts[0]);

                if (serverInfoParts.Length > 1)
                {
                    sessionCodeHolder.GetComponent<SessionCodeHolder>().sessionCode = serverInfoParts[1];
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
                Debug.LogError($"Erreur de connexion : {e.Message}");
            }
        }

        return null;
    }

    public void CreateRoom()
    {
        StartCoroutine(GetServerInfoAndConnect(true));
    }

    public void JoinRoom()
    {
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
