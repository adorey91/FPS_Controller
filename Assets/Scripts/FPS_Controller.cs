using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FPS_Controller : MonoBehaviour
{
    private Vector3 currentMovement = Vector3.zero;
    private float verticalRotation;
    private float cameraStanding;
    private CharacterController characterController;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 4.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private float crouchHeight = 1.3f;
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] bool isCrouched;
    [SerializeField] bool isGrounded;

    [Header("Look Settings")]
    [SerializeField] private float mouseSensitivity = 2.0f;
    [SerializeField] private float upDownLimit = 65f;
    [SerializeField] private Camera playerCamera;

    [Header("UI Text")]
    public GameObject walking;
    public GameObject running;
    public GameObject crouching;
    public GameObject jumping;
    

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        cameraStanding = standingHeight - 0.2f;
    }

    void Update()
    {
        handleCrouch();
        handleMovement();
        groundPlayer();
        handleLook();
        Quit();
    }

    void Quit()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
            Debug.Log("Quitting Application");
        }
        
    }

    void handleJump()
    {
        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            currentMovement.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            isGrounded = false;
            jumping.SetActive(true);
        }
        else
            jumping.SetActive(false);
    }

    void groundPlayer()
    {
        if (characterController.isGrounded)
        {
            currentMovement.y = 0f;
            isGrounded = true;
            handleJump();
        }
        else
            currentMovement.y += gravityValue * Time.deltaTime;
    }

    void handleMovement() // handles player movement
    {
        Vector3 horizontalMovement = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        horizontalMovement = transform.rotation * horizontalMovement;

        currentMovement.x = horizontalMovement.x * walkSpeed;
        currentMovement.z = horizontalMovement.z * walkSpeed;
        
        if (Input.GetKey(KeyCode.LeftShift) && !isCrouched) // makes character run
        {
            characterController.Move((currentMovement * 3f) * Time.deltaTime); // run is 3 times the current movement speed
            running.SetActive(true);
            walking.SetActive(false);
            crouching.SetActive(false);
        }
        else if(isCrouched) // if played is crouched
        {
            characterController.Move((currentMovement / 2f) * Time.deltaTime); // crouching is half the movement speed
            running.SetActive(false);
            walking.SetActive(false);
            crouching.SetActive(true);
        }
        else  // if no buttons are pressed, player walks.
        {
            characterController.Move(currentMovement * Time.deltaTime);
            running.SetActive(false);
            walking.SetActive(true);
            crouching.SetActive(false);
        }
    }

    void handleCrouch()
    {
        if (Input.GetKey(KeyCode.C) || Physics.Raycast(transform.position, Vector3.up, 2f))
        {
            isCrouched = true;
            characterController.height = crouchHeight;
            playerCamera.transform.localPosition = new Vector3(0, crouchHeight, 0);
        }
        else 
        {
            isCrouched = false;
            characterController.height = standingHeight;
            playerCamera.transform.localPosition = new Vector3(0, cameraStanding, 0);
        }
    }

    void handleLook() // handles look via mouse movement
    {
        float mouseRotationX = Input.GetAxis("Mouse X") * mouseSensitivity;
        this.transform.Rotate(0, mouseRotationX, 0);

        verticalRotation -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -upDownLimit, upDownLimit);
        
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation,0,0);
    }
}
