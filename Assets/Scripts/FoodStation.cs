using UnityEngine;
using UnityEngine.InputSystem;

public class FoodStation : MonoBehaviour, IInteractable
{
    public string foodType = "Sushi";
    public float prepTime = 1f;

    private bool isPreparing = false;
    private float prepTimer = 0f;
    private PlayerInventory currentInventory;

    void Update()
    {
        if (isPreparing)
        {
            prepTimer -= Time.deltaTime;

            if (prepTimer <= 0f)
            {
                isPreparing = false;

                if (currentInventory != null)
                {
                    currentInventory.SetFood(foodType);
                    UIMessage.instance.ShowMessage(foodType + " Ready!");
                }
            }
        }
    }

    public void Interact(PlayerInventory inventory)
    {
        if (inventory == null)
            return;

        if (inventory.HasFood())
        {
            UIMessage.instance.ShowMessage("Already holding food");
            Debug.Log("Already holding food: " + inventory.heldFood);
            return;
        }

        if (isPreparing)
        {
            UIMessage.instance.ShowMessage("Already preparing...");
            return;
        }

        currentInventory = inventory;
        isPreparing = true;
        prepTimer = prepTime;

        UIMessage.instance.ShowMessage("Preparing " + foodType + "...");
    }
}