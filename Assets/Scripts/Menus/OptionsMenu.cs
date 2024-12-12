using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    [Header("Option Sub-Menus")]
    [SerializeField] GameObject optionButtons;
    [SerializeField] GameObject audioSubMenu;
    [SerializeField] GameObject controlsSubMenu;
    [SerializeField] GameObject userSubMenu;

    public void ToogleAudio(bool showAudioPanel)
    {
        audioSubMenu.SetActive(showAudioPanel);
        optionButtons.SetActive(!showAudioPanel);
    }

    public void ToogleControls(bool showControlsPanel)
    {
        controlsSubMenu.SetActive(showControlsPanel);
        optionButtons.SetActive(!showControlsPanel);
    }

    public void ToogleUser(bool showUserPanel)
    {
        userSubMenu.SetActive(showUserPanel);
        optionButtons.SetActive(!showUserPanel);
    }
}
