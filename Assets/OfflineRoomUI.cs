using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OfflineRoomUI : MonoBehaviour
{
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button joinRoomButton;
    [SerializeField] private ClientManager clientManager;

    void Start()
    {
        createRoomButton.onClick.AddListener(StartServer);
        joinRoomButton.onClick.AddListener(StartClient);
    }

    private void StartServer()
    {
        clientManager.CreateRoom();
    }

    private void StartClient()
    {
        clientManager.JoinRoom();
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
