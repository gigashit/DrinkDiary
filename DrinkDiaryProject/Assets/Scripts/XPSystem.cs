using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class XPSystem : MonoBehaviour
{
    [Header("XP Requirements")]
    [SerializeField] private int level2XP = 100;
    [SerializeField] private int level3XP = 350;
    [SerializeField] private int level4XP = 700;
    [SerializeField] private int level5XP = 1200;
    [SerializeField] private int level6XP = 1800;
    [SerializeField] private int level7XP = 2500;
    [SerializeField] private int level8XP = 3300;
    [SerializeField] private int level9XP = 4500;
    [SerializeField] private int level10XP = 7000;

    [Header("UI Elements")]
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private RectTransform XPBar;
    [SerializeField] private TMP_Text XPText;
    [SerializeField] private RectTransform sessionXPBar;
    [SerializeField] private TMP_Text sessionLevelText;

    [Header("Script References")]
    [SerializeField] private SessionManager sessionManager;

    [HideInInspector] public int totalXP;
    [HideInInspector] public int userLevel;

    private int nextXP;
    private int previousXP;

    private void Start()
    {
        Invoke(nameof(DelayedXPCheck), 0.5f);
    }

    void DelayedXPCheck()
    {
        if (totalXP == 0)
        {
            UpdateXPNumber();
        }
    }

    public void UpdateXPNumber()
    {
        float totalServings = 0f;

        if (sessionManager.ActiveSession != null)
        {
            foreach (DrinkEntry drink in sessionManager.ActiveSession.drinks)
            {
                totalServings += drink.serving;
            }
        }

        if (sessionManager.SessionHistory.Count > 0)
        {
            foreach (DrinkSession session in  sessionManager.SessionHistory)
            {
                foreach (DrinkEntry drink in session.drinks)
                {
                    totalServings += drink.serving;
                }
            }
        }

        totalXP = Mathf.RoundToInt(totalServings * 10);

        UpdateLevel();
        UpdateVisuals();
    }

    void UpdateLevel()
    {
        var stringTable = LocalizationSettings
    .StringDatabase
    .GetTable("LevelTitles");

        string key;

        switch (totalXP)
        {
            case < 100: userLevel = 1; key = "level1title"; nextXP = level2XP; previousXP = 0; break;
            case < 350: userLevel = 2; key = "level2title"; nextXP = level3XP; previousXP = level2XP; break;
            case < 700: userLevel = 3; key = "level3title"; nextXP = level4XP; previousXP = level3XP; break;
            case < 1200: userLevel = 4; key = "level4title"; nextXP = level5XP; previousXP = level4XP; break;
            case < 1800: userLevel = 5; key = "level5title"; nextXP = level6XP; previousXP = level5XP; break;
            case < 2500: userLevel = 6; key = "level6title"; nextXP = level7XP; previousXP = level6XP; break;
            case < 3300: userLevel = 7; key = "level7title"; nextXP = level8XP; previousXP = level7XP; break;
            case < 4500: userLevel = 8; key = "level8title"; nextXP = level9XP; previousXP = level8XP; break;
            case < 7000: userLevel = 9; key = "level9title"; nextXP = level10XP; previousXP = level9XP; break;
            case >= 7000: userLevel = 10; key = "level10title"; nextXP = 0; break;
        }

        string localized = stringTable.GetEntry(key).GetLocalizedString();
        titleText.text = localized;
    }

    void UpdateVisuals()
    {
        levelText.text = userLevel.ToString();
        sessionLevelText.text = userLevel.ToString();

        if (nextXP != 0)
        {
            XPText.text = totalXP.ToString() + " / " + nextXP.ToString() + " XP";

            int XPNeeded = nextXP - previousXP;
            int XPGained = totalXP - previousXP;
            float scale = (float)XPGained / XPNeeded;

            Debug.Log("XP Gained = " + XPGained + ", XP Needed = " + XPNeeded + ", Bar Scale = " + scale);

            XPBar.localScale = new Vector3(scale, XPBar.localScale.y, XPBar.localScale.z);
            sessionXPBar.localScale = new Vector3(scale, XPBar.localScale.y, XPBar.localScale.z);
        }
        else
        {
            XPText.text = totalXP.ToString() + " XP";
            XPBar.localScale = Vector3.one;
            sessionXPBar.localScale = Vector3.one;
        }


    }
}
