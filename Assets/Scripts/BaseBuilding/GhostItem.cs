using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class GhostItem : MonoBehaviour
{
    public BoxCollider solidCollider; // set manually
 
    public Renderer mRenderer;
    private Material semiTransparentMat; // Used for debug - insted of the full trasparent
    private Material fullTransparentnMat;
    private Material selectedMaterial;
 
    public bool isPlaced;
 
    // A flag for the deletion algorithm
    public bool hasSamePosition = false;
    private void Start()
    {
        mRenderer = GetComponent<Renderer>();
        // We get them from the manager, because this way the referece always exists.
        semiTransparentMat = ConstructionManager.instance.ghostSemiTransparentMat;
        fullTransparentnMat = ConstructionManager.instance.ghostFullTransparentMat;
        selectedMaterial = ConstructionManager.instance.ghostSelectedMat;
 
        mRenderer.material = fullTransparentnMat; //change to semi if in debug else full
        // We disable the solid box collider - while it is not yet placed
        // (unless we are in construction mode - see update method)
        solidCollider.enabled = false;
    }
 
    private void Update()
    {
        if (ConstructionManager.instance.inConstructionMode)
        {
            Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), ConstructionManager.instance.player.GetComponent<Collider>());
        }

        // We need the solid collider so the ray cast will detect it
        if (ConstructionManager.instance.inConstructionMode && isPlaced)
        {
            solidCollider.enabled = true;
        }
 
        if (!ConstructionManager.instance.inConstructionMode)
        {
            solidCollider.enabled = false;
        }
 
        // Triggering the material
        if(ConstructionManager.instance.selectedGhost == gameObject)
        {
            mRenderer.material = selectedMaterial;
        }
        else
        {
            mRenderer.material = fullTransparentnMat; //change to semi if in debug else full
        }
    }
}