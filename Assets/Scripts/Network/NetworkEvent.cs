using Mirror;
using UnityEngine;

public class NetworkEvent : NetworkBehaviour
{
    [SerializeField] private Countdown readyCountdown;

    [ClientRpc]
    public void RpcStartReadyCountdown(int duration)
    {
        readyCountdown.SetDuration(duration);
        readyCountdown.StartCountdown();
    }
}
