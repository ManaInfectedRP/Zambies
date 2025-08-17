using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class CraftingSystem : MonoBehaviour
{
    public GameObject craftingScreenUI;
    public GameObject toolsScreenUI,survivalScreenUI,refineScreenUI;

    public List<string> inventoryItemList = new List<string>();

    //Category Buttons
    Button toolsBTN, survivalBTN, refineBTN;

    //Craft Buttons
    Button craftAxeBTN, craftPlankBTN;

    //Requirement Texts
    TextMeshProUGUI axeReq1, axeReq2, plankReq1;

    public bool isOpen;

    //All Blueprints
    public Blueprint axeBlueprint = new Blueprint("Axe", 1, 2, "Stone", 3, "Stick", 2);
    public Blueprint plankBlueprint = new Blueprint("Plank", 2, 1, "Log", 1, "Stick", 0);


    public static CraftingSystem instance { get; set; }
    void Awake()
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
        Cursor.visible = false;

        toolsBTN = craftingScreenUI.transform.Find("ToolsButton").GetComponent<Button>();
        toolsBTN.onClick.AddListener(delegate { OpenToolsCategory(); });

        survivalBTN = craftingScreenUI.transform.Find("SurvivalButton").GetComponent<Button>();
        survivalBTN.onClick.AddListener(delegate { OpenSurvivalCategory(); });

        refineBTN = craftingScreenUI.transform.Find("RefinedButton").GetComponent<Button>();
        refineBTN.onClick.AddListener(delegate { OpenRefineCategory(); });
        // AXE
        axeReq1 = toolsScreenUI.transform.Find("Axe").transform.Find("Req1").GetComponent<TextMeshProUGUI>();
        axeReq2 = toolsScreenUI.transform.Find("Axe").transform.Find("Req2").GetComponent<TextMeshProUGUI>();

        craftAxeBTN = toolsScreenUI.transform.Find("Axe").transform.Find("Button").GetComponent<Button>();
        craftAxeBTN.onClick.AddListener(delegate { CraftAnyItem(axeBlueprint); });

        // Plank
        plankReq1 = refineScreenUI.transform.Find("Plank").transform.Find("Req1").GetComponent<TextMeshProUGUI>();
        
        craftPlankBTN = refineScreenUI.transform.Find("Plank").transform.Find("Button").GetComponent<Button>();
        craftPlankBTN.onClick.AddListener(delegate { CraftAnyItem(plankBlueprint); });
    }

    void OpenToolsCategory()
    {
        craftingScreenUI.SetActive(false);
        toolsScreenUI.SetActive(true);
        refineScreenUI.SetActive(false);
        survivalScreenUI.SetActive(false);
    }

    void OpenSurvivalCategory()
    {
        craftingScreenUI.SetActive(false);
        toolsScreenUI.SetActive(false);
        refineScreenUI.SetActive(false);
        survivalScreenUI.SetActive(true);
    }
    
    void OpenRefineCategory()
    {
        craftingScreenUI.SetActive(false);
        toolsScreenUI.SetActive(false);
        refineScreenUI.SetActive(true);
        survivalScreenUI.SetActive(false);
    }

    void CraftAnyItem(Blueprint blueprintToCraft)
    {
        SoundManager.instance.PlaySound(SoundManager.instance.craftingSound);

        StartCoroutine(CraftedDelayedForSound(blueprintToCraft));

        //remove resources from inventory
        if (blueprintToCraft.numOfRequirements == 1)
        {
            InventorySystem.instance.RemoveItem(blueprintToCraft.req1, blueprintToCraft.req1Amount);
        }
        else if (blueprintToCraft.numOfRequirements == 2)
        {
            InventorySystem.instance.RemoveItem(blueprintToCraft.req1, blueprintToCraft.req1Amount);
            InventorySystem.instance.RemoveItem(blueprintToCraft.req2, blueprintToCraft.req2Amount);
        }

        //refresh list
        StartCoroutine(calculate());
    }

    public IEnumerator CraftedDelayedForSound(Blueprint blueprintToCraft)
    {
        yield return new WaitForSeconds(1f);
        /*
            for each numberOfItemsToProduce in Blueprint
            run the Loop and AddToInventory
        */
        for (int i = 0; i < blueprintToCraft.numberOfItemsToProduce; i++)
        {
            //add item into inventory
            InventorySystem.instance.AddToInventory(blueprintToCraft.itemName);
        }
    }

    public IEnumerator calculate()
    {
        yield return 0;
        InventorySystem.instance.RecalculateList();
        RefreshNeededItems();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && !isOpen)
        {

            craftingScreenUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;

            SelectionManager.instance.DisableSelection();
            SelectionManager.instance.GetComponent<SelectionManager>().enabled = false;
            
            Cursor.visible = true;
            isOpen = true;

        }
        else if (Input.GetKeyDown(KeyCode.C) && isOpen)
        {
            craftingScreenUI.SetActive(false);
            toolsScreenUI.SetActive(false);
            survivalScreenUI.SetActive(false);
            refineScreenUI.SetActive(false);

            if (!InventorySystem.instance.isOpen)
                Cursor.lockState = CursorLockMode.Locked;
            
            SelectionManager.instance.EnableSelection();
            SelectionManager.instance.GetComponent<SelectionManager>().enabled = true;

            Cursor.visible = false;
            isOpen = false;
        }
    }


    public void RefreshNeededItems()
    {
        int stone_count = 0;
        int stick_count = 0;
        int log_count = 0;

        inventoryItemList = InventorySystem.instance.itemList;

        foreach (string itemName in inventoryItemList)
        {
            switch (itemName)
            {
                case "Stone":
                    stone_count++;
                    break;
                case "Stick":
                    stick_count++;
                    break;
                case "Log":
                    log_count++;
                    break;
            }
        }

        //--AXE--
        axeReq1.text = "3x Stone [" + stone_count + "]";
        axeReq2.text = "2x Stick [" + stick_count + "]";

        if (stone_count >= 3 && stick_count >= 2 && InventorySystem.instance.CheckSlotsAvailable(1))
        {
            craftAxeBTN.gameObject.SetActive(true);
        }
        else
        {
            craftAxeBTN.gameObject.SetActive(false);
        }
        
        //--Plank--
        plankReq1.text = "1x Log [" + log_count + "]";

        if (log_count >= 1 && InventorySystem.instance.CheckSlotsAvailable(2))
        {
            craftPlankBTN.gameObject.SetActive(true);
        }
        else
        {
            craftPlankBTN.gameObject.SetActive(false);
        }
    }
}
