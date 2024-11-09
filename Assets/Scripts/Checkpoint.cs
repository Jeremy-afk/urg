using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Checkpoint : NetworkBehaviour
{
    // Dictionary to track each player's "crossed" status for this checkpoint
    private Dictionary<NetworkIdentity, bool> playerCrossedStatus = new();

    private NetworkIdentity mainPlayer; // Just for debug delete afterwards!!!!!!!!

    private void OnTriggerEnter(Collider other)
    {
        // Ensure this code only runs on the server to maintain authoritative state
        if (!isServer) return;

        // Check if the object passing through is the player
        if (other.CompareTag("Player") && other.TryGetComponent(out NetworkIdentity playerIdentity))
        {
            // If the player has not yet crossed this checkpoint, mark it as crossed
            if (!HasPlayerCrossed(playerIdentity))
            {
                mainPlayer = playerIdentity; // Just for debug delete afterwards!!!!!!!!
                playerCrossedStatus[playerIdentity] = true; // Mark this checkpoint as crossed for the player
                Debug.Log("Player " + playerIdentity.netId + " crossed checkpoint: " + gameObject.name);

                // Optionally, notify a lap manager or finish line here (this would be for better performances)
                // For example: finishLine.OnCheckpointCrossed(playerIdentity, this);
            }
        }
    }

    private void Update()
    {
        // Just for debug, delete this Update function afterwards
        if (mainPlayer)
        {
            print($"{gameObject.name} : playerCrossedStatus = {playerCrossedStatus[mainPlayer]}");
        }
    }

    // Check if a specific player has crossed this checkpoint
    public bool HasPlayerCrossed(NetworkIdentity playerIdentity)
    {
        // Return true if the player has crossed this checkpoint, otherwise false
        return playerCrossedStatus.ContainsKey(playerIdentity) && playerCrossedStatus[playerIdentity];
    }

    // Reset this checkpoint's "crossed" status for a specific player
    public void ResetCheckpointForPlayer(NetworkIdentity playerIdentity)
    {
        playerCrossedStatus[playerIdentity] = false;
    }

    // Reset the checkpoint for all players (used at the start of a new race or lap)
    public void ResetCheckpointForAllPlayers()
    {
        foreach (NetworkIdentity playerIdentity in playerCrossedStatus.Keys)
        {
            playerCrossedStatus[playerIdentity] = false;
        }
    }
}
