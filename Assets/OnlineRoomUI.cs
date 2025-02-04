using Mirror;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OnlineRoomUI : MonoBehaviour
{
    [SerializeField] private float triesPerSecond = 1.0f;

    [Header("UI Players")]
    [SerializeField] private List<PlayerListItemUI> playerListItemUIList = new List<PlayerListItemUI>();

    [Header("Buttons")]
    [SerializeField] private Image loadingIcon;
    [SerializeField] private Button readyGameButton;
    [SerializeField] private Button leaveGameButton;

    [Space]
    [SerializeField] private TextMeshProUGUI sessionCodeText;

    private float retryTimer;
    private bool needSetup = true;

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
        leaveGameButton.onClick.AddListener(() => {
            Room.StopClient();
        });

        if (TryGetPlayerRoom(out MyNetworkRoomPlayer player))
        {
            SetupReadyButton(player);
            needSetup = false;
        }
        else
        {
            loadingIcon.gameObject.SetActive(true);
            readyGameButton.interactable = false;
            needSetup = true;
        }
    }

    private void Update()
    {
        if (needSetup)
        {
            if (retryTimer >= 1.0f / triesPerSecond)
            {

                if (TryGetPlayerRoom(out MyNetworkRoomPlayer player))
                {
                    SetupReadyButton(player);
                    UpdateUI();
                    needSetup = false;
                }
                else
                {
                    retryTimer = 0.0f;
                }
            }
            retryTimer += Time.deltaTime;
        }
    }

    private void SetupReadyButton(MyNetworkRoomPlayer player)
    {
        readyGameButton.onClick.AddListener(() => {
            if (Room.IsAllPlayersReady())
            {
                return;
            }

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

        UpdateSessionCode();

        loadingIcon.gameObject.SetActive(false);
        readyGameButton.interactable = true;
    }

    private bool TryGetPlayerRoom(out MyNetworkRoomPlayer player)
    {
        NetworkIdentity playerId = NetworkClient.localPlayer;

        if (playerId)
        {
            player = playerId.GetComponent<MyNetworkRoomPlayer>();
            return true;
        }

        player = null;
        return false;
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

    public void UpdateSessionCode()
    {
        sessionCodeText.text = NetworkRoomData.Instance.SessionCode;
    }
}
