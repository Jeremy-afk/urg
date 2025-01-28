using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerListItemUI : MonoBehaviour
{
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI playerReadyText;
    
    public void UpdateUI(string playerName, bool playerReady)
    {
        playerNameText.text = playerName;
        playerReadyText.text = playerReady ?
                "<color=green>Ready</color>" :
                "<color=red>Not Ready</color>";
    }
}
