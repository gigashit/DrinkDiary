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
            startTime = DateTime.Now
        };
        SaveActiveSession();
    }

    public void AddDrink(DrinkEntry drink)
    {
        if (ActiveSession != null)
        {
            ActiveSession.drinks.Add(drink);
            SaveActiveSession();
        }
    }

    public void EndSession(string optionalNote = "")
    {
        if (ActiveSession != null)
        {
            ActiveSession.endTime = DateTime.Now;
            ActiveSession.notes = optionalNote;
            SessionHistory.Add(ActiveSession);
            SaveHistory();

            File.Delete(activeSessionPath); // Clear active session
            ActiveSession = null;
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
    }

    private void SaveHistory()
    {
        string json = JsonUtility.ToJson(new SessionListWrapper(SessionHistory));
        File.WriteAllText(sessionHistoryPath, json);
    }

    private void LoadHistory()
    {
        if (!File.Exists(sessionHistoryPath)) return;

        string json = File.ReadAllText(sessionHistoryPath);
        SessionListWrapper wrapper = JsonUtility.FromJson<SessionListWrapper>(json);
        SessionHistory = wrapper.sessions;
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
