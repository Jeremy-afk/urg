using Mirror;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [Scene]
    [SerializeField] private string roomOnlineScene = "RoomOnline";
    [SerializeField] private Finish finishLine;
    [SerializeField] private GameObject finishedUi;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("Multiple GameManager instances detected in the scene. Only one GameManager is allowed.");
            Destroy(gameObject);
        }
        Instance = this;
    }

    public void ShowFinishedUi()
    {
        finishedUi.SetActive(true);
    }

    /// <summary>
    /// Register a player for the race
    /// </summary>
    /// <param name="player"></param>
    /// <returns>The team number</returns>
    public uint RegisterPlayer(NetworkIdentity player)
    {
        return finishLine.RegisterPlayer(player);
    }

    [Client]
    public void ReturnToLobby(bool isConfirmed)
    {
        if (!isConfirmed)
        {
            Popup.Instance.MakePopup(
                "Return to lobby",
                "Are you sure you want to return to the lobby?",
                "No, stay", "Yes, back to lobby",
                redActionListener: () => ReturnToLobby(true));
        }
        else
        {
            // Leave the server and return to lobby
            MyNetworkRoomManager.singleton.StopClient();
        }
    }

    [ServerCallback]
    public void ReturnToRoom()
    {
        MyNetworkRoomManager.singleton.ServerChangeScene("RoomOnline");
    }
}
