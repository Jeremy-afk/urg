using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject playMenu;
    [SerializeField] GameObject optionsMenu;

    public void Play()
    {
        playMenu.SetActive(true);
        gameObject.SetActive(false);
    }

    public void Options()
    {
        optionsMenu.SetActive(true);
        gameObject.SetActive(false);
    }
}
