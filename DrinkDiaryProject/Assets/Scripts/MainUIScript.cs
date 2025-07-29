using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUIScript : MonoBehaviour
{
    [Header("Checks")]
    [HideInInspector] public bool isSessionOn;

    [Header("UI References")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button warningPanelButton;
    [SerializeField] private TMP_InputField setupInputField;
    [SerializeField] private Button setupContinueButton;
    [SerializeField] private Button drinkSelectionButton;
    [SerializeField] private GameObject drinkSelectionPanel;
    [SerializeField] private GameObject sessionScreen;
    [SerializeField] private GameObject dimmerPanel;
    [SerializeField] private GameObject sessionSetupPanel;
    [SerializeField] private GameObject warningPanel;
    [SerializeField] private TMP_Text sessionScreenTitleText;

    private string sessionNameInput;

    private SessionManager sessionManager;

    void Start()
    {
        LoadSessionData();
        SetupMainUI();
        SetupButtons();
        InitializeScripts();
    }

    private void InitializeScripts()
    {
        sessionManager = FindFirstObjectByType<SessionManager>();
    }

    private void SetupButtons()
    {
        playButton.onClick.AddListener(OpenSessionScreen);
        warningPanelButton.onClick.AddListener(CloseWarningPanel);
        setupContinueButton.onClick.AddListener(CloseSetupPanel);
        setupInputField.onValueChanged.AddListener(UpdateContinueButtonState);
        drinkSelectionButton.onClick.AddListener(OpenDrinkSelectionPanel);
    }

    private void OpenDrinkSelectionPanel()
    {
        drinkSelectionPanel.SetActive(true);
    }

    public void CloseDrinkSelectionPanel()
    {
        drinkSelectionPanel.SetActive(false);
    }

    private void UpdateContinueButtonState(string text)
    {
        sessionNameInput = text;

        if (sessionNameInput != "")
        {
            setupContinueButton.interactable = true;
        }
        else
        {
            setupContinueButton.interactable = false;
        }
    }

    private void OpenSessionScreen()
    {
        dimmerPanel.SetActive(!isSessionOn);
        warningPanel.SetActive(!isSessionOn);
        sessionScreen.SetActive(true);

        if (isSessionOn)
        {
            sessionScreenTitleText.text = sessionManager.ActiveSession.sessionName;
        }
    }

    private void CloseWarningPanel()
    {
        warningPanel.SetActive(false);

        sessionSetupPanel.SetActive(!isSessionOn);
    }

    private void CloseSetupPanel()
    {
        sessionSetupPanel.SetActive(false);
        sessionManager.StartNewSession(sessionNameInput);
        sessionScreenTitleText.text = sessionNameInput;
        dimmerPanel.SetActive(false);
    }

    private void SetupMainUI()
    {
        sessionScreen.SetActive(false);
        sessionSetupPanel.SetActive(false);
        warningPanel.SetActive(false);
        dimmerPanel.SetActive(false);
        drinkSelectionPanel.SetActive(false);

        sessionScreenTitleText.text = "";

        if (isSessionOn)
        {
            // Continue session UI setup here
        }
        else
        {
            // Regular UI setup logic here
        }
    }

    private void LoadSessionData()
    {
        if (PlayerPrefs.HasKey("isSessionOn"))
        {
            isSessionOn = (PlayerPrefs.GetInt("isSessionOn") > 0) ? true : false;
        }
        else
        {
            isSessionOn = false;
        }
    }
}
