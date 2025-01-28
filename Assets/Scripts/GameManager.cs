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
}
