using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    public static Popup Instance { get; private set; }


    [SerializeField] private bool showOnAwake = false;

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI mainText;

    [SerializeField] private Button greenActionButton;
    [SerializeField] private Button redActionButton;

    [SerializeField] private TextMeshProUGUI greenActionText;
    [SerializeField] private TextMeshProUGUI redActionText;

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Debug.LogWarning("Multiple Popup instances detected in the scene. Only one Popup is allowed.");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        greenActionButton.onClick.AddListener(() => gameObject.SetActive(false));
        redActionButton.onClick.AddListener(() => gameObject.SetActive(false));

        if (showOnAwake)
        {
            ShowPopup();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void SetTitle(string title)
    {
        titleText.text = title;
    }

    public void SetMainText(string main)
    {
        mainText.text = main;
    }

    public void SetGreenAction(string action)
    {
        greenActionText.text = action;
    }

    public void SetRedAction(string action)
    {
        redActionText.text = action;
    }

    public void SetGreenActionListener(UnityEngine.Events.UnityAction action)
    {
        greenActionButton.onClick.RemoveAllListeners();
        greenActionButton.onClick.AddListener(() => gameObject.SetActive(false));
        if (action != null)
        {
            greenActionButton.onClick.AddListener(action);
        }
    }

    public void SetRedActionListener(UnityEngine.Events.UnityAction action)
    {

        redActionButton.onClick.RemoveAllListeners();
        redActionButton.onClick.AddListener(() => gameObject.SetActive(false));
        if (action != null)
        {
            redActionButton.onClick.AddListener(action);
        }
    }

    public void MakePopup(string title, string main, string greenAction, string redAction, UnityEngine.Events.UnityAction greenActionListener = null, UnityEngine.Events.UnityAction redActionListener = null)
    {
        SetTitle(title);
        SetMainText(main);
        SetGreenAction(greenAction);
        SetRedAction(redAction);
        SetGreenActionListener(greenActionListener);
        SetRedActionListener(redActionListener);
        ShowPopup();
    }

    public void ShowPopup()
    {
        gameObject.SetActive(true);
    }
}
