using TMPro;
using UnityEngine;

public class SessionDrinkEntry : MonoBehaviour
{
    [SerializeField] private TMP_Text orderNumberText;
    [SerializeField] private TMP_Text drinkNameText;
    [SerializeField] private TMP_Text servingText;

    public void Setup(int orderNum, string name, float serving)
    {
        orderNumberText.text = orderNum.ToString();
        drinkNameText.text = name;
        servingText.text = serving.ToString();
    }
}
