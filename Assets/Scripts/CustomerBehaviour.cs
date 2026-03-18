using UnityEngine;
using TMPro;

public class CustomerBehaviour : MonoBehaviour
{
    public enum CustomerState
    {
        Entering,
        WaitingToOrder,
        DecidingOrder,
        WaitingForFood,
        Eating,
        Paying,
        Leaving
    }

    string[] thinkingTexts = 
    {
        "Hmm...",
        "Let me think...",
        "One moment...",
        "What do I want?"
    };

    string[] impatientOrderTexts =
    {
        "Excuse me?",
        "Hello?",
        "Are you taking my order?"
    };

    string[] angryOrderTexts =
    {
        "I'm getting impatient!",
        "Are you ignoring me?",
        "I'm leaving soon!"
    };

    public GameManager gameManager;

    public CustomerState currentState;
    public Transform seatPoint;
    public Transform exitPoint;

    public Transform[] enterWaypoints;
    public Transform[] exitWaypoints;

    public float moveSpeed = 2f;
    public float stopDistance = 0.25f;
    public float rotationSpeed = 5f;

    public string wantedFood = "";
    public TextMeshProUGUI customerText;
    public GameObject moneyObject;
    public SeatPoint seat;

    public GameObject servedSushiVisual;
    public GameObject servedRamenVisual;
    public GameObject servedRollVisual;

    public float timeToOrder = 40f;
    public float timeToReceiveFood = 40f;
    public float decisionTime = 3f;

    private float decisionTimer;
    private int warningStage = 0;
    private float waitTimer;
    private Animator animator;
    private float eatTimer = 5f;
    private int currentWaypointIndex = 0;
    private bool usingExitPath = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        gameManager = FindFirstObjectByType<GameManager>();
        StartEntering();
    }

    void Update()
    {
        if (currentState == CustomerState.Entering)
        {
            FollowPath(enterWaypoints, seatPoint, OnReachedSeat);
        }
        else if (currentState == CustomerState.WaitingToOrder)
        {
            waitTimer -= Time.deltaTime;

            if (waitTimer <= 10f && waitTimer > 5f && warningStage == 0)
            {
                warningStage = 1;
                UpdateText(impatientOrderTexts[Random.Range(0, impatientOrderTexts.Length)]);
            }
            else if (waitTimer <= 5f && waitTimer > 0f && warningStage == 1)
            {
                warningStage = 2;
                UpdateText(angryOrderTexts[Random.Range(0, angryOrderTexts.Length)]);
            }

            if (waitTimer <= 0f)
            {
                UpdateText("That's it! I'm leaving!");
                LeaveRestaurant();
            }
        }
        else if (currentState == CustomerState.DecidingOrder)
        {
            decisionTimer -= Time.deltaTime;

            if (decisionTimer <= 0f)
            {
                FinishChoosingOrder();
            }
        }
        else if (currentState == CustomerState.WaitingForFood)
        {
            waitTimer -= Time.deltaTime;

            if (waitTimer <= 10f && waitTimer > 5f && warningStage == 0)
            {
                warningStage = 1;
                UpdateText("Where is my food?");
            }
            else if (waitTimer <= 5f && waitTimer > 0f && warningStage == 1)
            {
                warningStage = 2;
                UpdateText("This is taking too long!");
            }

            if (waitTimer <= 0f)
            {
                UpdateText("No food! I'm leaving!");
                LeaveRestaurant();
            }
        }
        else if (currentState == CustomerState.Eating)
        {
            eatTimer -= Time.deltaTime;

            if (eatTimer <= 0f)
            {
                currentState = CustomerState.Paying;
                UpdateText("Payment ready");
                HideAllServedVisuals();

                if (moneyObject != null)
                    moneyObject.SetActive(true);
            }
        }
        else if (currentState == CustomerState.Leaving)
        {
            FollowPath(exitWaypoints, exitPoint, OnReachedExit);
        }
    }

    void StartEntering()
    {
        if (seat != null)
            seat.isOccupied = true;

        currentWaypointIndex = 0;
        usingExitPath = false;
        currentState = CustomerState.Entering;
        UpdateText("Walking to table");

        SetWalking(true);
        SetSitting(false);
    }

    void FollowPath(Transform[] waypoints, Transform finalTarget, System.Action onArriveFinal)
    {
        Transform target = null;

        if (waypoints != null && currentWaypointIndex < waypoints.Length)
        {
            target = waypoints[currentWaypointIndex];
        }
        else
        {
            target = finalTarget;
        }

        if (target == null) return;

        MoveToTarget(target, () =>
        {
            if (waypoints != null && currentWaypointIndex < waypoints.Length)
            {
                currentWaypointIndex++;
            }
            else
            {
                onArriveFinal?.Invoke();
            }
        });
    }

    void MoveToTarget(Transform target, System.Action onArrive)
    {
        Vector3 targetPosition = target.position;
        targetPosition.y = transform.position.y;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        if (Vector3.Distance(transform.position, targetPosition) <= stopDistance)
        {
            onArrive?.Invoke();
        }
    }

void OnReachedSeat()
{
    Debug.Log(gameObject.name + " reached seat");

    if (seatPoint != null)
    {
        transform.position = new Vector3(seatPoint.position.x, transform.position.y, seatPoint.position.z);
        transform.rotation = seatPoint.rotation;
    }

    warningStage = 0;
    currentState = CustomerState.WaitingToOrder;
    waitTimer = timeToOrder;
    UpdateText("Press E to take order");

    SetWalking(false);
    SetSitting(true);

    Debug.Log("Set walking false, sitting true");
}

    void OnReachedExit()
    {
        if (seat != null)
        {
            seat.isOccupied = false;

            if (seat.customerZone != null)
                seat.customerZone.customer = null;
        }

        if (moneyObject != null)
            moneyObject.SetActive(false);

        SetWalking(false);
        SetSitting(false);
        UpdateText("");
        gameObject.SetActive(false);
    }

    public void TakeOrder()
    {
        if (currentState != CustomerState.WaitingToOrder)
            return;

        currentState = CustomerState.DecidingOrder;
        decisionTimer = decisionTime;
        UpdateText(thinkingTexts[Random.Range(0, thinkingTexts.Length)]);
    }

    void FinishChoosingOrder()
{
    string[] foods = { "Sushi", "Ramen", "Roll" };
    wantedFood = foods[Random.Range(0, foods.Length)];

    warningStage = 0;
    currentState = CustomerState.WaitingForFood;
    waitTimer = timeToReceiveFood;

    UpdateText(wantedFood);
}

    public bool TryServeFood(string heldFood)
    {
        if (currentState != CustomerState.WaitingForFood)
            return false;

        if (heldFood == wantedFood)
            {
                warningStage = 0;
                currentState = CustomerState.Eating;
                eatTimer = 5f;
                UpdateText("Eating...");
                ShowServedVisual(heldFood);

                if (gameManager != null)
                    gameManager.AddCompletedOrder();

                return true;
            }

        UpdateText("Wrong food!");
        return false;
    }

    public void LeaveRestaurant()
    {
        if (currentState == CustomerState.Leaving)
            return;

        UpdateText("Leaving...");

        SetSitting(false);
        SetWalking(false);
        PlayStandUp();

        Invoke(nameof(BeginLeavingAfterStand), 0.8f);
    }

    void BeginLeavingAfterStand()
    {
        currentWaypointIndex = 0;
        usingExitPath = true;
        currentState = CustomerState.Leaving;

        SetWalking(true);
    }

    void ShowServedVisual(string foodName)
    {
        HideAllServedVisuals();

        if (foodName == "Sushi" && servedSushiVisual != null)
            servedSushiVisual.SetActive(true);

        if (foodName == "Ramen" && servedRamenVisual != null)
            servedRamenVisual.SetActive(true);

        if (foodName == "Roll" && servedRollVisual != null)
            servedRollVisual.SetActive(true);
    }

    void HideAllServedVisuals()
    {
        if (servedSushiVisual != null) servedSushiVisual.SetActive(false);
        if (servedRamenVisual != null) servedRamenVisual.SetActive(false);
        if (servedRollVisual != null) servedRollVisual.SetActive(false);
    }

    void UpdateText(string message)
    {
        if (customerText != null && customerText.text != message)
            customerText.text = message;
    }
        void SetWalking(bool walking)
    {
        if (animator != null)
            animator.SetBool("isWalking", walking);
    }

    void SetSitting(bool sitting)
    {
        if (animator != null)
            animator.SetBool("isSitting", sitting);
    }

    void PlayStandUp()
    {
        if (animator != null)
            animator.SetTrigger("StandUp");
    }
}