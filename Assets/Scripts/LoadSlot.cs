using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class LoadSlot : MonoBehaviour
{
    private Button button;
    private TextMeshProUGUI buttonText;

    public int slotNumber;

    void Awake()
    {
        button = GetComponent<Button>();
        buttonText = transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
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

    public void Start()
    {
        button.onClick.AddListener(() =>
        {
            if (SaveManager.instance.isSlotEmpty(slotNumber) == false)
            {
                SaveManager.instance.StartLoadedGame(slotNumber);
                SaveManager.instance.DeSelectButton();
            }
            else
            {
                // Do Nothing
            }
        });
    }
}
