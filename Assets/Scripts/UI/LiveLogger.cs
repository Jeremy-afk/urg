using TMPro;
using UnityEngine;

public class LiveLogger : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI logTextTemplate;

    static public LiveLogger Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Multiple LiveLogger instances detected in the scene. Only one LiveLogger is allowed.");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    static public void Log(string message, LogType logType = LogType.Log)
    {

        if (Instance == null)
        {
            Debug.LogWarning("LiveLogger instance not found. Log will not be displayed.");
            return;
        }

        Instance.HandleLog(message, logType);
    }

    private void HandleLog(string logString, LogType type)
    {
        TextMeshProUGUI logText = Instantiate(logTextTemplate, logTextTemplate.transform.parent);
        logText.text = logString;
        logText.color = GetColorFromLogType(type);
        logText.gameObject.SetActive(true);
        Destroy(logText.gameObject, 10.0f);
    }

    static private Color GetColorFromLogType(LogType type)
    {
        switch (type)
        {
            case LogType.Error:
            case LogType.Exception:
            case LogType.Assert:
                return Color.red;
            case LogType.Warning:
                return Color.yellow;
            default:
                return Color.white;
        }
    }
}
