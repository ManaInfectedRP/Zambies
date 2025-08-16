using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EquipableItem : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // * GetMouseButton *
        // 0 Left Mouse
        // 1 Right Mouse
        // 2 Scroll Click
        if (Input.GetMouseButtonDown(0) && !InventorySystem.instance.isOpen && !CraftingSystem.instance.isOpen && !SelectionManager.instance.handIsVisible)
        {

            animator.SetTrigger("hit");
        }
    }

    public void GetHit()
    {
        
        GameObject selectedTree = SelectionManager.instance.selectedTree;

        if (selectedTree != null)
        {
            selectedTree.GetComponent<ChoppableTree>().GetHit();
        }
    }
}
