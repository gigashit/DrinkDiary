using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DrinkManager : MonoBehaviour
{
    public static DrinkManager Instance;

    private string savePath => Path.Combine(Application.persistentDataPath, "drinks.json");

    public List<Drink> savedDrinks = new();

    private SessionManager sessionManager;

    void Awake()
    {
        sessionManager = FindFirstObjectByType<SessionManager>();

        if (Instance == null)
        {
            Instance = this;
            LoadDrinks();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddDrink(Drink newDrink)
    {
        savedDrinks.Add(newDrink);
        SaveDrinks();
    }

    public void AddDrinkToSession(Drink chosenDrink)
    {
        var entry = new DrinkEntry
        {
            drinkName = chosenDrink.name,
            alcoholUnits = chosenDrink.AlcoholUnits,
            consumedAt = System.DateTime.Now
        };

        sessionManager.AddDrink(entry);
    }


    public List<Drink> GetDrinksSortedAlphabetically()
    {
        List<Drink> copy = new List<Drink>(savedDrinks);
        copy.Sort((a, b) => a.name.CompareTo(b.name));
        return copy;
    }

    public Drink GetDrinkByName(string name)
    {
        return savedDrinks.Find(d => d.name == name);
    }

    private void SaveDrinks()
    {
        string json = JsonUtility.ToJson(new DrinkListWrapper(savedDrinks));
        File.WriteAllText(savePath, json);
    }

    private void LoadDrinks()
    {
        if (!File.Exists(savePath)) return;

        string json = File.ReadAllText(savePath);
        DrinkListWrapper wrapper = JsonUtility.FromJson<DrinkListWrapper>(json);
        savedDrinks = wrapper.drinks;
    }

    [System.Serializable]
    private class DrinkListWrapper
    {
        public List<Drink> drinks;

        public DrinkListWrapper(List<Drink> list)
        {
            drinks = list;
        }
    }
}
