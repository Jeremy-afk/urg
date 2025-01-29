using Mirror;
using System.Collections;
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
    [SerializeField] private Button carSelectionLeftButton;
    [SerializeField] private Button carSelectionRightButton;
    [SerializeField] private TextMeshProUGUI carSelectedText;
    [SerializeField] private List<GameObject> carPrefabs;

    private int carPrefabChosenIndex = 0;


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
        Room.playerPrefab = carPrefabs[carPrefabChosenIndex];
        carSelectedText.text = carPrefabs[carPrefabChosenIndex].name;

        readyGameButton.onClick.AddListener(() =>
        {
            MyNetworkRoomPlayer player = NetworkClient.localPlayer.GetComponent<MyNetworkRoomPlayer>();
            player.CmdChangeReadyState(!player.readyToBegin);
            UpdateUI();
        });
        leaveGameButton.onClick.AddListener(() =>
        {
            MyNetworkRoomManager.singleton.StopClient();
        });
        carSelectionLeftButton.onClick.AddListener(() =>
        {
            --carPrefabChosenIndex;
            if (carPrefabChosenIndex < 0)
            {
                carPrefabChosenIndex = carPrefabs.Count - 1;
            }

            carSelectedText.text = carPrefabs[carPrefabChosenIndex].name;

            //NetworkClient.localPlayer.GetComponent<MyNetworkRoomPlayer>().SetCarSelectionIndex(carPrefabChosenIndex);
        });
        carSelectionRightButton.onClick.AddListener(() =>
        {
            ++carPrefabChosenIndex;
            if (carPrefabChosenIndex >= carPrefabs.Count)
            {
                carPrefabChosenIndex = 0;
            }

            carSelectedText.text = carPrefabs[carPrefabChosenIndex].name;
            //NetworkClient.localPlayer.GetComponent<MyNetworkRoomPlayer>().SetCarSelectionIndex(carPrefabChosenIndex);
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
