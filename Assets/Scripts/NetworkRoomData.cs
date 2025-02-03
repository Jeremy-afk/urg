using Mirror;

public class NetworkRoomData : NetworkBehaviour
{
    [field: SyncVar]
    public string SessionCode { get; private set; }

    public static NetworkRoomData Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetSessionCode(string code)
    {
        SessionCode = code;
    }
}
