using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject audioMenu;
    [SerializeField] GameObject controlsMenu;
    [SerializeField] GameObject userMenu;

    public void Audio()
    {
        audioMenu.SetActive(true);
    }

    public void AudioReturn()
    {
        audioMenu.SetActive(false);
    }

    public void Controls()
    {
        controlsMenu.SetActive(true);
    }

    public void ControlsReturn()
    {
        controlsMenu.SetActive(false);
    }

    public void User()
    {
        userMenu.SetActive(true);
    }

    public void UserReturn()
    {
        userMenu.SetActive(false);
    }

    public void Return()
    {
        mainMenu.SetActive(true);
        gameObject.SetActive(false);
    }
}
