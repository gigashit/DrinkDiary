using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DrinkManager : MonoBehaviour
{
    public static DrinkManager Instance;

    private string savePath => Path.Combine(Application.persistentDataPath, "drinks.json");

    public List<Drink> savedDrinks = new();

    [Header("Script References")]
    [SerializeField] private SessionManager sessionManager;
    [SerializeField] private DrinkSelectorUI drinkSelectorUI;

    public bool drinksLoaded = false;

    void Awake()
    {
        LoadDrinks();
    }

    public void AddDrink(Drink newDrink)
    {
        savedDrinks.Add(newDrink);
        Debug.Log("New drink added:" + newDrink.name);
        SaveDrinks();
    }

    public void AddDrinkToSession(Drink chosenDrink)
    {
        var entry = new DrinkEntry
        {
            drinkName = chosenDrink.name,
            serving = drinkSelectorUI.GetServingsAmount(chosenDrink.TotalVolumeCl, chosenDrink.AlcoholPercentage)
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

        drinksLoaded = true;
    }

    public void ClearDrinkData()
    {
        if (File.Exists(savePath)) { File.Delete(savePath); }
        savedDrinks.Clear();

        Debug.Log("Drink data cleared.");
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
