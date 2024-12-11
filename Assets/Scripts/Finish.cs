using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Collections;

public class Finish : NetworkBehaviour
{
    [SerializeField] private int countdownDelay = 2;
    [SerializeField] private int requiredLaps = 3;
    [SerializeField] private Checkpoint[] checkpoints;

    private Dictionary<NetworkIdentity, int> playerLapCount = new();

    public void RegisterPlayers(NetworkIdentity[] playerIdentities)
    {
        foreach (var playerIdentity in playerIdentities)
        {
            RegisterPlayer(playerIdentity);
        }
    }

    public void RegisterPlayer(NetworkIdentity playerIdentity)
    {
        if (playerIdentity)
        {
            playerLapCount[playerIdentity] = 0;
            ResetPlayerCheckpoints(playerIdentity);
            Debug.Log("Player " + playerIdentity.netId + " registered successfully.");
        }
        else
        {
            Debug.LogError("Attempted to register a null or invalid player.");
        }
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

    private void CheckLapCompletion(NetworkIdentity playerIdentity)
    {
        // Check if all checkpoints have been crossed for this specific player
        if (IsAllCheckpointsCrossed(playerIdentity))
        {
            // Increment lap count for the player
            if (playerLapCount.ContainsKey(playerIdentity)) playerLapCount[playerIdentity]++;
            else
            {
                Debug.LogWarning("Player was not registered. Registering player for the first time.");
                playerLapCount.Add(playerIdentity, 1);
            }

            ResetPlayerCheckpoints(playerIdentity);
            RpcLogLapCompletion(playerIdentity.netId, playerLapCount[playerIdentity]);

            if (playerLapCount[playerIdentity] >= requiredLaps)
            {
                FinishRace(playerIdentity);
            }
        }
        else
        {
            Debug.LogWarning("Checkpoint check failed. Lap not counted.");
        }
    }

    private void FinishRace(NetworkIdentity playerIdentity)
    {
        Debug.Log("Player " + playerIdentity.netId + " has finished the race!");
        Movements playerMovements = playerIdentity.GetComponent<Movements>();
        playerMovements.SetMovementActive(false);

        // Show the ui only on the client that finished the race
        if (playerIdentity.isLocalPlayer)
        {
            // TODO: Play a finish sound effect
            GameManager.Instance.ShowFinishedUi();
        }
    }

    private bool IsAllCheckpointsCrossed(NetworkIdentity playerIdentity)
    {
        foreach (var cp in checkpoints)
        {
            if (!cp) continue;

            // Might be better in performance to later make the checkpoints update a hashtable here instead of checking every checkpoint
            bool passed = cp.HasPlayerCrossed(playerIdentity);

            if (!passed)
            {
                Debug.Log($"Checkpoint {cp.gameObject.name} has not been crossed. Lap not completed.", gameObject);
                return false; // If any checkpoint is not crossed, return false
            }
        }
        return true; // All checkpoints have been crossed by this player
    }

    private void ResetPlayerCheckpoints(NetworkIdentity playerIdentity)
    {
        foreach (var cp in checkpoints)
        {
            if (!cp) continue;

            cp.ResetCheckpointForPlayer(playerIdentity); // Reset each checkpoint
        }
    }

    [ClientRpc]
    private void RpcLogLapCompletion(uint playerId, int laps)
    {
        Debug.Log("Player " + playerId + " completed a lap! Total Laps: " + laps);
    }
}
