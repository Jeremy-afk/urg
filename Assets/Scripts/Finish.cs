using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Finish : NetworkBehaviour
{
    [SerializeField] private int countdownPredelay = 2;
    [SerializeField] private int countdownDelay = 3;
    [SerializeField] private int waitForReturnToRoom = 20;
    [SerializeField] private int requiredLaps = 3;
    [SerializeField] private Checkpoint[] checkpoints;

    [Header("Client Events")]
    [SerializeField] private UnityEvent<int> onCountdownStart; // Countdown time in seconds
    [SerializeField] private UnityEvent onRaceStart;
    [SerializeField] private UnityEvent<int> onLapCompleted; // int is the number of laps completed
    [SerializeField] private UnityEvent<int> onPlayerFinishedRace;
    [SerializeField] private UnityEvent onRaceFinished;
    [SerializeField] private UnityEvent<ContestantData[]> onPlacementPublished;
    [SerializeField] private UnityEvent<int> onWaitReturnRoomBegin;

    private Dictionary<NetworkIdentity, int> playerLapCount = new();

    private NetworkIdentity[] placements; // Players in order of finish, only communicated to clients at the end

    private Player localPlayer;
    private int localPlayerPosition = 0;
    private int playerExpectedCount = 0;
    private int playerRegisteredCount = 0;
    private int finishedPlayerCount = 0;
    private int teamCount = 0;
    private bool registrationsOpened = true;
    private bool inRace = true;

    private void Start()
    {
        AudioManager.Instance.PlaySoundEffect(AudioManager.Instance.startSound, 0.25f);

        if (isServer)
        {
            NetworkManager nm = FindObjectOfType<NetworkManager>();
            playerExpectedCount = nm.numPlayers;
        }
    }

    /// <summary>
    /// Register a list of players as a team
    /// </summary>
    /// <param name="playerIdentities"></param>
    /// <returns>The team number</returns>
    public uint RegisterPlayersAsTeam(NetworkIdentity[] playerIdentities)
    {
        if (!registrationsOpened)
        {
            Debug.LogError("Attempted to register a team after registrations were closed.");
            return 0;
        }

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
        if (!registrationsOpened)
        {
            Debug.LogError("Attempted to register a player after registrations were closed.");
            return 0;
        }

        if (playerIdentity)
        {
            playerLapCount[playerIdentity] = 0;
            ResetPlayerCheckpoints(playerIdentity);

            if (countAsTeam)
            {
                teamCount++;
            }

            CheckStart();

            if (playerIdentity.isLocalPlayer) localPlayer = playerIdentity.GetComponent<Player>();

            Debug.Log("Player " + playerIdentity.netId + " registered successfully.");

            return (uint)teamCount;
        }

        Debug.LogError("Attempted to register a null or invalid player.");

        return 0;
    }

    [Server]
    private void CheckStart()
    {
        playerRegisteredCount++;
        if (playerRegisteredCount >= playerExpectedCount)
        {
            StartCountdown();
        }
    }

    [Server]
    public void StartCountdown()
    {
        registrationsOpened = false;
        placements = new NetworkIdentity[playerLapCount.Count];
        StartCoroutine(StartRaceAnimation());
    }

    [Server]
    private IEnumerator StartRaceAnimation()
    {
        Debug.Log("Waiting for countdown");

        yield return new WaitForSeconds(countdownPredelay);

        Debug.Log("Countdown started");

        RpcCountdownStart(countdownDelay);

        yield return new WaitForSeconds(countdownDelay);

        Debug.Log("Countdown finished");

        StartRace();
    }

    [Server]
    private void StartRace()
    {
        foreach (var playerIdentity in playerLapCount.Keys)
        {
            Movements playerMovements = playerIdentity.GetComponent<Movements>();
            playerMovements.SetMovementActive(true);
            LiveLogger.Log("Set movement active for player " + playerIdentity.netId);
        }
        Debug.Log("Race started, moving enabled!");

        RpcRaceStart();
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

    [ClientRpc]
    private void RpcCountdownStart(int duration)
    {
        onCountdownStart.Invoke(duration);
    }

    [ClientRpc]
    private void RpcRaceStart()
    {
        onRaceStart.Invoke();
    }

    [Server]
    private void OnRaceFinished()
    {
        Debug.Log("Race is finished !!!");

        // Communicate the placements to all clients
        RpcUpdatePlacement(placements);

        // Wait for a while before returning to the room
        StartCoroutine(WaitForReturnToRoom());
    }

    [Server]
    private void OnPlayerFinishedRace(NetworkIdentity playerIdentity)
    {
        Debug.Log("Player " + playerIdentity.netId + " has finished the race!");

        Movements playerMovements = playerIdentity.GetComponent<Movements>();
        playerMovements.SetMovementActive(false);

        placements[finishedPlayerCount] = playerIdentity;
        finishedPlayerCount++;
        RpcPlayerFinishedRace(playerIdentity.netId, finishedPlayerCount);

        if (finishedPlayerCount >= playerLapCount.Count)
        {
            // All players have finished
            OnRaceFinished();
        }
    }

    [ClientRpc]
    private void RpcLapCompletion(uint playerIdentity, int laps)
    {
        // If we're the player that completed the lap, update the UI
        if (localPlayer && playerIdentity == localPlayer.netId)
        {
            AudioManager.Instance.PlaySoundEffect(AudioManager.Instance.newLap);
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
            onPlayerFinishedRace.Invoke(confirmedPlacement);
        }
    }

    [Server]
    private IEnumerator WaitForReturnToRoom()
    {
        RpcWaitForReturnToRoom();
        yield return new WaitForSeconds(waitForReturnToRoom);

        GameManager.Instance.ReturnToRoom();
    }

    [ClientRpc]
    private void RpcWaitForReturnToRoom()
    {
        onWaitReturnRoomBegin.Invoke(waitForReturnToRoom);
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

    [ClientRpc]
    public void RpcUpdatePlacement(NetworkIdentity[] newPlacements)
    {
        placements = newPlacements;
        onRaceFinished.Invoke();

        // Make the placements table
        ContestantData[] placementData = new ContestantData[placements.Length];
        for (int i = 0; i < placements.Length; i++)
        {
            // TODO: Retrieve the name of the player, for now let's use its netId
            placementData[i] = new ContestantData(placements[i].netId.ToString(), i + 1, placements[i].isLocalPlayer);
        }

        onPlacementPublished.Invoke(placementData);
    }

    [ClientRpc]
    public void PlayMusicOnStart()
    {
        AudioManager.Instance.PlayMusic(AudioManager.Instance.raceTheme);
    }

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
