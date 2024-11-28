using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Finish : NetworkBehaviour
{
    public Checkpoint[] checkpoints; // Array of checkpoints to track
    private Dictionary<NetworkIdentity, int> playerLapCount = new(); // Track laps per player

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
        // Ensure this player has initialized their checkpoint data
        //InitializePlayerCheckpointStatus(playerIdentity); // I think this shouldn't be called here since it resets every checkpoint for the player

        // Check if all checkpoints have been crossed for this specific player
        if (AllCheckpointsCrossed(playerIdentity))
        {
            // Increment lap count for the player
            if (playerLapCount.ContainsKey(playerIdentity))
                playerLapCount[playerIdentity]++;
            else
                playerLapCount.Add(playerIdentity, 1);

            // Debug.Log("Player " + playerIdentity.netId + " completed a lap! Total Laps: " + playerLapCount[playerIdentity]);

            // Reset this player's checkpoints for the next lap
            ResetPlayerCheckpoints(playerIdentity);

            // Optionally, send the updated lap count to clients
            RpcLogLapCompletion(playerIdentity.netId, playerLapCount[playerIdentity]);
        }
    }

    // Check if all checkpoints in the array have been crossed for a specific player
    private bool AllCheckpointsCrossed(NetworkIdentity playerIdentity)
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

    // Reset all checkpoints for the next lap for a specific player
    private void ResetPlayerCheckpoints(NetworkIdentity playerIdentity)
    {
        foreach (var cp in checkpoints)
        {
            if (!cp) continue;

            cp.ResetCheckpointForPlayer(playerIdentity); // Reset each checkpoint
        }
    }

    // Public method to initialize player checkpoint status if they are new
    public void InitializePlayerCheckpointStatus(NetworkIdentity playerIdentity)
    {
        if (playerIdentity)
        {
            playerLapCount[playerIdentity] = 0;
            ResetPlayerCheckpoints(playerIdentity);
        }
    }

    // ClientRpc to log the lap completion on all clients
    [ClientRpc]
    private void RpcLogLapCompletion(uint playerId, int laps)
    {
        Debug.Log("Player " + playerId + " completed a lap! Total Laps: " + laps);
    }
}
