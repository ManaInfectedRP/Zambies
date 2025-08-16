using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ChoppableTree : MonoBehaviour
{

    public Animator animator;

    [Header("Booleans?")]
    public bool playerInRange;
    public bool canBeChopped;


    [Header("Variables")]
    public float treeHealth;
    public float treeMaxHealth;

    public float carloriesSpentChoppingWood = 5;

    void Start()
    {
        treeHealth = treeMaxHealth;
        animator = transform.parent.transform.parent.GetComponent<Animator>();
    }

    void Update()
    {
        if (canBeChopped)
        {
            GlobalState.instance.resourceHealth = treeHealth;
            GlobalState.instance.resourceMaxHealth = treeMaxHealth;
        }

        if (treeHealth <= 0)
        {
            TreeIsDead();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void GetHit()
    {
        animator.SetTrigger("shake");

        treeHealth -= 1;
        PlayerState.instance.currentCalories -= carloriesSpentChoppingWood;

        if (treeHealth <= 0)
        {
            TreeIsDead();
        }
    }

    void TreeIsDead()
    {
        Vector3 treePosition = transform.position;

        Destroy(transform.parent.transform.parent.gameObject);
        canBeChopped = false;
        SelectionManager.instance.selectedTree = null;
        SelectionManager.instance.chopHolder.gameObject.SetActive(false);

        GameObject brokenTree = Instantiate(Resources.Load<GameObject>("ChoppedTree"), treePosition, Quaternion.Euler(0, 0, 0));
    }
}
