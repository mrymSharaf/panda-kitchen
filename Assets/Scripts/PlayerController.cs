using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    public float speed = 4f;
    public float rotationSpeed = 10f;
    public GameObject heldFoodVisual;

    private Rigidbody rb;
    private PlayerControls controls;
    private Vector2 moveInput;
    private Animator animator;
    private PlayerInventory inventory;

    void Awake()
    {
        controls = new PlayerControls();
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        inventory = GetComponent<PlayerInventory>();

        if (heldFoodVisual != null)
            heldFoodVisual.SetActive(false);
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(-moveInput.x, 0, -moveInput.y);

        if (movement.magnitude > 0.01f)
        {
            Vector3 moveDirection = movement.normalized;

            Vector3 newPosition = rb.position + moveDirection * speed * Time.fixedDeltaTime;
            rb.MovePosition(newPosition);

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            Quaternion smoothRotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(smoothRotation);
        }
    }

    void Update()
    {
        bool walking = moveInput.magnitude > 0.1f;
        bool holding = inventory != null && inventory.HasFood();

        animator.SetBool("isWalking", walking);
        animator.SetBool("isHolding", holding);

        if (heldFoodVisual != null)
            heldFoodVisual.SetActive(holding);
    }
}