using Mirror;
using System;
using UnityEngine;

public class MyNetworkRoomPlayer : NetworkRoomPlayer
{
    [SyncVar]
    private int carSelectionIndex = 0;
    [SyncVar]
    private string username = "Untitled Racer";

    #region Singleton
    private MyNetworkRoomManager room;
    private MyNetworkRoomManager Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as MyNetworkRoomManager;
        }
    }
    #endregion

    #region Commands
    [Command]
    public void CmdSetCarSkin(int index)
    {
        carSelectionIndex = index;
    }

    [Command]
    public void CmdSetUsername(string newUsername)
    {
        username = newUsername;
    }
    #endregion

    #region Getters & Setters

    public int GetCarSelectionIndex() => carSelectionIndex;
    public string GetUsername() => username;

    #endregion

    public override void ReadyStateChanged(bool oldReadyState, bool newReadyState)
    {
        base.ReadyStateChanged(oldReadyState, newReadyState);

        FindObjectOfType<OnlineRoomUI>().UpdateUI();
    }

    public override void OnClientEnterRoom()
    {
        base.OnClientEnterRoom();
        FindObjectOfType<OnlineRoomUI>().UpdateUI();
    }

    public override void OnClientExitRoom()
    {
        base.OnClientExitRoom();
        try
        {
            FindObjectOfType<OnlineRoomUI>().UpdateUI();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}
