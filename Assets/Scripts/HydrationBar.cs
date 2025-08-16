using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HydrationBar : MonoBehaviour
{
    private Slider slider;
    public TextMeshProUGUI hydrationCounter;

    public GameObject playerState;

    private float currenthydration;
    private float maxhydration;

    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    void Update()
    {
        currenthydration = playerState.GetComponent<PlayerState>().currentHydrationPercent;
        maxhydration = playerState.GetComponent<PlayerState>().maxHydrationPercent;

        float fillValue = currenthydration / maxhydration; // * Runs hydration slider between 0 - 1 ( 100 currenthydration/100 maxhydration )
        slider.value = fillValue;

        hydrationCounter.text = currenthydration + "%"; // * Shows something like 95/100
    }
}
