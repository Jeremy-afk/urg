using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool isCrossed = false; // State of the checkpoint
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object passing through is the player
        if (other.CompareTag("Player") && !isCrossed)
        {
            isCrossed = true; // Mark this checkpoint as crossed
            Debug.Log("Checkpoint crossed: " + gameObject.name);
        }
    }

    // Reset this checkpoint to "uncrossed" for the next lap
    public void ResetCheckpoint()
    {
        isCrossed = false;
    }

}
