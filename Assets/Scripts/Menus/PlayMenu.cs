using UnityEngine;

public class PlayMenu : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject soloMenu;
    [SerializeField] GameObject joinLobbyMenu;
    [SerializeField] GameObject createLobbyMenu;

    public void Solo()
    {
        soloMenu.SetActive(true);
        gameObject.SetActive(false);
    }

    public void JoinLobby()
    {
        joinLobbyMenu.SetActive(true);
    }

    public void CreateLobby()
    {
        createLobbyMenu.SetActive(true);
    }

    public void Return()
    {
        mainMenu.SetActive(true);
        gameObject.SetActive(false);
    }
}
