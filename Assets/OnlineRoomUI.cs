using Mirror;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OnlineRoomUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private List<PlayerListItemUI> playerListItemUIList = new List<PlayerListItemUI>();
    [SerializeField] private Button readyGameButton;
    [SerializeField] private Button leaveGameButton;


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
        LiveLogger.Log("OnlineRoom start");

        readyGameButton.onClick.AddListener(() => {
            MyNetworkRoomPlayer player = NetworkClient.localPlayer.GetComponent<MyNetworkRoomPlayer>();

            if (player.readyToBegin)
            {
                readyGameButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Ready";
            }
            else
            {
                readyGameButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Not ready";
            }

            player.CmdChangeReadyState(!player.readyToBegin);
            UpdateUI();
        });
        leaveGameButton.onClick.AddListener(() => {
            MyNetworkRoomManager.singleton.StopClient();
        });
    }

    public void UpdateUI()
    {
        for (int i = 0; i < playerListItemUIList.Count; i++)
        {
            playerListItemUIList[i].playerNameText.text = "Waiting For Player...";
            playerListItemUIList[i].playerReadyText.text = string.Empty;
        }

        int j = 0;
        foreach (var player in Room.roomSlots)
        {
            playerListItemUIList[j].playerNameText.text = "Player " + (j + 1);
            playerListItemUIList[j].playerReadyText.text = player.GetComponent<MyNetworkRoomPlayer>().readyToBegin ?
                "<color=green>Ready</color>" :
                "<color=red>Not Ready</color>";

            ++j;
        }
    }

}
