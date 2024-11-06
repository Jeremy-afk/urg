using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finish : MonoBehaviour
{
    public Checkpoint[] checkpoints; // Array of checkpoints to track
    private int lapCount = 0;        // Number of laps completed

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object passing through is the player
        if (other.CompareTag("Player"))
        {
            // Check if all checkpoints have been crossed
            if (AllCheckpointsCrossed())
            {
                lapCount++; // Increment the lap count
                Debug.Log("Lap Completed! Total Laps: " + lapCount);

                // Reset all checkpoints for the next lap
                ResetAllCheckpoints();
            }
        }
    }

    // Check if all checkpoints in the array have been crossed
    private bool AllCheckpointsCrossed()
    {
        foreach (Checkpoint checkpoint in checkpoints)
        {
            if (!checkpoint.isCrossed)
            {
                return false; // If any checkpoint is not crossed, return false
            }
        }
        return true; // All checkpoints have been crossed
    }

    // Reset all checkpoints for the next lap
    private void ResetAllCheckpoints()
    {
        foreach (Checkpoint checkpoint in checkpoints)
        {
            checkpoint.ResetCheckpoint();
        }
    }
}
