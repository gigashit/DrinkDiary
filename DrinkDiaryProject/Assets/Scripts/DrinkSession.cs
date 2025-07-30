using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DrinkSession
{
    public string sessionName;
    public string startTimeString; // store time as string
    [NonSerialized] public DateTime startTime; // not serialized
    public List<DrinkEntry> drinks = new List<DrinkEntry>();
    public string notes;

    public bool IsOngoing;
}

[System.Serializable]
public class DrinkEntry
{
    public string drinkName;
    public float serving;
    public int orderNumber;
}

[System.Serializable]
public class DrinkIngredient
{
    public string name;
    public int amountCl;
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
            {
                if (ingredient.alcoholPercent > 0)
                {
                    total += ingredient.amountCl;
                }
            }

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
            {
                alcoholAmount += i.amountCl * (i.alcoholPercent / 100f);
            }

            return (alcoholAmount / totalVolume) * 100f;
        }
    }
}


