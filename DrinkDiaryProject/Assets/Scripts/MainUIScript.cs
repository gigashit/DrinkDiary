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
    [SerializeField] private GameObject sessionScreen;
    [SerializeField] private GameObject dimmerPanel;
    [SerializeField] private GameObject sessionSetupPanel;
    [SerializeField] private GameObject warningPanel;

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
        warningPanel.SetActive(!isSessionOn);

        dimmerPanel.SetActive(true);
        sessionScreen.SetActive(true);
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
        dimmerPanel.SetActive(false);
    }

    private void SetupMainUI()
    {
        sessionScreen.SetActive(false);
        sessionSetupPanel.SetActive(false);
        warningPanel.SetActive(false);
        dimmerPanel.SetActive(false);

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
