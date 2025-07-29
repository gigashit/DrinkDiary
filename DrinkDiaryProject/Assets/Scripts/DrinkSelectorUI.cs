using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DrinkSelectorUI : MonoBehaviour
{
    [Header("UI References")]
    public Button selectDrinkButton;
    public Button openDrinkCreationButton;
    public Button addDrinkToSessionButton;
    public GameObject noSavedDrinksWarning;
    public GameObject dropdownPanelBG;
    public Transform dropdownContentParent;
    public GameObject drinkItemButtonPrefab;
    public TextMeshProUGUI volumeText;
    public TextMeshProUGUI percentText;
    public TextMeshProUGUI servingText;

    [Header("Dynamic Sizing")]
    public RectTransform dropdownPanel;           // Panel that will resize
    public int maxVisibleItems = 8;
    public float itemHeight = 140f;                // Match LayoutElement


    private Drink selectedDrink;

    private DrinkManager drinkManager;
    private MainUIScript mainUIScript;

    private const float alcoholDensityValue = 0.06575f;

    void Start()
    {
        noSavedDrinksWarning.SetActive(false);
        selectDrinkButton.onClick.AddListener(ToggleDropdown);
        addDrinkToSessionButton.onClick.AddListener(AddDrinkToSession);
        dropdownPanelBG.SetActive(false);
        StartCoroutine(DelayedPopulateDropdown());

        drinkManager = FindFirstObjectByType<DrinkManager>();
        mainUIScript = FindFirstObjectByType<MainUIScript>();

    }

    void ToggleDropdown()
    {
        dropdownPanelBG.SetActive(!dropdownPanelBG.activeSelf);
    }

    void PopulateDropdown()
    {
        if (drinkManager.savedDrinks.Count == 0)
        {
            selectDrinkButton.gameObject.SetActive(false);
            noSavedDrinksWarning.SetActive(true);
            return;
        }

        // Clear old buttons
        foreach (Transform child in dropdownContentParent)
            Destroy(child.gameObject);

        List<Drink> drinks = drinkManager.GetDrinksSortedAlphabetically();

        foreach (var drink in drinks)
        {
            GameObject buttonObj = Instantiate(drinkItemButtonPrefab, dropdownContentParent);
            TextMeshProUGUI label = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            label.text = drink.name;

            Button btn = buttonObj.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                SelectDrink(drink);
            });
        }

        // Optionally auto-select the first drink
        SelectDrink(drinks[0]);

        ResizeDropdown(drinks.Count);
    }

    private IEnumerator DelayedPopulateDropdown()
    {
        while (!drinkManager.drinksLoaded)
        {
            yield return new WaitForSeconds(0.5f);
        }

        PopulateDropdown();
    }

    void ResizeDropdown(int itemCount)
    {
        float targetHeight;

        ScrollRect scrollRect = dropdownPanel.GetComponent<ScrollRect>();

        if (itemCount <= maxVisibleItems)
        {
            targetHeight = itemCount * itemHeight;
            scrollRect.vertical = false;
        }
        else
        {
            targetHeight = maxVisibleItems * itemHeight;
            scrollRect.vertical = true;
        }

        // Resize the DropdownPanel
        var size = dropdownPanel.sizeDelta;
        size.y = targetHeight;
        dropdownPanel.sizeDelta = size;
    }


    void SelectDrink(Drink drink)
    {
        selectedDrink = drink;

        // Update main button label
        selectDrinkButton.GetComponentInChildren<TextMeshProUGUI>().text = drink.name;

        // Update stat fields
        volumeText.text = drink.TotalVolumeCl.ToString();
        percentText.text = drink.AlcoholPercentage.ToString();
        servingText.text = GetServingsAmount(drink.TotalVolumeCl, drink.AlcoholPercentage).ToString();

        dropdownPanelBG.SetActive(false);
    }

    void AddDrinkToSession()
    {
        drinkManager.AddDrinkToSession(selectedDrink);
        mainUIScript.CloseDrinkSelectionPanel();
    }

    public float GetServingsAmount(float vol, float per)
    {
        return Mathf.Round(vol * per * alcoholDensityValue) * 0.1f;
    }


    public Drink GetSelectedDrink()
    {
        return selectedDrink;
    }
}
