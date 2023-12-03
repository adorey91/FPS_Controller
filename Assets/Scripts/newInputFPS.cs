using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class newInputFPS : MonoBehaviour
{
    Vector2 playerInput;
    private Rigidbody rb;
    private CapsuleCollider player;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float crouchHeight = 1.3f;
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] bool isCrouched;
    [SerializeField] private bool isRunning;
    private bool isGrounded;

    [Header("Look Settings")]
    [SerializeField] private float mouseSensitivity = 0.5f;
    [SerializeField] private float upDownLimit = 35f;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float crouchingCamera = 0.1f;
    [SerializeField] private float standingCamera = 0.5f;
    private float verticalRotation;

    

    public void Start()
    {
        Cursor.visible = false;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        player = GetComponent<CapsuleCollider>();
    }

    void Update()
    {
        MovePlayer();

        if (playerInput != null)
            moveSpeed = walkSpeed;
    }

    void Quit()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
            Debug.Log("Quitting Application");
        }
    }
    public void Move(InputAction.CallbackContext context)
    {
        playerInput = context.ReadValue<Vector2>();
    }

    void MovePlayer()
    {
        PlayerGrounded();
        Vector3 cameraForward = playerCamera.transform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();


        Vector3 moveVector = playerInput.x * Vector3.right + playerInput.y * cameraForward;
        moveVector.Normalize();

        if (isRunning)
            moveSpeed = walkSpeed * 3;
        if (isCrouched)
            moveSpeed = walkSpeed / 2;
        if (!isRunning && !isCrouched)
            moveSpeed = walkSpeed;

        transform.Translate(moveVector * moveSpeed * Time.deltaTime, Space.World);
    }

    public void Run(InputAction.CallbackContext context)
    {
        if (context.performed)
            isRunning = true;
        if (context.canceled)
            isRunning = false;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        }
    }


    public void Crouch(InputAction.CallbackContext context)     // this isnt working right?
    {
        if (context.performed)
        {
            isCrouched = true;
            player.height = crouchHeight;
            playerCamera.transform.localPosition = new Vector3(0, crouchingCamera, 0);
        }
        else if (context.canceled)
        {
            // Only uncrouch if there is no obstacle above
            RaycastHit hit;
            if (!Physics.Raycast(transform.position, Vector3.up, out hit, 2f))
            {
                isCrouched = false;
                player.height = standingHeight;
                playerCamera.transform.localPosition = new Vector3(0, standingCamera, 0);
            }
        }
    }

    void PlayerGrounded()
    {
        if (GetComponent<Rigidbody>().velocity.y == 0)
            isGrounded = true;
        else
            isGrounded = false;
    }

    public void Look(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 mouseDelta = context.ReadValue<Vector2>();

            float mouseRotationX = mouseDelta.x * mouseSensitivity;
            this.transform.Rotate(0, mouseRotationX, 0);

            verticalRotation -= mouseDelta.y * mouseSensitivity;
            verticalRotation = Mathf.Clamp(verticalRotation, -upDownLimit, upDownLimit);

            playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
        }
    }
}
