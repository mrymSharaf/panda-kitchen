using UnityEngine;
using TMPro;

public class CustomerSpawner : MonoBehaviour
{
    public GameObject[] customerPrefabs;
    public SeatPoint[] seats;
    public Transform exitPoint;
    public Transform spawnPoint;
    public float spawnInterval = 8f;

    float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            TrySpawnCustomer();
            timer = 0f;
        }
    }

    void TrySpawnCustomer()
    {
        SeatPoint freeSeat = null;

        foreach (SeatPoint seat in seats)
        {
            if (!seat.isOccupied)
            {
                freeSeat = seat;
                break;
            }
        }

        if (freeSeat != null)
        {
            freeSeat.isOccupied = true;
            SpawnCustomer(freeSeat);
        }
    }

    void SpawnCustomer(SeatPoint seat)
    {
        if (customerPrefabs == null || customerPrefabs.Length == 0)
        {
            Debug.LogError("No customer prefabs assigned in CustomerSpawner.");
            seat.isOccupied = false;
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogError("Spawn Point is missing in CustomerSpawner.");
            seat.isOccupied = false;
            return;
        }

        int randomIndex = Random.Range(0, customerPrefabs.Length);
        GameObject prefabToSpawn = customerPrefabs[randomIndex];

        GameObject newCustomer = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);

        CustomerBehaviour customer = newCustomer.GetComponent<CustomerBehaviour>();

        if (customer != null)
        {
            customer.seatPoint = seat.transform;
            customer.exitPoint = exitPoint;
            customer.enterWaypoints = seat.enterWaypoints;
            customer.exitWaypoints = seat.exitWaypoints;
            customer.seat = seat;
            customer.customerText = seat.customerTextUI;

            customer.moneyObject = seat.moneyObject;
            customer.servedSushiVisual = seat.servedSushiVisual;
            customer.servedRamenVisual = seat.servedRamenVisual;
            customer.servedRollVisual = seat.servedRollVisual;

            if (seat.customerZone != null)
            {
                seat.customerZone.customer = customer;
            }

            if (seat.moneyObject != null)
            {
                MoneyPickup moneyPickup = seat.moneyObject.GetComponent<MoneyPickup>();

                if (moneyPickup != null)
                {
                    moneyPickup.customer = customer;
                    moneyPickup.gameManager = FindFirstObjectByType<GameManager>();
                }
            }
        }
        else
        {
            Debug.LogError("Spawned customer prefab has no CustomerBehaviour.");
            seat.isOccupied = false;
        }
    }
}