using kcp2k;
using Mirror;
using TMPro;
using UnityEngine;

public class MyNetworkRoomManager : NetworkRoomManager
{
    [Header("OPTIONS")]
    [Tooltip("Delay after ready")]
    [SerializeField] private bool raceStartDelay;

    public void StartSelfHost()
    {
        StartServer();
    }

    public void StartSelfClient()
    {
        networkAddress = "localhost";
        StartClient();
    }

    // Check that whenever the last player disconnects completely, the server shuts down
    public override void OnRoomServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnRoomServerDisconnect(conn);
        Debug.Log($"Player {conn.identity} left the game.");
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
        }
        else
        {
            Debug.Log($"Port already set to {transport.port} - the server is being reused.");

        }

        base.Start();
    }

    // Helper function for getting the command line arguments
    private string GetArg(string name)
    {
        var args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            //Debug.Log(args[i]);
            if (args[i] == name && args.Length > i + 1)
            {
                return args[i + 1];
            }
        }
        return "ERROR";
    }

    //public static new MyNetworkRoomManager singleton => NetworkManager.singleton as MyNetworkRoomManager;

    /// <summary>
    /// This is called on the server when a networked scene finishes loading.
    /// </summary>
    /// <param name="sceneName">Name of the new scene.</param>
    public override void OnRoomServerSceneChanged(string sceneName)
    {
        // TODO: We may start the race from there or from the game manager script

        // spawn the initial batch of Rewards
        //if (sceneName == GameplayScene)
        //    Spawner.InitialSpawn();


        if (sceneName == offlineScene)
        {
            TextMeshProUGUI sessionCodeText = FindObjectOfType<TextMeshProUGUI>();
        }
    }

    /// <summary>
    /// Called just after GamePlayer object is instantiated and just before it replaces RoomPlayer object.
    /// This is the ideal point to pass any data like player name, credentials, tokens, colors, etc.
    /// into the GamePlayer object as it is about to enter the Online scene.
    /// </summary>
    /// <param name="roomPlayer"></param>
    /// <param name="gamePlayer"></param>
    /// <returns>true unless some code in here decides it needs to abort the replacement</returns>
    public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer, GameObject gamePlayer)
    {
        //PlayerScore playerScore = gamePlayer.GetComponent<PlayerScore>();
        //playerScore.index = roomPlayer.GetComponent<NetworkRoomPlayer>().index;
        return true;
    }

    public override void OnRoomStopClient()
    {
        base.OnRoomStopClient();
    }

    public override void OnRoomStopServer()
    {
        base.OnRoomStopServer();
    }

    /*
        This code below is to demonstrate how to do a Start button that only appears for the Host player
        showStartButton is a local bool that's needed because OnRoomServerPlayersReady is only fired when
        all players are ready, but if a player cancels their ready state there's no callback to set it back to false
        Therefore, allPlayersReady is used in combination with showStartButton to show/hide the Start button correctly.
        Setting showStartButton false when the button is pressed hides it in the game scene since NetworkRoomManager
        is set as DontDestroyOnLoad = true.
    */


    public override void OnRoomServerPlayersReady()
    {
        // calling the base method calls ServerChangeScene as soon as all players are in Ready state.
        if (Mirror.Utils.IsHeadless())
        {
            base.OnRoomServerPlayersReady();
        }
        else
        {
            base.OnRoomServerPlayersReady();
        }
    }

    /*public override void OnStopServer()
    {
        roomSlots.Clear();
    }*/

    bool showStartButton = false;

    public override void OnGUI()
    {
        // FUNCTION ONLY FOR DEBUGGING
        base.OnGUI();

        if (allPlayersReady && showStartButton && GUI.Button(new Rect(150, 300, 120, 20), "START GAME !!!!"))
        {
            // set to false to hide it in the game scene
            showStartButton = false;

            ServerChangeScene(GameplayScene);
        }
    }

    public void UpdateDisplay()
    {
        FindObjectOfType<OnlineRoomUI>().UpdateUI();
    }
}
