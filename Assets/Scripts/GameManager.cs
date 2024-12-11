using Mirror;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
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

    private void Start()
    {
        finishLine.StartCountdown();
    }

    [ClientRpc]
    public void ShowFinishedUi(NetworkIdentity player)
    {
        if (player.isLocalPlayer) finishedUi.SetActive(true);
    }

    public void RegisterPlayer(NetworkIdentity player)
    {
        finishLine.RegisterPlayer(player);
    }
}
