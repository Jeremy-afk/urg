using TMPro;
using UnityEngine;

public class UsernameManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private string playprefKey = "Username";

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
    }

    public void SaveUsername()
    {
        Username = usernameInputField.text;
        PlayerPrefs.SetString(playprefKey, Username);
    }

    public void SetUsername(string username)
    {
        this.Username = username;
    }
}
