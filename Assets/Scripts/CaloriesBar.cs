using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CaloriesBar : MonoBehaviour
{
    private Slider slider;
    public TextMeshProUGUI caloriesCounter;

    public GameObject playerState;

    private float currentCalories;
    private float maxCalories;

    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    void Update()
    {
        currentCalories = playerState.GetComponent<PlayerState>().currentCalories;
        maxCalories = playerState.GetComponent<PlayerState>().maxCalories;

        float fillValue = currentCalories / maxCalories; // * Runs Calories slider between 0 - 1 ( 100 currentCalories/100 maxCalories )
        slider.value = fillValue;

        caloriesCounter.text = currentCalories + "/" + maxCalories; // * Shows something like 95/100
    }
}
