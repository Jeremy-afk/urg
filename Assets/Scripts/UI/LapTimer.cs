using TMPro;
using UnityEngine;

public class LapTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private bool startOnAwake = false;

    private float time;
    private bool isRunning = false;

    public float Time => time;

    private void Awake()
    {
        if (startOnAwake)
        {
            StartTimer();
        }
    }

    private void Update()
    {
        if (isRunning)
        {
            time += UnityEngine.Time.deltaTime;
            if (timeText != null)
            {
                int minutes = Mathf.FloorToInt(time / 60);
                int seconds = Mathf.FloorToInt(time - minutes * 60);
                int centiemes = Mathf.FloorToInt((time - minutes * 60 - seconds) * 100);
                string timeString = string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, centiemes);
                timeText.text = timeString;
            }
        }
    }

    public void StartTimer()
    {
        time = 0;
        isRunning = true;
    }

    public void ResumeTimer()
    {
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }
}
