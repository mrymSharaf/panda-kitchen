using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerInteraction : MonoBehaviour
{
    private List<Collider> interactablesInRange = new List<Collider>();
    private PlayerInventory inventory;

    [Header("Interaction Range")]
    public float overlapRadius = 1.2f;
    public LayerMask interactableLayers = ~0;

    void Start()
    {
        inventory = GetComponent<PlayerInventory>();
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            RefreshInteractablesInRange();

            Collider closest = GetClosestInteractable();

            if (closest != null)
            {
                IInteractable interactable = closest.GetComponent<IInteractable>();

                if (interactable != null)
                {
                    Debug.Log("Interacting with: " + closest.name);
                    interactable.Interact(inventory);
                }
            }
            else
            {
                Debug.Log("No valid interactable in range");
            }
        }
    }

    void RefreshInteractablesInRange()
    {
        Collider[] overlaps = Physics.OverlapSphere(transform.position, overlapRadius, interactableLayers);

        interactablesInRange.Clear();

        foreach (Collider col in overlaps)
        {
            if (col == null || !col.enabled || !col.gameObject.activeInHierarchy)
                continue;

            if (col.GetComponent<IInteractable>() != null)
            {
                if (!interactablesInRange.Contains(col))
                {
                    interactablesInRange.Add(col);
                    Debug.Log("Added interactable: " + col.name);
                }
            }
        }
    }

    private Collider GetClosestInteractable()
    {
        Collider closestMoney = null;
        float closestMoneyDistance = Mathf.Infinity;

        Collider closestOther = null;
        float closestOtherDistance = Mathf.Infinity;

        foreach (Collider col in interactablesInRange)
        {
            if (col == null || !col.enabled || !col.gameObject.activeInHierarchy)
                continue;

            IInteractable interactable = col.GetComponent<IInteractable>();
            if (interactable == null)
                continue;

            Vector3 closestPoint = col.ClosestPoint(transform.position);
            float distance = Vector3.Distance(transform.position, closestPoint);

            Debug.Log("Candidate: " + col.name + " | distance: " + distance);

            if (col.GetComponent<MoneyPickup>() != null)
            {
                if (distance < closestMoneyDistance)
                {
                    closestMoneyDistance = distance;
                    closestMoney = col;
                }
            }
            else
            {
                if (distance < closestOtherDistance)
                {
                    closestOtherDistance = distance;
                    closestOther = col;
                }
            }
        }

        if (closestMoney != null)
        {
            Debug.Log("Choosing MONEY: " + closestMoney.name);
            return closestMoney;
        }

        if (closestOther != null)
        {
            Debug.Log("Choosing OTHER: " + closestOther.name);
            return closestOther;
        }

        return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IInteractable>() != null)
        {
            if (!interactablesInRange.Contains(other))
            {
                interactablesInRange.Add(other);
                Debug.Log("Entered interactable: " + other.name);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (interactablesInRange.Contains(other))
        {
            interactablesInRange.Remove(other);
            Debug.Log("Exited interactable: " + other.name);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, overlapRadius);
    }
}