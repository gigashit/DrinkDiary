using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SessionManager : MonoBehaviour
{
    private string activeSessionPath => Path.Combine(Application.persistentDataPath, "active_session.json");
    private string sessionHistoryPath => Path.Combine(Application.persistentDataPath, "session_history.json");

    public DrinkSession ActiveSession { get; private set; }
    public List<DrinkSession> SessionHistory { get; private set; } = new List<DrinkSession>();

    [Header("Script References")]
    [SerializeField] private MainUIScript mainUIScript;
    [SerializeField] private XPSystem xpSystem;

    void Awake()
    {
        LoadHistory();
        LoadActiveSession();
    }

    public void StartNewSession(string sessionName)
    {
        ActiveSession = new DrinkSession
        {
            sessionName = sessionName,
            startTime = DateTime.Now,
            startTimeString = DateTime.Now.ToString("o")
        };
        SaveActiveSession();
        mainUIScript.isSessionOn = true;
        PlayerPrefs.SetInt("isSessionOn", 1);
    }

    public void AddDrink(DrinkEntry drink)
    {
        if (ActiveSession != null)
        {
            drink.orderNumber = ActiveSession.drinks.Count + 1;
            ActiveSession.drinks.Add(drink);
            mainUIScript.UpdateSessionDrinkListUI(ActiveSession.drinks);
            xpSystem.UpdateXPNumber();
            SaveActiveSession();
        }
    }

    public void EndSession(string optionalNote = "")
    {
        if (ActiveSession != null)
        {
            if (ActiveSession.drinks.Count > 0)
            {
                ActiveSession.notes = optionalNote;
                SessionHistory.Add(ActiveSession);
                SaveHistory();
            }

            File.Delete(activeSessionPath); // Clear active session
            ActiveSession = null;
            mainUIScript.isSessionOn = false;
            PlayerPrefs.SetInt("isSessionOn", 0);

        }
    }

    private void SaveActiveSession()
    {
        if (ActiveSession == null) return;
        string json = JsonUtility.ToJson(ActiveSession);
        File.WriteAllText(activeSessionPath, json);
    }

    private void LoadActiveSession()
    {
        if (!File.Exists(activeSessionPath)) return;

        string json = File.ReadAllText(activeSessionPath);
        ActiveSession = JsonUtility.FromJson<DrinkSession>(json);

        if (!string.IsNullOrEmpty(ActiveSession.startTimeString))
        {
            ActiveSession.startTime = DateTime.Parse(ActiveSession.startTimeString);
        }

        mainUIScript.UpdateSessionDrinkListUI(ActiveSession.drinks);
    }

    private void SaveHistory()
    {
        string json = JsonUtility.ToJson(new SessionListWrapper(SessionHistory));
        File.WriteAllText(sessionHistoryPath, json);
        mainUIScript.UpdateHistoryListUI();
    }

    private void LoadHistory()
    {
        if (!File.Exists(sessionHistoryPath)) return;

        string json = File.ReadAllText(sessionHistoryPath);
        SessionListWrapper wrapper = JsonUtility.FromJson<SessionListWrapper>(json);
        SessionHistory = wrapper.sessions;

        foreach (var session in SessionHistory)
        {
            if (!string.IsNullOrEmpty(session.startTimeString))
            {
                session.startTime = DateTime.Parse(session.startTimeString);
            }
        }

        mainUIScript.UpdateHistoryListUI();
        Invoke(nameof(DelayedLevelUpdate), 0.2f);
    }

    void DelayedLevelUpdate()
    {
        xpSystem.UpdateXPNumber();
    }

    public void ClearSessionData()
    {
        if (File.Exists(sessionHistoryPath)) { File.Delete(sessionHistoryPath); }
        if (File.Exists(activeSessionPath)) { File.Delete(activeSessionPath); }
        if (PlayerPrefs.HasKey("isSessionOn")) { PlayerPrefs.DeleteKey("isSessionOn"); }

        Debug.Log("Session data cleared");
    }

    [Serializable]
    private class SessionListWrapper
    {
        public List<DrinkSession> sessions = new();

        public SessionListWrapper(List<DrinkSession> sessions)
        {
            this.sessions = sessions;
        }
    }
}
