using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public int branchesInInventory = 0;

    public void AddBranch(int amount)
    {
        branchesInInventory += amount;
        Debug.Log("Picked up a branch! Total: " + branchesInInventory);
    }
}