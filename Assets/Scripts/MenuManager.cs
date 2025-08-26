using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance { get; set; }
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

    public GameObject menuCanavas;
    public GameObject uiCanvas;
    public GameObject saveMenu;
    public GameObject settingsMenu;
    public GameObject menu;

    public bool isMenuOpen = false;
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.M) && !isMenuOpen)
        {
            Debug.Log("Escape key pressed - toggle menu");
            // Implement menu toggle logic here

            menuCanavas.SetActive(true);
            uiCanvas.SetActive(false);
            isMenuOpen = true;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            SelectionManager.instance.DisableSelection();
            SelectionManager.instance.GetComponent<SelectionManager>().enabled = false;

        }
        else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.M) && isMenuOpen)
        {
            Debug.Log("Escape key pressed - toggle menu");
            // Implement menu toggle logic here
            saveMenu.SetActive(false);
            settingsMenu.SetActive(false);
            menu.SetActive(true);

            menuCanavas.SetActive(false);
            uiCanvas.SetActive(true);
            isMenuOpen = false;

            if (!InventorySystem.instance.isOpen && !CraftingSystem.instance.isOpen)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            
            SelectionManager.instance.EnableSelection();
            SelectionManager.instance.GetComponent<SelectionManager>().enabled = true;
        }
    }
}
