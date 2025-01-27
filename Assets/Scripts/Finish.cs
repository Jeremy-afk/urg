using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Finish : NetworkBehaviour
{
    [SerializeField] private int countdownDelay = 2;
    [SerializeField] private int requiredLaps = 3;
    [SerializeField] private Checkpoint[] checkpoints;

    [Header("Client Events")]
    [SerializeField] private UnityEvent<int> onLapCompleted; // int is the number of laps completed
    [SerializeField] private UnityEvent<int> onNewPlacement; // int is the new position
    [SerializeField] private UnityEvent<int> onRaceFinished;

    private Dictionary<NetworkIdentity, int> playerLapCount = new();

    private Player localPlayer;
    private int localPlayerPosition = 0;
    private int finishedPlayerCount = 0;
    private int teamCount = 0;
    private bool inRace = true;

    private void Start()
    {
        if (isLocalPlayer) localPlayer = GetComponent<Player>();
    }

    /// <summary>
    /// Register a list of players as a team
    /// </summary>
    /// <param name="playerIdentities"></param>
    /// <returns>The team number</returns>
    public uint RegisterPlayersAsTeam(NetworkIdentity[] playerIdentities)
    {
        foreach (var playerIdentity in playerIdentities)
        {
            RegisterPlayer(playerIdentity, false);
        }
        teamCount++;

        return (uint)teamCount;
    }

    /// <summary>
    /// Register a single player and attribute a team number to it
    /// </summary>
    /// <param name="playerIdentity"></param>
    /// <param name="countAsTeam"></param>
    /// <returns>The team number</returns>
    public uint RegisterPlayer(NetworkIdentity playerIdentity, bool countAsTeam = true)
    {
        if (playerIdentity)
        {
            playerLapCount[playerIdentity] = 0;
            ResetPlayerCheckpoints(playerIdentity);
            Debug.Log("Player " + playerIdentity.netId + " registered successfully.");

            if (countAsTeam)
            {
                teamCount++;
            }

            return (uint)teamCount;
        }

        Debug.LogError("Attempted to register a null or invalid player.");

        return 0;
    }

    public void StartCountdown()
    {
        StartCoroutine(StartRaceAnimation());
    }

    private IEnumerator StartRaceAnimation()
    {
        Debug.Log("Waiting for countdown");

        yield return new WaitForSeconds(countdownDelay);

        Debug.Log("Countdown started");
        // TODO: Make a countdown animation (use a separate script for this)
        Debug.Log("Countdown finished");
        StartRace();
    }

    private void StartRace()
    {
        foreach (var playerIdentity in playerLapCount.Keys)
        {
            Movements playerMovements = playerIdentity.GetComponent<Movements>();
            playerMovements.SetMovementActive(true);
        }

        Debug.Log("Race started, moving enabled!");
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only the server will check for the completion of laps
        if (!isServer) return;

        if (other.CompareTag("Player") && other.TryGetComponent(out NetworkIdentity playerIdentity))
        {
            CheckLapCompletion(playerIdentity);
        }
    }

    #region Race Events

    [Server]
    private void OnPlayerFinishedRace(NetworkIdentity playerIdentity)
    {
        Debug.Log("Player " + playerIdentity.netId + " has finished the race!");

        Movements playerMovements = playerIdentity.GetComponent<Movements>();
        playerMovements.SetMovementActive(false);

        finishedPlayerCount++;
        RpcPlayerFinishedRace(playerIdentity.netId, finishedPlayerCount);
    }

    [ClientRpc]
    private void RpcLapCompletion(uint playerIdentity, int laps)
    {
        // If we're the player that completed the lap, update the UI
        if (playerIdentity == localPlayer.netId)
        {
            onLapCompleted.Invoke(laps);
        }
    }

    [ClientRpc]
    private void RpcPlayerFinishedRace(uint playerIdentity, int confirmedPlacement)
    {
        localPlayerPosition = confirmedPlacement;
        inRace = false;

        Debug.Log("Player " + playerIdentity + " finished the race!");

        // Take care of it if we're the player that finished the race
        if (playerIdentity == localPlayer.netId)
        {
            Movements playerMovements = localPlayer.GetComponent<Movements>();
            playerMovements.SetMovementActive(false);

            GameManager.Instance.ShowFinishedUi();
            onRaceFinished.Invoke(localPlayerPosition);
        }
    }

    #endregion

    #region Lap & checkpoints Management

    [Server]
    private void CheckLapCompletion(NetworkIdentity playerIdentity)
    {
        // Check if all checkpoints have been crossed for this specific player
        if (IsAllCheckpointsCrossed(playerIdentity))
        {
            // Increment lap count for the player
            if (playerLapCount.ContainsKey(playerIdentity)) playerLapCount[playerIdentity]++;
            else
            {
                Debug.LogWarning("Warning: Player was not registered. Registering player for the first time.");
                playerLapCount.Add(playerIdentity, 1);
            }

            ResetPlayerCheckpoints(playerIdentity);

            if (playerLapCount[playerIdentity] >= requiredLaps)
            {
                OnPlayerFinishedRace(playerIdentity);
            }
            else
            {
                Debug.Log("Player " + playerIdentity.netId + " completed lap " + playerLapCount[playerIdentity] + " out of " + requiredLaps);
                RpcLapCompletion(playerIdentity.netId, playerLapCount[playerIdentity]);
            }
        }
        else
        {
            Debug.LogWarning("Checkpoint check failed. Lap not counted.");
        }
    }

    [Server]
    private bool IsAllCheckpointsCrossed(NetworkIdentity playerIdentity)
    {
        foreach (var cp in checkpoints)
        {
            if (!cp)
            {
                Debug.LogWarning("Warning: Null checkpoint. Verification skipped.");
                continue;
            }

            bool passed = cp.HasPlayerCrossed(playerIdentity);
            print($"{(passed ? "Passed" : "Failed")} verification for checkpoint \"{cp.gameObject.name}\"");

            if (!passed)
            {
                Debug.Log($"Not all checkpoint crossed. Lap invalidated.", gameObject);
                return false;
            }
        }
        return true;
    }

    private void ResetPlayerCheckpoints(NetworkIdentity playerIdentity)
    {
        foreach (var cp in checkpoints)
        {
            if (!cp) continue;

            cp.ResetCheckpointForPlayer(playerIdentity); // Reset each checkpoint
        }
    }

    #endregion

    #region Getters
    public int GetPlayerCompletedLapCount(NetworkIdentity playerIdentity)
    {
        return playerLapCount[playerIdentity];
    }

    public int GetRequiredLaps()
    {
        return requiredLaps;
    }

    public int GetCountdownDelay()
    {
        return countdownDelay;
    }
    #endregion

    // Preview of the current placementv (it may not be 100% accurate since this is only computed client side)
    [Client]
    public int RecomputeRunningPosition()
    {
        if (!inRace)
        {
            return localPlayerPosition;
        }

        int newPosition = 0;

        // TODO: Implement a running position system

        localPlayerPosition = newPosition;
        return newPosition;
    }
}
