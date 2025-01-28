using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SessionCodeHolder : MonoBehaviour
{
    public string sessionCode = "";

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded_SetupSessionCodeUI;
    }

    private void OnSceneLoaded_SetupSessionCodeUI(Scene sceneLoaded, LoadSceneMode loadSceneMode)
    {
        if (sessionCode == "") { return; }

        if (sceneLoaded.name == "RoomOnline")
        {
            GameObject.Find("SessionCodeText").GetComponent<TextMeshProUGUI>().text = "Session code : " + sessionCode;
        }
    }
}
