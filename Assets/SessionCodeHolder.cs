using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SessionCodeHolder : NetworkBehaviour
{
    [SyncVar]
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

    public override void OnStartClient()
    {
        base.OnStartClient();

        try
        {
            GameObject.Find("SessionCodeText").GetComponent<TextMeshProUGUI>().text = "Session code : " + sessionCode;
        }
        catch (Exception e)
        {
            Debug.Log("No session code text UI found in this scene.");
        }
    }
}
