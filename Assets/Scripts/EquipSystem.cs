using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipSystem : MonoBehaviour
{
    public static EquipSystem instance { get; set; }

    // -- UI -- //
    [Header("UI")]
    public GameObject quickSlotsPanel;

    public List<GameObject> quickSlotsList = new List<GameObject>();

    [Header("Numbers")]
    public GameObject numbersHolder;
    public int selectedNumber = -1;
    public GameObject selectedItem;

    [Header("Tools")]
    public GameObject toolHolder;
    public GameObject selectedItemModel;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }


    private void Start()
    {
        PopulateSlotList();
    }

    void Update()
    {
        for (int i = 1; i <= 7; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + (i - 1)))
            {
                HandleKeyInput(i);
                break;
            }
        }
    }

    void HandleKeyInput(int keyNumber)
    {
        switch (keyNumber)
        {
            case 1:
                SelectQuickSlot(1);
                break;
            case 2:
                SelectQuickSlot(2);
                break;
            case 3:
                SelectQuickSlot(3);
                break;
            case 4:
                SelectQuickSlot(4);
                break;
            case 5:
                SelectQuickSlot(5);
                break;
            case 6:
                SelectQuickSlot(6);
                break;
            case 7:
                SelectQuickSlot(7);
                break;
            default:
                Debug.Log("Unknown key");
                break;
        }
    }

    void SelectQuickSlot(int number)
    {
        if (checkIfSlotIsFull(number) == true)
        {
            if (selectedNumber != number)
            {
                selectedNumber = number;

                // Unselect previously selected Item
                if (selectedItem != null)
                {
                    selectedItem.gameObject.GetComponent<InventoryItem>().isSelected = false;
                }

                selectedItem = GetSelectedItem(number);
                selectedItem.GetComponent<InventoryItem>().isSelected = true;

                SetEquippedModel(selectedItem);

                // Changing Color
                foreach (Transform child in numbersHolder.transform)
                {
                    child.transform.Find("Text").GetComponent<TextMeshProUGUI>().color = Color.gray;
                }

                TextMeshProUGUI toBeChanged = numbersHolder.transform.Find("number" + number).transform.Find("Text").GetComponent<TextMeshProUGUI>();
                toBeChanged.color = Color.white;
            }
            else // We are Trying to select the same {Slot}
            {
                selectedNumber = -1; //Null?
                if (selectedItem != null)
                {
                    selectedItem.gameObject.GetComponent<InventoryItem>().isSelected = false;
                    selectedItem = null;
                }

                if (selectedItemModel != null)
                {
                    DestroyImmediate(selectedItemModel.gameObject);
                    selectedItemModel = null;
                }

                // Changing Color
                foreach (Transform child in numbersHolder.transform)
                {
                    child.transform.Find("Text").GetComponent<TextMeshProUGUI>().color = Color.gray;
                }
            }
        }
    }

    void SetEquippedModel(GameObject selectedItem)
    {
        if (selectedItemModel != null)
        {
            DestroyImmediate(selectedItemModel.gameObject);
            selectedItemModel = null;
        }

        string selectedItemName = selectedItem.name.Replace("(Clone)", "");
        selectedItemModel = Instantiate(Resources.Load<GameObject>(selectedItemName + "_Model"),
         new Vector3(0.95f, 0.77f, 1.08f), Quaternion.Euler(28.72f, -15f, 0f));
        selectedItemModel.transform.SetParent(toolHolder.transform, false);
    }

    bool checkIfSlotIsFull(int slotNumber)
    {
        if (quickSlotsList[slotNumber-1].transform.childCount > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    GameObject GetSelectedItem(int number)
    {
        return quickSlotsList[number - 1].transform.GetChild(0).gameObject;
    }

    private void PopulateSlotList()
    {
        foreach (Transform child in quickSlotsPanel.transform)
        {
            if (child.CompareTag("QuickSlot"))
            {
                quickSlotsList.Add(child.gameObject);
            }
        }
    }

    public void AddToQuickSlots(GameObject itemToEquip)
    {
        // Find next free slot
        GameObject availableSlot = FindNextEmptySlot();
        // Set transform of our object
        itemToEquip.transform.SetParent(availableSlot.transform, false);

        InventorySystem.instance.RecalculateList();
    }


    private GameObject FindNextEmptySlot()
    {
        foreach (GameObject slot in quickSlotsList)
        {
            if (slot.transform.childCount == 0)
            {
                return slot;
            }
        }
        return new GameObject();
    }

    public bool CheckIfFull()
    {
        int counter = 0;

        foreach (GameObject slot in quickSlotsList)
        {
            if (slot.transform.childCount > 0)
            {
                counter += 1;
            }
        }

        if (counter == 7)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}