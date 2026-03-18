using UnityEngine;
using UnityEngine.UI;

public class CustomerZone : MonoBehaviour, IInteractable
{
    public CustomerBehaviour customer;
    public GameManager gameManager;
    public int paymentAmount = 20;
    
    public void Interact(PlayerInventory inventory)
    {
        if (customer == null)
        {
            Debug.Log("Customer is missing");
            return;
        }
        Debug.Log("This zone belongs to: " + customer.name);
        Debug.Log("Customer state is: " + customer.currentState);

        if (inventory == null)
        {
            Debug.Log("PlayerInventory is missing");
            return;
        }

        Debug.Log("Customer state is: " + customer.currentState);

        // 1) Take order
        if (customer.currentState == CustomerBehaviour.CustomerState.WaitingToOrder)
        {
            customer.TakeOrder();
            return;
        }

        // 2) Serve food
        if (customer.currentState == CustomerBehaviour.CustomerState.WaitingForFood)
        {
            if (!inventory.HasFood())
            {
                UIMessage.instance.ShowMessage("You are not holding any food");
                Debug.Log("You are not holding any food");
                return;
            }

            bool served = customer.TryServeFood(inventory.heldFood);

            if (served)
            {
                inventory.ClearFood();
                Debug.Log("Correct food served");
                UIMessage.instance.ShowMessage("Order served!");
            }
            else
            {
                UIMessage.instance.ShowMessage("Wrong order! Use trash can.");
                Debug.Log("Wrong food served. Go to the trash can to discard it.");
            }

            return;
        }

        // 3) Customer is eating
        if (customer.currentState == CustomerBehaviour.CustomerState.Eating)
        {
            Debug.Log("Customer is still eating");
            return;
        }

        if (customer.currentState == CustomerBehaviour.CustomerState.Paying)
        {
            Debug.Log("Customer zone ignored because payment must be picked from table");
            return;
        }
    }
}