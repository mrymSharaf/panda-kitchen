using UnityEngine;

public class TrashBin : MonoBehaviour, IInteractable
{
    public void Interact(PlayerInventory inventory)
    {
        if (inventory == null)
        {
            Debug.Log("No PlayerInventory found");
            return;
        }

        if (inventory.HasFood())
        {
            Debug.Log("Discarded: " + inventory.heldFood);
            inventory.ClearFood();
            UIMessage.instance.ShowMessage("Food discarded");
        }
        else
        {
            Debug.Log("No food to discard");
            UIMessage.instance.ShowMessage("No food to discard");
        }
    }
}
