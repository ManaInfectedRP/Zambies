using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InventoryItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Booleans?")]
    // --- Is this item trashable --- //
    public bool isTrashable;

    [Header("Item Info")]
    // --- Item Info UI --- //
    private GameObject itemInfoUI;

    private TextMeshProUGUI itemInfoUI_itemName;
    private TextMeshProUGUI itemInfoUI_itemDescription;
    private TextMeshProUGUI itemInfoUI_itemFunctionality;

    public string thisName, thisDescription, thisFunctionality;

    [Header("Consupmtion")]
    // --- Consumption --- //
    private GameObject itemPendingConsumption;
    public bool isConsumable;

    public float healthEffect;
    public float caloriesEffect;
    public float hydrationEffect;

    [Header("Equipping")]
    public bool isEquippable;
    private GameObject itemPendingEquipping;
    public bool isInsideQuickSlot;
    public bool isSelected;

    [Header("Building")]
    public bool isUseable;

    private void Start()
    {
        if (InventorySystem.instance == null)
        {
            Debug.LogError("InventorySystem.instance is null!");
            return;
        }

        if (InventorySystem.instance.ItemInfoUI == null)
        {
            Debug.LogError("InventorySystem.instance.ItemInfoUI is null!");
            return;
        }

        itemInfoUI = InventorySystem.instance.ItemInfoUI;

        // itemName
        var nameTransform = itemInfoUI.transform.Find("itemName");
        if (nameTransform == null)
            Debug.LogError("itemName object not found under ItemInfoUI!");
        else
            itemInfoUI_itemName = nameTransform.GetComponent<TextMeshProUGUI>();

        // itemDescription
        var descTransform = itemInfoUI.transform.Find("itemDescription");
        if (descTransform == null)
            Debug.LogError("itemDescription object not found under ItemInfoUI!");
        else
            itemInfoUI_itemDescription = descTransform.GetComponent<TextMeshProUGUI>();

        // itemFunctionality
        var funcTransform = itemInfoUI.transform.Find("itemFunctionality");
        if (funcTransform == null)
            Debug.LogError("itemFunctionality object not found under ItemInfoUI!");
        else
            itemInfoUI_itemFunctionality = funcTransform.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (isSelected)
        {
            gameObject.GetComponent<DragDrop>().enabled = false;
        }
        else
        {
            gameObject.GetComponent<DragDrop>().enabled = true;
        }
    }

    // Triggered when the mouse enters into the area of the item that has this script.
    public void OnPointerEnter(PointerEventData eventData)
    {
        itemInfoUI.SetActive(true);
        itemInfoUI_itemName.text = thisName;
        itemInfoUI_itemDescription.text = thisDescription;
        itemInfoUI_itemFunctionality.text = thisFunctionality;
    }

    // Triggered when the mouse exits the area of the item that has this script.
    public void OnPointerExit(PointerEventData eventData)
    {
        itemInfoUI.SetActive(false);
    }

    // Triggered when the mouse is clicked over the item that has this script.
    public void OnPointerDown(PointerEventData eventData)
    {
        //Right Mouse Button Click on
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (isConsumable)
            {
                // Setting this specific gameobject to be the item we want to destroy later
                itemPendingConsumption = gameObject;
                consumingFunction(healthEffect, caloriesEffect, hydrationEffect);
            }

            if (isEquippable && isInsideQuickSlot == false && EquipSystem.instance.CheckIfFull() == false)
            {
                EquipSystem.instance.AddToQuickSlots(gameObject);
                isInsideQuickSlot = true;
            }

            if (isUseable)
            {
                ConstructionManager.instance.itemToBeDestroyed = gameObject;
                gameObject.SetActive(false);
                UseItem();
            }
        }
    }

    // Triggered when the mouse button is released over the item that has this script.
    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (isConsumable && itemPendingConsumption == gameObject)
            {
                DestroyImmediate(gameObject);
                InventorySystem.instance.RecalculateList();
                CraftingSystem.instance.RefreshNeededItems();
            }
        }
    }

    public void UseItem()
    {
        itemInfoUI.SetActive(false);

        InventorySystem.instance.isOpen = false;
        InventorySystem.instance.inventoryScreenUI.SetActive(false);

        CraftingSystem.instance.isOpen = false;
        CraftingSystem.instance.craftingScreenUI.SetActive(false);
        CraftingSystem.instance.survivalScreenUI.SetActive(false);
        CraftingSystem.instance.refineScreenUI.SetActive(false);
        CraftingSystem.instance.constructionScreenUI.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SelectionManager.instance.EnableSelection();
        SelectionManager.instance.enabled = true;

        switch (gameObject.name)
        {
            case "WoodFoundation(Clone)":
                ConstructionManager.instance.ActivateConstructionPlacement("WoodFoundationModel");
                break;  
            case "WoodFoundation":
                ConstructionManager.instance.ActivateConstructionPlacement("WoodFoundationModel");
                break;

            default:
                break;
        }
    }

    private void consumingFunction(float healthEffect, float caloriesEffect, float hydrationEffect)
    {
        itemInfoUI.SetActive(false);

        healthEffectCalculation(healthEffect);

        caloriesEffectCalculation(caloriesEffect);

        hydrationEffectCalculation(hydrationEffect);

    }
    private static void healthEffectCalculation(float healthEffect)
    {
        // --- Health --- //

        float healthBeforeConsumption = PlayerState.instance.currentHealth;
        float maxHealth = PlayerState.instance.maxHealth;

        if (healthEffect != 0)
        {
            if ((healthBeforeConsumption + healthEffect) > maxHealth)
            {
                PlayerState.instance.setHealth(maxHealth);
            }
            else
            {
                PlayerState.instance.setHealth(healthBeforeConsumption + healthEffect);
            }
        }
    }
    private static void caloriesEffectCalculation(float caloriesEffect)
    {
        // --- Calories --- //

        float caloriesBeforeConsumption = PlayerState.instance.currentCalories;
        float maxCalories = PlayerState.instance.maxCalories;

        if (caloriesEffect != 0)
        {
            if ((caloriesBeforeConsumption + caloriesEffect) > maxCalories)
            {
                PlayerState.instance.setCalories(maxCalories);
            }
            else
            {
                PlayerState.instance.setCalories(caloriesBeforeConsumption + caloriesEffect);
            }
        }
    }
    private static void hydrationEffectCalculation(float hydrationEffect)
    {
         // --- Hydration --- //

        float hydrationBeforeConsumption = PlayerState.instance.currentHydrationPercent;
        float maxHydration = PlayerState.instance.maxHydrationPercent;

        if (hydrationEffect != 0)
        {
            if ((hydrationBeforeConsumption + hydrationEffect) > maxHydration)
            {
                PlayerState.instance.setHydration(maxHydration);
            }
            else
            {
                PlayerState.instance.setHydration(hydrationBeforeConsumption + hydrationEffect);
            }
        }
    }


}