using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    private NetworkIdentity playerIdentity;
    private Finish finishLine;

    private TextMeshProUGUI timerText;
    static float timer;

    private TextMeshProUGUI lapText;

    // Start is called before the first frame update
    void Start()
    {
        playerIdentity = GetComponent<NetworkIdentity>();
        finishLine = GameObject.Find("Finish").GetComponent<Finish>();

        timerText = GameObject.Find("GameUi/LapTimer/Timer").GetComponent< TextMeshProUGUI>();
        lapText = GameObject.Find("GameUi/LapTimer/LapCount").GetComponent<TextMeshProUGUI>();

        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        int lapCount = finishLine.GetPlayerCompletedLap(playerIdentity) + 1;
        int requiredLap = finishLine.GetRequiredLaps();
        if (lapCount <= requiredLap)
        {
            timer += Time.deltaTime;
            int minutes = Mathf.FloorToInt(timer / 60);
            int seconds = Mathf.FloorToInt(timer - minutes * 60);
            int centiemes = Mathf.FloorToInt((timer - minutes * 60 - seconds) * 100);

            string time = string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, centiemes);
            timerText.text = time;

            string laps = string.Format("Lap {0:0} / {1:0}", lapCount, requiredLap);
            lapText.text = laps;
        }
    }
}
