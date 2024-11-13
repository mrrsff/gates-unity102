using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class BasicPlayerController : MonoBehaviour
{
    #region Configuration
    // Using new Input System
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction sprintAction;
    private InputAction jumpAction;
    private InputAction attackAction;
    
    private void InitializeInputs()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        sprintAction = playerInput.actions["Sprint"];
        jumpAction = playerInput.actions["Jump"];
        attackAction = playerInput.actions["Attack"];
        
        sprintAction.performed += _ => SetSprinting(true);
        sprintAction.canceled += _ => SetSprinting(false);
        
        jumpAction.performed += Jump;
        attackAction.performed += ShootWeapon;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void UpdateValues()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        lookInput = lookAction.ReadValue<Vector2>();
    }
    #endregion

    private Transform cameraTransform;
    private Transform cameraJoint;
    private Weapon weapon;
    
    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool isSprinting;
    
    private float pitch;
    private float yaw;
    
    public float moveSpeed = 5;
    public float maxYLook = 80;
    public float mouseSensitivity = 1;
    private void Awake()
    {
        InitializeInputs(); // DO NOT REMOVE OR MOVE THIS LINE
        rb = GetComponent<Rigidbody>();
        
        cameraJoint = transform.Find("CameraJoint");
        cameraTransform = cameraJoint.Find("Camera");
        
        weapon = GetComponentInChildren<Weapon>();
    }
    private void Update()
    {
        UpdateValues(); // DO NOT REMOVE OR MOVE THIS LINE

        Move();
        Look();
    }
    
    private void ShootWeapon(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            weapon.SetIsFiring(true);
        }
        else if (context.canceled)
        {
            weapon.SetIsFiring(false);
        }
    }
    private void Jump(InputAction.CallbackContext context)
    {
        // TODO: You should check whether player is grounded or not.
    }
    
    private void SetSprinting(bool value)
    {
        // TODO: Set sprinting and maybe you want to change the FOV of the camera?
    }
    
    private void Move()
    {
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection *= isSprinting ? 2 : 1;
        
        // TODO: Maybe you want to add acceleration to the player movement here instead of directly setting the velocity
        rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);
    }
    
    private void Look()
    {
        pitch -= lookInput.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -maxYLook, maxYLook);
        
        yaw = transform.eulerAngles.y + lookInput.x * mouseSensitivity;
        
        transform.localEulerAngles = new Vector3(0, yaw, 0);
        cameraTransform.localEulerAngles = new Vector3(pitch, 0, 0);
    }
}
