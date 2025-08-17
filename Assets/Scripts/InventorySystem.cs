using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    //instance
    public static InventorySystem instance { get; set; }

    //Variables
    [Header("--|Variables|--")]
    public GameObject inventoryScreenUI;
    public GameObject ItemInfoUI;

    public List<GameObject> slotList = new List<GameObject>();
    public List<string> itemList = new List<string>();
    private GameObject itemToAdd;
    private GameObject whatSlotToEquip;

    public bool isOpen;
    //public bool isFull; // If Inventory is Full

    [Header("--|Pick-Up|--")]
    public GameObject pickupAlert;
    public TextMeshProUGUI pickupText;
    public Image pickupImage;

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


    void Start()
    {
        isOpen = false;

        PopulateSlotList();

        Cursor.visible = false;
    }

    private void PopulateSlotList()
    {
        foreach (Transform child in inventoryScreenUI.transform)
        {
            if (child.CompareTag("Slot"))
            {
                slotList.Add(child.gameObject);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && !isOpen && !ConstructionManager.instance.inConstructionMode)
        {
            Debug.Log("{Inventory} is pressed");
            inventoryScreenUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;

            SelectionManager.instance.DisableSelection();
            SelectionManager.instance.GetComponent<SelectionManager>().enabled = false;
            Cursor.visible = true;
            isOpen = true;
        }
        else if (Input.GetKeyDown(KeyCode.I) && isOpen)
        {
            inventoryScreenUI.SetActive(false);

            if (!CraftingSystem.instance.isOpen)
                Cursor.lockState = CursorLockMode.Locked;


            SelectionManager.instance.EnableSelection();
            SelectionManager.instance.GetComponent<SelectionManager>().enabled = true;

            Cursor.visible = false;
            isOpen = false;
        }
    }

    public void AddToInventory(string itemName)
    {
        SoundManager.instance.PlaySound(SoundManager.instance.pickupItemSound);

        whatSlotToEquip = FindNextEmptySlot();

        itemToAdd = Instantiate(Resources.Load<GameObject>(itemName), whatSlotToEquip.transform.position, whatSlotToEquip.transform.rotation);
        itemToAdd.transform.SetParent(whatSlotToEquip.transform);

        itemList.Add(itemName);

        TriggerPickupPopUp(itemName, itemToAdd.GetComponent<Image>().sprite);

        RecalculateList();
        CraftingSystem.instance.RefreshNeededItems();
    }

    public void TriggerPickupPopUp(string itemName, Sprite itemSprite)
    {
        pickupAlert.SetActive(true);

        pickupText.text = itemName;
        pickupImage.sprite = itemSprite;

        StartCoroutine(HidePickupAlertAfterDelay(2f));
    }

    private IEnumerator HidePickupAlertAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        pickupAlert.gameObject.SetActive(false);
    }

    private GameObject FindNextEmptySlot()
    {
        foreach (GameObject slot in slotList)
        {
            if (slot.transform.childCount == 0)
            {
                return slot;
            }
        }

        return new GameObject();
    }

    public bool CheckSlotsAvailable(int emptyNeeded)
    {
        int emptySlot = 0;
        foreach (GameObject slot in slotList)
        {
            if (slot.transform.childCount <= 0)
            {
                emptySlot += 1;
            }
        }
        if (emptySlot >= emptyNeeded)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    /*  Inventory slots = 21,
        21 - EmptyNeeded = Result,
        emptyNeeded = 1,
        21 - 1 = Result 20
    */

    public void RemoveItem(string nameToRemove, int amountToRemove)
    {
        int counter = amountToRemove;

        for (var i = slotList.Count - 1; i >= 0; i--)
        {
            if (slotList[i].transform.childCount > 0)
            {
                if (slotList[i].transform.GetChild(0).name == nameToRemove + "(Clone)" && counter != 0)
                {
                    DestroyImmediate(slotList[i].transform.GetChild(0).gameObject);
                    counter--;
                }
            }
        }

        RecalculateList();
        CraftingSystem.instance.RefreshNeededItems();
    }

    public void RecalculateList()
    {
        itemList.Clear();

        foreach (GameObject slot in slotList)
        {
            if (slot.transform.childCount > 0)
            {
                string name = slot.transform.GetChild(0).name; // "Name" (Clone)
                string str1 = "(Clone)"; // (Clone)

                string result = name.Replace(str1, ""); // "Name" (Clone) - (Clone) = "Name"

                itemList.Add(result);
            }
        }
    }
}