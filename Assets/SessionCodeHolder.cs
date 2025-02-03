using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
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
            Debug.LogError("No session code text UI found in this scene.");
        }
    }

    private void OnSessionCodeUpdated(string oldValue, string newValue)
    {
        Debug.LogError("Ancien code :" + oldValue);
        Debug.LogError("Nouveau code : " + newValue);

        sessionCode = newValue;
    }


    [Command(requiresAuthority = false)]
    public void CmdSetSessionCode(string newSessionCode)
    {
        sessionCode = newSessionCode;
    }

}
