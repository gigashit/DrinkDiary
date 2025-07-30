using System;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class HistoryEntry : MonoBehaviour
{
    [SerializeField] private TMP_Text sessionTitle;
    [SerializeField] private TMP_Text sessionDate;
    [SerializeField] private TMP_Text servingsAmount;
    [SerializeField] private TMP_Text favoriteDrinkText;

    public MainUIScript mainUIScript;

    public void Setup(DrinkSession session, string formattedDate)
    {
        sessionTitle.text = session.sessionName;
        sessionDate.text = formattedDate;

        float totalServings = 0;

        foreach (DrinkEntry drinkEntry in session.drinks)
        {
            totalServings += drinkEntry.serving;
        }

        servingsAmount.text = totalServings.ToString();

        favoriteDrinkText.text = GetFavoriteDrink(session.drinks).drinkName;
    }

    private static System.Random random = new System.Random();

    private DrinkEntry GetFavoriteDrink(List<DrinkEntry> list)
    {
        var nameGroups = list
            .GroupBy(x => x.drinkName)
            .Select(g => new { drinkName = g.Key, Total = g.Sum(x => x.serving) })
            .ToList();

        float maxTotal = nameGroups.Max(g =>  g.Total);

        var topNames = nameGroups
            .Where(g => g.Total == maxTotal)
            .Select(g => g.drinkName)
            .ToList();

        string selectedName = topNames[random.Next(topNames.Count)];

        return list.First(z => z.drinkName == selectedName);
    }
}
