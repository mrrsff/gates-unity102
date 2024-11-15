using System;
using System.Collections;
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
        attackAction.canceled += ShootWeapon;
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

    private Camera mainCamera;
    private Transform cameraTransform;
    private Transform cameraJoint;
    private Weapon weapon;
    
    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool isSprinting;
    private bool isGrounded;
    
    private float pitch;
    private float yaw;

    public float maxHealth = 100f;
    private float currentHealth;
    public float CurrentHealth => currentHealth;
    
    public float moveSpeed = 5;
    public float maxYLook = 80;
    public float mouseSensitivity = 1;
    private void Awake()
    {
        InitializeInputs(); // DO NOT REMOVE OR MOVE THIS LINE

        currentHealth = maxHealth;
        
        rb = GetComponent<Rigidbody>();
        cameraJoint = transform.Find("CameraJoint");
        cameraTransform = cameraJoint.Find("Camera");
        mainCamera = cameraTransform.GetComponent<Camera>();
        
        weapon = GetComponentInChildren<Weapon>();
    }
    private void Update()
    {
        UpdateValues(); // DO NOT REMOVE OR MOVE THIS LINE

        CheckGrounded();
        
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
        if (!isGrounded) return;
        rb.AddForce(Vector3.up * 5, ForceMode.Impulse);
        
        CrosshairConfigurator.Instance.Recoil(10);
    }
    private void CheckGrounded()
    {
        // Check if player is grounded
        var ray = new Ray(transform.position, Vector3.down);
        isGrounded = Physics.Raycast(ray, 1.5f);
    }
    
    private void SetSprinting(bool value)
    {
        isSprinting = value;
        // Change fov
        StartCoroutine(LerpFOV(isSprinting ? 90 : 80, 0.1f));
    }
    
    private IEnumerator LerpFOV(float targetFOV, float duration)
    {
        var startFOV = mainCamera.fieldOfView;
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            mainCamera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, time / duration);
            yield return null;
        }
        mainCamera.fieldOfView = targetFOV;
    }
    
    private void Move()
    {
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection *= isSprinting ? 1.5f : 1;
        
        rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);
    }
    
    private void Look()
    {
        // align with time scale to make it frame rate independent
        lookInput *= Time.deltaTime;
        
        pitch -= lookInput.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -maxYLook, maxYLook);
        
        yaw = transform.eulerAngles.y + lookInput.x * mouseSensitivity;
        
        transform.localEulerAngles = new Vector3(0, yaw, 0);
        cameraTransform.localEulerAngles = new Vector3(pitch, 0, 0);
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    private void Die()
    {
        // Handle player death
        GameManager.Instance.GameOver();
    }
    
}
