using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject playMenu;
    [SerializeField] GameObject optionsMenu;

    public void TooglePlayMenu(bool showPlayMenu)
    {
        playMenu.SetActive(showPlayMenu);
        gameObject.SetActive(!showPlayMenu);
    }

    public void ToogleOptions(bool showOptions)
    {
        optionsMenu.SetActive(showOptions);
        mainMenu.SetActive(!showOptions);
    }

    public void StartScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
