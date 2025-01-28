using kcp2k;
using Mirror;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MyNetworkRoomManager : NetworkRoomManager
{
    public override void Start()
    {
        string portString = GetArg("-port");
        if (portString == "ERROR")
        {
            Debug.LogError("Error when setting port");
        }

        KcpTransport transport = MyNetworkRoomManager.singleton.GetComponent<KcpTransport>();
        ushort port;
        if (ushort.TryParse(portString, out port))
        {
            Debug.Log($"Conversion réussie : {port}");
            transport.port = port;
        }
        else
        {
            Debug.LogError("Échec de la conversion : la chaîne n'est pas un nombre valide ou est hors plage.");
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

    bool showStartButton;

    public override void OnRoomServerPlayersReady()
    {
        // calling the base method calls ServerChangeScene as soon as all players are in Ready state.
        if (Mirror.Utils.IsHeadless())
        {
            base.OnRoomServerPlayersReady();
        }
        else
        {
            // TODO: Launch coroutine that begins the race in 3 seconds like fall guys
            // OR just enable a button for the leader to start the game

            showStartButton = true;
            // SceneManager.LoadScene(GameplayScene); // DO NOT USE THIS FUNCTION TO CHANGE SCENE
        }
    }

    /*public override void OnStopServer()
    {
        roomSlots.Clear();
    }*/

    public override void OnGUI()
    {
        // FUNCTION ONLY FOR DEBUGGING
        base.OnGUI();

        if (allPlayersReady && showStartButton && GUI.Button(new Rect(150, 300, 120, 20), "START GAME !!!!"))
        {
            // set to false to hide it in the game scene
            showStartButton = false;

            ServerChangeScene(GameplayScene); // TODO: FUNCTION TO USE TO CHANGE SCENE
        }
    }

    public void UpdateDisplay()
    {
        FindObjectOfType<OnlineRoomUI>().UpdateUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            UpdateDisplay();
        }
    }
}
