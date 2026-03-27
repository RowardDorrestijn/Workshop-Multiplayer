using System.Collections;
using UnityEngine;

public class BranchController : InteractableObject
{
    override public void Interact(GameObject player)
    {
        InventoryManager inventoryManager = player.GetComponent<InventoryManager>();
        inventoryManager.AddBranch(1);
        Destroy(gameObject);
    }
}
