using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lapText;
    [SerializeField] private Finish finishLine;

    private void Start()
    {
        UpdateLapUI(0);
    }

    public void UpdateLapUI(int completedLapsCount)
    {
        int requiredLap = finishLine.GetRequiredLaps();
        if (completedLapsCount + 1 <= requiredLap)
        {
            string laps = string.Format("Lap {0:0} / {1:0}", completedLapsCount + 1, requiredLap);
            lapText.text = laps;
        }
    }
}
