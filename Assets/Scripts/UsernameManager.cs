using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UsernameManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private string playprefKey = "Username";
    [SerializeField] private UnityEvent<string> onNewUsername;
    public string Username { get; private set; }

    private void OnEnable()
    {
        if (PlayerPrefs.HasKey(playprefKey))
        {
            Username = PlayerPrefs.GetString(playprefKey);
            usernameInputField.text = Username;
        }
    }

    private void OnDisable()
    {
        SaveUsername();
        onNewUsername.Invoke(Username);
    }

    public void SaveUsername()
    {
        if (string.IsNullOrEmpty(Username))
        {
            PlayerPrefs.DeleteKey(playprefKey);
        }
        else
        {
            Username = usernameInputField.text;
            PlayerPrefs.SetString(playprefKey, Username);
        }
    }
}
