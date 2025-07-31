using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

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
    [SerializeField] private Button backFromSessionButton;
    [SerializeField] private Button clearSessionDataButton;
    [SerializeField] private Button clearDrinkDataButton;
    [SerializeField] private GameObject drinkSelectionPanel;
    [SerializeField] private GameObject sessionScreen;
    [SerializeField] private GameObject dimmerPanel;
    [SerializeField] private GameObject sessionSetupPanel;
    [SerializeField] private GameObject warningPanel;
    [SerializeField] private TMP_Text sessionScreenTitleText;
    [SerializeField] private Transform sessionDrinkListContent;
    [SerializeField] private GameObject inSessionInfo;
    [SerializeField] private TMP_Text activeSessionNameText;
    [SerializeField] private TMP_Text activeSessionStartDateText;
    [SerializeField] private Button concludeSessionFromMainMenuButton;
    [SerializeField] private Button concludeSessionFromSessionScreenButton;
    [SerializeField] private TMP_Text totalSessionServingsText;
    [SerializeField] private TMP_Text setupDateNowText;
    [SerializeField] private TMP_Text sessionTitleDateText;

    [Header("Conclude Panel UI")]
    [SerializeField] private GameObject concludeSessionPanel;
    [SerializeField] private TMP_Text concludeSessionNameText;
    [SerializeField] private TMP_Text concludeSessionDateText;
    [SerializeField] private TMP_Text concludeSessionServingsText;
    [SerializeField] private TMP_Text concludeSessionExplainerText;
    [SerializeField] private Button finalizeConclusionButton;
    [SerializeField] private Button backFromConcludePanelButton;

    [Header("History Panel References")]
    [SerializeField] private Button openHistoryPanelButton;
    [SerializeField] private GameObject historyPanel;
    [SerializeField] private Transform historyContent;
    [SerializeField] private GameObject historyEntryPrefab;
    [SerializeField] private Button backFromHistoryPanelButton;

    [Header("Prefabs")]
    [SerializeField] private GameObject sessionDrinkEntryPrefab;

    [Header("Script References")]
    [SerializeField] private SessionManager sessionManager;
    [SerializeField] private DrinkManager drinkManager;

    [Header("Colors")]
    [SerializeField] private Color latestColor;

    private string sessionNameInput;

    void Start()
    {
        LoadSessionData();
        SetupMainUI();
        SetupButtons();
    }

    private void SetupButtons()
    {
        playButton.onClick.AddListener(OpenSessionScreen);
        warningPanelButton.onClick.AddListener(CloseWarningPanel);
        setupContinueButton.onClick.AddListener(CloseSetupPanel);
        setupInputField.onValueChanged.AddListener(UpdateContinueButtonState);
        drinkSelectionButton.onClick.AddListener(OpenDrinkSelectionPanel);
        backFromSessionButton.onClick.AddListener(BackFromSessionScreen);
        clearSessionDataButton.onClick.AddListener(sessionManager.ClearSessionData);
        clearDrinkDataButton.onClick.AddListener(drinkManager.ClearDrinkData);
        concludeSessionFromMainMenuButton.onClick.AddListener(OpenConclusionScreen);
        concludeSessionFromSessionScreenButton.onClick.AddListener(OpenConclusionScreen);
        finalizeConclusionButton.onClick.AddListener(ConcludeSession);
        backFromConcludePanelButton.onClick.AddListener(CancelConclusion);
        openHistoryPanelButton.onClick.AddListener(OpenHistoryPanel);
        backFromHistoryPanelButton.onClick.AddListener(CloseHistoryPanel);
    }

    void OpenHistoryPanel()
    {
        dimmerPanel.SetActive(true);
        historyPanel.SetActive(true);
    }

    void CloseHistoryPanel()
    {
        dimmerPanel.SetActive(false);
        historyPanel.SetActive(false);
    }

    void OpenConclusionScreen()
    {
        DrinkSession session = sessionManager.ActiveSession;

        concludeSessionNameText.text = session.sessionName;
        concludeSessionDateText.text = FormatDateWithSuffix(session.startTime);
        concludeSessionExplainerText.text = SessionHistoryExplainerMessageSwitch(session.drinks.Count);

        dimmerPanel.SetActive(true);
        concludeSessionPanel.SetActive(true);
    }

    void ConcludeSession()
    {
        sessionManager.EndSession();
        UpdateSessionDrinkListUI(null);

        SetupMainUI();
    }

    void CancelConclusion()
    {
        concludeSessionPanel.SetActive(false);
        dimmerPanel.SetActive(false);
    }

    public void UpdateSessionDrinkListUI(List<DrinkEntry> drinkList)
    {
        foreach (Transform child in sessionDrinkListContent)
        {
            Destroy(child.gameObject);
        }

        if (drinkList != null)
        {
            foreach (DrinkEntry drinkEntry in drinkList)
            {
                GameObject prefab = Instantiate(sessionDrinkEntryPrefab, sessionDrinkListContent);
                prefab.transform.SetSiblingIndex(0);
                SessionDrinkEntry script = prefab.GetComponent<SessionDrinkEntry>();
                script.Setup(drinkEntry.orderNumber, drinkEntry.drinkName, drinkEntry.serving);

                if (drinkEntry.orderNumber == drinkList.Count)
                {
                    Image image = prefab.GetComponent<Image>();
                    image.color = latestColor;
                }
            }
        }

        UpdateTotalServingsNumber(drinkList);
    }

    public void UpdateHistoryListUI()
    {
        foreach (Transform child in historyContent)
        {
            Destroy(child.gameObject);
        }

        List<DrinkSession> drinkSessions = sessionManager.SessionHistory;

        if (drinkSessions != null && drinkSessions.Count > 0)
        {
            foreach (DrinkSession session in drinkSessions)
            {
                GameObject prefab = Instantiate(historyEntryPrefab, historyContent);
                prefab.transform.SetSiblingIndex(0);
                HistoryEntry script = prefab.GetComponent<HistoryEntry>();
                script.Setup(session, FormatDateWithSuffix(session.startTime));
            }
        }


    }

    private void UpdateTotalServingsNumber(List<DrinkEntry> drinkList)
    {
        float totalServings = 0;

        if (drinkList != null)
        {
            foreach (DrinkEntry drinkEntry in drinkList)
            {
                totalServings += drinkEntry.serving;
            }
        }

        totalSessionServingsText.text = totalServings.ToString();
        concludeSessionServingsText.text = totalServings.ToString();
    }

    private void OpenDrinkSelectionPanel()
    {
        dimmerPanel.SetActive(true);
        drinkSelectionPanel.SetActive(true);
    }

    public void CloseDrinkSelectionPanel()
    {
        dimmerPanel.SetActive(false);
        drinkSelectionPanel.SetActive(false);
    }

    void BackFromSessionScreen()
    {
        SetupMainUI();
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
            sessionTitleDateText.text = FormatDateWithSuffix(sessionManager.ActiveSession.startTime);
        }
    }

    private void CloseWarningPanel()
    {
        warningPanel.SetActive(false);

        sessionSetupPanel.SetActive(!isSessionOn);
        setupDateNowText.text = FormatDateWithSuffix(DateTime.Now);
    }

    private void CloseSetupPanel()
    {
        sessionSetupPanel.SetActive(false);
        sessionManager.StartNewSession(sessionNameInput);
        sessionScreenTitleText.text = sessionNameInput;
        sessionTitleDateText.text = FormatDateWithSuffix(DateTime.Now);
        dimmerPanel.SetActive(false);
    }

    private void SetupMainUI()
    {
        sessionScreen.SetActive(false);
        sessionSetupPanel.SetActive(false);
        warningPanel.SetActive(false);
        dimmerPanel.SetActive(false);
        drinkSelectionPanel.SetActive(false);
        concludeSessionPanel.SetActive(false);
        historyPanel.SetActive(false);

        sessionScreenTitleText.text = "";
        sessionTitleDateText.text = "";

        if (isSessionOn)
        {
            activeSessionNameText.text = sessionManager.ActiveSession.sessionName;
            activeSessionStartDateText.text = FormatDateWithSuffix(sessionManager.ActiveSession.startTime);
        }
        else
        {
            totalSessionServingsText.text = "0";
        }

            ChangePlayButtonLabel(isSessionOn);
        inSessionInfo.SetActive(isSessionOn);
    }

    public static string FormatDateWithSuffix(DateTime date)
    {
        int day = date.Day;
        string suffix;

        if (day >= 11 && day <= 13)
        {
            suffix = "th"; // Special case for 11th, 12th, 13th
        }
        else
        {
            switch (day % 10)
            {
                case 1: suffix = "st"; break;
                case 2: suffix = "nd"; break;
                case 3: suffix = "rd"; break;
                default: suffix = "th"; break;
            }
        }

        string month = date.ToString("MMM"); // e.g., "Jan", "Feb", "Jun"
        int year = date.Year;

        return $"{month} {day}{suffix} {year}";
    }

    void ChangePlayButtonLabel(bool inSession)
    {
        var stringTable = LocalizationSettings
            .StringDatabase
            .GetTable("Buttons");

        string key;
      
        if (inSession) { key = "playbutton_insession"; }
        else { key = "playbutton_nosession"; }

            // this will return the localized value for the currently selected locale
            string localized = stringTable.GetEntry(key).GetLocalizedString();

        TMP_Text label = playButton.gameObject.GetComponentInChildren<TMP_Text>();
        label.text = localized;
    }

    private string SessionHistoryExplainerMessageSwitch(int drinksAmount)
    {
        var stringTable = LocalizationSettings
    .StringDatabase
    .GetTable("SessionScreen");

        string key;
        if (drinksAmount > 0)
        {
            key = "sessionend_historyexplainer";
        }
        else
        {
            key = "sessionend_historyexplainer_nodrinks";
        }

        return stringTable.GetEntry(key).GetLocalizedString();
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
