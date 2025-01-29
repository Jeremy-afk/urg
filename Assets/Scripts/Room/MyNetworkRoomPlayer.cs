using Mirror;
using Mirror.Examples.AdditiveScenes;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class MyNetworkRoomPlayer : NetworkRoomPlayer
{
    [SyncVar]
    private int carSelectionIndex = 0;

    private MyNetworkRoomManager room;

    private MyNetworkRoomManager Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as MyNetworkRoomManager;
        }
    }

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
