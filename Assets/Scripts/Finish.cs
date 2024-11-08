using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Finish : NetworkBehaviour
{
    public Checkpoint[] checkpoints; // Array of checkpoints to track
    private Dictionary<NetworkIdentity, bool[]> playerCheckpointStatus = new Dictionary<NetworkIdentity, bool[]>(); // Track checkpoint status per player
    private Dictionary<NetworkIdentity, int> playerLapCount = new Dictionary<NetworkIdentity, int>(); // Track laps per player

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object passing through is the player
        if (other.CompareTag("Player") && other.TryGetComponent(out NetworkIdentity playerIdentity) && playerIdentity.isLocalPlayer)
        {
            // Call the Command to check and update the lap completion for this specific player
            CmdCheckLapCompletion(playerIdentity);
        }
    }

    [Command]
    private void CmdCheckLapCompletion(NetworkIdentity playerIdentity)
    {
        // Ensure this player has initialized their checkpoint data
        InitializePlayerCheckpointStatus(playerIdentity);

        // Check if all checkpoints have been crossed for this specific player
        if (AllCheckpointsCrossed(playerIdentity))
        {
            // Increment lap count for the player
            playerLapCount[playerIdentity]++;
            Debug.Log("Player " + playerIdentity.netId + " completed a lap! Total Laps: " + playerLapCount[playerIdentity]);

            // Reset this player's checkpoints for the next lap
            ResetPlayerCheckpoints(playerIdentity);

            // Optionally, send the updated lap count to clients
            RpcLogLapCompletion(playerIdentity.netId, playerLapCount[playerIdentity]);
        }
    }

    // Check if all checkpoints in the array have been crossed for a specific player
    private bool AllCheckpointsCrossed(NetworkIdentity playerIdentity)
    {
        bool[] checkpointsCrossed = playerCheckpointStatus[playerIdentity];
        foreach (bool isCrossed in checkpointsCrossed)
        {
            if (!isCrossed)
            {
                return false; // If any checkpoint is not crossed, return false
            }
        }
        return true; // All checkpoints have been crossed by this player
    }

    // Reset all checkpoints for the next lap for a specific player
    private void ResetPlayerCheckpoints(NetworkIdentity playerIdentity)
    {
        bool[] checkpointsCrossed = playerCheckpointStatus[playerIdentity];
        for (int i = 0; i < checkpointsCrossed.Length; i++)
        {
            checkpointsCrossed[i] = false; // Reset each checkpoint
        }
    }

    // Public method to initialize player checkpoint status if they are new
    public void InitializePlayerCheckpointStatus(NetworkIdentity playerIdentity)
    {
        if (!playerCheckpointStatus.ContainsKey(playerIdentity))
        {
            playerCheckpointStatus[playerIdentity] = new bool[checkpoints.Length]; // Initialize checkpoint array for this player
            playerLapCount[playerIdentity] = 0; // Initialize lap count for this player
        }
    }

    // ClientRpc to log the lap completion on all clients
    [ClientRpc]
    private void RpcLogLapCompletion(uint playerId, int laps)
    {
        Debug.Log("Player " + playerId + " completed a lap! Total Laps: " + laps);
    }
}
