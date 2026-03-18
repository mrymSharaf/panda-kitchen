using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
public string heldFood = "";

    public GameObject heldSushiVisual;
    public GameObject heldRamenVisual;
    public GameObject heldRollVisual;

    public bool HasFood()
    {
        return heldFood != "";
    }

    public void SetFood(string foodName)
    {
        heldFood = foodName;
        UpdateHeldVisual();
        Debug.Log("Holding: " + heldFood);
    }

    public void ClearFood()
    {
        Debug.Log("Cleared food: " + heldFood);
        heldFood = "";
        UpdateHeldVisual();
    }

    void UpdateHeldVisual()
    {
        if (heldSushiVisual != null) heldSushiVisual.SetActive(false);
        if (heldRamenVisual != null) heldRamenVisual.SetActive(false);
        if (heldRollVisual != null) heldRollVisual.SetActive(false);

        if (heldFood == "Sushi" && heldSushiVisual != null)
            heldSushiVisual.SetActive(true);

        if (heldFood == "Ramen" && heldRamenVisual != null)
            heldRamenVisual.SetActive(true);

        if (heldFood == "Roll" && heldRollVisual != null)
            heldRollVisual.SetActive(true);
    }
}
