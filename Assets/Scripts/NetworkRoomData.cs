using Mirror;
using UnityEngine;

public class NetworkRoomData : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnNewSessionCode))]
    public string SessionCode;

    [SyncVar(hook = nameof(Test))]
    public int test;

    public static NetworkRoomData Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Debug.Log("NetworkRoomData available");
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [ServerCallback]
    private void Update()
    {
        test++;
    }

    private void Test(int oldTest, int newTest)
    {
        test = newTest;
        LiveLogger.Log($"Test changed from {oldTest} to {newTest}");
    }

    private void OnNewSessionCode(string oldCode, string newCode)
    {
        SessionCode = newCode;
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
        RpcSessionCodeChanged(code);
        SessionCode = code;
    }

    [ClientRpc]
    private void RpcSessionCodeChanged(string code)
    {
        LiveLogger.Log($"Session code changed to {code}");
        SessionCode = code;
        GameObject text = GameObject.FindGameObjectWithTag("SessionCodeText");
        if (text != null)
        {
            text.GetComponent<TMPro.TextMeshProUGUI>().text = code;
        }
    }
}
