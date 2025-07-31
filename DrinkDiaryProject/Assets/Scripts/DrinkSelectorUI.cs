using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

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
    public GameObject drinkCreationPanel;
    public Button yesButton;
    public Button noButton;
    public GameObject step1Object;
    public Button backFromCreationButton;
    public TMP_Text creationErrorText;

    [Header("Drink Creation Single Step Elements")]
    public GameObject step2noObject;
    public TMP_InputField step2noNameField;
    public TMP_InputField step2noVolumeField;
    public TMP_InputField step2noPercentageField;
    public TMP_Text step2noServingText;
    public Button step2noAddDrinkButton;

    [Header("Drink Creation Multi Step Elements")]
    public GameObject step2yesObject;

    [Header("Dynamic Sizing")]
    public RectTransform dropdownPanel;           // Panel that will resize
    public int maxVisibleItems = 8;
    public float itemHeight = 140f;                // Match LayoutElement

    [Header("Script References")]
    [SerializeField] private DrinkManager drinkManager;
    [SerializeField] private MainUIScript mainUIScript;

    private string tempName;
    private int tempAmount;
    private float tempPerc;
    private bool dropdownOpen = false;

    private Drink selectedDrink;

    private Drink newCreatedDrink;
    private Drink lastSelectedDrink;

    private const float alcoholDensityValue = 0.06575f;

    void Start()
    {
        noSavedDrinksWarning.SetActive(false);
        creationErrorText.gameObject.SetActive(false);
        backFromCreationButton.gameObject.SetActive(false);

        dropdownPanelBG.SetActive(false);
        StartCoroutine(DelayedPopulateDropdown());
        AddButtonListeners();
        ResetCreationUI();
    }

    void AddButtonListeners()
    {
        selectDrinkButton.onClick.AddListener(ToggleDropdown);
        addDrinkToSessionButton.onClick.AddListener(AddDrinkToSession);
        openDrinkCreationButton.onClick.AddListener(OpenCreationPanel);
        yesButton.onClick.AddListener(OpenStep2YesPanel);
        noButton.onClick.AddListener(OpenStep2NoPanel);
        step2noNameField.onValueChanged.AddListener(CheckIfValidValuesStep2No);
        step2noVolumeField.onValueChanged.AddListener(CheckIfValidValuesStep2No);
        step2noPercentageField.onValueChanged.AddListener(CheckIfValidValuesStep2No);
        step2noAddDrinkButton.onClick.AddListener(ConfirmSingleDrink);
        backFromCreationButton.onClick.AddListener(ReturnFromDrinkSelection);

        Button dropDownPanelBGButton = dropdownPanelBG.GetComponent<Button>();
        dropDownPanelBGButton.onClick.AddListener(ToggleDropdown);
    }

    void ReturnFromDrinkSelection()
    {
        ResetCreationUI();
        mainUIScript.CloseDrinkSelectionPanel();
    }

    public void ResetCreationUI()
    {
        step2noServingText.text = "";
        step2noPercentageField.text = "";
        step2noVolumeField.text = "";
        step2noNameField.text = "";
        step2noObject.SetActive(false);

        step1Object.SetActive(true);
        drinkCreationPanel.SetActive(false);
    }

    void ConfirmSingleDrink()
    {
        string cleadedVolume = step2noVolumeField.text.Replace(',', '.');
        string cleanedPercentage = step2noPercentageField.text.Replace(',', '.');

        if (CheckIfValidVolume(cleadedVolume) && CheckIfValidPercentage(cleanedPercentage))
        {
            int parsedVolume = int.Parse(step2noVolumeField.text);
            float parcedPercentage = float.Parse(cleanedPercentage);

            var ingredient = new DrinkIngredient
            {
                name = step2noNameField.text,
                amountCl = parsedVolume,
                alcoholPercent = parcedPercentage
            };

            var drink = new Drink
            {
                name = ingredient.name,
                ingredients = new List<DrinkIngredient>()
            };

            drink.ingredients.Add(ingredient);

            drinkManager.AddDrink(drink);
            newCreatedDrink = drink;
            ResetCreationUI();
            PopulateDropdown();
            creationErrorText.gameObject.SetActive(false);
        }
        else
        {
            ShowError(CheckIfValidVolume(cleadedVolume), CheckIfValidPercentage(cleanedPercentage));
        }
    }

    void ShowError(bool volOK, bool pctOK)
    {
        var stringTable = LocalizationSettings
            .StringDatabase
            .GetTable("CreationErrors");

        string key;
        if (!volOK && !pctOK) key = "error_wrongvolandperc";
        else if (!volOK) key = "error_wrongvol";
        else /* !pctOK */        key = "error_wrongperc";

        // this will return the localized value for the currently selected locale
        string localized = stringTable.GetEntry(key).GetLocalizedString();
        creationErrorText.gameObject.SetActive(true);
        creationErrorText.text = localized;
    }

    void CheckIfValidValuesStep2No(string empty)
    {
        if (step2noNameField.text == "" || step2noVolumeField.text == "" || step2noPercentageField.text == "")
        {
            step2noAddDrinkButton.interactable = false;
        }
        else
        {
            step2noAddDrinkButton.interactable = true;

            int parsedVolume = int.Parse(step2noVolumeField.text);
            string cleanedPercentage = step2noPercentageField.text.Replace(',', '.');
            float parcedPercentage = float.Parse(cleanedPercentage);

            step2noServingText.text = GetServingsAmount((float)parsedVolume, parcedPercentage).ToString();
        }
    }

    public bool CheckIfValidVolume(string input)
    {
        if (int.TryParse(input, out int number))
        {
            return number >= 1 && number <= 999;
        }
        return false;
    }

    public bool CheckIfValidPercentage(string input)
    {
        if (float.TryParse(input, System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture, out float number))
        {
            return number >= 0f && number <= 99.9f;
        }
        return false;
    }

    void OpenCreationPanel()
    {
        step2noObject.SetActive(false);
        step2yesObject.SetActive(false);

        drinkCreationPanel.SetActive(true);
        step1Object.SetActive(true);
        backFromCreationButton.gameObject.SetActive(true);
    }

    void OpenStep2NoPanel()
    {
        step1Object.SetActive(false);
        step2noObject.SetActive(true);

        CheckIfValidValuesStep2No(null);
    }

    void OpenStep2YesPanel()
    {
        step1Object.SetActive(false);
        step2yesObject.SetActive(true);
    }

    void ToggleDropdown()
    {
        dropdownPanelBG.SetActive(!dropdownOpen);
        dropdownOpen = !dropdownOpen;
        Debug.Log("Toggling dropdown... Dropdown is now " + dropdownOpen);
    }

    void PopulateDropdown()
    {
        Debug.Log("Populating dropdown menu...");

        if (drinkManager.savedDrinks.Count == 0)
        {
            Debug.Log("No saved drinks detected");
            selectDrinkButton.gameObject.SetActive(false);
            noSavedDrinksWarning.SetActive(true);
            return;
        }
        else
        {
            selectDrinkButton.gameObject.SetActive(true);
            noSavedDrinksWarning.SetActive(false);
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
                SelectDrink(drink, false);
            });
        }

        if (newCreatedDrink != null)
        {
            SelectDrink(newCreatedDrink, true);
            newCreatedDrink = null;
        }
        else if (lastSelectedDrink != null)
        {
            SelectDrink(lastSelectedDrink, true);
        }
        else
        {
            SelectDrink(drinks[0], true);
        }

        ResizeDropdown(drinks.Count);
    }

    private IEnumerator DelayedPopulateDropdown()
    {
        yield return new WaitForSeconds(0.5f);

        if (drinkManager.savedDrinks.Count > 0)
        {
            while (!drinkManager.drinksLoaded)
            {
                yield return new WaitForSeconds(0.5f);
            }
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


    void SelectDrink(Drink drink, bool isAutomatic)
    {
        selectedDrink = drink;

        // Update main button label
        selectDrinkButton.GetComponentInChildren<TextMeshProUGUI>().text = drink.name;

        // Update stat fields
        volumeText.text = drink.TotalVolumeCl.ToString();
        percentText.text = drink.AlcoholPercentage.ToString();
        servingText.text = GetServingsAmount(drink.TotalVolumeCl, drink.AlcoholPercentage).ToString();

        if (!isAutomatic)
        {
            ToggleDropdown();
        }
    }

    void AddDrinkToSession()
    {
        drinkManager.AddDrinkToSession(selectedDrink);
        lastSelectedDrink = selectedDrink;
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
