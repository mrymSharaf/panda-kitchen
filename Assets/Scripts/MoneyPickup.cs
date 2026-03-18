using UnityEngine;
using UnityEngine.InputSystem;
public class MoneyPickup : MonoBehaviour, IInteractable
{
    public int paymentAmount = 20;
    public GameManager gameManager;
    public CustomerBehaviour customer;

    private void OnEnable()
    {
        Debug.Log("Money object ENABLED: " + gameObject.name);

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Debug.Log("Money collider enabled = " + col.enabled + ", isTrigger = " + col.isTrigger);
        }
        else
        {
            Debug.LogError("NO COLLIDER on money object: " + gameObject.name);
        }
    }

    public void Interact(PlayerInventory inventory)
    {
        Debug.Log("MoneyPickup.Interact called on: " + gameObject.name);

        if (gameManager == null)
        {
            Debug.LogError("GameManager is missing on " + gameObject.name);
            return;
        }

        if (customer == null)
        {
            Debug.LogError("Customer is missing on " + gameObject.name);
            return;
        }

        Debug.Log("Customer linked to money: " + customer.name);
        Debug.Log("Customer state when collecting: " + customer.currentState);

        if (customer.currentState != CustomerBehaviour.CustomerState.Paying)
        {
            Debug.Log("Customer is not ready to pay yet");
            return;
        }

        gameManager.AddMoney(paymentAmount);
        Debug.Log("Collected $" + paymentAmount + " from " + gameObject.name);

        gameObject.SetActive(false);
        customer.LeaveRestaurant();
    }
}
