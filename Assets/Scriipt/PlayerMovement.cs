using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float acceleration = 12f;
    public float deceleration = 12f;

    [Header("Jumping")]
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;
    public float fallMultiplier = 2f;

    [Header("Mouse Look")]
    public float sensitivity = 120f;
    public Transform cameraTransform;
    public float minLookX = -70f;
    public float maxLookX = 80f;

    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 currentMove;
    private float cameraPitch = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleMovement();
        HandleGravity();
        HandleJump();
        HandleCameraLookWithArrows(); // <— new

        controller.Move(velocity * Time.deltaTime);
    }


    void HandleMovement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 targetMove = (transform.right * x + transform.forward * z).normalized * moveSpeed;

        // Smooth acceleration & deceleration
        currentMove = Vector3.Lerp(currentMove, targetMove,
            (targetMove.magnitude > 0 ? acceleration : deceleration) * Time.deltaTime);

        velocity.x = currentMove.x;
        velocity.z = currentMove.z;
    }

    void HandleGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f; // keep grounded

        // Apply gravity
        velocity.y += gravity * (velocity.y < 0 ? fallMultiplier : 1f) * Time.deltaTime;
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    void HandleCameraLookWithArrows()
    {
        float lookX = Input.GetKey(KeyCode.RightArrow) ? 1 :
                      Input.GetKey(KeyCode.LeftArrow) ? -1 : 0;

        float lookY = Input.GetKey(KeyCode.UpArrow) ? 1 :
                      Input.GetKey(KeyCode.DownArrow) ? -1 : 0;

        // Horizontal rotation (left/right) rotates the PLAYER
        transform.Rotate(Vector3.up * lookX * sensitivity * Time.deltaTime);

        // Vertical rotation (up/down) rotates the CAMERA only
        cameraPitch -= lookY * sensitivity * Time.deltaTime;
        cameraPitch = Mathf.Clamp(cameraPitch, minLookX, maxLookX);

        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }


}
