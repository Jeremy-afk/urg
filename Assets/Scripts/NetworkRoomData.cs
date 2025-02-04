using Mirror;
using UnityEngine;

public class NetworkRoomData : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnNewSessionCode))]
    public string SessionCode;

    public static NetworkRoomData Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Debug.Log("NetworkRoomData created");
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnNewSessionCode(string oldCode, string newCode)
    {
        Debug.Log($"Session code changed from {oldCode} to {newCode}");
        GameObject text = GameObject.FindGameObjectWithTag("SessionCodeText");
        if (text != null)
        {
            text.GetComponent<TMPro.TextMeshProUGUI>().text = newCode;
        }
    }

    public void SetSessionCode(string code)
    {
        Debug.Log($"Setting session code to {code}");
        SessionCode = code;
    }
}
