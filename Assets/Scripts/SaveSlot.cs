using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SaveSlot : MonoBehaviour
{
    private Button button;
    private TextMeshProUGUI buttonText;

    public int slotNumber;

    public GameObject alertUI;
    Button yesButton;
    Button noButton;

    void Awake()
    {
        button = GetComponent<Button>();
        buttonText = transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();

        yesButton = alertUI.transform.Find("YesButton").GetComponent<Button>();
        noButton = alertUI.transform.Find("NoButton").GetComponent<Button>();
    }

    public void Start()
    {
        button.onClick.AddListener(() =>
        {
            if (SaveManager.instance.isSlotEmpty(slotNumber))
            {
                SaveGameConfirmed();
            }
            else
            {
                // Display Override Warning
                DisplayOverrideWarning();
            }
        });
    }

    void Update()
    {
        if (SaveManager.instance.isSlotEmpty(slotNumber))
        {
            buttonText.text = "Empty";
        }
        else
        {
            buttonText.text = PlayerPrefs.GetString("Slot" + slotNumber + "Description");
        }
    }

    public void DisplayOverrideWarning()
    {
        alertUI.SetActive(true);
        yesButton.onClick.AddListener(() =>
        {
            SaveGameConfirmed();

            alertUI.SetActive(false);
        });

        noButton.onClick.AddListener(() =>
        {
            alertUI.SetActive(false);
        });
    }

    public void SaveGameConfirmed()
    {
        SaveManager.instance.SaveGame(slotNumber);

        DateTime dt = DateTime.Now;
        string time = dt.ToString("yyyy-MM-dd HH:mm");

        string description = "Saved Game :" + slotNumber + " | " + time;
        buttonText.text = description;

        PlayerPrefs.SetString("Slot" + slotNumber + "Description", description);

        SaveManager.instance.DeSelectButton();
        
    }
}
