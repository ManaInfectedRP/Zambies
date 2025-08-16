using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Slider slider;
    public TextMeshProUGUI healthCounter;

    public GameObject playerState;

    private float currentHealth;
    private float maxHealth;

    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    void Update()
    {
        currentHealth = playerState.GetComponent<PlayerState>().currentHealth;
        maxHealth = playerState.GetComponent<PlayerState>().maxHealth;

        float fillValue = currentHealth / maxHealth; // * Runs health slider between 0 - 1 ( 100 currentHealth/100 maxHealth )
        slider.value = fillValue;

        healthCounter.text = currentHealth + "/" + maxHealth; // * Shows something like 95/100
    }
}
