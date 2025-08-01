using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class IngredientEntry : MonoBehaviour
{
    public TMP_InputField nameField;
    public TMP_InputField volumeField;
    public TMP_InputField percentageField;
    public TMP_Text servingsText;
    public Button removeButton;

    [HideInInspector] public string storedName;
    [HideInInspector] public string storedVolume;
    [HideInInspector] public string storedPercentage;
    [HideInInspector] public DrinkIngredient thisIngredient = new DrinkIngredient();
    [HideInInspector] public bool isValid = false;
    [HideInInspector] public ingredientErrorMessage errorMessage;

    [HideInInspector] public MainUIScript mainUIScript;
    [HideInInspector] public DrinkSelectorUI drinkSelectorUI;

    private bool isValidVolume = false;
    private bool isValidPercentage = false;

    private void Start()
    {
        nameField.onValueChanged.AddListener(CheckIfValuesValid);
        volumeField.onValueChanged.AddListener(CheckIfValuesValid);
        percentageField.onValueChanged.AddListener(CheckIfValuesValid);
        removeButton.onClick.AddListener(Remove);
    }

    void CheckIfValuesValid(string empty)
    {
        if (nameField.text != "" && volumeField.text != "" && percentageField.text != "")
        {
            storedName = nameField.text;

            isValidVolume = drinkSelectorUI.CheckIfValidVolume(volumeField.text);
            isValidPercentage = drinkSelectorUI.CheckIfValidPercentage(percentageField.text);

            if (isValidVolume)
            {
                storedVolume = volumeField.text;
            }

            if (isValidPercentage)
            {
                storedPercentage =  percentageField.text;
                storedPercentage.Replace(",", ".");
            }

            if (isValidVolume && isValidPercentage)
            {
                thisIngredient.name = storedName;
                thisIngredient.amountCl = int.Parse(storedVolume);
                thisIngredient.alcoholPercent = float.Parse(storedPercentage);

                drinkSelectorUI.UpdateTotalAmount();
                drinkSelectorUI.UpdateTotalPercentage();
                servingsText.text = drinkSelectorUI.GetServingsAmount(int.Parse(storedVolume), float.Parse(storedPercentage)).ToString();

                isValid = true;
            }
            else
            {
                isValid = false;
                errorMessage = ingredientErrorMessage.InvalidValues;
            }
        }
        else
        {
            isValid = false;
            errorMessage = ingredientErrorMessage.EmptyFields;
        }
    }

    void Remove()
    {
        drinkSelectorUI.RemoveIngredientEntry(this.gameObject);
    }

}


public enum ingredientErrorMessage
{
    EmptyFields,
    InvalidValues
}