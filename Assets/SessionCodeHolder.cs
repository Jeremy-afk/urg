using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SessionCodeHolder : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnSessionCodeUpdated))]
    public string sessionCode = "";

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnSessionCodeUpdated(string oldValue, string newValue)
    {
        Debug.LogError("Ancien code :" + oldValue);
        Debug.LogError("Nouveau code : " + newValue);

        sessionCode = newValue;

        if (SceneManager.GetActiveScene().name == "RoomOnline")
        {
            GameObject.Find("SessionCodeText").GetComponent<TextMeshProUGUI>().text = "Session code : " + sessionCode;
        }
    }


    [Command(requiresAuthority = false)]
    public void CmdSetSessionCode(string newSessionCode)
    {
        sessionCode = newSessionCode;
    }
}
