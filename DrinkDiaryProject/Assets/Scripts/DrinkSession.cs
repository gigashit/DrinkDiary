using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DrinkSession
{
    public string sessionName;
    public DateTime startTime;
    public DateTime? endTime; // Nullable: null = session ongoing
    public List<DrinkEntry> drinks = new List<DrinkEntry>();
    public string notes;

    public bool IsOngoing => endTime == null;
}

[System.Serializable]
public class DrinkEntry
{
    public string drinkName;
    public float alcoholUnits; // Or reference to a `Drink` model if you want
    public DateTime consumedAt;
}

[System.Serializable]
public class DrinkIngredient
{
    public string name;
    public float amountCl;
    public float alcoholPercent; // 0–100, e.g. 4.7% for beer
}

[System.Serializable]
public class Drink
{
    public string name;
    public List<DrinkIngredient> ingredients = new();

    public float TotalVolumeCl
    {
        get
        {
            float total = 0f;
            foreach (var ingredient in ingredients)
                total += ingredient.amountCl;
            return total;
        }
    }

    public float AlcoholPercentage
    {
        get
        {
            float totalVolume = TotalVolumeCl;
            if (totalVolume <= 0f) return 0f;

            float alcoholAmount = 0f;
            foreach (var i in ingredients)
                alcoholAmount += i.amountCl * (i.alcoholPercent / 100f);

            return (alcoholAmount / totalVolume) * 100f;
        }
    }

    public float AlcoholUnits => TotalVolumeCl * (AlcoholPercentage / 100f);
}


