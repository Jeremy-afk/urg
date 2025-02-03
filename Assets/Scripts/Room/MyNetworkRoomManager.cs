using kcp2k;
using Mirror;
using System.Linq;
using TMPro;
using UnityEngine;

public class MyNetworkRoomManager : NetworkRoomManager
{
    [Header("OPTIONS")]
    [Tooltip("Delay after ready")]
    [SerializeField] private int raceStartDelay;
    [SerializeField] private string sessionCodeTextTag = "SessionCodeText";
    private NetworkEvent networkEvent;

    public string sessionCode;

    #region Debugging
    public void StartSelfHost()
    {
        StartServer();
    }

    public void StartSelfClient()
    {
        networkAddress = "localhost";
        StartClient();
    }
    #endregion

    // Helper function for getting the command line arguments
    private string GetArg(string name)
    {
        var args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == name && args.Length > i + 1)
            {
                return args[i + 1];
            }
        }
        return "ERROR";
    }

    public override void Start()
    {
        Debug.Log("Starting server...");

        KcpTransport transport = MyNetworkRoomManager.singleton.GetComponent<KcpTransport>();
        if (transport.port == 7777)
        {
            // 7777 is the default port, that means it's the first time the server is launched
            // We need to set the port to the one given in the command line
            string portString = GetArg("-port");
            if (portString == "ERROR")
            {
                Debug.LogError("Error when setting port");
            }

            if (ushort.TryParse(portString, out ushort port))
            {
                Debug.Log($"Conversion réussie : {port}");
                transport.port = port;
            }
            else
            {
                Debug.LogError("Échec de la conversion : la chaîne n'est pas un nombre valide ou est hors plage.");
            }
            sessionCode = GetArg("-sessionCode");
            LiveLogger.Log($"Server started on session code {sessionCode}");
        }
        else
        {
            Debug.Log($"Port already set to {transport.port} - the server is being reused.");
        }

        base.Start();
    }

    // Check that whenever the last player disconnects completely, the server shuts down
    public override void OnRoomServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnRoomServerDisconnect(conn);
        //LiveLogger.Log($"Player {conn.identity} left the game.");
        if (numPlayers == 0)
        {
            Debug.Log("All players disconnected. Shutting down the server...");
            StopServer();
            Application.Quit();
        }
        else
        {
            Debug.Log($"{numPlayers} players remaining");
        }
    }

    public override void OnRoomServerPlayersReady()
    {
        Countdown countdown = FindObjectOfType<Countdown>();

        if (countdown == null)
        {
            Debug.LogError("Countdown not found !!");
            return;
        }

        networkEvent = FindObjectOfType<NetworkEvent>();
        networkEvent.RpcStartReadyCountdown(raceStartDelay);

        countdown.SetDuration(raceStartDelay);
        countdown.StartCountdown(() => {
            ServerChangeScene(GameplayScene);
        });
    }

    public override void OnRoomClientEnter()
    {
        base.OnRoomClientEnter();
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        FindObjectOfType<OnlineRoomUI>().UpdateUI();
        GameObject.FindGameObjectWithTag(sessionCodeTextTag).GetComponent<TextMeshProUGUI>().text = sessionCode;
    }

    public bool IsAllPlayersReady()
    {
        return roomSlots.All(player => player.readyToBegin);
    }
}



